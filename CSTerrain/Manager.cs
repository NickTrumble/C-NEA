using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace CSTerrain
{
    class Manager
    {
        public static float[,] noisemap { get; set; }
        public static float[,] tempmap { get; set; }
        public static float[,] moisturemap { get; set; }
        public static Bitmap noiseBitmap { get; set; }
        public static Stack<float[,]> UndoStack { get; set; }
        public static Stack<float[,]> RedoStack { get; set; }
        public Manager(int size)
        {
            noisemap = new float[size, size];
            tempmap = new float[size, size];
            moisturemap = new float[size, size];
            noiseBitmap = new Bitmap(size, size);
            UndoStack = new Stack<float[,]>();
            RedoStack = new Stack<float[,]>();
        }
    }
}
