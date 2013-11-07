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

using Broad.Camera;

namespace Games3Project2
{
    class LevelOne : Microsoft.Xna.Framework.GameComponent
    {
        public List<Platform> platforms;

        Texture2D platformWallTexture;
        Texture2D platformTexture;

        public LevelOne(Game game):
            base(game)
        {
            platforms = new List<Platform>();
            platformWallTexture = game.Content.Load<Texture2D>(@"Textures\walltexture");
            platformTexture = game.Content.Load<Texture2D>(@"Textures\platformtexture");
            //exterior walls
            platforms.Add(new Platform(game, new Vector3(Global.Constants.LevelOneWidth, 0, 0), Global.Constants.LevelOneWidth, Global.Constants.LevelOneHeight, platformWallTexture));
            platforms[0].rotation = Matrix.CreateRotationZ((float)Math.PI / 2);
            platforms.Add(new Platform(game, new Vector3(-Global.Constants.LevelOneWidth, 0, 0), Global.Constants.LevelOneWidth, Global.Constants.LevelOneHeight, platformWallTexture));
            platforms[1].rotation = Matrix.CreateRotationZ((float)Math.PI / 2);
            platforms.Add(new Platform(game, new Vector3(0, 0, Global.Constants.LevelOneWidth * 2), Global.Constants.LevelOneWidth, Global.Constants.LevelOneHeight, platformWallTexture));
            platforms[2].rotation = Matrix.CreateRotationX((float)Math.PI / 2);
            platforms.Add(new Platform(game, new Vector3(0, 0, -Global.Constants.LevelOneWidth * 2), Global.Constants.LevelOneWidth, Global.Constants.LevelOneHeight, platformWallTexture));
            platforms[3].rotation = Matrix.CreateRotationX((float)Math.PI / 2);
            //ceiling and floor
            platforms.Add(new Platform(game, new Vector3(0, Global.Constants.LevelOneHeight / 2, 0), Global.Constants.LevelOneWidth, Global.Constants.LevelOneHeight, platformWallTexture));
            platforms.Add(new Platform(game, new Vector3(0, -Global.Constants.LevelOneHeight / 2, 0), Global.Constants.LevelOneWidth, Global.Constants.LevelOneHeight, platformWallTexture));

            //platforms
            platforms.Add(new Platform(game, Vector3.Zero, 10, 10, platformTexture));
            platforms.Add(new Platform(game, new Vector3(-20, 20, 20), 5, 5, platformTexture));
            platforms.Add(new Platform(game, new Vector3(-20, 20, -20), 5, 5, platformTexture));
            platforms.Add(new Platform(game, new Vector3(-20, -20, 20), 5, 5, platformTexture));
            platforms.Add(new Platform(game, new Vector3(-20, -20, -20), 5, 5, platformTexture));
            platforms.Add(new Platform(game, new Vector3(20, 20, 20), 5, 5, platformTexture));
            platforms.Add(new Platform(game, new Vector3(20, -20, 20), 5, 5, platformTexture));
            platforms.Add(new Platform(game, new Vector3(20, -20, -20), 5, 5, platformTexture));
            platforms.Add(new Platform(game, new Vector3(20, 20, -20), 5, 5, platformTexture));
        }

        public void checkCollision(Collidable collidable, GameTime gt)
        {
            if (collidable.position.X > Global.Constants.LevelOneWidth)
            {
                collidable.position.X = Global.Constants.LevelOneWidth;
            }
            else if (collidable.position.X < -Global.Constants.LevelOneWidth)
            {
                collidable.position.X = -Global.Constants.LevelOneWidth;
            }
            else if (collidable.position.Y > Global.Constants.LevelOneHeight)
            {
                collidable.position.Y = Global.Constants.LevelOneHeight;
            }
            else if (collidable.position.Y < -Global.Constants.LevelOneHeight)
            {
                collidable.position.Y = -Global.Constants.LevelOneHeight;
            }
        }

        public void update(GameTime gt)
        {
            
        }

        public void draw(GameTime gt, BroadCamera camera)
        {
            foreach (Platform platform in platforms)
            {
                platform.draw(gt, camera);
            }
        }
    }
}
