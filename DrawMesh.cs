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
        public float[,] heightmap { get; set; }
        public int xoffset { get; set; }
        public int yoffset { get; set; }
        public int tilewidth { get; set; }
        public int tileheight { get; set; }
        public int scale { get; set; }
        public DrawMesh(float[,] inTerrain, int xo, int yo, int s)
        {
            heightmap = inTerrain;
            xoffset = xo;
            yoffset = yo;
            tilewidth = 1000 / heightmap.GetLength(0);
            tileheight = 800 / heightmap.GetLength(1);
            scale = s;
        }

        public Point PointCalc(int x, int y, float z)
        {
            int X = xoffset + (x - y) * tilewidth / 2;
            int Y = (int)(yoffset + (x + y) * tileheight / 2 - z * scale);
            return new Point(X, Y);
        }

        public Color GetColour(int elevation)
        {
            int intensity = Math.Min(127, elevation * 13);
            return Color.FromArgb(2 * intensity, 2 * intensity, 128 + intensity);
        }

        public void Draw(Graphics g)
        {
            Color[] Colours = new Color[50];
            
            for (int i = 0; i < 50; i++)
            {
                float a = i / 50f;
                Colours[i] = TerrainCmap.Interpolate_value(a);
            }
            int size = heightmap.GetLength(0);

            for (int i = 0; i < size - 1; i++)
            {
                for (int j = 0; j < size - 1; j++)
                {
                    Point[] corners1 = new Point[3];
                    corners1[0] = PointCalc(i, j, heightmap[i, j]);
                    corners1[1] = PointCalc(i, j + 1, heightmap[i, j + 1]);
                    corners1[2] = PointCalc(i + 1, j, heightmap[i + 1, j]);

                    float avgh = (heightmap[i, j] + heightmap[i + 1, j] + heightmap[i, j + 1]) / 3;
                    Brush b = new SolidBrush(Colours[(int)(50 * avgh)]);

                    g.FillPolygon(b, corners1);

                    Point[] corners2 = new Point[3];
                    corners2[0] = PointCalc(i + 1, j + 1, heightmap[i + 1, j + 1]);
                    corners2[1] = PointCalc(i, j + 1, heightmap[i, j + 1]);
                    corners2[2] = PointCalc(i + 1, j, heightmap[i + 1, j]);

                    float avgh2 = (heightmap[i, j] + heightmap[i + 1, j] + heightmap[i, j + 1]) / 3;
                    Brush b2 = new SolidBrush(Colours[(int)(50 * avgh2)]);

                    g.FillPolygon(b2, corners2);
                }
            }            
        }
    }
}
