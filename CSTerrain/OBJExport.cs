using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace CSTerrain
{
    //class used to export the noisemap to an obj file
    public class OBJExport
    {
        public float[,] terrain { get; set; }
        public int row { get; set; }
        public int col { get; set; }
        //constructor
        public OBJExport(float[,] inTerrain)
        {
            terrain = inTerrain;
            row = inTerrain.GetLength(0);
            col = inTerrain.GetLength(1);
        }

        //generates the vertices used in the obj file 
        public List<(float, float, float)> Generate_vertices(float scale)
        {
            List<(float, float, float)> vertices = new List<(float, float, float)>();
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    float x = i * scale;
                    float y = terrain[i, j];
                    float z = j * scale;
                    vertices.Add((x, y, z));
                }
            }
            return vertices.ToList();
        }

        //generates a list of indexed vertices connected to make faces
        public List<(int, int, int)> Gen_faces()
        {
            List<(int, int, int)> faces = new List<(int, int, int)>();

            for (int i = 0; i < row - 2; i++)
            {
                for (int j = 0; j < col - 2; j++)
                {
                    int tl = i * row + j + 1;
                    int tr = tl + 1;
                    int bl = tl + row;
                    int br = tr + col;
                    faces.Add((tl, tr, br));
                    faces.Add((tl, br, bl));
                }
            }

            return faces.ToList();
        }

        //combines all functions to one to export the map
        public async Task Export(string path, float scale)
        {
            int multiplyer = 2;
            StreamWriter f = new StreamWriter(path + "\\terrain.obj");
            List<(float, float, float)> vertices = Generate_vertices(scale);
            List<(int, int, int)> faces = Gen_faces();
            //MessageBox.Show($"faces:{faces.Count()} vertices:{vertices.Count()}");

            foreach (var v in vertices)
            {
                await f.WriteLineAsync($"v {v.Item1} {v.Item2 * multiplyer} {v.Item3}");
            }

            foreach (var face in faces)
            {
                await f.WriteLineAsync($"f {face.Item1} {face.Item2} {face.Item3}");
            }
            _ = MessageBox.Show("saved");

        }
    }
}