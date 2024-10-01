using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSTerrain
{
    public partial class MeshForm : Form
    {
        Button MainForm;
        float[,] heightmap;
        DrawMesh drawer;
        int scale = 70;
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
            yoffset = 150;
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
            Parallel.For(0, size, i =>
            {
                for (int j = 0; j < size; j++)
                {
                    Output[i, j] = (int)(10 * Input[i, j]);
                }
            });

            return Output;
        }

        public void TakeHeightmap(float[,] h)
        {
            heightmap = h;
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
