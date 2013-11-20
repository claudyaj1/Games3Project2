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
using MenuUtility;
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

        SpriteFont consolas;
        SpriteFont tahoma;

        Music music;
        Texture2D splashTexture;

        int splashTimer = 0;
        const int SPLASH_LENGTH = 3000;

        Menu mainMenu;
        Level levelManager;

        //local player joining
        List<PlayerIndex> connectedPlayers = new List<PlayerIndex>();
        List<PlayerIndex> joinedPlayers = new List<PlayerIndex>();

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
            Global.networkManager = new NetworkManagement();
            Global.graphics = graphics;
            Global.viewPort = Global.graphics.GraphicsDevice.Viewport.Bounds;
            Global.titleSafe = GetTitleSafeArea(.85f);
            axisReference = new Axis_Reference(GraphicsDevice, 1.0f);

            

            this.IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to drawWalls textures.
            Global.spriteBatch = new SpriteBatch(GraphicsDevice);

            music = new Music(this);
            //music.playBackgroundMusic();
            splashTexture = Content.Load<Texture2D>(@"Textures\splash");
            consolas = Content.Load<SpriteFont>(@"Fonts/Consolas");
            tahoma = Content.Load<SpriteFont>(@"Fonts/Tahoma");

            List<String> menuOptions = new List<String>();
            menuOptions.Add("Create New Game");
            menuOptions.Add("Join Game");
            menuOptions.Add("Exit");
            mainMenu = new Menu(menuOptions, "Juggernaut", new Vector2(Global.titleSafe.Left + 30,
                Global.viewPort.Height / 2 - (menuOptions.Count / 2 * consolas.MeasureString("C").Y)));

            levelManager = new Level();
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

            switch (Global.gameState)
            {
                #region Intro
                case Global.GameState.Intro:
                    splashTimer += Global.gameTime.ElapsedGameTime.Milliseconds;
                    if (splashTimer > SPLASH_LENGTH || 
                        Global.input.isFirstPress(Buttons.A) || 
                        Global.input.isFirstPress(Buttons.Start) ||
                        Global.input.isFirstPress(Keys.Space) || 
                        Global.input.isFirstPress(Keys.Enter) ||
                        Global.input.isFirstPress(Keys.A))
                    {
                        splashTimer = 0;
                        Global.gameState = Global.GameState.Menu;
                    }
                    break;
                #endregion
                #region Menu
                case Global.GameState.Menu:
                    switch (mainMenu.update())
                    {
                        case 0:
                            Global.networkManager.isHost = true;
                            Global.gameState = Global.GameState.SetupLocalPlayers;
                            break;
                        case 1:
                            Global.networkManager.isHost = false;
                            Global.gameState = Global.GameState.SetupLocalPlayers;
                            break;
                        case 2:
                            this.Exit();
                            break;
                    }
                    break;
                #endregion
                #region SetupLocalPlayers
                case Global.GameState.SetupLocalPlayers:
                    if (setupLocalPlayers())
                    {
                        if (Global.networkManager.isHost)
                        {
                            Global.gameState = Global.GameState.LevelPicking;
                        }
                        else
                        {
                            //Global.gameState = Global.GameState.NetworkJoining;
                            Global.gameState = Global.GameState.Playing;
                            //TEMP
                            levelManager.setupLevelOne();
                        }
                    }
                    if (Global.input.isFirstPress(Keys.Back))
                    {
                        Global.gameState = Global.GameState.Menu;
                    }
                    break;
                #endregion
                #region LevelPicking:
                case Global.GameState.LevelPicking:
                    if (updateLevelPicking())
                    {
                        //Global.gameState = Global.GameState.NetworkWaitingHost;
                        Global.gameState = Global.GameState.Playing;
                    }
                    break;
                #endregion
                #region NetworkJoining
                case Global.GameState.NetworkJoining:

                    break;
                #endregion
                #region NetworkWaitingHost
                case Global.GameState.NetworkWaitingHost:

                    break;
                #endregion
                #region Playing
                case Global.GameState.Playing:
                    foreach (LocalPlayer player in Global.localPlayers)
                    {
                        player.update();
                    }

                    levelManager.update();
                    break;
                #endregion
                #region Paused
                case Global.GameState.Paused:

                    break;
                #endregion
                #region GameOver
                case Global.GameState.GameOver:

                    break;
                #endregion
                #region NetworkQuit
                case Global.GameState.NetworkQuit:

                    break;
                #endregion
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Global.gameTime = gameTime;
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Global.spriteBatch.Begin();

            switch (Global.gameState)
            {
                #region Intro
                case Global.GameState.Intro:
                    Global.spriteBatch.Draw(splashTexture, Global.viewPort, Color.White);
                    break;
                #endregion
                #region Menu
                case Global.GameState.Menu:
                    mainMenu.draw();
                    break;
                #endregion
                #region SetupLocalPlayers
                case Global.GameState.SetupLocalPlayers:
                    drawLocalPlayerSetup();
                    break;
                #endregion
                #region LevelPicking:
                case Global.GameState.LevelPicking:
                    drawLevelPicking();
                    break;
                #endregion
                #region NetworkJoining
                case Global.GameState.NetworkJoining:

                    break;
                #endregion
                #region NetworkWaitingHost
                case Global.GameState.NetworkWaitingHost:

                    break;
                #endregion
                #region Playing
                case Global.GameState.Playing:
                    
                    foreach (LocalPlayer player in Global.localPlayers)
                    {
                        //necessary for multiplayer cursors to drawWalls properly.
                        Global.spriteBatch.End();
                        Global.CurrentCamera = player.camera;
                        Global.spriteBatch.Begin();
                        
                        levelManager.drawWalls();
                        levelManager.drawPlatforms();
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
                            Global.spriteBatch.DrawString(consolas, "Camera Position and View=\n" +
                                "X:" + Global.CurrentCamera.cameraPos.X.ToString() +
                                " Y:" + Global.CurrentCamera.cameraPos.Y.ToString() +
                                " Z:" + Global.CurrentCamera.cameraPos.Z.ToString(),
                                new Vector2(5f, 53f), Global.debugColor);
                            Global.spriteBatch.DrawString(consolas,
                                "Up:" + Global.CurrentCamera.view.Up.ToString() +
                                "\nLookAt: " + Global.CurrentCamera.view.Forward.ToString() +
                                "\nRight: " + Global.CurrentCamera.view.Right.ToString(),
                                new Vector2(5f, 95f), Global.debugColor);
                        }
                    }
                    break;
                #endregion
                #region Paused
                case Global.GameState.Paused:

                    break;
                #endregion
                #region GameOver
                case Global.GameState.GameOver:

                    break;
                #endregion
                #region NetworkQuit
                case Global.GameState.NetworkQuit:

                    break;
                #endregion
            }

            Global.spriteBatch.End();
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

        private bool setupLocalPlayers()
        {
            //Dynamically update the connected controllers
            for (PlayerIndex index = PlayerIndex.One; index <= PlayerIndex.Four; index++)
            {
                if (Global.input.isConnected(index) && !connectedPlayers.Contains(index) && !joinedPlayers.Contains(index))
                {
                    connectedPlayers.Add(index);
                }
                if (!Global.input.isConnected(index) && connectedPlayers.Contains(index))
                {
                    connectedPlayers.Remove(index);
                }
            }

            for (int i = 0; i < connectedPlayers.Count; ++i)
            {
                if(Global.input.isFirstPress(Buttons.A, connectedPlayers[i]))
                {
                    joinedPlayers.Add(connectedPlayers[i]);
                    connectedPlayers.RemoveAt(i--);
                }
            }

            foreach (PlayerIndex index in joinedPlayers)
            {
                if (Global.input.isFirstPress(Buttons.Start, index))
                {
                    for (int i = 0; i < joinedPlayers.Count; ++i)
                    {
                        Global.localPlayers.Add(new LocalPlayer(new Vector3(0, 20, 0), joinedPlayers[i], i + 1));
                    }
                    return true;
                }
            }

            if (joinedPlayers.Count == 4)
            {
                return true;
            }

            return false;
        }

        private void drawLocalPlayerSetup()
        {
            Global.spriteBatch.Draw(mainMenu.background, Global.viewPort, Color.White);
            float yOffset = consolas.MeasureString("A").Y;
            Vector2 startingPosition = new Vector2(Global.titleSafe.Left + 30, Global.titleSafe.Top + 100);

            for (PlayerIndex index = PlayerIndex.One; index <= PlayerIndex.Four; index++)
            {
                if (!Global.input.isConnected(index))
                {
                    Global.spriteBatch.DrawString(consolas, "Controller " + index.ToString() + " Disconnected", new Vector2(startingPosition.X, startingPosition.Y + (int)index * yOffset), Color.Red);
                }
                else if(connectedPlayers.Contains(index))
                {
                    Global.spriteBatch.DrawString(consolas, "Press A to Join", new Vector2(startingPosition.X, startingPosition.Y + (int)index * yOffset), Color.Black);
                }
                else if (joinedPlayers.Contains(index))
                {
                    Global.spriteBatch.DrawString(consolas, "Joined", new Vector2(startingPosition.X, startingPosition.Y + (int)index * yOffset), Color.Green);
                }
            }

            if (joinedPlayers.Count > 0)
            {
                Global.spriteBatch.DrawString(consolas, "Press Start to Continue", new Vector2(startingPosition.X, startingPosition.Y + 6 * yOffset), Color.Black);
            }
        }

        private bool updateLevelPicking()
        {
            bool didSelect = false;
            if (Global.input.isFirstPress(Buttons.A, Global.localPlayers[0].playerIndex) ||
                Global.input.isFirstPress(Keys.A) ||
                Global.input.isFirstPress(Keys.Enter))
            {
                levelManager.setupLevelOne();
                didSelect = true;
            }
            else if (Global.input.isFirstPress(Buttons.B, Global.localPlayers[0].playerIndex) ||
                Global.input.isFirstPress(Keys.B))
            {
                levelManager.setupLevelTwo();
                didSelect = true;
            }

            return didSelect;
        }

        private void drawLevelPicking()
        {
            Global.spriteBatch.Draw(mainMenu.background, Global.viewPort, Color.White);
            float yOffset = consolas.MeasureString("A").Y;
            Vector2 startingPosition = new Vector2(Global.titleSafe.Left + 30, Global.titleSafe.Top + 100);

            Global.spriteBatch.DrawString(consolas, "Press A For Level One", startingPosition, Color.Black);
            Global.spriteBatch.DrawString(consolas, "Press B For Level Two", new Vector2(startingPosition.X, startingPosition.Y + yOffset), Color.Black);
        }
    }
}
