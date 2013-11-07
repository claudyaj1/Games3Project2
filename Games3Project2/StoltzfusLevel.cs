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

        Texture2D platformTexture;

        public LevelOne(Game game):
            base(game)
        {
            platforms = new List<Platform>();
            platformTexture = game.Content.Load<Texture2D>(@"Textures\walltexture");
            //exterior walls
            platforms.Add(new Platform(game, new Vector3(Global.Constants.LevelOneWidth / 2, 0, 0), Global.Constants.LevelOneWidth, Global.Constants.LevelOneHeight, platformTexture));
            platforms[0].rotation = Matrix.CreateRotationZ((float)Math.PI / 2);
            platforms.Add(new Platform(game, new Vector3(-Global.Constants.LevelOneWidth / 2, 0, 0), Global.Constants.LevelOneWidth, Global.Constants.LevelOneHeight, platformTexture));
            platforms[1].rotation = Matrix.CreateRotationZ((float)Math.PI / 2);
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
