using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSTerrain
{
    //class to generate the perlin noise used to make the noisemap
    public class PerlinNoise : BaseNoise
    {

        //constructor taking noiise parameters
        public PerlinNoise(int size, int octaves, float pers, float scal) : base(size, octaves, pers, scal) { }

        //smoothens transitions between gradient vectors
        static float Fade_function(float f) => f * f * f * (f * (f * 6 - 15) + 10);//blend


        //generates a single octave of the perlin noise to be combined into multiple
        public override float Single_octave(float x, float y)
        {
            int xint = (int)fastfloor(x) % 256;
            int yint = (int)fastfloor(y) % 256;

            float xfloat = x - xint;
            float yfloat = y - yint;

            float i = Fade_function(xfloat);
            float j = Fade_function(yfloat);

            int tl = Ptable[Ptable[xint] + yint];
            int tr = Ptable[Ptable[xint + 1] + yint];
            int bl = Ptable[Ptable[xint] + yint + 1];
            int br = Ptable[Ptable[xint + 1] + yint + 1];

            float x1 = Lerp_function(i, Gradient_calc(tl, xfloat, yfloat), Gradient_calc(tr, xfloat - 1, yfloat));
            float x2 = Lerp_function(i, Gradient_calc(bl, xfloat, yfloat - 1), Gradient_calc(br, xfloat - 1, yfloat - 1));

            return Lerp_function(j, x1, x2);
        }

    }
}