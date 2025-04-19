using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class HomingBombProj : ModProjectile
    {
        public override string Texture => "KirboMod/Projectiles/HomingBomb/HomingBombProj";
        private bool groundcollide;
        private bool awake = false;
        ref float Power => ref Projectile.ai[1];

        private List<float> Targetdistances = new(); //targeting
        private NPC aggroTarget = null; //target the minion is currently focused on
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Homing bomb");

            // Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            DrawOriginOffsetY = -18;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
        }
        public override void AI()
        {
            Projectile.localAI[2]--;
            if (Projectile.velocity.Y == 0 || Projectile.wet)
            {
                awake = true;
            }

            //slow down when on ground
            if (Projectile.velocity.Y == 0 && awake)
            {
                Projectile.velocity.X *= 0.96f; //slow
            }

            //Gravity
            if (Projectile.localAI[2] < 0)
            {
                Projectile.ignoreWater = false;
            }
            else
            {
                Projectile.wet = false;
            }
            if (Projectile.wet & Projectile.localAI[2] < 0)
            {

                if (Projectile.velocity.Y < 0)
                {
                    Projectile.velocity.Y = 0;
                }
                groundcollide = true;
                int x = (int)(Projectile.Center.X + 8) / 16;
                int y = (int)(Projectile.Center.Y + 8) / 16;
                x = Utils.Clamp(x, 0, Main.maxTilesX);
                y = Utils.Clamp(y, 2, Main.maxTilesY);
                Tile tileAbove = Main.tile[x, y - 1];
                float liquidInTilesAbove = tileAbove.LiquidAmount;
                tileAbove = Main.tile[x, y - 2];
                float t = ((Main.maxTilesY * 16) - Projectile.Center.Y + 8) / 16 % 1;
                liquidInTilesAbove = MathHelper.Lerp(liquidInTilesAbove, tileAbove.LiquidAmount, t);
                float maxVel = Utils.Remap(liquidInTilesAbove, 255, 0, -20, 0, false);
                Projectile.velocity.Y -= 1;
                if (Projectile.velocity.Y < maxVel)
                {
                    Projectile.velocity.Y = maxVel;
                }
            }
            else
            {
                //if (!awake)
                {
                    Projectile.velocity.Y += 0.5f;
                }
            }
            if (Projectile.velocity.Y >= 10f)
            {
                Projectile.velocity.Y = 10f;
            }

            //Animation
            if (awake)
            {
                Projectile.localAI[1] += Projectile.velocity.X * .07f; //changes frames every 5 ticks 
            }

            //explode when in contact with npc
            for (int i = 0; i < Main.maxNPCs; i++) //loop statement that cycles completely every tick
            {
                NPC npc = Main.npc[i]; //any npc

                if (npc.Hitbox.Intersects(Projectile.Hitbox) && !npc.friendly && npc.active) //hitboxes touching
                {
                    Projectile.Kill();
                }
            }

            ////player here too incase pvp
            //for (int i = 0; i < Main.maxPlayers; i++) //loop statement that cycles completely every tick
            //{
            //    Player player = Main.player[i]; //any player 

            //    //hitboxes touching and player is on opposing team
            //    if (player.Hitbox.Intersects(Projectile.Hitbox) && player.InOpposingTeam(Main.player[Projectile.owner]))
            //    {
            //        Projectile.Kill();
            //    }
            //}


            //HOMING

            if (awake) //start targeting if hit the ground
            {
                Projectile.ai[0]++; //for nothing except radar
                Projectile.rotation = 0; //up straight

                Projectile.spriteDirection = Projectile.direction;

                //Targeting
                float distanceFromTarget = 2000f;

                if (aggroTarget == null || !aggroTarget.CanBeChasedBy()) //search target
                {
                    //start each number with a very big number so they can't be targeted if their npc doesn't exist
                    Targetdistances = Enumerable.Repeat(999999f, Main.maxNPCs).ToList();

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];

                        float distance = Vector2.Distance(Projectile.Center, npc.Center);

                        if (npc.CanBeChasedBy()) //checks if targetable
                        {
                            Vector2 positionOffset = new(0, -5);
                            bool inView = Collision.CanHitLine(Projectile.position + positionOffset, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);

                            //close, hittable, hostile and can see target
                            if (inView && npc.CanBeChasedBy() && distance < distanceFromTarget && npc.active)
                            {
                                Targetdistances.Insert(npc.whoAmI, (int)distance); //add to list of potential targets
                            }
                        }

                        if (i == Main.maxNPCs - 1)
                        {
                            int theTarget = -1;

                            //count up 'til reached maximum distance
                            for (float j = 0; j < distanceFromTarget; j++)
                            {
                                int Aha = Targetdistances.FindIndex(a => a == j); //count up 'til a target is found in that range

                                if (Aha > -1) //found target
                                {
                                    theTarget = Aha;

                                    break;
                                }
                            }

                            if (theTarget > -1) //exists
                            {
                                NPC npc2 = Main.npc[theTarget];

                                if (npc2 != null) //exists
                                {
                                    aggroTarget = npc2;
                                }
                            }
                            else
                            {
                                break; //just in case
                            }
                        }
                    }
                }
                else if (aggroTarget != null && aggroTarget.CanBeChasedBy()) //ATTACK
                {
                    Vector2 deltaPos = aggroTarget.Center - Projectile.Center; //start - end
                    float accel = 0.9f;
                    float distance = Math.Abs(deltaPos.X); 
                    if (deltaPos.Y <= -20 && groundcollide == true && distance < 400 && MathF.Sign(deltaPos.X) == MathF.Sign(Projectile.velocity.X) || (Projectile.velocity.X == 0 && groundcollide == true))
                    {
                        float? timeToReachNullable = TimeToTarget(Projectile.Center.X, aggroTarget.Center.X, Projectile.velocity.X);
                        
                        if (timeToReachNullable != null && timeToReachNullable.HasValue)
                        {
                            Projectile.localAI[2] = 10;
                            Projectile.ignoreWater = true;
                            float timeToReach = timeToReachNullable.Value;
                            float gravity = 0.5f;       //needs ~20 to correct aim for some reason?
                            float requiredYVelocity = ((deltaPos.Y + 20) - 0.5f * gravity * timeToReach * timeToReach) / timeToReach;
                            Projectile.velocity.Y = requiredYVelocity; //jump
                            groundcollide = false; //wait to touch floor again
                        }
                    }

                    int pseudoDirection = 1;
                    if (deltaPos.X < 0) //enemy is behind
                    {
                        pseudoDirection = -1; //change direction so it will go towards enemy
                    }
                    //use .X so it only effects horizontal movement
                    Projectile.velocity.X += accel * pseudoDirection;

                    //Direction
                    if (deltaPos.X >= 0)
                    {
                        Projectile.direction = 1;
                    }
                    else
                    {
                        Projectile.direction = -1;
                    }
                }
            }
        }
        public static float? TimeToTarget(float currentX, float targetX, float initialVelX, float acceleration = 0.9f)
        {
            float dx = targetX - currentX;
            int direction = dx >= 0 ? 1 : -1;
            float a = acceleration * direction;

            float discriminant = initialVelX * initialVelX + 2 * a * dx;
            if (discriminant < 0)
            {
                return null;
            }

            float sqrtDisc = (float)Math.Sqrt(discriminant);
            float timeToReachLate = (-initialVelX + sqrtDisc) / acceleration;
            float timeToReachEarly = (-initialVelX - sqrtDisc) / acceleration;
            if (timeToReachEarly < timeToReachLate && timeToReachEarly > 0)
            {
                timeToReachLate = timeToReachEarly;
            }
            return timeToReachLate >= 0 ? timeToReachLate : null;
        }
        public override void OnKill(int timeLeft) //when the projectile dies
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0.01f, //no zero else it won't launch right
                    ModContent.ProjectileType<Projectiles.HomingBombExplosion>(), Projectile.damage + (int)(Projectile.damage * Power), 12, Projectile.owner, 0, Power);
            }
        }

        public override bool? CanCutTiles()
        {
            return false; //can't cut grass and pots
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.oldVelocity.Y > 0f) //if was falling
            {
                groundcollide = true;
            }
            return false;
        }
        static Asset<Texture2D> ChainBombChain;
        static Asset<Texture2D> Radar;
        static Asset<Texture2D> Wheel;
        public override bool PreDraw(ref Color lightColor)
        {
            ChainBombChain ??= ModContent.Request<Texture2D>("KirboMod/Projectiles/ChainBombChain");
            Radar ??= ModContent.Request<Texture2D>("KirboMod/Projectiles/HomingBomb/HomingBombRadar");
            Wheel ??= ModContent.Request<Texture2D>("KirboMod/Projectiles/HomingBomb/HomingBombWheel");

            if (Projectile.ai[0] > 0 && Projectile.ai[0] < 30) //make radar
            {
                Main.EntitySpriteDraw(Radar.Value, Projectile.Center - Main.screenPosition,
                            Radar.Value.Bounds, (Projectile.GetAlpha(Color.White) * Projectile.Opacity) * ((255 - (Projectile.ai[0] * 10)) / 255),
                            MathHelper.ToRadians(Projectile.ai[0] * 10), Radar.Size() / 2, 1f + (Projectile.ai[0] * 0.05f), SpriteEffects.None, 0);
            }

            Power = 0; //reset power
            int type = Type;
            Vector2 origin = ChainBombChain.Size() * .5f;
            Color white = Color.White;
            float chainWidth = ChainBombChain.Height();
            for (int i = 0; i <= Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];

                if (proj.active && Projectile.whoAmI < i && proj.type == type // checking i < whoAmI makes it so only one of them connects
                     && Vector2.DistanceSquared(Projectile.Center, proj.Center) < 200 * 200)
                {
                    Power += 1; //add 1 to power
                    Vector2 center = proj.Center;
                    Vector2 directionToBomb = Projectile.Center - center;
                    float projRotation = directionToBomb.ToRotation() - MathHelper.PiOver2;
                    float distance = directionToBomb.Length();
                    if (float.IsNaN(distance))
                        continue;
                    float increment = chainWidth / distance;
                    for (float j = 0; j < 1; j += increment)
                    {
                        //j is  progress
                        Vector2 pos = center + directionToBomb * j;
                        pos -= Main.screenPosition;
                        Main.EntitySpriteDraw(ChainBombChain.Value, pos, null, white, projRotation, origin, 1f, SpriteEffects.None);
                    }
                }
            }
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteEffects dir = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 absOffset = new(0, -6);
            if (Projectile.wet)
            {
                absOffset.Y -= 14;
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + absOffset, null, Color.White, Projectile.rotation, texture.Size() / 2, Projectile.scale, dir);
            Vector2 rotationRelativeOffset = new Vector2(4 * Projectile.spriteDirection, 16).RotatedBy(Projectile.rotation);

            if (Collision.WetCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                absOffset.X -= Projectile.spriteDirection * 4;
                absOffset.Y += 1;
                texture = ModContent.Request<Texture2D>("KirboMod/Projectiles/HomingBomb/HomingBombFloater").Value;
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + rotationRelativeOffset + absOffset, null, Color.White, Projectile.rotation, texture.Size() / 2, Projectile.scale, dir);
            }
            else
            {
                texture = Wheel.Value;
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + rotationRelativeOffset + absOffset, null, Color.White, Projectile.localAI[1], texture.Size() / 2, Projectile.scale, dir);
            }
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = Projectile.localAI[0] > 0;
            return true;
        }
    }
}