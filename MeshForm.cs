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
        public MeshForm()
        {
            InitializeComponent();
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

        private void HeightMouseMove(object sender, MouseEventArgs e)
        {
            // X = xoffset + (x - y) * tilewidth / 2;
            // Y = yoffset + (x + y) * tileheight / 2 - z * 8;
            // x - y = 2 *(X - xoffset) / tilewidth
            // x + y - heightmap[x,y] * 8 = 2 * (Y - yoffset) / tileheight
        //    int tilewidth = 1000 / heightmap.GetLength(0);
        //    int tileheight = 800 / heightmap.GetLength(1);

        //    int mx = e.Location.X;
        //    int my = e.Location.Y;

        //    int X = xoffset + (mx - my) * tilewidth / 2;
        //    int Y = yoffset + (mx + my) * tileheight / 2 - heightmap[mx, my] * scale;
        //    if (X > heightmap.GetLength(0) - 1 || X < 0 || Y < 0 || Y > heightmap.GetLength(1) - 1)
        //    {
        //        return;
        //    }
        //    heightlabel.Text = $"[{X}, {Y}] = {((heightmap[X, Y].ToString().Length > 4) ? heightmap[X, Y].ToString().Substring(0, 5) : heightmap[X, Y].ToString())}";
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
        }

        private void PaintHandler(object sender, PaintEventArgs e)
        {
            drawer.Draw(e.Graphics);
            this.MouseMove += new MouseEventHandler(HeightMouseMove);
        }

        private void HomeForm(object sender, EventArgs e)
        {
            Form1 Home = new Form1();
            Home.Show();
            this.Hide();
        }
    }
}
