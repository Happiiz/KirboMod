using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace KirboMod.Particles
{
  
    public class ParticleSystem : ModSystem
    {
        public static List<Particle> particles = new();
        public static List<Particle> particlesBehindProj = new();
        public override void Load()
        {
            On_Main.DrawDust += DrawParticles;
            On_Dust.UpdateDust += UpdateParticles;
            IL_Main.DrawProjectiles += DrawParticlesBehindProjectiles;
        }

        private void DrawParticlesBehindProjectiles(ILContext il)
        {
            ILCursor cursor = new(il);
            for (int i = 0; i < 2; i++)
            {
                if (!cursor.TryGotoNext(MoveType.After, instr => instr.Match(OpCodes.Ldc_I4_0)))
                    return;
            }
            cursor.EmitDelegate(ParticlesBehindProj);
        }
        static void ParticlesBehindProj()
        {
            if (particlesBehindProj.Count <= 0)
                return;
            for (int i = 0; i < particlesBehindProj.Count; i++)
            {
                particlesBehindProj[i].Draw();
            }
        }
        private void UpdateParticles(On_Dust.orig_UpdateDust orig)
        {
            orig();
            if (particlesBehindProj.Count > 0)
            {
                for (int i = 0; i < particlesBehindProj.Count; i++)
                {
                    Particle particle = particlesBehindProj[i];
                    if (particle.timeLeft <= 0)
                    {
                        particlesBehindProj.RemoveAt(i);//todo: check if remove at works or if I have to offset i
                        i--;//account for shifting down the index(because removed the current particle)
                        continue;
                    }
                    particle.Update();
                    particle.Animate();
                }
            }
            if (particles.Count <= 0)
                return;
            for (int i = 0; i < particles.Count; i++)
            {
                Particle particle = particles[i];
                if(particle.timeLeft <= 0)
                {
                    particles.RemoveAt(i);//todo: check if remove at works or if I have to offset i
                    i--;//account for shifting down the index(because removed the current particle)
                    continue;
                }
                particle.Update();
                particle.Animate();
            }
        }

        private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            if (particles.Count <= 0)
                return;
            //params used for dust rendering
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Draw();
            }
            Main.spriteBatch.End();
        }
    }
    public abstract class Particle
    {
        public Texture2D texture;
        public ushort frame = 0;
        public ushort frameSpeed = 0;
        public ushort amountOfFrames = 1;
        public int timeLeft;
        public Vector2 velocity;
        public Vector2 position;
        public float rotation;
        public float scale;
        public float opacity;
        public Color color;
        public Vector2 squish;
        public float fadeInTime;
        public float fadeOutTime;
        private int duration;
        public float friction = 1;
        public float scaleVelocity;//todo: change to vec2
        public float scaleAcceleration;//todo: change to vec2
        public Rectangle? Frame { get => texture.Frame(1, amountOfFrames, frame); }    
        
        protected int Duration { get => duration; }
        public Vector2 ScreenPos { get => position - Main.screenPosition; }
        public Vector2 Origin { get => texture.Size() / 2; }
        public void Confirm(bool behindProjs = false)
        {
            duration = timeLeft;
            fadeInTime = MathF.Max(.00001f, fadeInTime);
            fadeOutTime = MathF.Max(.00001f, fadeInTime);
            if (frameSpeed == 0)
                frameSpeed = ushort.MaxValue;
            if (behindProjs)
            {
                ParticleSystem.particlesBehindProj.Add(this);
                return;
            }
            ParticleSystem.particles.Add(this);
        }
   
        public bool InsideScreenBounds(float leniency)
        {
            return position.X > Main.screenPosition.X - leniency && position.X < Main.screenPosition.X + Main.screenWidth + leniency 
                && position.Y > Main.screenPosition.Y - leniency && position.Y < Main.screenPosition.Y + Main.screenHeight + leniency;
        }
        public void Animate()
        {
            if((duration - timeLeft) % frameSpeed == 0)
            {
                frame++;
                frame %= amountOfFrames;
            }
        }
        public virtual void Update()
        {
            if(!InsideScreenBounds(400))
            {
                timeLeft = 0;
                return;
            }
            timeLeft--;
            position += velocity / 2;
            velocity *= friction;
            position += velocity / 2;

            scale += scaleVelocity / 2;
            scaleVelocity += scaleAcceleration;
            scale += scaleVelocity / 2;
            opacity = Utils.GetLerpValue(0, fadeInTime, timeLeft, true) * Utils.GetLerpValue(duration, duration - fadeOutTime, duration - timeLeft, true);
        }
        public virtual void Draw()
        {
            Main.EntitySpriteDraw(texture, position - Main.screenPosition, Frame, color * opacity, rotation, texture.Size() / 2, scale, SpriteEffects.None);
        }
    }

    //in case we need to handle multiplayer things. Not needed for now
    /*
    enum ParticleEfectType
    {
        NightmareWizardStar,//for spawning in the hand while shooting the stars
        NightmareWizardTeleport
    }
    interface IParticleEffect
    {
        void Serialize(BinaryWriter writer);
        void Deserialize(BinaryReader reader);
        void Spawn();
        void Sync();
        public ParticleEfectType Type { get; }
    }
    */
}
