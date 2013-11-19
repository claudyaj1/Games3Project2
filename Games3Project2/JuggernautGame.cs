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

using BoundingVolumeRendering;
using Camera3D;
using AxisReference;
using InputHandler;
using MusicClasses;
using ReticuleCursor;
using Geometry;
using Games3Project2.Globals;

namespace Games3Project2
{
    public class JuggernautGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        Axis_Reference axisReference;


        #region Debug Mode
        readonly Color debugColor = Color.DimGray;
        bool debugMode
        #if DEBUG
         = true;
        #else
         = false;
        #endif
        #endregion
        //TODO: Add Game States.
        public enum GameMode { MenuScreen, LobbyScreen, ScoreScreen};
        public GameMode currentGameMode = GameMode.MenuScreen;
        //TODO: Add Cole's Menu class code.

        SpriteFont consolas;
        SpriteFont tahoma;

        Music music;

        NetworkManagement networkManager;

        Texture2D cursorTex;
        LevelOne levelOne;

        public JuggernautGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
#if WINDOWS
            //Preserves the 1.7777 aspect ratio of 1920x1080 (16/9)
            //verses the 1.6 of 1280x800.
            //1280 / (16/9) = 720
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
#endif
        }

        protected override void Initialize()
        {
            Global.game = this;
            Global.graphics = graphics;
            Global.viewPort = Global.graphics.GraphicsDevice.Viewport.Bounds;
            Global.titleSafe = GetTitleSafeArea(.85f);
            axisReference = new Axis_Reference(GraphicsDevice, 1.0f);

            //Count the number of local players based upon controller count.
            for (int i = 1; i <= 4; i++)
            {
                if (Global.input.isConnected(i))
                {
                    Global.numLocalGamers++;
                    Global.numTotalGamers++;
                }
            }
            if (Global.numLocalGamers == 0)
            {
                //Hint: Keyboard.
                Global.numLocalGamers = 1;
                Global.numTotalGamers = 1;
            }
            //End Counting the # of gamers

            Global.localPlayers.Add(new LocalPlayer(Vector3.Zero, PlayerIndex.One, 1));

            networkManager = new NetworkManagement(this);

            this.IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            Global.spriteBatch = new SpriteBatch(GraphicsDevice);

            music = new Music(this);
            //music.playBackgroundMusic();

            consolas = Content.Load<SpriteFont>(@"Fonts/Consolas");
            tahoma = Content.Load<SpriteFont>(@"Fonts/Tahoma");
            cursorTex = Content.Load<Texture2D>(@"Textures\cursor");

            levelOne = new LevelOne();
        }

        protected override void UnloadContent()
        {
            music.stopBackgroundMusic();
        }

        protected override void Update(GameTime gameTime)
        {
            Global.gameTime = gameTime;
            Global.input.Update();
            // Allows the game to exit
            if (Global.input.DetectBackPressedByAnyPlayer())
            {
                // TODO: Depending on the game state...behavior of back button will differ.
                this.Exit();
            }
            if (Global.input.isFirstPress(Keys.OemTilde))
            {
                Global.debugMode = !Global.debugMode; //Toggle mode
            }

            foreach (LocalPlayer player in Global.localPlayers)
            {
                player.update();
            }

            levelOne.update();

            networkManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Global.gameTime = gameTime;
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (LocalPlayer player in Global.localPlayers)
            {
                Global.CurrentCamera = player.camera;
                Global.spriteBatch.Begin();

                levelOne.draw();
                foreach (LocalPlayer drawPlayer in Global.localPlayers)
                {
                    drawPlayer.draw();
                }
                //If in the game session and in debug mode.
                if (Global.debugMode)
                {
                    axisReference.Draw(Matrix.Identity, Global.CurrentCamera.view, Global.CurrentCamera.projection);
                    Global.spriteBatch.DrawString(consolas, "Press ~ to exit debug mode.",
                            new Vector2(5f, 35f), Color.PaleGreen);
                    Global.spriteBatch.DrawString(consolas, "Camera Position and View= " +
                        "X:" + Global.CurrentCamera.cameraPos.X.ToString() +
                        " Y:" + Global.CurrentCamera.cameraPos.Y.ToString() +
                        " Z:" + Global.CurrentCamera.cameraPos.Z.ToString(),
                        new Vector2(5f, 53f), debugColor);
                    Global.spriteBatch.DrawString(consolas,
                        "Up:" + Global.CurrentCamera.view.Up.ToString() +
                        "\nLookAt: " + Global.CurrentCamera.view.Forward.ToString() +
                        "\nRight: " + Global.CurrentCamera.view.Right.ToString(),
                        new Vector2(5f, 70f), debugColor);
                }

                Global.spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        protected Rectangle GetTitleSafeArea(float percent)
        {
            Rectangle retval = new Rectangle(Global.graphics.GraphicsDevice.Viewport.X, Global.graphics.GraphicsDevice.Viewport.Y,
                                             Global.graphics.GraphicsDevice.Viewport.Width, Global.graphics.GraphicsDevice.Viewport.Height);
#if XBOX
            float border = (1 - percent) / 2;
            retval.X = (int)(border * retval.Width);
            retval.Y = (int)(border * retval.Height);
            retval.Width = (int)(percent * retval.Width);
            retval.Height = (int)(percent * retval.Height);
#endif

            return retval;
        }
    }
}
