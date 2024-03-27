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

        private List<float> Targetdistances = new List<float>(); //targeting
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
            if (Projectile.wet)
            {
                
                if (Projectile.velocity.Y < 0)
                {
                    Projectile.velocity.Y = 0;
                }
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
                Projectile.velocity.Y += 0.5f;
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

                if (aggroTarget == null || !aggroTarget.active || aggroTarget.dontTakeDamage) //search target
                {
                    //start each number with a very big number so they can't be targeted if their npc doesn't exist
                    Targetdistances = Enumerable.Repeat(999999f, Main.maxNPCs).ToList();

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];

                        float distance = Vector2.Distance(Projectile.Center, npc.Center);

                        if (npc.CanBeChasedBy()) //checks if targetable
                        {
                            Vector2 positionOffset = new Vector2(0, -5);
                            bool inView = Collision.CanHitLine(Projectile.position + positionOffset, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);

                            //close, hittable, hostile and can see target
                            if (inView && !npc.friendly && !npc.dontTakeDamage && !npc.dontCountMe && distance < distanceFromTarget && npc.active)
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
                else if (aggroTarget != null && aggroTarget.active && !aggroTarget.dontTakeDamage) //ATTACK
                {
                    Vector2 direction = aggroTarget.Center - Projectile.Center; //start - end
                    float speed = Projectile.wet ? 30 : 15f;
                    float inertia = 15f;

                    //Jumping
                    Vector2 jumprange = aggroTarget.Center - Projectile.Center;
                    if (jumprange.X < 0)
                        jumprange.X = -jumprange.X;
                    float distance = Math.Abs(direction.X); //get absolute
                    if (jumprange.Y <= -20 && groundcollide == true && jumprange.X < 1000 || (Projectile.velocity.X == 0 && groundcollide == true)) //jump when a little close and no attack cycle and when touching ground
                    {
                        Projectile.velocity.Y = direction.Y / 20; //jump
                        groundcollide = false; //wait to touch floor again
                    }

                    int pseudoDirection = 1;
                    if (direction.X < 0) //enemy is behind
                    {
                        pseudoDirection = -1; //change direction so it will go towards enemy
                    }
                    //we put this instead of player.Center so it will always be moving top speed instead of slowing down when enemy is near but unreachable
                    //A "carrot on a stick" if you will

                    Vector2 carrotDirection = new Vector2(pseudoDirection * 50, 0); //start - end 
                    carrotDirection.Normalize();
                    carrotDirection *= speed;

                    //use .X so it only effects horizontal movement
                    Projectile.velocity.X = (Projectile.velocity.X * (inertia - 1) + carrotDirection.X) / inertia;

                    //Direction
                    if (direction.X >= 0)
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
        public override void OnKill(int timeLeft) //when the projectile dies
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0.01f, //no zero else it won't launch right
                ModContent.ProjectileType<Projectiles.HomingBombExplosion>(), Projectile.damage + (int)((Projectile.damage * 0.4) * Power), 12, Projectile.owner, 0, Power);
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
            
            Vector2 absOffset = new Vector2(0, -6);
            if (Projectile.wet)
            {
                absOffset.Y -= 14;
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + absOffset, null, Color.White, Projectile.rotation, texture.Size() / 2, Projectile.scale, dir);
            Vector2 rotationRelativeOffset = new Vector2(4 * Projectile.spriteDirection, 16).RotatedBy(Projectile.rotation);
            if (Projectile.wet)
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
            fallThrough = false; //don't fall through platforms
            return true;
        }
    }
}