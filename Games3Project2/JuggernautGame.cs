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
using Broad.Camera;
using Claudy.AxisReference;
using Claudy.Input;
using Claudy.Music;
using DeLeone.Cursor;

namespace Games3Project2
{
    public class JuggernautGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BroadCamera camera;
        Axis_Reference axisReference;

        //Game Generic Geometry
        DeLeone_Cursor cursor;
        //TODO: Add other Geometry?


        #region Debug Mode
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

        ClaudyInput input = ClaudyInput.Instance;

        Music music;

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
            axisReference = new Axis_Reference(GraphicsDevice, 1.0f);
            camera = new BroadCamera(this, new Vector3(-5f, 2f, -10f),
                Vector3.Zero, Vector3.Up);
            cursor = new DeLeone_Cursor(this, camera);
            Components.Add(cursor);

            this.IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            music = new Music(this);
            music.playBackgroundMusic();
        }

        protected override void UnloadContent()
        {
            music.stopBackgroundMusic();
        }

        protected override void Update(GameTime gameTime)
        {
            input.Update();
            // Allows the game to exit
            if (input.DetectBackPressedByAnyPlayer())
            {
                // TODO: Depending on the game state...behavior of back button will differ.
                this.Exit();
            }

            //If in the game session.
            camera.Update(gameTime, debugMode);
            cursor.Update(gameTime); //currently nop.

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //If in the game session and in debug mode.
            if (debugMode)
            {
                axisReference.Draw(Matrix.Identity, camera.view, camera.projection);
            }

            base.Draw(gameTime);
        }
    }
}
