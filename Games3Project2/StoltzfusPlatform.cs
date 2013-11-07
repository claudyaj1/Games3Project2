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
using Geometry;
using Broad.Camera;

namespace Games3Project2
{
    class Platform : Microsoft.Xna.Framework.GameComponent
    {
        #region Get/Set
        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                translation = Matrix.CreateTranslation(position.X, position.Y, position.Z);
            }
        }
        public float Width
        {
            get { return width; }
            set
            {
                width = value;
                scale = Matrix.CreateScale(width, 1, length);

            }
        }
        public float Length
        {
            get { return length; }
            set
            {
                length = value;
                scale = Matrix.CreateScale(width, 1, length);
            }
        }
        #endregion
        //general properties
        Vector3 position;
        float length;
        float width;
        public Texture2D texture;
        //matrices
        Matrix scale;
        Matrix translation;
        public Matrix rotation;
        //geometry
        Quad quad;

        public Platform(Game game, Vector3 pos_, float width_, float length_, Texture2D tex) :
            base(game)
        {
            Position = pos_;
            Width = width_;
            Length = length_;
            texture = tex;

            quad = new Quad(game.GraphicsDevice, tex, Color.White);
            quad.mirrorTexture = true;
            rotation = Matrix.Identity;
        }

        public void update()
        {

        }

        public void draw(GameTime gt, BroadCamera camera)
        {
            quad.Draw(gt, scale * translation * rotation, camera.view, camera.projection);
        }
    }
}
