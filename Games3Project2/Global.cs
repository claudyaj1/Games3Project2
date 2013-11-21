using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

using Camera3D;
using InputHandler;

namespace Games3Project2.Globals
{
    public static class Global
    {
        public static Input input = Input.Instance;
        public static NetworkManagement networkManager;
        public static List<LocalPlayer> localPlayers = new List<LocalPlayer>();
        public static List<RemotePlayer> remotePlayers = new List<RemotePlayer>();
        public static List<Bullet> bullets = new List<Bullet>();
        public static List<BugBot> bugBots = new List<BugBot>();
        public static SpriteBatch spriteBatch;
        public static GameTime gameTime;
        public static Game game;
        public enum GameState { Intro, Menu, SetupLocalPlayers, LevelPicking, NetworkWaitingHost, NetworkJoining, Playing, Paused, NetworkQuit, GameOver };
        public static GameState gameState = GameState.Intro;
        /// <summary>
        /// Describes the total number of gamers in the network session.
        /// </summary>
        public static byte numTotalGamers = 0;
        public static byte numLocalGamers = 0;
        public static bool debugMode 
        #region Debug Mode
        #if DEBUG
         = true;
        #else
         = false;
        #endif
        public static readonly Color debugColor = Color.Black;
        #endregion
        public static readonly Color HUD_COLOR = Color.WhiteSmoke;
        public static SpriteFont consolas;  //To be assigned in LoadContent()
        public static SpriteFont tahoma;    //To be assigned in LoadContent()

        public static Rectangle viewPort;
        public static Rectangle titleSafe;
        public static GraphicsDeviceManager graphics;
        public static Camera CurrentCamera
        {
            get { return currentCamera; }
            set
            {
                currentCamera = value;
                graphics.GraphicsDevice.Viewport = currentCamera.viewport;
            }
        }
        private static Camera currentCamera;
        

        public static class Constants
        {
            public static readonly byte MAX_PLAYERS_TOTAL = 4;
            public static readonly byte MAX_PLAYERS_LOCAL = 4;

            public static readonly float LEVEL_ONE_WIDTH = 100;
            public static readonly float LEVEL_ONE_LENGTH = 200;
            public static readonly float LEVEL_ONE_HEIGHT = 100;

            public static readonly float LEVEL_TWO_WIDTH = 100;
            public static readonly float LEVEL_TWO_LENGTH = 100;
            public static readonly float LEVEL_TWO_HEIGHT = 165;

            public static readonly float JET_PACK_INCREMENT = 0.00006f;
            public static readonly float JET_PACK_DECREMENT = JET_PACK_INCREMENT * 0.4f; //Yes, this must remain positive.
            public static readonly float JET_FUEL_INCREMENT = .3f;
            public static readonly float JET_FUEL_DECREMENT = .6f;
            public static readonly float JET_PACK_Y_VELOCITY_CAP = 0.0010f;
            public static readonly float GRAVITY = 0.0006f;
            public static readonly float MAX_JET_FUEL = 100f;

            public static readonly float MOVEMENT_VELOCITY = 3.1f;
            public static readonly float SPIN_RATE = 100f;
            public static readonly float PLAYER_RADIUS = 5f;

            public static readonly int MAX_HEALTH = 100;
            public static readonly int BULLET_HIT_HEALTH_IMPACT = 10;
            public static readonly int MAX_JUG_HEALTH = 300;
            public static readonly int JUG_BULLET_DAMAGE = 30;
            public static readonly float RIGHT_HANDED_WEAPON_OFFSET = 0.1f;
            public static readonly float BULLET_SPEED = 2f;
            public static readonly float BULLET_RADIUS = .5f;

            public static readonly float WALL_BUFFER = 5;

            public static readonly string MSG_IS_JUG = " is the juggernaut";
            public static readonly string MSG_KILLED_JUG = " killed the juggernaut";
            public static readonly string MSG_KILLED_BY_JUG_1ST = "You were killed by the juggernaut";
            public static readonly string MSG_KILLED_BY_JUG_3RD = " was killed by the juggernaut";
            public static readonly string HUD_HEALTH = "HEALTH: ";
            public static readonly string HUD_JET = "JETFUEL: ";
            public static readonly string HUD_SCORE = "YOUR SCORE: ";
            public static readonly string HUD_YOU_JUG = "YOU ARE THE JUGGERNAUT";
            public static readonly string MSG_JOINED = " joined";
            public static readonly string MSG_DISCONNECTED = " disconnected";
        }
    }
}
