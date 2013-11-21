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
    class Level : Microsoft.Xna.Framework.GameComponent
    {
        public List<Platform> platforms;
        List<Platform> walls;

        Texture2D platformWallTexture;
        Texture2D platformTexture;
        public int currentLevel;


        const int smallPlatformSize = 10;
        const int mediumPlatformSize = 20;
        const int largePlatformSize = 40;
        const float standardSpacing = 40;

        public Level():
            base(Global.game)
        {
            platforms = new List<Platform>();
            walls = new List<Platform>();

            platformWallTexture = Global.game.Content.Load<Texture2D>(@"Textures\walltexture");
            platformTexture = Global.game.Content.Load<Texture2D>(@"Textures\platformtexture");

            currentLevel = 1;
        }

        public void checkCollision(Collidable collidable)
        {
            //hard boundary checks
            float xCheck = 0;
            float zCheck = 0;
            float yCheck = 0;
            switch(currentLevel)
            {
                case 1:
                    xCheck = Global.Constants.LEVEL_ONE_WIDTH - collidable.Radius;
                    zCheck = Global.Constants.LEVEL_ONE_LENGTH - collidable.Radius;
                    yCheck = Global.Constants.LEVEL_ONE_HEIGHT - collidable.Radius;
                    break;
                case 2:
                    xCheck = Global.Constants.LEVEL_TWO_WIDTH - collidable.Radius;
                    zCheck = Global.Constants.LEVEL_TWO_LENGTH - collidable.Radius;
                    yCheck = Global.Constants.LEVEL_TWO_HEIGHT - collidable.Radius;
                    break;
            }
            
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
            foreach (BugBot bot in Global.bugBots)
            {
                bot.update();
            }
        }

        /// <summary>
        /// Call before the player avatars are drawn.
        /// </summary>
        public void drawWalls()
        {
            foreach (Platform wall in walls)
            {
                wall.draw();
            }
        }

        /// <summary>
        /// Call after the players are drawn. Not true says claudy...a half truth at best.
        /// </summary>
        public void drawPlatforms()
        {
            foreach (Platform platform in platforms)
            {
                platform.draw();
            }

            foreach (BugBot bot in Global.bugBots)
            {
                bot.draw();
            }
            
        }

        public void setupLevelOne()
        {
            currentLevel = 1;
            walls.Clear();
            platforms.Clear();
            //exterior walls
            walls.Add(new Platform(new Vector3(Global.Constants.LEVEL_ONE_WIDTH, 0, 0), Global.Constants.LEVEL_ONE_WIDTH, Global.Constants.LEVEL_ONE_HEIGHT * 2, platformWallTexture, Platform.PlatformType.VerticalZ));
            walls[0].rotation = Matrix.CreateRotationZ((float)Math.PI / 2);
            walls.Add(new Platform(new Vector3(-Global.Constants.LEVEL_ONE_WIDTH, 0, 0), Global.Constants.LEVEL_ONE_WIDTH, Global.Constants.LEVEL_ONE_HEIGHT * 2, platformWallTexture, Platform.PlatformType.VerticalZ));
            walls[1].rotation = Matrix.CreateRotationZ((float)Math.PI / 2);
            walls.Add(new Platform(new Vector3(0, 0, Global.Constants.LEVEL_ONE_LENGTH), Global.Constants.LEVEL_ONE_WIDTH, Global.Constants.LEVEL_ONE_HEIGHT, platformWallTexture, Platform.PlatformType.VerticalX));
            walls[2].rotation = Matrix.CreateRotationX((float)Math.PI / 2);
            walls.Add(new Platform(new Vector3(0, 0, -Global.Constants.LEVEL_ONE_LENGTH), Global.Constants.LEVEL_ONE_WIDTH, Global.Constants.LEVEL_ONE_HEIGHT, platformWallTexture, Platform.PlatformType.VerticalX));
            walls[3].rotation = Matrix.CreateRotationX((float)Math.PI / 2);
            //ceiling and floor
            walls.Add(new Platform(new Vector3(0, Global.Constants.LEVEL_ONE_HEIGHT, 0), Global.Constants.LEVEL_ONE_WIDTH, Global.Constants.LEVEL_ONE_LENGTH, platformWallTexture, Platform.PlatformType.Horizontal));
            walls.Add(new Platform(new Vector3(0, -Global.Constants.LEVEL_ONE_HEIGHT, 0), Global.Constants.LEVEL_ONE_WIDTH, Global.Constants.LEVEL_ONE_LENGTH, platformWallTexture, Platform.PlatformType.Horizontal));

            //platforms
            //platforms.Add(new Platform(Global.game, Vector3.Zero, 10, 10, platformTexture));
            //central platform
            platforms.Add(new Platform(Vector3.Zero, largePlatformSize, largePlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            //top central platform
            platforms.Add(new Platform(new Vector3(0, standardSpacing, 0), smallPlatformSize, smallPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            //top four platforms
            platforms.Add(new Platform(new Vector3(standardSpacing, 2 * standardSpacing, 2 * standardSpacing), mediumPlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(new Vector3(standardSpacing, 2 * standardSpacing, -2 * standardSpacing), mediumPlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(new Vector3(-standardSpacing, 2 * standardSpacing, 2 * standardSpacing), mediumPlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(new Vector3(-standardSpacing, 2 * standardSpacing, -2 * standardSpacing), mediumPlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            //bottom four platforms
            platforms.Add(new Platform(new Vector3(standardSpacing, -standardSpacing, 2 * standardSpacing), mediumPlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(new Vector3(standardSpacing, -standardSpacing, -2 * standardSpacing), mediumPlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(new Vector3(-standardSpacing, -standardSpacing, 2 * standardSpacing), mediumPlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(new Vector3(-standardSpacing, -standardSpacing, -2 * standardSpacing), mediumPlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            //edge platforms
            platforms.Add(new Platform(new Vector3(0, standardSpacing, Global.Constants.LEVEL_ONE_LENGTH - mediumPlatformSize), largePlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(new Vector3(0, -2 * standardSpacing, Global.Constants.LEVEL_ONE_LENGTH - mediumPlatformSize), largePlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(new Vector3(0, standardSpacing, -Global.Constants.LEVEL_ONE_LENGTH + mediumPlatformSize), largePlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(new Vector3(0, -2 * standardSpacing, -Global.Constants.LEVEL_ONE_LENGTH + mediumPlatformSize), largePlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));

            Global.bugBots.Add(new BugBot(new Vector3(75, 0, -180), .09f, new Vector3(-75, -50, -175), new Vector3(-50, 50, 190), new Vector3(92, 120, 95), new Vector3(75, 0, -180)));
            Global.bugBots.Add(new BugBot(new Vector3(-75, 0, -180), .09f, new Vector3(75, -50, -175), new Vector3(50, 50, 190), new Vector3(-92, -120, 95), new Vector3(-75, 0, -180)));
            Global.bugBots.Add(new BugBot(new Vector3(-75, 0, 180), .09f, new Vector3(75, 50, 175), new Vector3(50, -50, -190), new Vector3(-92, 120, -95), new Vector3(75, 0, 180)));
            Global.bugBots.Add(new BugBot(new Vector3(75, 0, 180), .09f, new Vector3(75, 50, -175), new Vector3(50, 50, 190), new Vector3(92, 120, -95), new Vector3(75, 0, -180)));
        }

        public void setupLevelTwo()
        {
            currentLevel = 2;
            walls.Clear();
            platforms.Clear();

            //exterior walls
            walls.Add(new Platform(new Vector3(Global.Constants.LEVEL_TWO_WIDTH, 0, 0), Global.Constants.LEVEL_TWO_HEIGHT, Global.Constants.LEVEL_TWO_WIDTH, platformWallTexture, Platform.PlatformType.VerticalZ));
            walls[0].rotation = Matrix.CreateRotationZ((float)Math.PI / 2);
            walls.Add(new Platform(new Vector3(-Global.Constants.LEVEL_TWO_WIDTH, 0, 0), Global.Constants.LEVEL_TWO_HEIGHT, Global.Constants.LEVEL_TWO_WIDTH, platformWallTexture, Platform.PlatformType.VerticalZ));
            walls[1].rotation = Matrix.CreateRotationZ((float)Math.PI / 2);
            walls.Add(new Platform(new Vector3(0, 0, Global.Constants.LEVEL_TWO_LENGTH), Global.Constants.LEVEL_TWO_WIDTH, Global.Constants.LEVEL_TWO_HEIGHT, platformWallTexture, Platform.PlatformType.VerticalX));
            walls[2].rotation = Matrix.CreateRotationX((float)Math.PI / 2);
            walls.Add(new Platform(new Vector3(0, 0, -Global.Constants.LEVEL_TWO_LENGTH), Global.Constants.LEVEL_TWO_WIDTH, Global.Constants.LEVEL_TWO_HEIGHT, platformWallTexture, Platform.PlatformType.VerticalX));
            walls[3].rotation = Matrix.CreateRotationX((float)Math.PI / 2);
            //ceiling and floor
            walls.Add(new Platform(new Vector3(0, Global.Constants.LEVEL_TWO_HEIGHT, 0), Global.Constants.LEVEL_TWO_WIDTH, Global.Constants.LEVEL_TWO_LENGTH, platformWallTexture, Platform.PlatformType.Horizontal));
            walls.Add(new Platform(new Vector3(0, -Global.Constants.LEVEL_TWO_HEIGHT, 0), Global.Constants.LEVEL_TWO_WIDTH, Global.Constants.LEVEL_TWO_LENGTH, platformWallTexture, Platform.PlatformType.Horizontal));

            //central platform
            platforms.Add(new Platform(Vector3.Zero, largePlatformSize, largePlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            //edge platforms one (x plane)
            platforms.Add(new Platform(new Vector3(Global.Constants.LEVEL_TWO_WIDTH - mediumPlatformSize, standardSpacing, 0), mediumPlatformSize, largePlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(new Vector3(-Global.Constants.LEVEL_TWO_WIDTH + mediumPlatformSize, standardSpacing, 0), mediumPlatformSize, largePlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            //edge platforms two (z plane)
            platforms.Add(new Platform(new Vector3(0, 2 * standardSpacing, Global.Constants.LEVEL_TWO_LENGTH - mediumPlatformSize), largePlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(new Vector3(0, 2 * standardSpacing, -Global.Constants.LEVEL_TWO_LENGTH + mediumPlatformSize), largePlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            //top central platform
            platforms.Add(new Platform(new Vector3(0, 3 * standardSpacing, 0), mediumPlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            //bottom edge platforms (z plane)
            platforms.Add(new Platform(new Vector3(0, -standardSpacing, Global.Constants.LEVEL_TWO_LENGTH - mediumPlatformSize), largePlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(new Vector3(0, -standardSpacing, -Global.Constants.LEVEL_TWO_LENGTH + mediumPlatformSize), largePlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            //bottom second central platform
            platforms.Add(new Platform(new Vector3(0, -2 * standardSpacing, 0), mediumPlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            //bottom edge platforms (x plane)
            platforms.Add(new Platform(new Vector3(Global.Constants.LEVEL_TWO_WIDTH - mediumPlatformSize, -3 * standardSpacing, 0), mediumPlatformSize, largePlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(new Vector3(-Global.Constants.LEVEL_TWO_WIDTH + mediumPlatformSize, -3 * standardSpacing, 0), mediumPlatformSize, largePlatformSize, platformTexture, Platform.PlatformType.Horizontal));
        }
    }
}
