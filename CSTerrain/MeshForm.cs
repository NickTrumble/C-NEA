using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSTerrain
{ 
    public partial class MeshForm : Form
    {
        public bool started = true;
        Button MainForm;
        float[,] heightmap;
        DrawMesh drawer;
        int scale = 100;
        int xoffset, yoffset;
        Bitmap b;

        public static TrackBar rotationslider, xpanslider, ypanslider;
        public MeshForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Width = 1000;
            this.Height = 800;
            xoffset = Width / 2;
            yoffset = Height / 3;
            MainForm = new Button
            {
                Location = new Point(0, 0),
                Size = new Size(100, 30),
                Text = "Main Form"
            };
            Controls.Add(MainForm);
            MainForm.Click += new EventHandler(HomeForm);

            this.Paint += new PaintEventHandler(PaintHandler);

            rotationslider = new TrackBar
            {
                Location = new Point(0, 35),
                Maximum = 360,
                Minimum = 0,
                Value = 45
            };
            Controls.Add(rotationslider);
            rotationslider.ValueChanged += new EventHandler(Rotation);

            xpanslider = new TrackBar
            {
                Location = new Point(0, 80),
                Maximum = 180,
                Minimum = -180,
                Value = 0
            };
            Controls.Add(xpanslider);
            xpanslider.ValueChanged += new EventHandler(XPan);

            ypanslider = new TrackBar
            {
                Location = new Point(0, 125),
                Maximum = 180,
                Minimum = -180,
                Value = 0
            };
            Controls.Add(ypanslider);
            ypanslider.ValueChanged += new EventHandler(YPan);
        }

        public void YPan(object sender, EventArgs e)
        {
            yoffset = (Height / 2) + ypanslider.Value;
            Invalidate();
        }

        public void XPan(object sender, EventArgs e)
        {
            xoffset = (Width / 2) + xpanslider.Value;
            Invalidate();
        }

        public void Rotation(object sender, EventArgs e)
        {
            b = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(b))
            {
                drawer.Draw(g);
            }
            Invalidate();
        }

        public float[,] Round(float[,] Input, float resolution)
        {
            int size = Input.GetLength(0);
            float[,] Output = new float[size, size];
            Parallel.For(0, size, i =>
            {
                for (int j = 0; j < size; j++)
                {
                    Output[i, j] = (int)(resolution * Input[i, j]) / resolution;
                }
            });

            return Output;
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
            drawer = new DrawMesh(Round(heightmap, 40), xoffset, yoffset, scale);
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
                e.Graphics.DrawImage(b, xpanslider.Value, ypanslider.Value);
            }
        }

        private void HomeForm(object sender, EventArgs e)
        {
            Form1 Home = new Form1();
            Home.Show();
            this.Hide();
            Home.TakeNoisemap(heightmap);
        }
    }
}