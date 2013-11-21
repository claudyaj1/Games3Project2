using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Games3Project2.Globals;
using Camera3D;
using ReticuleCursor;
using InputHandler;
using Geometry;


namespace Games3Project2
{
   
    public class Heatmap
    {
        //Holds all points to be contained in the heatmap
        public List<Vector3> points;

        Sphere marker;

        public Heatmap(Color color)
        {
            marker = new Sphere(Global.game, color, new Vector3(0,0,0));
            marker.localScale = Matrix.CreateScale(0.5f);
            marker.SetWireframe(1);
            points = new List<Vector3>();
        }

        public void addPoint(Vector3 point)
        {
            points.Add(point);
        }

        public void draw()
        {
            foreach (Vector3 p in points)
            {
                marker.Position = p;
                marker.Update(Global.gameTime);
                marker.Draw(Global.CurrentCamera);
            }
        }
    }
}
