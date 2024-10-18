using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSTerrain
{
    //generates a colourmap on the program
    public class TerrainCmap
    {
        //list of base collours used in the cmap
        public static Color[] cmapC =
        {
            Color.FromArgb(29, 116, 214), Color.FromArgb(45, 209, 111),
            Color.FromArgb(241, 252, 156), Color.FromArgb(128, 93, 87),
            Color.White
        };

        public static Color GetBiomeC(int i, int j)
        {
            Color[] colours =
            {
                //forest, rainforest, desert, tundra, savanah, arcitc, taiga, grass, mediterainian
                Color.FromArgb(34, 139, 34), Color.FromArgb(0, 198, 34),
                Color.FromArgb(255, 202, 113), Color.FromArgb(114,140,116),
                Color.FromArgb(219, 113, 7), Color.FromArgb(209, 230, 230),
                Color.FromArgb(0, 100, 0), Color.FromArgb(124, 252, 0),
                Color.FromArgb(254, 205, 50)
            };
            //temp  moist
            //0.75  0.75    rainforest
            //0.7   0.2     desert
            //0.6   0.3 0.4 medit   savanah
            //0.5   0.4 0.5 grass   temp forest
            //0.4   0.3     taiga
            //0.2   0.3     tundra
            //0.1   0.2     arctic

            
            float moist = Manager.moisturemap[i, j];
            float hieght = Manager.noisemap[i, j];
            float temp = Manager.tempmap[i, j];
            float abs = moist * moist + temp * temp;
            if (moist >= 0.75 && temp >= 0.75)
            {
                // rainforest
                float t = (abs - 0.75f) / 0.25f;
            }
            else if (temp >= 0.7 && moist <= 0.2)
            {
                // deset
                float t = (hieght - 0.7f) / 0.05f;
            }
            else if (temp >= 0.6 && moist <= 0.3)
            {
                if (moist <= 0.3)
                {
                    // medit
                    float t = (hieght - 0.6f) / 0.1f;
                } else if (moist <= 0.4)
                {
                    //savanah
                    float t = (hieght - 0.6f) / 0.1f;
                }
            }
            else if (temp >= 0.5 && moist <= 0.4)
            {
                if (moist <= 0.4)
                {
                    // grass
                    float t = (hieght - 0.1f) / 0.2f;
                } else if (moist <= 0.5)
                {
                    // temp forest
                    float t = (hieght - 0.1f) / 0.2f;
                }
            }
            else if (temp <= 0.1 && moist <= 0.2)
            {
                // arctic
                float t = (abs) / 0.1f;
            }
            else if (temp <= 0.2 && moist <= 0.3)
            {
                // tundra
                float t = (abs - 0.1f) / 0.1f;
            }
            else if (temp <= 0.4 && moist >= 0.3)
            {
                // taiga
                float t = (abs - 0.2f) / 0.2f;
            }
        }

        //interpolates between base colours in the cmap depending on the noise value
        public static Color Interpolate_value(int i, int j)
        {
            float noise_value = Manager.noisemap[i, j];
            if (noise_value < 0.1) // waterd
            {
                return cmapC[0];
            }
            else if (noise_value < 0.3) // sansd
            {
                float t = (noise_value - 0.1f) / 0.2f;
                return Colour_lerp(cmapC[0], cmapC[1], t);
            }
            else if (noise_value < 0.5) // grass
            {
                float t = (noise_value - 0.3f) / 0.2f;
                return Colour_lerp(cmapC[1], cmapC[2], t);
            }
            else if (noise_value < 0.7) // rock
            {
                float t = (noise_value - 0.5f) / 0.2f;
                return Colour_lerp(cmapC[2], cmapC[3], t);
            }
            else // snow
            {
                float t = (noise_value - 0.7f) / 0.3f;
                return Colour_lerp(cmapC[3], cmapC[4], t);
            }



            if (noise_value < 0.1) // waterd
            {
                return cmapC[0];
            }
            else if (noise_value < 0.3) // sansd
            {
                float t = (noise_value - 0.1f) / 0.2f;
                return Colour_lerp(cmapC[0], cmapC[1], t);
            }
            else if (noise_value < 0.5) // grass
            {
                float t = (noise_value - 0.3f) / 0.2f;
                return Colour_lerp(cmapC[1], cmapC[2], t);
            }
            else if (noise_value < 0.7) // rock
            {
                float t = (noise_value - 0.5f) / 0.2f;
                return Colour_lerp(cmapC[2], cmapC[3], t);
            }
            else // snow
            {
                float t = (noise_value - 0.7f) / 0.3f;
                return Colour_lerp(cmapC[3], cmapC[4], t);
            }
        }

        // linear interpolation betweeb two colours
        public static Color Colour_lerp(Color c1, Color c2, float interpolation_value)
        {
            interpolation_value = Math.Max(Math.Min(interpolation_value, 1), 0);

            int r = (int)((1 - interpolation_value) * c1.R + interpolation_value * c2.R);
            int g = (int)((1 - interpolation_value) * c1.G + interpolation_value * c2.G);
            int b = (int)((1 - interpolation_value) * c1.B + interpolation_value * c2.B);

            return Color.FromArgb(r, g, b);
        }
    }
}
