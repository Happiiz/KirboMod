using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class PlasmaBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 35; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 2;
            Projectile.scale = 1;
            Projectile.penetrate = -1; //infinite
                                       //VFX.LoadTextures();
                                       //List<Vector2> pos = Lightning.CreateEvenlySpacedVector2sOnLine(Projectile.Center, Projectile.Center + new Vector2(0, 500), 10);
                                       //Lightning.PublicRandomlyOffset(ref pos);
                                       //lightningTest = new Lightning[1];
                                       //lightningTest[0] = Lightning.CreateLightning(pos, new() { Lightning.RandPlasmaCol }, 20);

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Helper.CheckCircleCollision(targetHitbox, Projectile.Center, 80);
        }
        public override void AI()
        {
            Projectile.frameCounter++;//using the frame counter not as a frrame counter...
            Projectile.rotation = Projectile.velocity.ToRotation();
            //dust effects
            Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(80, 80), DustID.FireworksRGB, null, 0, Color.Cyan, 2);
            dust.velocity += Projectile.velocity;
            dust = Dust.CloneDust(dust);
            if (dust.dustIndex != 6000)
            {
                dust.scale *= 0.7f;
                dust.color = Color.White;
            }
            dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(80, 80), DustID.FireworksRGB, null, 0, Color.GreenYellow, 2);
            dust.velocity += Projectile.velocity;
            dust = Dust.CloneDust(dust);
            if (dust.dustIndex != 6000)
            {
                dust.scale *= 0.7f;
                dust.color = Color.White;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 100; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
            {
                Vector2 speed = Main.rand.NextVector2Circular(20, 20); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.TerraBlade, speed, Scale: 2f); //Makes dust in a messy circle
                d.noGravity = true;
            }
        }

        //Lightning[] lightningTest;
        public override bool PreDraw(ref Color lightColor)
        {
            //drawPos = Projectile.Center - Main.screenPosition;
            //Main.EntitySpriteDraw(texture, drawPos, null, new Color(255,255,255,0) * 0.05f, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None);
            float rotation = Projectile.rotation;
            Projectile.rotation = 0;
            //VFX.DrawElectricOrb(Projectile, new Vector2(6));
            Projectile.rotation = rotation;
            Texture2D main = ModContent.Request<Texture2D>("KirboMod/Projectiles/PlasmaOrb/PlasmaOrbBase").Value;
            int skipCount = 6;
            for (int i = Projectile.oldPos.Length - 1; i >= 0; i-= skipCount)
            {
                int index = i - Projectile.frameCounter % skipCount;
                if(index < 0 || index >= Projectile.oldPos.Length)
                {
                    continue;
                }
                Vector2 oldPos = Projectile.oldPos[index] + Projectile.Size / 2;
                float scaleMult = Utils.Remap(i, 0, Projectile.oldPos.Length - 1, 1f, .7f);
                float progress = (float)index / Projectile.oldPos.Length;
                Color c = Color.Lerp(Color.Teal, Color.Cyan, progress);
                c.A = 128;
                c *= 1 - progress;
                Main.EntitySpriteDraw(main, oldPos - Main.screenPosition, null, c, 0, main.Size() / 2, scaleMult, SpriteEffects.None, 0f);
            }
            DrawPlasmaBlast(Projectile, Main.spriteBatch);
            return false;
        }
        //it's like this because I was gonna implement player draw layer using this but realized that actually I shouldn't do it by using this method.
        //
        static void DrawPlasmaBlast(Projectile proj, SpriteBatch sb)
        {
            Color col = Color.White;
            col.A = 0;
            Texture2D main = ModContent.Request<Texture2D>("KirboMod/Projectiles/PlasmaOrb/PlasmaOrbBase").Value;
            Texture2D strands1 = ModContent.Request<Texture2D>("KirboMod/Projectiles/PlasmaOrb/PlasmaOrbStrands1").Value;
            Texture2D strands2 = ModContent.Request<Texture2D>("KirboMod/Projectiles/PlasmaOrb/PlasmaOrbStrands2").Value;           
            sb.Draw(main, proj.Center - Main.screenPosition, null, Color.White with { A = 128 }, 0, main.Size() / 2, 1f, SpriteEffects.None, 0f);
            float time = (float)(Main.GlobalTimeWrappedHourly * 100f);
            int strand1FlashCycleLength = 30;
            int strand2FlashCycleLength = (int)(strand1FlashCycleLength * Helper.Phi);
            float strand1Opacity = Utils.GetLerpValue(0, strand1FlashCycleLength / 2f, time % strand1FlashCycleLength, true) * Utils.GetLerpValue(strand1FlashCycleLength - 1, strand1FlashCycleLength / 2f, time % strand1FlashCycleLength, true);
            int strand1FlashCycleIndex = (int)(time / strand1FlashCycleLength);
            float rotation = (int)(strand1FlashCycleIndex * Helper.Phi) * MathF.PI * 0.5f;
            SpriteEffects fx = (SpriteEffects)(strand1FlashCycleIndex % 3);
            sb.Draw(strands1, proj.Center - Main.screenPosition, null, col * strand1Opacity, rotation, strands1.Size() / 2, 1f, fx, 0f);
            float strand2Opacity = Utils.GetLerpValue(0, strand2FlashCycleLength / 2f, time % strand2FlashCycleLength, true) * Utils.GetLerpValue(strand2FlashCycleLength - 1, strand2FlashCycleLength / 2f, time % strand2FlashCycleLength, true);
            int strand2FlashCycleIndex = (int)((time) / strand2FlashCycleLength);
            rotation = (int)(strand2FlashCycleIndex * Helper.Phi) * MathF.PI * 0.5f;
            fx = (SpriteEffects)(strand2FlashCycleIndex % 3);

            sb.Draw(strands2, proj.Center - Main.screenPosition, null, col * strand2Opacity, rotation, strands2.Size() / 2, 1f, fx, 0f);
        }
        ///// <summary>
        ///// 
        ///// might need to make a lightning drawer struct or static method for drawing the lightning and a bunch of branches at once AAAAAAAAA
        ///// also need to make a shader to properly draw the cap with a lerped color across the length of the line
        ///// </summary>
        //public struct Lightning
        //      {
        //	public static List<Vector2> CreateEvenlySpacedVector2sOnLine(Vector2 lineStart, Vector2 lineEnd, int segments)
        //          {
        //		List<Vector2> points = new List<Vector2>();
        //		points.Add(lineEnd);
        //              for (int i = 0; i < segments; i++)
        //              {
        //			points.Add((lineStart - lineEnd) * ((float)i / segments));
        //              }
        //		return points;
        //          }
        //          public static Color RandPlasmaCol { get => Main.rand.NextBool() ? Color.Green : Color.Cyan; }
        //	/// <summary>
        //	/// TODO:  MAKE PUBLIC STATIC
        //	/// </summary>
        //	/// <param name="vectors"></param>
        //	/// <param name="maxRandomization"></param>
        //          void RandomlyOffset(ref List<Vector2> vectors, float maxRandomization = 64)
        //          {
        //		//maybe make more complex system to account for very short lightnings by using distance traveled instead of joints to calculate progress?
        //		//don't randomize the start and end
        //              for (int i = 1; i < vectors.Count - 1; i++)
        //              {
        //			float progress = (float)i / vectors.Count;
        //			vectors[i] += Main.rand.NextVector2Circular(maxRandomization, maxRandomization) * progress;
        //              }

        //          }
        //	/// <summary>
        //	///todo: delete this
        //	/// </summary>
        //	/// <param name="vectors"></param>
        //	/// <param name="maxRandomization"></param>
        //	public  static void PublicRandomlyOffset(ref List<Vector2> vectors, float maxRandomization = 64)
        //	{
        //		//maybe make more complex system to account for very short lightnings by using distance traveled instead of joints to calculate progress?
        //		//don't randomize the start and end
        //		for (int i = 1; i < vectors.Count - 1; i++)
        //		{
        //			float progress = (float)i / vectors.Count;
        //			vectors[i] += Main.rand.NextVector2Circular(maxRandomization, maxRandomization) * progress;
        //		}

        //	}
        //	static Lightning CreateLightning(List<LightningVertex> vertices, float startWidth, int branchAmount = 3)
        //	{
        //		List<LightningLine> lines = new();
        //		for (int i = 0; i < vertices.Count - 1; i++)
        //		{
        //			lines.Add(new LightningLine(vertices[i], vertices[i + 1]));
        //		}
        //		Lightning primaryLightning = new Lightning(lines, startWidth, branchAmount);
        //		primaryLightning.Branch();

        //		return primaryLightning;
        //	}
        //	/// <summary>
        //	/// BRANCHFEATURE IS UNFINISHED AND WILL CAUSE NULL REFERENCE EXCEPTION IF BRANCH AMOUNT IS MORE THAN 0
        //	/// </summary>
        //	/// <param name="points"></param>
        //	/// <param name="colors"></param>
        //	/// <param name="startWidth"></param>
        //	/// <param name="branchAmount"></param>
        //	/// <returns></returns>
        //	public static Lightning CreateLightning(List<Vector2> points, List<Color> colors, float startWidth, int branchAmount = 0)
        //          {
        //		List<LightningVertex> vertices = new();
        //              for (int i = 0; i < points.Count; i++)
        //              {
        //				//makes the colors repeat in a pattern until all points have been filled
        //			vertices.Add(new LightningVertex(colors[i % colors.Count], points[i]));
        //              }
        //		List<LightningLine> lines = new();
        //              for (int i = 0; i < vertices.Count - 1; i++)
        //              {
        //			lines.Add(new LightningLine(vertices[i], vertices[i + 1]));
        //              }
        //		Lightning primaryLightning = new Lightning(lines, startWidth, branchAmount);
        //		if(branchAmount > 0)
        //			primaryLightning.Branch();

        //		return primaryLightning;
        //          }
        //	List<Lightning> Branch()
        //          {
        //		int vertexAmount = lightningLines.Count / 2 - 1;// always a third of he current lightning's vertex count - 1
        //		List<LightningLine> linesToBranchFrom = new(branchAmount);
        //		List<LightningLine> linesThatCanBranchFrom = lightningLines;
        //              for (int i = 0; i < branchAmount; i++)
        //              {
        //			int lineIndexToBranchFrom = Main.rand.Next(linesThatCanBranchFrom.Count);
        //			//use the start vertex so it can branch off the very top of the lightning and not branch  from the end of the lightning
        //			linesToBranchFrom[i] = linesThatCanBranchFrom[lineIndexToBranchFrom];
        //		}
        //		List<Lightning> branches = new();
        //              for (int j = 0; j < linesToBranchFrom.Count; j++)
        //              {
        //			//initializes the branch and starts it with 1line
        //			List<LightningVertex> vertices = new();
        //			vertices.Add(linesToBranchFrom[j].startVertex);
        //			float lineLength = fullLightningLength / vertexAmount / 3 * (Main.rand.NextFloat() + 0.5f);
        //			float randRotation = linesToBranchFrom[j].Direction.ToRotation() + Main.rand.NextFloat();
        //			if(randRotation < 0 || randRotation > MathF.PI)
        //                  {

        //                  }

        //			vertices.Add(new LightningVertex(RandPlasmaCol, linesToBranchFrom[j].Direction * lineLength));

        //			//TODO FINISH THIS IF YOU WANT BRANCHES
        //			for (int i = 1; i < vertexAmount; i++)
        //			{
        //				lineLength = fullLightningLength / vertexAmount / 3 * (Main.rand.NextFloat() + 0.5f);
        //				LightningLine lineJustCreated = new LightningLine(vertices[i - 1], vertices[i]);
        //				vertices.Add(new LightningVertex(RandPlasmaCol, linesToBranchFrom[j].Direction * lineLength));
        //			}
        //		}

        //		branchAmount -= Main.rand.Next(branchAmount) + 1;

        //		return null;
        //	}

        //          Lightning(List<LightningLine> lightningLines, float startWidth, int branchAmount = 0)
        //          {
        //		this.branchAmount = branchAmount;
        //		this.startWidth = startWidth;
        //		this.lightningLines = lightningLines;
        //		fullLightningLength = 0;
        //              for (int i = 0; i < lightningLines.Count; i++)
        //              {
        //			fullLightningLength += (lightningLines[i].startVertex.vertexPos - lightningLines[i].endVertex.vertexPos).Length();
        //              }
        //          }
        //	int branchAmount;
        //	List<LightningLine> lightningLines;
        //	float fullLightningLength;
        //	float startWidth;

        //	public void Draw()
        //          {

        //              for (int i = 0; i < lightningLines.Count; i++)
        //              {
        //			float lengthToThisVertex = 0;
        //                  for (int j = 0; j < i; j++)
        //                  {
        //				lengthToThisVertex += (lightningLines[j].startVertex.vertexPos - lightningLines[j].endVertex.vertexPos).Length();
        //			}
        //			lightningLines[i].DrawLineWithCap(startWidth * (1 - lengthToThisVertex / fullLightningLength));
        //              }
        //          }

        //	struct LightningLine
        //	{

        //		internal List<LightningVertex> ExtractVerticesFromLineSequence(List<LightningLine> lines)
        //              {
        //			List<LightningVertex> vertices = new();
        //			vertices.Add(lines[0].startVertex);
        //                  for (int i = 0; i < lines.Count; i++)
        //                  {
        //				vertices.Add(lines[i].endVertex);
        //                  }
        //			return vertices;
        //              }


        //              internal LightningLine(LightningVertex start, LightningVertex end)
        //              {
        //			startVertex = start;
        //			endVertex = end;
        //              }
        //		internal LightningVertex startVertex;
        //		internal LightningVertex endVertex;
        //              public Vector2 Direction { get => Vector2.Normalize(endVertex.vertexPos - startVertex.vertexPos); }
        //              public void DrawLine(float width)
        //		{
        //			Vector2 texScale = new Vector2((startVertex.vertexPos - endVertex.vertexPos).Length(), width) * 0.0078125f;//1/128, texture is 128x128
        //			Main.EntitySpriteDraw(VFX.glowLine.Value, (startVertex.vertexPos) - Main.screenPosition, null, Color.Red, (endVertex.vertexPos - startVertex.vertexPos).ToRotation(), new Vector2(0, 128), texScale, SpriteEffects.None);
        //		}
        //		public void DrawLineWithCap(float width, bool drawWhiteInside = true)
        //		{
        //			Vector2 texScale = new Vector2((startVertex.vertexPos - endVertex.vertexPos).Length(), width) * 0.0078125f;//1/128, texture is 128x128
        //			Vector2 capScale = new Vector2(width) * 0.0078125f;//1/128, texture is 128x128
        //			float rotation = (endVertex.vertexPos - startVertex.vertexPos).ToRotation();
        //			Main.EntitySpriteDraw(VFX.GlowLine, (startVertex.vertexPos) - Main.screenPosition, null, startVertex.vertexColor, rotation, new Vector2(0, 128), texScale, SpriteEffects.None);
        //			Main.EntitySpriteDraw(VFX.GlowLineCap, (startVertex.vertexPos) - Main.screenPosition, null, startVertex.vertexColor, rotation + MathF.PI, new Vector2(64), capScale, SpriteEffects.None);
        //			Main.EntitySpriteDraw(VFX.GlowLineCap, (endVertex.vertexPos) - Main.screenPosition, null, startVertex.vertexColor, rotation, new Vector2(64), capScale, SpriteEffects.None);
        //                  if (drawWhiteInside)
        //                  {
        //				texScale.X *= 0.5f;
        //				capScale *= 0.5f;
        //				//account for the y multiplication with offsets
        //				Vector2 posOffset = Direction * width * 0.5f;
        //				Main.EntitySpriteDraw(VFX.GlowLine, (startVertex.vertexPos) - Main.screenPosition, null, startVertex.vertexColor, rotation, new Vector2(0, 128), texScale, SpriteEffects.None);
        //				Main.EntitySpriteDraw(VFX.GlowLineCap, (startVertex.vertexPos) - Main.screenPosition + posOffset, null, startVertex.vertexColor, rotation + MathF.PI, new Vector2(64), capScale, SpriteEffects.None);
        //				Main.EntitySpriteDraw(VFX.GlowLineCap, (endVertex.vertexPos) - Main.screenPosition - posOffset, null, startVertex.vertexColor, rotation, new Vector2(64), capScale, SpriteEffects.None);

        //			}
        //		}
        //	}
        //	struct LightningVertex
        //	{
        //		internal LightningVertex(Color color, Vector2 pos)
        //		{
        //			vertexColor = color;
        //			vertexPos = pos;
        //		}
        //		internal Color vertexColor;
        //		internal Vector2 vertexPos;
        //	}
        //}		
    }
}