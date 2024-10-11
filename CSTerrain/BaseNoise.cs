using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;


namespace CSTerrain
{
    public class BaseNoise
    {
        public static int[] Ptable { get; set; }
        public static int Num_samples { get; set; }
        public static int octaves { get; set; }
        public static float persistance { get; set; }
        public static float scale { get; set; }
        public static float[,] moisturemap { get; set; }

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

        public float[,] Gen_Moisture()
        {
            Ptable = Permutation_Gen.Generation(Num_samples);
            float[,] moisturemap = Gen_array();
            return moisturemap;
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

        public float RidgedNoise(float val)
        {
            return 2f * (0.5f - Math.Abs(0.5f - val));
        }


        // linear interpolation between a and b, with interpolation value = t
        public static float Lerp_function(float f, float a, float b) => a + f * (b - a);//interpolation

        public static Bitmap Gen_bitmap2(float[,] noisemap, float[,] moisturemap)
        {
            int size = noisemap.GetLength(0);
            Bitmap b = new Bitmap(size, size);

            //lock the bitmap into memory
            BitmapData bmpd = b.LockBits(new Rectangle(0, 0, size, size), ImageLockMode.ReadWrite, b.PixelFormat);
            //gets a pointer to point to the memory location ofthe data
            IntPtr pointer = bmpd.Scan0;

            int bytes = bmpd.Stride * size;//bits per row * height(size)
            byte[] values = new byte[bytes]; //create an array for the pixel data

            //copy values of the bitmap data into the array values
            Marshal.Copy(pointer, values, 0, bytes);

            Parallel.For(0, size, i =>
            {
                for (int j = 0; j < size; j++)
                {
                    float val = noisemap[i, j];
                    Color c = TerrainCmap.Interpolate_value(val, moisturemap[i, j]);//calculate colour

                    int position = (j * bmpd.Stride) + (i * 4);//rows * bits per row + columns * bits per pixel 


                    values[position] = c.B;
                    values[position + 1] = c.G;
                    values[position + 2] = c.R;
                    values[position + 3] = c.A;
                }
            });
            //copy the modified pixels back into the bitmap
            Marshal.Copy(values, 0, pointer, bytes);

            //unlock(update) the bitymap
            b.UnlockBits(bmpd);

            return b;
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

        public static float[,] IslandShaper(float[,] noisemap, float t)
        {
            int size = noisemap.GetLength(0);
            float[,] newMap = new float[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float x = 2f * i / size - 1f;
                    float y = 2f * j / size - 1f;

                    float distance = 1.5f - (1f - x * x) * (1f - y * y);

                    newMap[i, j] = Lerp_function(t, noisemap[i, j], 1 - distance);
                }
            }

            return newMap;
        }

        public static int fastfloor(float x) => (x >= 0) ? (int)x : (int)(x - 1);
    }

    //Generates the permutation table for Perlin noise class
    public class Permutation_Gen
    {
        //shuffles the permutatiojn table using a fisher yates shuffle
        public static int[] Shuffle_table(int size, int[] table)
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