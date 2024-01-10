using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace KirboMod.Systems
{    /// <summary>
     /// add this to every held projectile you want to use the custom trail system in. 
     /// <br>It's a workaround to issues with layering and draw delay I was having</br>
     /// </summary>
    public interface ITrailedHeldProjectile
    {
        void AddTrail();
    }
    public class HeldProjTrailSystem : ModSystem
    {
        public class Trail
        {
            Vector2[] positions;
            float[] rotations;
            float width;
            Color startColor;
            Color endColor;
            public static void AddSubtractive(Projectile proj, float width, Color startColor, Color endColor)
            {
                Trail trail = TrailFromProj(proj, width, startColor, endColor);
                if (trail != null)
                {
                    AddToArray(trail, ref subtractiveTrails);
                }
            }
            public static void AddAlphaBlend(Projectile proj, float width, Color startColor, Color endColor)
            {
                Trail trail = TrailFromProj(proj, width, startColor, endColor);
                if (trail != null)
                {
                    AddToArray(trail, ref alphaBlendTrails);
                }
            }
            public static void AddAdditive(Projectile proj, float width, Color startColor, Color endColor)
            {
                Trail trail = TrailFromProj(proj, width, startColor, endColor);
                if (trail != null)
                {
                    AddToArray(trail, ref additiveTrails);
                }
            }
            static Trail TrailFromProj(Projectile proj, float width, Color startColor, Color endColor)
            {
                Trail trail = new Trail();
                int trailLength = 0;
                for (int i = 0; i < proj.oldPos.Length; i++)
                {
                    if (proj.oldPos[i] == Vector2.Zero)
                        break;
                    trailLength++;
                }
                if (trailLength <= 0)
                {
                    return null;
                }
                trailLength++;//for the center
                trail.positions = new Vector2[trailLength];
                trail.rotations = new float[trailLength];
                for (int i = 1; i < trailLength; i++)
                {
                    trail.positions[i] = proj.oldPos[i - 1] + proj.Size / 2;
                    trail.rotations[i] = proj.oldRot[i - 1];
                }
                trail.positions[0] = proj.Center;
                trail.rotations[0] = proj.rotation;
                trail.width = width;
                trail.startColor = startColor;
                trail.endColor = endColor;
                return trail;
            }
            static void AddToArray(Trail trail, ref Trail[] trails)
            {
                if (trails == null)
                {
                    trails = new Trail[1] { trail };
                }
                else
                {
                    Array.Resize(ref trails, trails.Length + 1);
                    trails[^1] = trail;
                }
            }
            public VertexPositionColor[] GetVertexPositionColorDataAndIndices(out int[] indices, int indexOffset = 0)
            {
                int trailLength = positions.Length * 2;
                List<VertexPositionColor> result = new List<VertexPositionColor>();// new VertexPositionColor[positions.Length * 2];

                for (int i = 1; i < trailLength; i += 2)
                {
                    if (positions[i / 2] == Vector2.Zero)
                    {
                        continue;
                    }
                    float progress = (float)i / trailLength;
                    float opacity = Utils.GetLerpValue(trailLength, trailLength * .5f, i, true);
                    Vector2 normal = (rotations[i / 2] - MathHelper.PiOver4).ToRotationVector2() * width * .5f;
                    Color color = Color.Lerp(endColor, startColor, progress) * opacity;

                    //color.A = (byte)(255 * opacity);
                    result.Add(new VertexPositionColor(new Vector3(positions[i / 2] + normal, 0), color));
                    result.Add(new VertexPositionColor(new Vector3(positions[i / 2] - normal, 0), color));
                }
                int arrayLength = result.Count * 3 - 6;
                if (arrayLength < 3)
                    arrayLength = 3;

                indices = new int[arrayLength];
                for (int i = 0; i < indices.Length; i += 3)
                {
                    indices[i] = i / 3 + indexOffset;
                    indices[i + 1] = i / 3 + 1 + indexOffset;
                    indices[i + 2] = i / 3 + 2 + indexOffset;
                }
                return result.ToArray();
            }
        }
        public static bool IsDarkEnvironment(Player plr, out byte spaceAlpha)
        {
            //Main.atmo is the transparency of stars
            spaceAlpha = (byte)(Main.atmo * 255);
            return !plr.ZoneUnderworldHeight && (plr.ZoneOverworldHeight && !Main.dayTime);

        }
        static BasicEffect effect;
        static GraphicsDevice _device;
        private static Trail[] subtractiveTrails = null;
        private static Trail[] alphaBlendTrails = null;
        private static Trail[] additiveTrails = null;
        public override void Load()
        {
            if(Main.dedServ || Main.netMode == NetmodeID.Server)
            {
                return;
            }
            _device = Main.instance.GraphicsDevice;
            Main.QueueMainThreadAction(() =>
            {
                effect = new BasicEffect(_device);
                effect.VertexColorEnabled = true;
            });
            //will make it draw before held projectiles(which are not drawn in the same place as other projectiles)
            //it will also draw after most projectiles, making them affected by the trail's subtractive blending
            On_Main.DrawPlayers_AfterProjectiles += On_Main_DrawPlayers_AfterProjectiles;
        }
        private void On_Main_DrawPlayers_AfterProjectiles(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            if (subtractiveTrails == null && additiveTrails == null && alphaBlendTrails == null)
            {
                orig(self);
                return;
            }
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || proj.ModProjectile is not ITrailedHeldProjectile)
                {
                    continue;
                }
                ((ITrailedHeldProjectile)proj.ModProjectile).AddTrail();
            }
            SetupGraphicsDeviceAttributes();
            TryDrawingTrailsInArray(ref subtractiveTrails, GetSubtractiveBlendState());
            TryDrawingTrailsInArray(ref additiveTrails, BlendState.Additive);
            TryDrawingTrailsInArray(ref alphaBlendTrails, BlendState.AlphaBlend);
            orig(self);
        }
        static void TryDrawingTrailsInArray(ref Trail[] trails, BlendState blendstate)
        {
            if (trails != null)
            {
                _device.BlendState = blendstate;
                DrawTrailInArray(ref trails);
                trails = null;
            }
        }
        private static void DrawTrailInArray(ref Trail[] trails)
        {
            for (int j = 0; j < trails.Length; j++)
            {
                Trail trail = trails[j];
                if (trail == null)
                {
                    continue;
                }
                VertexPositionColor[] trailVertices = trail.GetVertexPositionColorDataAndIndices(out int[] indices);
                if (trailVertices.Length <= 2)
                {
                    continue;
                }
                for (int i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
                {
                    EffectPass pass = effect.CurrentTechnique.Passes[i];
                    pass.Apply();
                    _device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, trailVertices, vertexOffset: 0, trailVertices.Length, indices, indexOffset: 0, primitiveCount: indices.Length / 3);
                }
            }
        }
        private static void SetupGraphicsDeviceAttributes()
        {
            effect.World = Matrix.CreateTranslation(-new Vector3(Main.screenPosition.X, Main.screenPosition.Y, 0));
            effect.View = Main.GameViewMatrix.TransformationMatrix;
            Viewport viewport = Main.instance.GraphicsDevice.Viewport;
            effect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, -1, 1);
            _device.RasterizerState = RasterizerState.CullNone;
        }
        public override void Unload()
        {
            subtractiveTrails = null;
            additiveTrails = null;
            alphaBlendTrails = null;
            _device = null;
            effect = null;
        }
        //stolen from light's bane
        private static BlendState GetSubtractiveBlendState()
        {
            BlendState subtractive;
            _ = BlendState.AlphaBlend;//idk why these are here
            _ = BlendState.Additive;//idk why these are here
            subtractive = new BlendState
            {
                ColorBlendFunction = BlendFunction.ReverseSubtract,
                ColorDestinationBlend = Blend.One,
                ColorSourceBlend = Blend.SourceAlpha,
                AlphaBlendFunction = BlendFunction.ReverseSubtract,
                AlphaDestinationBlend = Blend.One,
                AlphaSourceBlend = Blend.SourceAlpha
            };
            return subtractive;
        }
    }
}