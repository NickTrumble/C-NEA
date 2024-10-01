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
        Button perlinregenbtn, simplexregenbtn, savebtn, MeshFormbtn;
        float[,] noisemapp;
        Bitmap noise_bitmap;
        NumericUpDown scaleupdown, sizeupdown, octavesupdown, persistenceupdown, islandmixupdown;


        public Form1()
        {
            InitializeComponent();
            this.Width = 650;
            this.Height = 500;
            this.Text = "Noisemap Visualiser";

            picturebox1 = new PictureBox
            {
                Size = new Size(500, 500),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            picturebox1.MouseDown += new MouseEventHandler(Picturebox_click);
            picturebox1.MouseUp += new MouseEventHandler(Release_handler);
            picturebox1.MouseWheel += new MouseEventHandler(Scroll_handle);

            Controls.Add(picturebox1);

            timer1 = new Timer
            {
                Enabled = true
            };
            timer1.Tick += new EventHandler(Timer1_Tick);

            timer2 = new Timer
            {
                Interval = 5
            };
            timer2.Tick += new EventHandler(Drag_handler);

            heightlabel = new Label
            {
                Location = new Point(500, 0),
                Text = "[0, 0] = 0.0"
            };
            Controls.Add(heightlabel);
            radiuslabel = new Label
            {
                Location = new Point(500, 25),
                Text = $"Radius: {radius}"
            };
            Controls.Add(radiuslabel);

            Label regenlabel = new Label
            {
                Location = new Point(520, 50),
                Text = "Regenerate using:"
            };
            Controls.Add(regenlabel);

            perlinregenbtn = new Button
            {
                Location = new Point(499, 75),
                Size = new Size(68, 30),
                Text = "Perlin"
            };
            perlinregenbtn.Click += new EventHandler(PerlinRegen);
            Controls.Add(perlinregenbtn);

            simplexregenbtn = new Button
            {
                Location = new Point(568, 75),
                Size = new Size(68, 30),
                Text = "Simplex"
            };
            Controls.Add(simplexregenbtn);
            simplexregenbtn.Click += new EventHandler(SimplexRegen);

            savebtn = new Button
            {
                Text = "Save Noisemap",
                Location = new Point(499, 430),
                Size = new Size(136, 30)
            };
            savebtn.Click += new EventHandler(Saveobj);
            Controls.Add(savebtn);

            scaleupdown = new NumericUpDown
            {
                Location = new Point(570, 110),
                Value = 6,
                Minimum = 1,
                Maximum = 16,
                Increment = 1
            };
            Controls.Add(scaleupdown);
            Label scalelabel = new Label
            {
                Location = new Point(500, 110),
                AutoSize = true,
                Text = "Scale:"
            };
            Controls.Add(scalelabel);

            sizeupdown = new NumericUpDown
            {
                Location = new Point(570, 135),
                Minimum = 50,
                Maximum = 2000,
                Value = 500,
                Increment = 2
            };
            Controls.Add(sizeupdown);
            Label sizelabel = new Label
            {
                Location = new Point(500, 135),
                Text = "Size:",
                AutoSize = true
            };
            Controls.Add(sizelabel);

            octavesupdown = new NumericUpDown
            {
                Location = new Point(570, 160),
                Minimum = 1,
                Maximum = 10,
                Value = 6,
                Increment = 2
            };
            Controls.Add(octavesupdown);
            Label octaveslabel = new Label
            {
                Text = "Octaves: ",
                Location = new Point(500, 160),
                AutoSize = true
            };
            Controls.Add(octaveslabel);

            persistenceupdown = new NumericUpDown
            {
                Location = new Point(570, 185),
                Minimum = 0.1M,
                Maximum = 2M,
                Value = 0.5M,
                Increment = 0.1M,
                DecimalPlaces = 1
            };
            Controls.Add(persistenceupdown);
            Label persistancelabel = new Label
            {
                Text = "Persistance: ",
                Location = new Point(500, 185),
                AutoSize = true
            };
            Controls.Add(persistancelabel);

            Label IslandMix = new Label
            {
                Text = "Island value: ",
                Location = new Point(500, 210),
                AutoSize = true
            };
            Controls.Add(IslandMix);

            islandmixupdown = new NumericUpDown
            {
                Location = new Point(570, 210),
                Minimum = -5,
                Maximum = 10,
                Value = 1
            };
            Controls.Add(islandmixupdown);

            MeshFormbtn = new Button
            {
                Location = new Point(500, 235),
                Text = "Create Mesh",
                Size = new Size(136, 30)
            };
            Controls.Add(MeshFormbtn);
            MeshFormbtn.Click += new EventHandler(ToMeshForm);
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
            intensity = e.Button == MouseButtons.Left ? 0.1f : -0.05f;

            float sf =  noisemapp.GetLength(0) / 500f;

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
        private void Release_handler(object sender, MouseEventArgs e) => timer2.Stop();

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
            Generate(0.006f, 6, 0.5f, 500, 0, 0f);
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
                noisemapp = PerlinNoise.Normalise(Pnoisegen.Gen_array());
                noisemapp = SimplexNoise.Normalise(Snoisegen.Gen_array());
                noisemapp = BaseNoise.IslandShaper(noisemapp, mix);
                Draw_Bitmap(Pnoisegen.Gen_bitmap(noisemapp));
            }
            else
            {
                noisemapp = SimplexNoise.Normalise(Snoisegen.Gen_array());
                noisemapp = BaseNoise.IslandShaper(noisemapp, mix);
                Draw_Bitmap(Snoisegen.Gen_bitmap(noisemapp));
            }
        }
    }
}
