using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KirboMod.Particles
{
  
    public class ParticleSystem : ModSystem
    {
        public static List<Particle> particles = new();
        public override void Load()
        {
            On_Main.DrawDust += DrawParticles;
            On_Dust.UpdateDust += UpdateParticles;
        }

        private void UpdateParticles(On_Dust.orig_UpdateDust orig)
        {
            orig();
            if (particles.Count <= 0)
                return;
            for (int i = 0; i < particles.Count; i++)
            {
                Particle particle = particles[i];
                if(particle.timeLeft <= 0)
                {
                    particles.RemoveAt(i);//todo: check if remove at works or if I have to offset i
                    i--;//account for shifting down the index
                    continue;
                }
                particle.Update();
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
    //Make your custom particle effect inherit from this
    //maybe an oop approach is not needed
    public abstract class Particle
    {
        public Texture2D texture;
        public int timeLeft;
        public Vector2 velocity;
        public Vector2 position;
        public float rotation;
        public float scale;
        public float opacity;
        public Color color;
        public Vector2 squish;
        public int fadeInTime;
        public int fadeOutTime = 5;
        private int duration;
        public float friction = 1;
        public Vector2 ScreenPos { get => position - Main.screenPosition; }
        public Vector2 Origin { get => texture.Size() / 2; }

        //putting it in a method in case I decide to add more in it, like multiplayer syncing
        public void Confirm()
        {
            duration = timeLeft;
            fadeInTime = MathF.Max(float.Epsilon, fadeInTime);
            ParticleSystem.particles.Add(this);
        }
   
        bool InsideScreenBounds(float leniency)
        {
            return position.X > Main.screenPosition.X - leniency && position.X < Main.screenPosition.X + Main.screenWidth + leniency 
                && position.Y > Main.screenPosition.Y - leniency && position.Y < Main.screenPosition.Y + Main.screenHeight + leniency;
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
            //remedi
            opacity = Utils.GetLerpValue(0, fadeInTime, timeLeft, true) * Utils.GetLerpValue(duration, duration - fadeOutTime, timeLeft, true);
        }
        public virtual void Draw()
        {
            Main.EntitySpriteDraw(texture, position - Main.screenPosition, null, color * opacity, rotation, texture.Size() / 2, scale, SpriteEffects.None);
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
