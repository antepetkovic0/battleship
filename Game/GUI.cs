using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Game
{
    static class GUI
    {
        private static readonly Bitmap hitImage = new Bitmap(Properties.Resources.hit);
        private static readonly Bitmap missImage = new Bitmap(Properties.Resources.miss);
        public static readonly Bitmap gridImage;

        static GUI()
        {
            gridImage = new Bitmap(Properties.Resources.grid0);
        }

        public static readonly Brush[] colors = new SolidBrush[6]
        {
            new SolidBrush(Color.Yellow),
            new SolidBrush(Color.Red),
            new SolidBrush(Color.Green),
            new SolidBrush(Color.Blue),
            new SolidBrush(Color.DarkViolet),
            new SolidBrush(Color.White), //opponent move
        };

        public static int Coord(PictureBox pictureBox, int coord)
        {
            //grid borders
            if (coord < 33 || coord > 342)
                return -1;
            else
                return coord;
        }

        public static int GetCell(int coord)
        {
            return (coord - 33) / 31;
        }

        //drawing ships inner frame cell on mouse move
        public static void DrawInnerFrameCell(int cellX, int cellY, int color, Form form, PictureBox pictureBox)
        {
            Graphics g = pictureBox.CreateGraphics();
            Pen framePen = new Pen(colors[color]);
            g.DrawRectangle(framePen, (cellX + 1) * 31 + 3, (cellY + 1) * 31 + 3, 25, 25);
        }

        //drawing ship cells on mouse click
        public static void DrawColoredCell(int cellX, int cellY, int color, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(colors[color], (cellX + 1) * 31 + 1, (cellY + 1) * 31 + 1, 30, 30);
        }

        //draw player shipset
        static public void DrawShipSet(int[,] shipSet, PaintEventArgs e)
        {
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    if (shipSet[x, y] != -1)
                    {
                        DrawColoredCell(x, y, shipSet[x,y], e);
                    }
                }
            }
        }

        //drawing for miss
        static public void DrawHitCell(int cellX, int cellY, PaintEventArgs e)
        {
            e.Graphics.DrawImage(hitImage, (cellX + 1) * 31 + 1, (cellY + 1) * 31 + 1);
        }

        //drawing for hit
        static public void DrawMissCell(int cellX, int cellY, PaintEventArgs e)
        {
            e.Graphics.DrawImage(missImage, (cellX + 1) * 31 + 1, (cellY + 1) * 31 + 1);
        }

        //drawing our shots on opponent
        static public void DrawOpponentBoard(bool[,] isHit, int[,] opponentBoard, PaintEventArgs e)
        {
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    if (isHit[x, y])
                    {
                        if (opponentBoard[x, y] != -1)
                        {
                            DrawHitCell(x, y, e);
                        }
                        else
                        {
                            DrawMissCell(x, y, e);
                        }
                    }
                }
            }
        }
    }
}
