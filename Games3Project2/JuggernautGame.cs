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
using Sandham.Primative;

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
        Sandham_Ball sandball;

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

            //Geometry
            sandball = new Sandham_Ball(this, Color.Red, Vector3.One);
            sandball.SetCullMode(2);
            sandball.SetWireframe(0);

            this.IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            music = new Music(this);
            music.playBackgroundMusic();

            consolas = Content.Load<SpriteFont>(@"Fonts/Consolas");
            tahoma = Content.Load<SpriteFont>(@"Fonts/Tahoma");
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
            if (input.isFirstPress(Keys.OemTilde))
            {
                //Flip mode
                debugMode = !debugMode;
            }

            //If in the game session.
            camera.Update(gameTime, debugMode, PlayerIndex.One);
            cursor.Update(gameTime); //currently nop.

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            //TODO: Put drawing code here that expects spritebatch to have begun already.
            sandball.Draw(camera);

            //If in the game session and in debug mode.
            if (debugMode)
            {
                axisReference.Draw(Matrix.Identity, camera.view, camera.projection);
                spriteBatch.DrawString(consolas, "Press ~ to exit debug mode.",
                        new Vector2(5f, 35f), Color.PaleGreen);
                spriteBatch.DrawString(consolas, "Camera Position and View= " +
                    "X:" + camera.cameraPos.X.ToString() +
                    " Y:" + camera.cameraPos.Y.ToString() +
                    " Z:" + camera.cameraPos.Z.ToString(),
                    new Vector2(5f, 53f), debugColor);
                spriteBatch.DrawString(consolas,
                    "Up:" + camera.view.Up.ToString() +
                    " LookAt: " + camera.view.Forward.ToString() +
                    " Right: " + camera.view.Right.ToString(),
                    new Vector2(5f, 70f), debugColor);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
