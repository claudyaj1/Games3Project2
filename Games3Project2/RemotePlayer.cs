using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

using Games3Project2.Globals;
using Camera3D;
using Geometry;

namespace Games3Project2
{
    public class RemotePlayer : Collidable
    {
        public int networkPlayerID; //TODO: This needs set
        public int score;
        Sphere sphere;
        Cube cube;
        Matrix cubeTransformation;
        const int gunLength = 3;
        public bool isJuggernaut;
        public float yaw;
        public float pitch;
        public RemotePlayer(Vector3 pos, int networkPlayerID) :
            base(Global.game, pos, Vector3.Zero, Global.Constants.PLAYER_RADIUS)
        {
            this.networkPlayerID = networkPlayerID;
            score = 0;
            Color sphereColor = Color.Red;
            switch(networkPlayerID)
            {
                case 1:
                    sphereColor = Color.Red;
                    break;
                case 2:
                    sphereColor = Color.Blue;
                    break;
                case 3:
                    sphereColor = Color.Green;
                    break;
                case 4:
                    sphereColor = Color.Yellow;
                    break;
            }
            Texture2D blankTex = Global.game.Content.Load<Texture2D>(@"Textures\blankTexture");
            sphere = new Sphere(Global.game, sphereColor, pos);
            cube = new Cube(blankTex, sphereColor); 
            sphere.localScale = Matrix.CreateScale(5);
            sphere.SetWireframe(1);
            cube.wireFrame = false;
            cube.textured = false;
            cubeTransformation = Matrix.CreateScale(1, 1, gunLength) * Matrix.CreateTranslation(new Vector3(radius, 0, gunLength));
            isJuggernaut = false;
            yaw = 0;
            pitch = 0;
        }

        public void update()
        {
            position += velocity;
            sphere.Position = position;
            sphere.Update(Global.gameTime);
        }

        public void draw()
        {
            sphere.Draw(Global.CurrentCamera);
            cube.Draw(Global.CurrentCamera, cubeTransformation *
                Matrix.CreateRotationX(MathHelper.ToRadians(pitch)) * Matrix.CreateRotationY(MathHelper.ToRadians(yaw))
                * Matrix.CreateTranslation(position));
        }
    }
}
