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
using AI;

namespace Games3Project2
{
    public class Level : Microsoft.Xna.Framework.GameComponent
    {
        public List<Platform> platforms;
        List<Platform> walls;
        List<Vector3> spawnPoints;

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
            spawnPoints = new List<Vector3>();

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
                case 3:
                    xCheck = Global.Constants.LEVEL_THREE_WIDTH - collidable.Radius;
                    zCheck = Global.Constants.LEVEL_THREE_LENGTH - collidable.Radius;
                    yCheck = Global.Constants.LEVEL_THREE_HEIGHT - collidable.Radius;
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

        public bool checkForCollision(Collidable collidable)
        {
            //hard boundary checks
            float xCheck = 0;
            float zCheck = 0;
            float yCheck = 0;
            switch (currentLevel)
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
                case 3:
                    xCheck = Global.Constants.LEVEL_THREE_WIDTH - collidable.Radius;
                    zCheck = Global.Constants.LEVEL_THREE_LENGTH - collidable.Radius;
                    yCheck = Global.Constants.LEVEL_THREE_HEIGHT - collidable.Radius;
                    break;
            }

            if (collidable.Position.X > xCheck)
            {
                return true;
            }
            else if (collidable.Position.X < -xCheck)
            {
                return true;
            }

            if (collidable.Position.Y > yCheck)
            {
                return true;
            }
            else if (collidable.Position.Y < -yCheck)
            {
                return true;
            }

            if (collidable.Position.Z > zCheck)
            {
                return true;
            }
            else if (collidable.Position.Z < -zCheck)
            {
                return true;
            }

            //platforms
            foreach (Platform platform in platforms)
            {
                if (platform.didCollide(collidable))
                {
                    return true;
                }
            }

            return false;
        }

        public void update()
        {
            foreach (LocalPlayer player in Global.localPlayers)
            {
                checkCollision(player);
            }
            foreach (RemotePlayer player in Global.remotePlayers)
            {
                if (checkForCollision(player))
                {
                    player.Velocity = Vector3.Zero;
                }
            }
            foreach (BugBot bot in Global.bugBots)
            {
                bot.update();
            }
            foreach (Turret turret in Global.turrets)
            {
                turret.update();
            }
            for (int i = 0; i < Global.BulletManager.bullets.Count; ++i)
            {
                if(Global.BulletManager.bullets[i].state == Bullet.State.Active && checkForCollision(Global.BulletManager.bullets[i]))
                {
                    Global.BulletManager.bullets[i].disable();
                }
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
            foreach (BugBot bot in Global.bugBots)
            {
                bot.draw();
            }
            foreach (Turret turret in Global.turrets)
            {
                turret.draw();
            }
        }

        public void respawnPlayer(LocalPlayer player)
        {
            int index = Global.rand.Next(0, spawnPoints.Count * 100) / 100; //better distribution C# Random sucks
            player.respawn(spawnPoints[index]);
        }

        public void setupLevel()
        {
            switch (currentLevel)
            {
                case 1:
                    setupLevelOne();
                    break;
                case 2:
                    setupLevelTwo();
                    break;
                case 3:
                    setupLevelThree();
                    break;
            }
        }

        private void setupLevelOne()
        {
            walls.Clear();
            platforms.Clear();
            spawnPoints.Clear();
            Global.bugBots.Clear();
            Global.turrets.Clear();
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

            //AI
            Global.bugBots.Add(new BugBot(new Vector3(75, 0, -180), 1, .09f, new Vector3(-75, -50, -175), new Vector3(-50, 50, 190), new Vector3(92,90, 95), new Vector3(75, 0, -180)));
            Global.bugBots.Add(new BugBot(new Vector3(-75, 0, -180), 2, .09f, new Vector3(75, -50, -175), new Vector3(50, 50, 190), new Vector3(-92, -120, 95), new Vector3(-75, 0, -180)));
            Global.bugBots.Add(new BugBot(new Vector3(-75, 0, 180), 3, .09f, new Vector3(75, 50, 175), new Vector3(50, -50, -190), new Vector3(-92, 90, -95), new Vector3(75, 0, 180)));
            Global.bugBots.Add(new BugBot(new Vector3(75, 0, 180), 4, .09f, new Vector3(75, 50, -175), new Vector3(50, 50, 190), new Vector3(92, 90, -95), new Vector3(75, 0, -180)));

            Global.turrets.Add(new Turret(new Vector3(95, -95, 195)));
            Global.turrets.Add(new Turret(new Vector3(95, -95, -195)));
            Global.turrets.Add(new Turret(new Vector3(-95, -95, 195)));
            Global.turrets.Add(new Turret(new Vector3(-95, -95, -195)));
            Global.turrets.Add(new Turret(new Vector3(0, -95, 0)));

            //spawn points
            spawnPoints.Add(new Vector3(4, 50, 175));
            spawnPoints.Add(new Vector3(5, 50, -175));
            spawnPoints.Add(new Vector3(5, -70, 180));
            spawnPoints.Add(new Vector3(5, -70, -180));
            spawnPoints.Add(new Vector3(0, 10, 0));

            spawnAllPlayers();
        }

        private void setupLevelTwo()
        {
            currentLevel = 2;
            walls.Clear();
            platforms.Clear();
            spawnPoints.Clear();
            Global.bugBots.Clear();
            Global.turrets.Clear();
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

            //AI
            Global.bugBots.Add(new BugBot(new Vector3(75, 0, -18), 1, .09f, new Vector3(-75, 130, -17), new Vector3(-50, 50, 19), new Vector3(92, 0, 95), new Vector3(75, 130, -18)));
            Global.bugBots.Add(new BugBot(new Vector3(-75, -45, -18), 2, .09f, new Vector3(75, -45, 17), new Vector3(50, -50, 19), new Vector3(-92, 130, 95), new Vector3(-75, 0, -18)));
            Global.bugBots.Add(new BugBot(new Vector3(-75, -100, 18), 3, .09f, new Vector3(75, -150,35), new Vector3(50, 130, -19), new Vector3(-92, 90, -95), new Vector3(75, -45, 18)));
            Global.bugBots.Add(new BugBot(new Vector3(75, -150, 18), 4, .09f, new Vector3(75, 130, -17), new Vector3(50, -45, 19), new Vector3(92, 50, -95), new Vector3(75, -150, -18)));

            Global.turrets.Add(new Turret(new Vector3(95, -160, 95)));
            Global.turrets.Add(new Turret(new Vector3(-95, -160, 95)));
            Global.turrets.Add(new Turret(new Vector3(95, -160, -95)));
            Global.turrets.Add(new Turret(new Vector3(-95, -160, -95)));
            Global.turrets.Add(new Turret(new Vector3(0, -160, 0)));

            spawnPoints.Add(new Vector3(0, -30, -90));
            spawnPoints.Add(new Vector3(0, -30, 90));
            spawnPoints.Add(new Vector3(0, 10, 0));
            spawnPoints.Add(new Vector3(0, -70, 0));

            spawnAllPlayers();
        }

        private void setupLevelThree()
        {
            currentLevel = 3;
            walls.Clear();
            platforms.Clear();
            spawnPoints.Clear();
            Global.bugBots.Clear();
            Global.turrets.Clear();

            //exterior walls
            walls.Add(new Platform(new Vector3(Global.Constants.LEVEL_THREE_WIDTH, 0, 0), Global.Constants.LEVEL_THREE_HEIGHT, Global.Constants.LEVEL_THREE_WIDTH, platformWallTexture, Platform.PlatformType.VerticalZ));
            walls[0].rotation = Matrix.CreateRotationZ((float)Math.PI / 2);
            walls.Add(new Platform(new Vector3(-Global.Constants.LEVEL_THREE_WIDTH, 0, 0), Global.Constants.LEVEL_THREE_HEIGHT, Global.Constants.LEVEL_THREE_WIDTH, platformWallTexture, Platform.PlatformType.VerticalZ));
            walls[1].rotation = Matrix.CreateRotationZ((float)Math.PI / 2);
            walls.Add(new Platform(new Vector3(0, 0, Global.Constants.LEVEL_THREE_LENGTH), Global.Constants.LEVEL_THREE_WIDTH, Global.Constants.LEVEL_THREE_HEIGHT, platformWallTexture, Platform.PlatformType.VerticalX));
            walls[2].rotation = Matrix.CreateRotationX((float)Math.PI / 2);
            walls.Add(new Platform(new Vector3(0, 0, -Global.Constants.LEVEL_THREE_LENGTH), Global.Constants.LEVEL_THREE_WIDTH, Global.Constants.LEVEL_THREE_HEIGHT, platformWallTexture, Platform.PlatformType.VerticalX));
            walls[3].rotation = Matrix.CreateRotationX((float)Math.PI / 2);
            //ceiling and floor
            walls.Add(new Platform(new Vector3(0, Global.Constants.LEVEL_THREE_HEIGHT, 0), Global.Constants.LEVEL_THREE_WIDTH, Global.Constants.LEVEL_THREE_LENGTH, platformWallTexture, Platform.PlatformType.Horizontal));
            walls.Add(new Platform(new Vector3(0, -Global.Constants.LEVEL_THREE_HEIGHT, 0), Global.Constants.LEVEL_THREE_WIDTH, Global.Constants.LEVEL_THREE_LENGTH, platformWallTexture, Platform.PlatformType.Horizontal));

            //top platforms
            platforms.Add(new Platform(new Vector3(standardSpacing, standardSpacing, standardSpacing), mediumPlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(new Vector3(standardSpacing, -standardSpacing, standardSpacing), mediumPlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(new Vector3(standardSpacing, standardSpacing, -standardSpacing), mediumPlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(new Vector3(standardSpacing, -standardSpacing, -standardSpacing), mediumPlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            //middle platform
            platforms.Add(new Platform(Vector3.Zero, largePlatformSize, largePlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            //bottom platforms
            platforms.Add(new Platform(new Vector3(-standardSpacing, standardSpacing, standardSpacing), mediumPlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(new Vector3(-standardSpacing, -standardSpacing, standardSpacing), mediumPlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(new Vector3(-standardSpacing, standardSpacing, -standardSpacing), mediumPlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));
            platforms.Add(new Platform(new Vector3(-standardSpacing, -standardSpacing, -standardSpacing), mediumPlatformSize, mediumPlatformSize, platformTexture, Platform.PlatformType.Horizontal));

            //AI
            Global.bugBots.Add(new BugBot(new Vector3(-50, 60, 0), 1, .09f, new Vector3(0, 60, 50), new Vector3(0, -60, 50), new Vector3(50, -60, 0), new Vector3(50, 60, 0)));
            Global.bugBots.Add(new BugBot(new Vector3(50, 60, 0), 2, .09f, new Vector3(0, 60, -50), new Vector3(0, -60, -50), new Vector3(0, -60, 50), new Vector3(0, 60, 50)));

            Global.turrets.Add(new Turret(new Vector3(70, -70, 70)));
            Global.turrets.Add(new Turret(new Vector3(-70, -70, 70)));
            Global.turrets.Add(new Turret(new Vector3(70, -70, -70)));
            Global.turrets.Add(new Turret(new Vector3(-70, -70, -70)));
            Global.turrets.Add(new Turret(new Vector3(0, -70, 0)));

            //spawn points
            spawnPoints.Add(Vector3.Zero);
            spawnPoints.Add(new Vector3(40, 50, 40));
            spawnPoints.Add(new Vector3(-40, 50, -40));
            spawnPoints.Add(new Vector3(40, -30, 40));
            spawnPoints.Add(new Vector3(-40, -30, -40));
        }

        public void spawnAllPlayers()
        {
            List<Vector3> takenSpawns = new List<Vector3>();
            for (int i = 0; i < Global.localPlayers.Count; ++i)
            {
                int index = Global.rand.Next(0, spawnPoints.Count * 100) / 100; //better distribution C# Random sucks
                while(takenSpawns.Contains(spawnPoints[index]))
                    index = Global.rand.Next(0, spawnPoints.Count);
                Global.localPlayers[i].respawn(spawnPoints[index]);
                takenSpawns.Add(spawnPoints[index]);
            }
        }
    }
}
