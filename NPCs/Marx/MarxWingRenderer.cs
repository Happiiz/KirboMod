using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.NPCs.Marx
{
    public class MarxWingRenderer
    {
        public const byte FrameIndexDefault = 0;
        public const byte FrameIndexReflectionsStart = 1;
        public const byte FrameIndexReflectionsEnd = 4;
        public const byte FrameIndexDitherStart = 5;
        public const byte FrameIndexDitherEnd = 31;
        public const sbyte CellIDFullbright = 0;
        public const sbyte CellIDReflective = 1;
        public const sbyte CellIDDotted = 2;
        static Asset<Texture2D> wingCellSheet;
        //CHANGE THIS TO STATIC READONLY ONCE EVERYTHING IS FINISHED
        public static readonly Color[] Palette = new Color[]
        {
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Magenta,
            Color.Cyan,
            Color.Yellow,
        };
        public static int FrameCount => 32;
        MarxWingCell[] wingCellsFront;
        MarxWingCell[] wingCellsBack;
        List<int> freeOffsetIndicesBack;
        List<int> freeOffsetIndicesFront;
        static Vector2[][] drawOffsets = new Vector2[][]
        {
        [
           new(170,70), new(150,104), new(110,104), new(190,104),
           new(130, 138), new(170,138), new(90,138), new(50,138),
                new(30,172), new(70,172), new Vector2(110,172),
                new(50,206), new Vector2(90,206)
        ]
        };
        public void Update()
        {

            for (int i = 0; i < wingCellsBack.Length; i++)
            {
                MarxWingCell cell = wingCellsBack[i];
                if (cell.Inactive)
                {
                    continue;
                }
                cell.timeLeft--;
                //is just made inactive
                if (cell.Inactive)
                {

                    freeOffsetIndicesBack.Add(i);
                }
            }
            int time = (int)(Main.timeForVisualEffects % 10000);
            int duration = FrameIndexDitherEnd - FrameIndexDitherStart;
            if (time % duration == 0)
            {
                AddRandomDotted();
            }
            if (Main.rand.NextBool(duration / 2))
            {
                if(freeOffsetIndicesBack.Count > wingCellsBack.Length - 5)
                {
                    AddRandomDotted();
                }
            }
           
        }

        private void AddRandomDotted()
        {
            int randFreeIndexIndex = Main.rand.Next(freeOffsetIndicesBack.Count);
            int randFreeIndex = freeOffsetIndicesBack[randFreeIndexIndex];
            freeOffsetIndicesBack.RemoveAt(randFreeIndexIndex);
            MarxWingCell cell = wingCellsBack[randFreeIndex];
            cell.timeLeft = (sbyte)FrameIndexDitherEnd;
            cell.type = CellIDDotted;
            cell.color = (byte)Main.rand.Next(Palette.Length);
        }

        public MarxWingRenderer()
        {
            int count = drawOffsets[0].Length;
            wingCellsBack = new MarxWingCell[count];
            wingCellsFront = new MarxWingCell[count];
            for (int i = 0; i < count; i++)
            {
                wingCellsBack[i] = new((byte)i);
                wingCellsFront[i] = new((byte)i);
            }
            freeOffsetIndicesBack = new(count);
            freeOffsetIndicesFront = new(count);
            for (int i = 0; i < count; i++)
            {
                freeOffsetIndicesBack.Add(i);
                freeOffsetIndicesFront.Add(i);
            }
        }

        public void RenderFrame(int frameIndex, SpriteBatch sb, Vector2 marxCenter, Vector2 screenPos, float rotation, float scale)
        {
            RenderCellList(wingCellsBack, frameIndex, sb, marxCenter, screenPos, rotation, scale);
            //RenderCellList(wingCellsFront, frameIndex, sb, marxCenter, screenPos, rotation, scale);
            Vector2[] offsets = drawOffsets[frameIndex];
            List<Vector2> randomlyChosenOffsets = new();
            List<Vector2> remainingAVailable = offsets.ToList();
            int amount = (int)(offsets.Length * 0.5f);
            for (int i = 0; i < amount; i++)
            {
                int randIndex = Main.rand.Next(remainingAVailable.Count);
                randomlyChosenOffsets.Add(remainingAVailable[randIndex]);
                remainingAVailable.RemoveAt(randIndex);
            }
            offsets = randomlyChosenOffsets.ToArray();
            for (int i = 0; i < offsets.Length; i++)
            {
                Rectangle frame = new(0, 0, MarxWingCell.FrameWidth, MarxWingCell.FrameHeight);
                Vector2 drawOffset = offsets[i];
                drawOffset = drawOffset.RotatedBy(rotation) * scale;
                Vector2 drawPos = marxCenter + drawOffset;
                sb.Draw(wingCellSheet.Value, drawPos - screenPos, frame, Palette[Main.rand.Next(Palette.Length)], rotation, frame.Size() / 2, scale * 2f, SpriteEffects.None, 0);

                //adding reflection
                if (Main.rand.NextBool(4))
                {
                    Rectangle reflectionFrame = wingCellSheet.Frame(1, FrameCount, 0, Main.rand.Next(FrameIndexReflectionsStart, FrameIndexReflectionsEnd + 1));
                    sb.Draw(wingCellSheet.Value, drawPos - screenPos, reflectionFrame, Color.White, rotation, reflectionFrame.Size() / 2, scale * 2f, Main.rand.NextBool() ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                }
            }
        }
        static void RenderCellList(MarxWingCell[] cells, int frameIndex, SpriteBatch sb, Vector2 marxCenter, Vector2 screenPos, float rotation, float scale)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                MarxWingCell cell = cells[i];
                if (cell.Inactive)
                {
                    continue;
                }
                Vector2 drawOffset = drawOffsets[frameIndex][cell.indexInDrawOffsetArray];
                drawOffset = drawOffset.RotatedBy(rotation) * scale;
                Vector2 drawPos = marxCenter + drawOffset;
                Rectangle frame = cell.GetFrame();
                sb.Draw(wingCellSheet.Value, drawPos - screenPos, frame, cell.GetColor(), rotation, frame.Size() / 2, scale * 2f, SpriteEffects.None, 0);
                if (cell.type == CellIDReflective)
                {
                    Rectangle reflectionFrame = wingCellSheet.Frame(1, FrameCount, 0, Main.rand.Next(FrameIndexReflectionsStart, FrameIndexReflectionsEnd + 1));

                    sb.Draw(wingCellSheet.Value, drawPos - screenPos, reflectionFrame, Color.White, rotation, reflectionFrame.Size() / 2, scale * 2f, SpriteEffects.None, 0);
                }
            }
        }

        public static void Initialize()
        {
            wingCellSheet = ModContent.Request<Texture2D>("KirboMod/NPCs/Marx/MarxWingCell");

        }
        //make it last a random amount from 1 to 4 frames
        //weighted towards 1 using a sqr in easing function
        //make cells be able to be replaced when they're about to expire??
        //then there's also the dither effect cells which may be another layer???
        //so will have to make 2 sets, one for regular flashing and the other for dithered
        private class MarxWingCell
        {
            public const int FrameHeight = 23;
            public const int FrameWidth = 21;
            public byte indexInDrawOffsetArray;
            //fullbright + reflection stripe, fullbright, or dithered
            public sbyte type;
            public sbyte timeLeft;
            //use this to index Palette
            //change palette to static readonly field once everything is finished
            public byte color;
            public bool Inactive => timeLeft <= 0;
            public MarxWingCell(byte indexInDrawOffsetArray)
            {
                timeLeft = -1;
                type = 0;
                this.indexInDrawOffsetArray = indexInDrawOffsetArray;
            }

            public Rectangle GetFrame()
            {

                if (type == CellIDDotted)
                {
                    int frameIndex = FrameIndexDitherStart + timeLeft;
                    if (frameIndex > 32)
                    {
                        frameIndex = 32;
                    }
                    return wingCellSheet.Frame(1, FrameCount, 0, frameIndex);
                }
                return new Rectangle(0, 0, FrameWidth, FrameHeight);

            }
            public Color GetColor()
            {
                Color c = Palette[color];
                Vector3 hsl = Main.rgbToHsl(c);
                hsl.Y = .4f;
                hsl.Z -= 0.1f;
                c = Main.hslToRgb(hsl);
                return c;
            }
        }
    }
}
