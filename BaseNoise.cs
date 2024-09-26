using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CSTerrain
{
    public class BaseNoise
    {
        public static int[] Ptable { get; set; }
        public static int Num_samples { get; set; }
        public static int octaves { get; set; }
        public static float persistance { get; set; }
        public static float scale { get; set; }

        //array for gradiant vectors
        public static float[][] gradients =
        {
            new float[] { 1, 1 }, new float[] { -1, 1 }, new float[] { 1, -1 }, new float[] { -1, -1 },
            new float[] { 1, 0 }, new float[] { -1, 0 }, new float[] { 0, 1 }, new float[] { 0, -1 }
        };

        public BaseNoise(int size, int octave, float pers, float scal)//constructor
        {
            Ptable = Permutation_Gen.Generation(size);
            Num_samples = size;
            octaves = octave;
            persistance = pers;
            scale = scal;
        }

        //Dot product between gradiant vector g and position vector (x, y)
        public static float Dot_product(float[] g, float x, float y) => g[0] * x + g[1] * y;

        //generates a gradiant using permutation table and gradianbts
        public static float Gradient_calc(float corner, float x, float y) => Dot_product(gradients[(int)corner % 8], x, y);

        //combines multiple octaves of perlin noise to generate the nosiemap
        public float Noise_method(float x, float y)
        {
            float amplitude = 1;
            float frequency = 1;
            float noise = 0;
            float max = 0;
            for (int i = 0; i < octaves; i++)
            {
                noise += Single_octave(x * frequency, y * frequency) * amplitude;
                max += amplitude;
                amplitude *= persistance;
                frequency *= 2f;
            }
            return noise / max;

        }

        public virtual float Single_octave(float x, float y) { return 0f; }

        public float[,] Gen_array()
        {
            int size = Num_samples;
            float[,] noisemap = new float[size, size];

            Parallel.For(0, size, i =>
            {
                for (int j = 0; j < size; j++)
                {
                    float x = i * scale;
                    float y = j * scale;
                    noisemap[i, j] = (Noise_method(x, y) + 1) / 2;
                }
            });
            return noisemap;
        }

        //generatesd the array in bitmap form
        public Bitmap Gen_bitmap(float[,] noisemap)
        {
            int size = Num_samples;
            Bitmap noise_bitmap = new Bitmap(size, size);

            Parallel.For(0, size, i =>
            {
                for (int j = 0; j < size; j++)
                {
                    Color c = TerrainCmap.Interpolate_value(noisemap[i, j]);
                    lock (noise_bitmap)
                    {
                        noise_bitmap.SetPixel(i, j, c);
                    }
                }
            });
            return noise_bitmap;
        }

        //normaklises the whole noisemap
        public static float[,] Normalise(float[,] noisemap)
        {
            int size = noisemap.GetLength(0);
            float min = 1;
            float max = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (noisemap[i, j] < min)
                    {
                        min = noisemap[i, j];
                    }
                    else if (noisemap[i, j] > max)
                    {
                        max = noisemap[i, j];
                    }
                }
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    noisemap[i, j] = (noisemap[i, j] - min) / (max - min);
                }
            }
            return noisemap;
        }

        public static int fastfloor(float x) => (x >= 0) ? (int)x : (int)(x - 1);
    }

    //Generates the permutation table for Perlin noise class
    public class Permutation_Gen
    {
        //shuffles the permutatiojn table using a fisher yates shuffle
        static int[] Shuffle_table(int size, int[] table)
        {
            Random rnd = new Random(Environment.TickCount ^ DateTime.Now.Millisecond);

            for (int i = 0; i < size; i++)
            {
                int a = rnd.Next(size);
                int noise_bitmap = rnd.Next(size);

                int t = table[a];
                table[a] = table[noise_bitmap];
                table[noise_bitmap] = t;
            }
            return table;//swap random indicies of table with other randoms
        }

        //fills half an array with values 1 - size and then duplicates to the second hjalf
        public static int[] Generation(int size)
        {
            int[] table = new int[size * 2];
            for (int i = 0; i < size; i++)
            {
                table[i] = i;
                table[size + i] = i;
            } //initialise wiht values 1 to size - 1


            //return shuffled table
            return Shuffle_table(size, table);
        }
    }
}
