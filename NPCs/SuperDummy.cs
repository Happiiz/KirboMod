using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KirboMod.NPCs
{
    internal class SuperDummy : ModNPC
    {
        public override string Texture => "Terraria/Images/NPC_" + NPCID.SkeletronHead;
        public override void SetDefaults()
        {
            NPC.lifeMax = 40000000;
            NPC.defense = 50;
            NPC.knockBackResist = 0;
            NPC.noGravity = true;
            NPC.HitSound = Terraria.ID.SoundID.NPCHit2;
            NPC.TargetClosest();
            NPC.Size = new Vector2(200);
        }
        public override void AI()
        {
            NPC.Size = new Vector2(100);
            NPC.timeLeft = 100;
        }
    }
    public class FixedZenithTest : GlobalProjectile
    {
        static void RectVisualizer(Rectangle rectangle)
        {
            AABBLineVisualizer(rectangle.TopLeft() + new Vector2(rectangle.Width / 2, 0), rectangle.BottomLeft() + new Vector2(rectangle.Width / 2, 0), rectangle.Width);
        }
        public override void Load()
        {
            On_Projectile.Colliding += FixedZenithCollision;
        }
        public override void Unload()
        {
            On_Projectile.Colliding -= FixedZenithCollision;
        }
        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            if (ProjectileID.Sets.IsAWhip[projectile.type])
            {
                projectile.WhipSettings.RangeMultiplier = 5;
                for (int j = 0; j < projectile.MaxUpdates; j++)
                {       
                    projectile.WhipPointsForCollision.Clear();
                    List<Vector2> oldCollisionPoints = new();
                    projectile.ai[0] -= j;
                    Projectile.FillWhipControlPoints(projectile, projectile.WhipPointsForCollision);
                    projectile.ai[0]--;
                    Projectile.FillWhipControlPoints(projectile, oldCollisionPoints);
                    projectile.ai[0] += j + 1;
                    float whipRange = projectile.velocity.Length() * projectile.WhipSettings.RangeMultiplier;
                    Vector2 rectSize = new Vector2(100);//first check if inside a big rectangle that covers the enttire whip + more, to avoid checking colllision for entities who are outside the whip's range
                    rectSize *= whipRange;
                    Rectangle performanceThing = Utils.CenteredRectangle(projectile.WhipPointsForCollision[projectile.WhipPointsForCollision.Count / 2], rectSize);
                    //RectVisualizer(performanceThing);
                    for (int i = projectile.WhipPointsForCollision.Count - 1; i >= 0; i--)//first check the much less expensive rectangles
                    {
                        Point point = projectile.WhipPointsForCollision[i].ToPoint();
                        Rectangle myRect = projectile.Hitbox;
                        myRect.Location = new Point(point.X - myRect.Width / 2, point.Y - myRect.Height / 2);
                        RectVisualizer(myRect);
                    }
                    float loopEnd = (projectile.WhipPointsForCollision.Count) * 0.66f;
                    for (int i = projectile.WhipPointsForCollision.Count - 1; i >= loopEnd; i -= 2)//-2 for less AABBvLine checks, so more performance
                    {

                        Vector2 start = projectile.WhipPointsForCollision[i];
                        Vector2 end = oldCollisionPoints[i];
                        AABBLineVisualizer(start, end, (i == projectile.WhipPointsForCollision.Count - 1) ? 25 : 35);//thinner hitbox at the tip so it's not disjointed. The inside hitboxes are purposefully disjointed to cover up potential gaps
                        
                    }
                }   
            }
            else if(projectile.type == ProjectileID.FinalFractal)
            {
                float timerIncr =  Utils.Remap(projectile.velocity.Length() * 2, 900, 0, 0.7f, 2);          
                float hitboxLength = 40f;
                for (int i = 15; i < projectile.oldPos.Length; i+= 15)
                {
                    float relativeSwordTimer = projectile.localAI[0] - i * timerIncr - timerIncr;   //idk why this multiplication fixes it??? what??
                    if (!(relativeSwordTimer < 0f) && !(relativeSwordTimer > 60))
                    {
                        Vector2 hitboxCenter = projectile.oldPos[i] + projectile.Size / 2f;
                        Vector2 hitboxEdge = (projectile.oldRot[i] + (float)Math.PI / 2f).ToRotationVector2();
                        AABBLineVisualizer(hitboxCenter - hitboxEdge * hitboxLength, hitboxCenter + hitboxEdge * hitboxLength, 20f);         
                    }
                }
                Vector2 mainHitboxCenter = (projectile.rotation + (float)Math.PI / 2f).ToRotationVector2();
                if(projectile.localAI[0] < 59)
                    AABBLineVisualizer(projectile.Center - mainHitboxCenter * hitboxLength, projectile.Center + mainHitboxCenter * hitboxLength, 20f);
            }
        }
        static void AABBLineVisualizer(Vector2 lineStart, Vector2 lineEnd, float lineWidth)
        {
            Texture2D blankTexture = Terraria.GameContent.TextureAssets.Extra[195].Value;
            Vector2 texScale = new Vector2((lineStart - lineEnd).Length(), lineWidth) * 0.00390625f;//1/256, texture is 256x256
            Main.EntitySpriteDraw(blankTexture, lineStart - Main.screenPosition, null, Color.Red * 0.5f, (lineEnd - lineStart).ToRotation(), new Vector2(0, 128), texScale, SpriteEffects.None);
        }
        private bool FixedZenithCollision(On_Projectile.orig_Colliding orig, Projectile self, Microsoft.Xna.Framework.Rectangle myRect, Microsoft.Xna.Framework.Rectangle targetRect)
        {
            if (self.type == ProjectileID.FinalFractal)
            {
                //this timerIncr = Utils.Remap(...) isn't in vanilla
                float timerIncr = Utils.Remap(self.velocity.Length() * 2, 900, 0, 0.7f, 2);
                float collisionPoint = 0f;
                float hitboxLength = 40f;
                //       i = 14 in vanilla
                for (int i = 15; i < self.oldPos.Length; i += 15)
                {
                                             //self.LocalAI[0] - i in vanilla
                    float relativeSwordTimer = self.localAI[0] - i * timerIncr - timerIncr;
                    if (!(relativeSwordTimer < 0f) && !(relativeSwordTimer > 60f))
                    {
                        Vector2 hitboxCenter = self.oldPos[i] + self.Size / 2f;
                        Vector2 hitboxEdge = (self.oldRot[i] + (float)Math.PI / 2f).ToRotationVector2();
                        if (Collision.CheckAABBvLineCollision(targetRect.TopLeft(), targetRect.Size(), hitboxCenter - hitboxEdge * hitboxLength, hitboxCenter + hitboxEdge * hitboxLength, 20f, ref collisionPoint))
                        {
                            return true;
                        }
                    }
                }
                Vector2 mainHitboxCenter = (self.rotation + (float)Math.PI / 2f).ToRotationVector2();
                //doesn't check for localAI[0] < 59 in vanilla
                if (self.localAI[0] < 59 && Collision.CheckAABBvLineCollision(targetRect.TopLeft(), targetRect.Size(), self.Center - mainHitboxCenter * hitboxLength, self.Center + mainHitboxCenter * hitboxLength, 20f, ref collisionPoint))
                {
                    return true;
                }
                return false;
            }
            else if (ProjectileID.Sets.IsAWhip[self.type])
            {
                float stupidThing = 1;
                self.WhipPointsForCollision.Clear();
                List<Vector2> oldCollisionPoints = new();
                Projectile.FillWhipControlPoints(self, self.WhipPointsForCollision);
                Vector2 rectSize = new Vector2(100);//first check if inside a big rectangle, to avoid checking colllision for entities who are outside the whip's range
                float whipRange = self.velocity.Length() * self.WhipSettings.RangeMultiplier;
                //we could multiply the rect size by the whip's progress, but I don't think the calculations necessary for it aren't worth it.
                rectSize *= whipRange;//multiply by whip range
                Rectangle performanceThing = Utils.CenteredRectangle(self.WhipPointsForCollision[self.WhipPointsForCollision.Count / 2], rectSize);
                if (!performanceThing.Intersects(targetRect))
                    return false;
                self.ai[0]--;//ai0 is the counter of whip AI
                Projectile.FillWhipControlPoints(self, oldCollisionPoints);
                self.ai[0]++;
                Point hitboxCenter;
                for (int i = self.WhipPointsForCollision.Count - 1; i >= 0; i--)//first check the much less expensive rectangle hitboxes
                {
                    hitboxCenter = self.WhipPointsForCollision[i].ToPoint();
                    //location is top left
                    myRect.Location = new Point(hitboxCenter.X - myRect.Width / 2, hitboxCenter.Y - myRect.Height / 2);
                    if (myRect.Intersects(targetRect))
                    {
                        return true;
                    }
                }
                float loopEnd = self.WhipPointsForCollision.Count * 0.66f;
                for (int i = self.WhipPointsForCollision.Count - 1; i >= loopEnd; i -= 2)//-2 for less AABBvLine checks, so more performance
                {
                    if (Collision.CheckAABBvLineCollision(targetRect.TopLeft(), targetRect.Size(), self.WhipPointsForCollision[i], oldCollisionPoints[i], (i == self.WhipPointsForCollision.Count - 1) ? 25 : 35, ref stupidThing))
                        return true;
                }
                return false;
            }
            else
            return orig.Invoke(self, myRect, targetRect);
        }
    }
}
