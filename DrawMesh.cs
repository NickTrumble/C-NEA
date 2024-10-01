using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CSTerrain
{
    class DrawMesh
    {
        public int[,] heightmap { get; set; }
        public int xoffset { get; set; }
        public int yoffset { get; set; }
        public int tilewidth { get; set; }
        public int tileheight { get; set; }
        public int scale { get; set; }
        public DrawMesh(int[,] inTerrain, int xo, int yo, int s)
        {
            heightmap = inTerrain;
            xoffset = xo;
            yoffset = yo;
            tilewidth = 1000 / heightmap.GetLength(0);
            tileheight = 800 / heightmap.GetLength(1);
            scale = s;
        }

        public Point PointCalc(int x, int y, int z)
        {
            int X = xoffset + (x - y) * tilewidth / 2;
            int Y = yoffset + (x + y) * tileheight / 2 - z * scale;
            return new Point(X, Y);
        }

        public Color GetColour(int elevation)
        {
            int intensity = Math.Min(127, elevation * 13);
            return Color.FromArgb(128 + intensity, 2 * intensity, 128 + intensity);
        }

        public void Draw(Graphics g)
        {
            Color[] Colours = new Color[12]
            {
                GetColour(0), GetColour(1), GetColour(2), GetColour(3), GetColour(4), GetColour(5),
                GetColour(6), GetColour(7), GetColour(8), GetColour(9), GetColour(9), GetColour(10)
            };
            int size = heightmap.GetLength(0);

            for (int i = 0; i < size - 1; i++)
            {
                for (int j = 0; j < size - 1; j++)
                {
                    Point[] corners1 = new Point[3];
                    corners1[0] = PointCalc(i, j, heightmap[i, j]);
                    corners1[1] = PointCalc(i, j + 1, heightmap[i, j + 1]);
                    corners1[2] = PointCalc(i + 1, j, heightmap[i + 1, j]);

                    int avgh = (heightmap[i, j] + heightmap[i + 1, j] + heightmap[i, j + 1]) / 3;
                    Brush b = new SolidBrush(Colours[avgh + 1]);

                    g.FillPolygon(b, corners1);

                    Point[] corners2 = new Point[3];
                    corners2[0] = PointCalc(i + 1, j + 1, heightmap[i + 1, j + 1]);
                    corners2[1] = PointCalc(i, j + 1, heightmap[i, j + 1]);
                    corners2[2] = PointCalc(i + 1, j, heightmap[i + 1, j]);

                    int avgh2 = (heightmap[i, j] + heightmap[i + 1, j] + heightmap[i, j + 1]) / 3;
                    Brush b2 = new SolidBrush(Colours[avgh2 + 1]);

                    g.FillPolygon(b2, corners2);
                }
            }
        }
    }
}
