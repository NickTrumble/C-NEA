using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSTerrain
{
    public class SimplexNoise : BaseNoise
    {
        public float unskew = (3 - (float)Math.Sqrt(3)) / 6;// converts skewed coords to normal
        public float skew = 0.5f * (float)(Math.Sqrt(3) - 1);
        public SimplexNoise(int size, int octave, float pers, float scal) : base(size, octave, pers, scal) { }

        public override float Single_octave(float xin, float yin)
        {
            float skew_factor = (xin + yin) * skew;
            int i = fastfloor(xin + skew_factor);
            int j = fastfloor(yin + skew_factor);

            float unskew_factor = (i + j) * unskew;
            float x = xin - (i - unskew_factor);
            float y = yin - (j - unskew_factor);

            int ioffset, joffset;
            // square cut diagonaly, bl to tr 
            if (x > y)// if closer to the x axis, in the bottom triangle else in the top treiangle
            {
                //if in bottom, next corner is br, then tr
                ioffset = 1;
                joffset = 0;
            }
            else
            {
                //if in top, next corner is tl, then tr
                ioffset = 0;
                joffset = 1;
            }

            int ii = i & 255;
            int jj = j & 255;

            //all values are unskewed, and 2 * unskew represents unskewing 2 units across

            float middlex = x - ioffset + unskew; // move from xdis to xdis + ioffset, moving to the next corner
            float middley = y - joffset + unskew; // move from ydis to ydis + joffset, moving to the next corner
            float finalx = x - 1.0f + 2.0f * unskew; // mover from xdis to xdis + 1, moving to the final corner
            float finaly = y - 1.0f + 2.0f * unskew; // mover from ydis to ydis + 1, moving to the final corner

            //calculate contributiuons from each corner
            float n1 = Corner_contribution(x, y, ii + Ptable[jj]);
            float n2 = Corner_contribution(middlex, middley, ii + ioffset + Ptable[jj + joffset]);
            float n3 = Corner_contribution(finalx, finaly, ii + 1 + Ptable[jj + 1]);

            return (n1 + n2 + n3) * 40f;//return total cointribution
        }

        public float Corner_contribution(float x, float y, int index)
        {
            float t = 0.5f - x * x - y * y;// radius of influence from center point of triangle - distance to point (x^2 + y^2)
            if (t < 0f)
            {
                return 0f;//if distance oiut of radius for first corner, contribution = 0
            }
            else
            {
                t *= t;//apply a smooth fall off
                return t * t * Gradient_calc(Ptable[index], x, y);
                // return distance to center ^4 * random gradient
            }
        }


    }
}