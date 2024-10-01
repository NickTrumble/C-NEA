using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSTerrain
{
    public partial class MeshForm : Form
    {
        Button MainForm;
        int[,] heightmap;
        DrawMesh drawer;
        int scale = 8;
        int xoffset, yoffset;
        Label heightlabel;
        Bitmap b;
        public MeshForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Width = 1000;
            this.Height = 800;
            xoffset = Width / 2;
            yoffset = 80;
            MainForm = new Button
            {
                Location = new Point(0, 0),
                Size = new Size(100, 30),
                Text = "Main Form"
            };
            Controls.Add(MainForm);
            MainForm.Click += new EventHandler(HomeForm);

            this.Paint += new PaintEventHandler(PaintHandler);

            heightlabel = new Label
            {
                Location = new Point(0, 35),
                Text = "[i, j] = 0.00",
                AutoSize = true
            };
            Controls.Add(heightlabel);
        }


        public int[,] FloatToInt(float[,] Input)
        {
            int size = Input.GetLength(0);
            int[,] Output = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Output[i, j] = (int)(10 * Input[i, j]);
                }
            }
            return Output;
        }

        public void TakeHeightmap(float[,] h)
        {
            heightmap = FloatToInt(h);
            drawer = new DrawMesh(heightmap, xoffset, yoffset, scale);
            CreateBitmap();
            Invalidate();
        }

        public void CreateBitmap()
        {
            b = new Bitmap(Width, Height);
            Graphics g = Graphics.FromImage(b);
            drawer.Draw(g);
        }

        private void PaintHandler(object sender, PaintEventArgs e)
        {
            if (b != null)
            {
                e.Graphics.DrawImage(b, 0, 0);
            }
        }

        private void HomeForm(object sender, EventArgs e)
        {
            Form1 Home = new Form1();
            Home.Show();
            this.Hide();
        }
    }
}
