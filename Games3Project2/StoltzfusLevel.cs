using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Games3Project2.Globals;
using Camera3D;

namespace Games3Project2
{
    class LevelOne : Microsoft.Xna.Framework.GameComponent
    {
        public List<Platform> platforms;
        List<Platform> walls;

        Texture2D platformWallTexture;
        Texture2D platformTexture;

        public LevelOne(Game game):
            base(game)
        {
            platforms = new List<Platform>();
            walls = new List<Platform>();
            platformWallTexture = game.Content.Load<Texture2D>(@"Textures\walltexture");
            platformTexture = game.Content.Load<Texture2D>(@"Textures\platformtexture");
            //exterior walls
            walls.Add(new Platform(game, new Vector3(Global.Constants.LEVEL_ONE_WIDTH, 0, 0), Global.Constants.LEVEL_ONE_WIDTH, Global.Constants.LEVEL_ONE_HEIGHT * 2, platformWallTexture, Platform.PlatformType.VerticalZ));
            walls[0].rotation = Matrix.CreateRotationZ((float)Math.PI / 2);
            walls.Add(new Platform(game, new Vector3(-Global.Constants.LEVEL_ONE_WIDTH, 0, 0), Global.Constants.LEVEL_ONE_WIDTH, Global.Constants.LEVEL_ONE_HEIGHT * 2, platformWallTexture, Platform.PlatformType.VerticalZ));
            walls[1].rotation = Matrix.CreateRotationZ((float)Math.PI / 2);
            walls.Add(new Platform(game, new Vector3(0, 0, Global.Constants.LEVEL_ONE_WIDTH * 2), Global.Constants.LEVEL_ONE_WIDTH, Global.Constants.LEVEL_ONE_HEIGHT, platformWallTexture, Platform.PlatformType.VerticalX));
            walls[2].rotation = Matrix.CreateRotationX((float)Math.PI / 2);
            walls.Add(new Platform(game, new Vector3(0, 0, -Global.Constants.LEVEL_ONE_WIDTH * 2), Global.Constants.LEVEL_ONE_WIDTH, Global.Constants.LEVEL_ONE_HEIGHT, platformWallTexture, Platform.PlatformType.VerticalX));
            walls[3].rotation = Matrix.CreateRotationX((float)Math.PI / 2);
            //ceiling and floor
            walls.Add(new Platform(game, new Vector3(0, Global.Constants.LEVEL_ONE_HEIGHT, 0), Global.Constants.LEVEL_ONE_WIDTH, Global.Constants.LEVEL_ONE_HEIGHT * 2, platformWallTexture, Platform.PlatformType.Horizontal));
            walls.Add(new Platform(game, new Vector3(0, -Global.Constants.LEVEL_ONE_HEIGHT, 0), Global.Constants.LEVEL_ONE_WIDTH, Global.Constants.LEVEL_ONE_HEIGHT * 2, platformWallTexture, Platform.PlatformType.Horizontal));

            //platforms
            //platforms.Add(new Platform(game, Vector3.Zero, 10, 10, platformTexture));
            platforms.Add(new Platform(game, new Vector3(-20, 20, 20), 5, 5, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(game, new Vector3(-20, 20, -20), 5, 5, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(game, new Vector3(-20, -20, 20), 5, 5, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(game, new Vector3(-20, -20, -20), 5, 5, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(game, new Vector3(20, 20, 20), 5, 5, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(game, new Vector3(20, -20, 20), 5, 5, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(game, new Vector3(20, -20, -20), 5, 5, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(game, new Vector3(20, 20, -20), 5, 5, platformTexture, Platform.PlatformType.Horizontal));
        }

        public void checkCollision(Collidable collidable)
        {
            //hard boundary checks
            
            float xCheck = Global.Constants.LEVEL_ONE_WIDTH - collidable.Radius;
            float zCheck = Global.Constants.LEVEL_ONE_LENGTH - collidable.Radius;
            float yCheck = Global.Constants.LEVEL_ONE_HEIGHT - collidable.Radius;
            if (collidable.Position.X > xCheck)
            {
                collidable.Position = new Vector3(xCheck, collidable.Position.Y, collidable.Position.Z);
            }
            else if (collidable.Position.X < -xCheck)
            {
                collidable.Position = new Vector3(-xCheck, collidable.Position.Y, collidable.Position.Z);
            }

            if (collidable.Position.Y > yCheck)
            {
                collidable.Position = new Vector3(collidable.Position.X, yCheck, collidable.Position.Z);
            }
            else if (collidable.Position.Y < -yCheck)
            {
                collidable.Position = new Vector3(collidable.Position.X, -yCheck, collidable.Position.Z);
            }

            if (collidable.Position.Z > zCheck)
            {
                collidable.Position = new Vector3(collidable.Position.X, collidable.Position.Y, zCheck);
            }
            else if (collidable.Position.Z < -zCheck)
            {
                collidable.Position = new Vector3(collidable.Position.X, collidable.Position.Y, -zCheck);
            }
            
            //platforms
            foreach (Platform platform in platforms)
            {
                platform.collide(collidable);
            }
        }

        public void update()
        {
            foreach (LocalPlayer player in Global.localPlayers)
            {
                checkCollision(player);
            }
        }

        public void draw()
        {
            foreach (Platform wall in walls)
            {
                wall.draw();
            }
            foreach (Platform platform in platforms)
            {
                platform.draw();
            }
        }
    }
}
