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
        Camera camera;
        Axis_Reference axisReference;

        //Game Generic Geometry
        Cursor cursor;
        //TODO: Add other Geometry?
        Sphere ball;
        Cylinder column;

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

        Input input = Input.Instance;

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
            axisReference = new Axis_Reference(GraphicsDevice, 1.0f);
            camera = new Camera(this, new Vector3(-5f, 2f, -10f),
                Vector3.Zero, Vector3.Up);
            cursor = new Cursor(this, camera);
            cursor.Position = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            //Geometry
            ball = new Sphere(this, Color.Red, Vector3.One);
            ball.SetCullMode(2);
            ball.SetWireframe(0);
            column = new Cylinder(this, Color.Blue, 
                new Vector3(-3f, 0f, -3f));
            column.SetCullMode(2);
            column.SetWireframe(0);

            //Count the number of local players based upon controller count.
            for (int i = 1; i <= 4; i++)
            {
                if (input.isConnected(i))
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

            levelOne = new LevelOne(this);
        }

        protected override void UnloadContent()
        {
            music.stopBackgroundMusic();
        }

        protected override void Update(GameTime gameTime)
        {
            Global.gameTime = gameTime;
            input.Update();
            // Allows the game to exit
            if (input.DetectBackPressedByAnyPlayer())
            {
                // TODO: Depending on the game state...behavior of back button will differ.
                this.Exit();
            }
            if (input.isFirstPress(Keys.OemTilde))
            {
                debugMode = !debugMode; //Toggle mode
            }

            //If in the game session. e.g. if(networkManager.networkSession != null)
            camera.Update(debugMode, PlayerIndex.One);
            cursor.Update(); //currently nop.

            networkManager.Update(gameTime);

            //TODO: Remove or relocate these.
            ball.Update(gameTime);
            column.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Global.gameTime = gameTime;
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Global.spriteBatch.Begin();

            //TODO: Put drawing code here that expects spritebatch to have begun already.
            //sandball.Draw(camera);
            //sandColumn.Draw(camera);
            levelOne.draw(camera);
            cursor.Draw();
            //If in the game session and in debug mode.
            if (debugMode)
            {
                axisReference.Draw(Matrix.Identity, camera.view, camera.projection);
                Global.spriteBatch.DrawString(consolas, "Press ~ to exit debug mode.",
                        new Vector2(5f, 35f), Color.PaleGreen);
                Global.spriteBatch.DrawString(consolas, "Camera Position and View= " +
                    "X:" + camera.cameraPos.X.ToString() +
                    " Y:" + camera.cameraPos.Y.ToString() +
                    " Z:" + camera.cameraPos.Z.ToString(),
                    new Vector2(5f, 53f), debugColor);
                Global.spriteBatch.DrawString(consolas,
                    "Up:" + camera.view.Up.ToString() +
                    " LookAt: " + camera.view.Forward.ToString() +
                    " Right: " + camera.view.Right.ToString(),
                    new Vector2(5f, 70f), debugColor);
            }
            Global.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
