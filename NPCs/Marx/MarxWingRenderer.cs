using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
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
            new(0,255,0),//Color.Green is (0,128,0) for some reason
            Color.Blue,
            Color.Magenta,
            Color.Cyan,
            Color.Yellow,
        };
        public static int FrameCount => 32;
        MarxWingCell[] wingCellsBack;
        List<int> freeOffsetIndicesBack;
        static readonly Vector2[][] drawOffsets = new Vector2[][]
 {
    [// Frame 1
        new(65, 60), new(254, 60), new(55, 77), new(75, 77), new(95, 77), new(224, 77), new(244, 77), new(264, 77), new(65, 94), new(85, 94), new(105, 94), new(125, 94), new(194, 94), new(214, 94), new(234, 94), new(254, 94), new(95, 111), new(115, 111), new(135, 111), new(184, 111), new(204, 111), new(224, 111), new(105, 128), new(125, 128), new(194, 128), new(214, 128)
    ],
    [// Frame 2
        new(65, 60), new(254, 60), new(55, 77), new(75, 77), new(95, 77), new(224, 77), new(244, 77), new(264, 77), new(65, 94), new(85, 94), new(105, 94), new(125, 94), new(194, 94), new(214, 94), new(234, 94), new(254, 94), new(95, 111), new(115, 111), new(135, 111), new(184, 111), new(204, 111), new(224, 111), new(105, 128), new(125, 128), new(194, 128), new(214, 128)
    ],
    [// Frame 3
        new(65, 60), new(254, 60), new(55, 77), new(75, 77), new(95, 77), new(224, 77), new(244, 77), new(264, 77), new(65, 94), new(85, 94), new(105, 94), new(125, 94), new(194, 94), new(214, 94), new(234, 94), new(254, 94), new(95, 111), new(115, 111), new(135, 111), new(184, 111), new(204, 111), new(224, 111), new(105, 128), new(125, 128), new(194, 128), new(214, 128)
    ],
    [// Frame 4
        new(65, 60), new(254, 60), new(55, 77), new(75, 77), new(95, 77), new(224, 77), new(244, 77), new(264, 77), new(65, 94), new(85, 94), new(105, 94), new(125, 94), new(194, 94), new(214, 94), new(234, 94), new(254, 94), new(95, 111), new(115, 111), new(135, 111), new(184, 111), new(204, 111), new(224, 111), new(105, 128), new(125, 128), new(194, 128), new(214, 128)
    ],
    [// Frame 5
        new(65, 60), new(254, 60), new(55, 77), new(75, 77), new(95, 77), new(224, 77), new(244, 77), new(264, 77), new(65, 94), new(85, 94), new(105, 94), new(125, 94), new(194, 94), new(214, 94), new(234, 94), new(254, 94), new(95, 111), new(115, 111), new(135, 111), new(184, 111), new(204, 111), new(224, 111), new(105, 128), new(125, 128), new(194, 128), new(214, 128)
    ],
    [// Frame 6
        new(65, 60), new(254, 60), new(55, 77), new(75, 77), new(95, 77), new(224, 77), new(244, 77), new(264, 77), new(65, 94), new(85, 94), new(105, 94), new(125, 94), new(194, 94), new(214, 94), new(234, 94), new(254, 94), new(95, 111), new(115, 111), new(135, 111), new(184, 111), new(204, 111), new(224, 111), new(105, 128), new(125, 128), new(194, 128), new(214, 128)
    ],
    [// Frame 7
        new(65, 60), new(254, 60), new(55, 77), new(75, 77), new(95, 77), new(224, 77), new(244, 77), new(264, 77), new(65, 94), new(85, 94), new(105, 94), new(125, 94), new(194, 94), new(214, 94), new(234, 94), new(254, 94), new(95, 111), new(115, 111), new(135, 111), new(184, 111), new(204, 111), new(224, 111), new(105, 128), new(125, 128), new(194, 128), new(214, 128)
    ],
    [// Frame 8
        new(65, 60), new(254, 60), new(55, 77), new(75, 77), new(95, 77), new(224, 77), new(244, 77), new(264, 77), new(65, 94), new(85, 94), new(105, 94), new(125, 94), new(194, 94), new(214, 94), new(234, 94), new(254, 94), new(95, 111), new(115, 111), new(135, 111), new(184, 111), new(204, 111), new(224, 111), new(105, 128), new(125, 128), new(194, 128), new(214, 128)
    ],
    [// Frame 9
        new(65, 60), new(254, 60), new(55, 77), new(75, 77), new(95, 77), new(224, 77), new(244, 77), new(264, 77), new(65, 94), new(85, 94), new(105, 94), new(125, 94), new(194, 94), new(214, 94), new(234, 94), new(254, 94), new(95, 111), new(115, 111), new(135, 111), new(184, 111), new(204, 111), new(224, 111), new(105, 128), new(125, 128), new(194, 128), new(214, 128)
    ],
    [// Frame 10
        new(65, 60), new(254, 60), new(55, 77), new(75, 77), new(95, 77), new(224, 77), new(244, 77), new(264, 77), new(65, 94), new(85, 94), new(105, 94), new(125, 94), new(194, 94), new(214, 94), new(234, 94), new(254, 94), new(95, 111), new(115, 111), new(135, 111), new(184, 111), new(204, 111), new(224, 111), new(105, 128), new(125, 128), new(194, 128), new(214, 128)
    ],
    [// Frame 11
        new(65, 60), new(254, 60), new(55, 77), new(75, 77), new(95, 77), new(224, 77), new(244, 77), new(264, 77), new(65, 94), new(85, 94), new(105, 94), new(125, 94), new(194, 94), new(214, 94), new(234, 94), new(254, 94), new(95, 111), new(115, 111), new(135, 111), new(184, 111), new(204, 111), new(224, 111), new(105, 128), new(125, 128), new(194, 128), new(214, 128)
    ],
    [// Frame 12
        new(65, 60), new(254, 60), new(55, 77), new(75, 77), new(95, 77), new(224, 77), new(244, 77), new(264, 77), new(65, 94), new(85, 94), new(105, 94), new(125, 94), new(194, 94), new(214, 94), new(234, 94), new(254, 94), new(95, 111), new(115, 111), new(135, 111), new(184, 111), new(204, 111), new(224, 111), new(105, 128), new(125, 128), new(194, 128), new(214, 128)
    ],
    [// Frame 13
        new(65, 60), new(254, 60), new(55, 77), new(75, 77), new(95, 77), new(224, 77), new(244, 77), new(264, 77), new(65, 94), new(85, 94), new(105, 94), new(125, 94), new(194, 94), new(214, 94), new(234, 94), new(254, 94), new(95, 111), new(115, 111), new(135, 111), new(184, 111), new(204, 111), new(224, 111), new(105, 128), new(125, 128), new(194, 128), new(214, 128)
    ],
    [// Frame 14
        new(65, 60), new(254, 60), new(55, 77), new(75, 77), new(95, 77), new(224, 77), new(244, 77), new(264, 77), new(65, 94), new(85, 94), new(105, 94), new(125, 94), new(194, 94), new(214, 94), new(234, 94), new(254, 94), new(95, 111), new(115, 111), new(135, 111), new(184, 111), new(204, 111), new(224, 111), new(105, 128), new(125, 128), new(194, 128), new(214, 128)
    ],
    [// Frame 15
        new(65, 60), new(254, 60), new(55, 77), new(75, 77), new(95, 77), new(224, 77), new(244, 77), new(264, 77), new(65, 94), new(85, 94), new(105, 94), new(125, 94), new(194, 94), new(214, 94), new(234, 94), new(254, 94), new(95, 111), new(115, 111), new(135, 111), new(184, 111), new(204, 111), new(224, 111), new(105, 128), new(125, 128), new(194, 128), new(214, 128)
    ],
    [// Frame 16
        new(90, 55), new(254, 60), new(80, 71), new(100, 71), new(120, 71), new(224, 77), new(244, 77), new(264, 77), new(90, 88), new(110, 88), new(130, 88), new(194, 94), new(214, 94), new(234, 94), new(254, 94), new(120, 105), new(140, 105), new(160, 105), new(184, 111), new(204, 111), new(224, 111), new(130, 122), new(150, 122), new(194, 128), new(214, 128)
    ],
    [// Frame 17
        new(90, 55), new(254, 60), new(80, 71), new(100, 71), new(120, 71), new(224, 77), new(244, 77), new(264, 77), new(90, 88), new(110, 88), new(130, 88), new(194, 94), new(214, 94), new(234, 94), new(254, 94), new(120, 105), new(140, 105), new(160, 105), new(184, 111), new(204, 111), new(224, 111), new(130, 122), new(150, 122), new(194, 128), new(214, 128)
    ],
    [// Frame 18
        new(229, 55), new(65, 60), new(199, 71), new(219, 71), new(239, 71), new(55, 77), new(75, 77), new(95, 77), new(189, 88), new(209, 88), new(229, 88), new(65, 94), new(85, 94), new(105, 94), new(125, 94), new(159, 105), new(179, 105), new(199, 105), new(95, 111), new(115, 111), new(135, 111), new(169, 122), new(189, 122), new(105, 128), new(125, 128)
    ],
    [// Frame 19
        new(229, 55), new(65, 60), new(199, 71), new(219, 71), new(239, 71), new(55, 77), new(75, 77), new(95, 77), new(189, 88), new(209, 88), new(229, 88), new(65, 94), new(85, 94), new(105, 94), new(125, 94), new(159, 105), new(179, 105), new(199, 105), new(95, 111), new(115, 111), new(135, 111), new(169, 122), new(189, 122), new(105, 128), new(125, 128)
    ],
    [// Frame 20
        new(88, 58), new(231, 58), new(78, 75), new(98, 75), new(118, 75), new(201, 75), new(221, 75), new(241, 75), new(88, 92), new(108, 92), new(128, 92), new(191, 92), new(211, 92), new(231, 92), new(78, 109), new(98, 109), new(118, 109), new(138, 109), new(181, 109), new(201, 109), new(221, 109), new(241, 109), new(108, 126), new(128, 126), new(191, 126), new(211, 126)
    ],
    [// Frame 21
        new(82, 37), new(102, 37), new(217, 37), new(237, 37), new(92, 54), new(112, 54), new(207, 54), new(227, 54), new(102, 71), new(122, 71), new(197, 71), new(217, 71), new(112, 88), new(132, 88), new(187, 88), new(207, 88), new(102, 105), new(122, 105), new(142, 105), new(177, 105), new(197, 105), new(217, 105), new(112, 122), new(132, 122), new(187, 122), new(207, 122)
    ],
    [// Frame 22
        new(60, 59), new(80, 59), new(239, 59), new(259, 59), new(50, 76), new(70, 76), new(90, 76), new(229, 76), new(249, 76), new(269, 76), new(80, 93), new(100, 93), new(120, 93), new(199, 93), new(219, 93), new(239, 93), new(90, 110), new(110, 110), new(130, 110), new(189, 110), new(209, 110), new(229, 110), new(100, 127), new(120, 127), new(199, 127), new(219, 127)
    ],
    [// Frame 23
        //teleport animation so no points for wings
    ],
    [// Frame 24
        //teleport animation so no points for wings
    ],
    [// Frame 25
        //teleport animation so no points for wings
    ],
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
                if (freeOffsetIndicesBack.Count > wingCellsBack.Length - 5)
                {
                    AddRandomDotted();
                }
            }

        }

        private void AddRandomDotted()
        {
            int randFreeIndexIndex = Main.rand.Next(freeOffsetIndicesBack.Count);
            if (freeOffsetIndicesBack.Count > 0)
            {
                int randFreeIndex = freeOffsetIndicesBack[randFreeIndexIndex];
                freeOffsetIndicesBack.RemoveAt(randFreeIndexIndex);
                MarxWingCell cell = wingCellsBack[randFreeIndex];
                cell.timeLeft = (sbyte)FrameIndexDitherEnd;
                cell.type = CellIDDotted;
                cell.color = (byte)Main.rand.Next(Palette.Length);
            }
        }

        public MarxWingRenderer()
        {
            int count = drawOffsets[0].Length;
            wingCellsBack = new MarxWingCell[count];
            for (int i = 0; i < count; i++)
            {
                wingCellsBack[i] = new((byte)i);
            }
            freeOffsetIndicesBack = new(count);
            for (int i = 0; i < count; i++)
            {
                freeOffsetIndicesBack.Add(i);
            }
        }

        public void RenderFrame(int frameIndex, SpriteBatch sb, Vector2 marxCenter, Vector2 screenPos, float rotation, float scale, int marxSheetWidth = 640, Vector2 globalOffset = default, Vector2 offsetL = default, Vector2 offsetR = default)
        {
            RenderCellList(wingCellsBack, frameIndex, sb, marxCenter, screenPos, rotation, scale, marxSheetWidth, globalOffset, offsetL, offsetR);
            //RenderCellList(wingCellsFront, frameIndex, sb, marxCenter, screenPos, rotation, scale);
            if(frameIndex >= drawOffsets.Length)
            {
                return;
            }
            Vector2[] offsets = drawOffsets[frameIndex];
            List<Vector2> randomlyChosenOffsets = [];
            List<Vector2> remainingAVailable = [.. offsets];
            int amount = (int)(offsets.Length * 0.5f);
            for (int i = 0; i < amount; i++)
            {
                int randIndex = Main.rand.Next(remainingAVailable.Count);
                randomlyChosenOffsets.Add(remainingAVailable[randIndex]);
                remainingAVailable.RemoveAt(randIndex);
            }
            offsets = [.. randomlyChosenOffsets];
            int halfwayPoint = marxSheetWidth / 2;
            for (int i = 0; i < offsets.Length; i++)
            {
                Rectangle frame = new(0, 0, MarxWingCell.FrameWidth, MarxWingCell.FrameHeight);
                Vector2 drawOffset = offsets[i] * 2f;
                if (drawOffset.X < halfwayPoint)
                {
                    drawOffset += offsetL;
                }
                else
                {
                    drawOffset += offsetR;
                }
                drawOffset += globalOffset;
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
        static void RenderCellList(MarxWingCell[] cells, int frameIndex, SpriteBatch sb, Vector2 marxCenter, Vector2 screenPos, float rotation, float scale, int marxSheetWidth = 640, Vector2 globalOffset = default, Vector2 offsetL = default, Vector2 offsetR = default)
        {
            int halfwayPoint = marxSheetWidth / 2;

            for (int i = 0; i < cells.Length; i++)
            {
                MarxWingCell cell = cells[i];
                if (cell.Inactive)
                {
                    continue;
                }
                if(frameIndex >= drawOffsets.Length)
                {
                    continue;
                }
                Vector2[] currentFrameDrawOffsets = drawOffsets[frameIndex];
                if(cell.indexInDrawOffsetArray >= currentFrameDrawOffsets.Length)
                {
                    continue;
                }
                Vector2 drawOffset = currentFrameDrawOffsets[cell.indexInDrawOffsetArray] * 2f;

                if (drawOffset.X < halfwayPoint)
                {
                    drawOffset += offsetL;
                }
                else
                {
                    drawOffset += offsetR;
                }
                drawOffset += globalOffset;

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
