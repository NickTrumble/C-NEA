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
    public partial class Form1 : Form
    {
        PictureBox picturebox1;
        Timer timer1, timer2;
        int radius = 30;
        float intensity;
        Label heightlabel, radiuslabel;
        Button perlinregenbtn, simplexregenbtn, savebtn, MeshFormbtn, Undobtn, Redobtn;
        float[,] noisemapp;
        Bitmap noise_bitmap;
        NumericUpDown scaleupdown, sizeupdown, octavesupdown, persistenceupdown, islandmixupdown, intensityud;
        static bool started;
        Stack<float[,]> UndoStack, RedoStack;
        float[,] temp;
        public Form1()
        {
            InitializeComponent();
            this.Width = 650;
            this.Height = 500;
            this.Text = "Noisemap Visualiser";

            UndoStack = new Stack<float[,]>();
            RedoStack = new Stack<float[,]>();


            picturebox1 = new PictureBox
            {
                Size = new Size(500, 500),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            picturebox1.MouseDown += new MouseEventHandler(Picturebox_click);
            picturebox1.MouseUp += new MouseEventHandler(Release_handler);
            picturebox1.MouseWheel += new MouseEventHandler(Scroll_handle);

            Controls.Add(picturebox1);

            timer1 = new Timer { Enabled = true };
            timer1.Tick += new EventHandler(Timer1_Tick);

            timer2 = new Timer { Interval = 5 };
            timer2.Tick += new EventHandler(Drag_handler);

            //Height label
            heightlabel = LabelMaker(new Point(500, 0), "[0, 0] = 0.0");

            //Radius label
            radiuslabel = LabelMaker(new Point(500, 25), "Radius: 30");

            //Regen label
            LabelMaker(new Point(520, 50), "Regenerate using:");

            //Scale label & nud
            LabelMaker(new Point(500, 110), "Scale:");
            scaleupdown = NudMaker(new Point(570, 110), 6f, 1f, 16f, 1f);

            //Size label & nud
            LabelMaker(new Point(500, 135), "Size:");
            sizeupdown = NudMaker(new Point(570, 135), 500f, 50f, 2000f, 2f);

            //Octaves label & nud
            LabelMaker(new Point(500, 160), "Octaves: ");
            octavesupdown = NudMaker(new Point(570, 160), 6f, 1f, 10f, 2f);

            //Persistance label & nud
            LabelMaker(new Point(500, 185), "Persistance: ");
            persistenceupdown = NudMaker(new Point(570, 185), 0.5f, 0.1f, 2f, 0.1f);

            //island label & nud
            LabelMaker(new Point(500, 210), "Island value: ");
            islandmixupdown = NudMaker(new Point(570, 210), 1f, -5f, 10f, 1f);

            perlinregenbtn = ButtonMaker(new Point(499, 75), "Perlin", new Size(68, 30));
            perlinregenbtn.Click += new EventHandler(PerlinRegen);

            simplexregenbtn = ButtonMaker(new Point(568, 75), "Simplex", new Size(68, 30));
            simplexregenbtn.Click += new EventHandler(SimplexRegen);

            savebtn = ButtonMaker(new Point(499, 430), "Save Noisemap", new Size(136, 30));
            savebtn.Click += new EventHandler(Saveobj);

            MeshFormbtn = ButtonMaker(new Point(500, 235), "Create Mesh", new Size(136, 30));
            MeshFormbtn.Click += new EventHandler(ToMeshForm);

            Undobtn = ButtonMaker(new Point(500, 270), "Undo", new Size(68, 30));
            Undobtn.Click += new EventHandler(Undo);
            Redobtn = ButtonMaker(new Point(568, 270), "Redo", new Size(68, 30));
            Redobtn.Click += new EventHandler(Redo);

            intensityud = NudMaker(new Point(568, 300), 1f, 1f, 10f, 1f);
            LabelMaker(new Point(500, 300), "Intensity: ");
        }

        private void Undo(object sender, EventArgs e)
        {
            if (UndoStack.Count > 0)
            {
                RedoStack.Push(CopytoNewfloat(noisemapp));
                noisemapp = UndoStack.Pop();
                temp = CopytoNewfloat(noisemapp);
                Draw_Bitmap(FloattoBitmap(noisemapp));
            }
        }

        private void Redo(object sender, EventArgs e)
        {
            if (RedoStack.Count > 0)
            {
                UndoStack.Push(CopytoNewfloat(noisemapp));
                noisemapp = RedoStack.Pop();
                Draw_Bitmap(FloattoBitmap(noisemapp));
            }
        }

        private float[,] CopytoNewfloat(float[,] a)
        {
            int size = a.GetLength(0);
            float[,] b = new float[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    b[i, j] = a[i, j];
                }
            }
            return b;
        }

        private Bitmap FloattoBitmap(float[,] a)
        {
            int size = a.GetLength(0);
            Bitmap bit = new Bitmap(size, size);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    bit.SetPixel(i, j, TerrainCmap.Interpolate_value(noisemapp[i, j]));
                }
            }
            return bit;
        }

        private Button ButtonMaker(Point location, string text, Size size)
        {
            Button button = new Button
            {
                Text = text,
                Size = size,
                Location = location
            };
            Controls.Add(button);
            return button;
        }

        private Label LabelMaker(Point location, string text)
        {
            Label label = new Label
            {
                Text = text,
                AutoSize = true,
                Location = location
            };
            Controls.Add(label);
            return label;
        }

        private NumericUpDown NudMaker(Point location, float val, float min, float max, float inc)
        {
            if (inc >= 1)
            {
                min = (int)min;
                max = (int)max;
                val = (int)val;
            }
            NumericUpDown nud = new NumericUpDown
            {
                Location = location,
                Increment = (decimal)inc,
                Minimum = (decimal)min,
                Maximum = (decimal)max,
                Value = (decimal)val
            };
            Controls.Add(nud);
            return nud;
        }

        public void TakeNoisemap(float[,] noisemap)
        {
            noisemapp = noisemap;
            temp = noisemap;
            Draw_Bitmap(BaseNoise.Gen_bitmap(noisemapp));
        }

        private void ToMeshForm(object sender, EventArgs e)
        {
            MeshForm form2 = new MeshForm();
            form2.Show();
            form2.TakeHeightmap(noisemapp);
            this.Hide();

        }

        private void SimplexRegen(object sender, EventArgs e)
        {
            Generate((float)scaleupdown.Value * 0.001f, (int)octavesupdown.Value,
                     (float)persistenceupdown.Value, (int)sizeupdown.Value, 1, (float)islandmixupdown.Value / 10f);
        }

        //Regenerate Noisemap and redraw bitmap on click in perlin noise
        private void PerlinRegen(object sender, EventArgs e)
        {
            Generate((float)scaleupdown.Value * 0.001f, (int)octavesupdown.Value,
                     (float)persistenceupdown.Value, (int)sizeupdown.Value, 0, (float)islandmixupdown.Value / 10f);
        }

        //updatges the height label on the value of noisemap at mouses location
        private void Mousemove(object sender, MouseEventArgs e)
        {

            float sf = noisemapp.GetLength(0) / 500f;

            int x = (int)(e.Location.X * sf);
            int y = (int)(e.Location.Y * sf);
            if (x > noisemapp.GetLength(0) - 1 || x < 0 || noisemapp == null || y < 0 || y > noisemapp.GetLength(1) - 1)
            {
                return;
            }
            heightlabel.Text = $"[{x}, {y}] = {((noisemapp[x, y].ToString().Length > 4) ? noisemapp[x, y].ToString().Substring(0, 5) : noisemapp[x, y].ToString())}";
        }

        //handles the radius size through the scroll wheel
        private void Scroll_handle(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)//scroll down
            {
                radius += 5;
            }
            else//scroll up
            {
                if (radius > 6)
                {
                    radius -= 5;
                }
            }
            radiuslabel.Text = $"Radius: {radius}";
        }

        //saves the noisemap as an obj file on click with either home or college file paths
        private async void Saveobj(object sender, EventArgs e)
        {
            float scale = 0.015f;
            //college = P:\\csharpterrain\\csharpterrain
            //home = C:\\Users\\iantr\\source\\repos\\CSTerrain\\CSTerrain
            string path = "C:\\Users\\iantr\\source\\repos\\CSTerrain\\CSTerrain";

            OBJExport exp = new OBJExport(noisemapp);
            await exp.Export(path, scale);
        }

        //increases the heightmap value on left click/hold and decreases in right click/hold
        private void Picturebox_click(object sender, MouseEventArgs e)
        {
            int val = (int)intensityud.Value;
            intensity = e.Button == MouseButtons.Left ? 0.01f * val : val * -0.005f;

            float sf = noisemapp.GetLength(0) / 500f;

            int x = (int)(e.Location.X * sf);
            int y = (int)(e.Location.Y * sf);

            Update_bitmap(x, y, radius, intensity);
            timer2.Start();
        }

        //calculates the rectangle with the least space to fit the radius then increases the heightmap using a circle 
        //formula depoending on the value of intensity and then redraws
        private void Update_bitmap(int x, int y, int radius, float intesity)
        {
            int xmin = Math.Max(0, x - radius);
            int xmax = Math.Min(noisemapp.GetLength(0) - 1, x + radius);

            int ymin = Math.Max(0, y - radius);
            int ymax = Math.Min(noisemapp.GetLength(1) - 1, y + radius);

            Edit_bitmap(xmin, xmax, ymin, ymax, radius, intesity, x, y);
            Draw_Bitmap(noise_bitmap);
        }

        //edits the bitmap depending on parameters from updatebitmap
        private Bitmap Edit_bitmap(int xmin, int xmax, int ymin, int ymax, int radius, float base_intensity, int x, int y)
        {
            noise_bitmap = new Bitmap(noisemapp.GetLength(0) - 1, noisemapp.GetLength(1) - 1);
            for (int i = xmin; i < xmax; i++)
            {
                for (int j = ymin; j < ymax; j++)
                {
                    float distance = (float)(Math.Pow((i - x), 2) + Math.Pow((j - y), 2));
                    if (distance < radius * radius)
                    {
                        float intensity = base_intensity * (1 - (distance / (radius * radius)));
                        noisemapp[i, j] = Math.Max(Math.Min(1, intensity + noisemapp[i, j]), 0);
                    }
                    Color c = TerrainCmap.Interpolate_value(noisemapp[i, j]);
                    noise_bitmap.SetPixel(i, j, c);
                }
            }
            return noise_bitmap;
        }

        //updates the timer to stop the mouse moving and changing the heightmap
        private void Release_handler(object sender, MouseEventArgs e)
        {

            UndoStack.Push(CopytoNewfloat(temp));
            RedoStack.Clear();
            timer2.Stop();
            temp = CopytoNewfloat(noisemapp);
        }

        //redraw the nopisemap bitmap on picturebox 1
        private void Draw_Bitmap(Bitmap noise_bitmap)
        {
            using (Graphics g = picturebox1.CreateGraphics())
            {
                g.DrawImage(noise_bitmap, new Rectangle(0, 0, picturebox1.Width, picturebox1.Height));
            }
        }

        //upodates the position based on the mouse position when mouse is held
        private void Drag_handler(object sender, EventArgs e)
        {
            float sf = noisemapp.GetLength(0) / 500f;

            int x = (int)(PointToClient(MousePosition).X * sf);
            int y = (int)(PointToClient(MousePosition).Y * sf);

            Update_bitmap(x, y, radius, intensity);
        }

        //generates the noisemap and draws it at hte start of the program
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (started != true)
            {
                Generate(0.006f, 6, 0.5f, 500, 0, 0f);
                started = true;
            }
            picturebox1.MouseMove += new MouseEventHandler(Mousemove);
            timer1.Stop();
        }

        //Generates a new noise map and draws it on picturebox 1
        private void Generate(float scale, int octaves, float persistance, int size, int mode, float mix)
        {
            PerlinNoise Pnoisegen = new PerlinNoise(size, octaves, persistance, scale);
            SimplexNoise Snoisegen = new SimplexNoise(size, octaves, persistance, scale);

            if (mode == 0)
            {
                noisemapp = BaseNoise.Normalise(Pnoisegen.Gen_array());
                noisemapp = BaseNoise.IslandShaper(noisemapp, mix);
                Draw_Bitmap(PerlinNoise.Gen_bitmap2(noisemapp));
            }
            else
            {
                noisemapp = SimplexNoise.Normalise(Snoisegen.Gen_array());
                noisemapp = BaseNoise.IslandShaper(noisemapp, mix);
                Draw_Bitmap(SimplexNoise.Gen_bitmap2(noisemapp));
            }
            temp = CopytoNewfloat(noisemapp);
            UndoStack = new Stack<float[,]>();
            RedoStack = new Stack<float[,]>();
        }
    }
}
