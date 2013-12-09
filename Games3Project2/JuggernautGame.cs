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
using Microsoft.Xna.Framework.Net;

using BoundingVolumeRendering;
using Camera3D;
using AxisReference;
using InputHandler;
using MenuUtility;
using MusicClasses;
using ReticuleCursor;
using Geometry;
using Games3Project2.Globals;
using Networking;

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
        Texture2D background;

        int splashTimer = 0;
        const int SPLASH_LENGTH = 3000;

        Menu mainMenu;
        Menu levelMenu;
        Menu createGameMenu;
        Menu joinGameMenu;
        Level levelManager;

        Boolean debug = true;

        //local player joining
        List<PlayerIndex> connectedPlayers = new List<PlayerIndex>();
        List<PlayerIndex> joinedPlayers = new List<PlayerIndex>();

        public JuggernautGame()
        {
            Components.Add(new GamerServicesComponent(this));
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
            Global.networkManager = new NetworkManager();
            Global.graphics = graphics;
            Global.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Global.viewPort = Global.graphics.GraphicsDevice.Viewport.Bounds;
            Global.titleSafe = GetTitleSafeArea(.85f);
            axisReference = new Axis_Reference(GraphicsDevice, 1.0f);
            BoundingSphereRenderer.Initialize(GraphicsDevice, 45);
            Global.heatmapKills = new Heatmap(Color.Black);
            Global.heatmapDeaths = new Heatmap(Color.Red);
            Global.heatmapUsedJetpack = new Heatmap(Color.Green);

            this.IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to drawWalls textures.
            Global.spriteBatch = new SpriteBatch(GraphicsDevice);

            Global.shot = Global.game.Content.Load<SoundEffect>(@"Audio/plasma").CreateInstance();
            Global.jetpack = Global.game.Content.Load<SoundEffect>(@"Audio/cole").CreateInstance();
            Global.jetpack.IsLooped = true;
            Global.menusong = Global.game.Content.Load<SoundEffect>(@"Audio/menusong").CreateInstance();
            Global.menusong.IsLooped = true;
            Global.actionsong = Global.game.Content.Load<SoundEffect>(@"Audio/actionsong").CreateInstance();
            Global.actionsong.IsLooped = true;
            Global.actionsong.Volume = 0.5f;

            music = new Music(this);
            //music.playBackgroundMusic();
            splashTexture = Content.Load<Texture2D>(@"Textures\splash");
            consolas = Content.Load<SpriteFont>(@"Fonts/Consolas");
            Global.consolas = consolas;
            tahoma = Content.Load<SpriteFont>(@"Fonts/Tahoma");
            Global.tahoma = tahoma;
            background = Content.Load<Texture2D>(@"Textures\menubackground");

            List<String> menuOptions = new List<String>();
            menuOptions.Add("Single Player");
            menuOptions.Add("Create New Multiplayer Game");
            menuOptions.Add("Join Multiplayer Game");
            menuOptions.Add("Heatmaps");
            menuOptions.Add("Exit");
            mainMenu = new Menu(menuOptions, "Juggernaut", new Vector2(Global.titleSafe.Left + 30,
                Global.viewPort.Height / 2 - (menuOptions.Count / 2 * consolas.MeasureString("C").Y)));

            List<String> levelOptions = new List<string>();
            levelOptions.Add("Level One");
            levelOptions.Add("Level Two");
            levelOptions.Add("Level Three");
            levelMenu = new Menu(levelOptions, "Juggernaut", new Vector2(Global.titleSafe.Left + 30,
                Global.viewPort.Height / 2 - (menuOptions.Count / 2 * consolas.MeasureString("C").Y)));

            List<String> gameOptions = new List<String>();
            gameOptions.Add("Local");
            gameOptions.Add("System Link");
            gameOptions.Add("Player Match");
            createGameMenu = new Menu(gameOptions, "Juggernaut", new Vector2(Global.titleSafe.Left + 30,
                Global.viewPort.Height / 2 - (menuOptions.Count / 2 * consolas.MeasureString("C").Y)));

            List<String> joinGameOptions = new List<string>();
            joinGameOptions.Add("System Link");
            joinGameOptions.Add("Player Match");
            joinGameMenu = new Menu(joinGameOptions, "Juggernaut", new Vector2(Global.titleSafe.Left + 30,
                Global.viewPort.Height / 2 - (menuOptions.Count / 2 * consolas.MeasureString("C").Y)));

            levelManager = new Level();
            Global.levelManager = levelManager;

            for (int i = 0; i < Global.Constants.MAX_ALLOCATED_BULLETS; ++i)
            {
                Global.BulletManager.bullets.Add(new Bullet());
            }
        }

        protected override void UnloadContent()
        {
            music.stopBackgroundMusic();
        }

        protected override void Update(GameTime gameTime)
        {
            Global.gameTime = gameTime;
            Global.input.Update();
            Global.networkManager.update();
            // Allows the game to exit
            if (Global.input.DetectBackPressedByAnyPlayer())
            {
                // TODO: Depending on the game state...behavior of back button will differ.
                this.Exit();
            }
            if (Global.input.isFirstPress(Keys.OemTilde) ||
                Global.input.isFirstPress(Buttons.DPadDown, PlayerIndex.One) ||
                Global.input.isFirstPress(Buttons.DPadDown, PlayerIndex.Two) ||
                Global.input.isFirstPress(Buttons.DPadDown, PlayerIndex.Three) ||
                Global.input.isFirstPress(Buttons.DPadDown, PlayerIndex.Four))
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
                #endregion //Intro

                #region Menu
                case Global.GameState.Menu:
                    Global.actionsong.Stop();
                    Global.menusong.Play();
                    switch (mainMenu.update())
                    {
                        case 0: //Single Player

                            break;
                        case 1: //Create New Game (Networking or Local)
                            Global.networkManager.hostSessionType = NetworkManager.HostSessionType.Host;
                            Global.gameState = Global.GameState.SetupLocalPlayers;
                            break;
                        case 2: //Join Game
                            //local player joining code
                            Global.gameState = Global.GameState.SetupLocalPlayers;
                            Global.networkManager.hostSessionType = NetworkManager.HostSessionType.Client;
                            break;
                        case 3: //Heatmaps
                            this.Exit();
                            Global.gameState = Global.GameState.SetupLocalPlayersHeatmap;
                            Global.numLocalGamers = 1;
                            joinedPlayers.Clear();
                            joinedPlayers.Add(PlayerIndex.One);
                            break;
                        case 4: //Exit
                            this.Exit();
                            break;
                    }
                    break;
                #endregion //Menu

                #region CreateMenu
                case Global.GameState.CreateMenu:
                    if (Global.input.isFirstPress(Buttons.B))
                    {
                        Global.gameState = Global.GameState.Menu;
                    }
                    switch (createGameMenu.update())
                    {
                        case 0: //Local
                            Global.networkManager.sessionType = NetworkSessionType.Local;
                            Global.gameState = Global.GameState.LevelPicking;
                            break;
                        case 1: //System Link
                            Global.networkManager.sessionType = NetworkSessionType.SystemLink;
                            Global.gameState = Global.GameState.LevelPicking;
                            break;
                        case 2: //Player Match
                            Global.networkManager.sessionType = NetworkSessionType.PlayerMatch;
                            Global.gameState = Global.GameState.LevelPicking;
                            break;
                    }
                    break;
                #endregion //CreateMenu

                #region JoinMenu
                case Global.GameState.JoinMenu:
                    if (Global.input.isFirstPress(Buttons.B))
                    {
                        Global.gameState = Global.GameState.Menu;
                    }
                    switch (joinGameMenu.update())
                    {
                        case 0: //System Link
                            Global.networkManager.sessionType = NetworkSessionType.SystemLink;
                            Global.gameState = Global.GameState.Lobby;
                            int localIndex1 = 1;
                            if (Global.networkManager.joinSession())
                            {
                                foreach (LocalNetworkGamer gamer in Global.networkManager.networkSession.LocalGamers)
                                {
                                    gamer.Tag = new LocalPlayer(Vector3.Zero, gamer.SignedInGamer.PlayerIndex, localIndex1++, gamer);
                                    Global.localPlayers.Add((LocalPlayer)gamer.Tag);
                                }
                            }
                            break;
                        case 1: //Player Match
                            Global.networkManager.sessionType = NetworkSessionType.PlayerMatch;
                            Global.gameState = Global.GameState.Lobby;
                            int localIndex2 = 1;
                            if(Global.networkManager.joinSession())
                            {
                                foreach (LocalNetworkGamer gamer in Global.networkManager.networkSession.LocalGamers)
                                {
                                    gamer.Tag = new LocalPlayer(Vector3.Zero, gamer.SignedInGamer.PlayerIndex, localIndex2++, gamer);
                                    Global.localPlayers.Add((LocalPlayer)gamer.Tag);
                                }
                            }
                            break;
                    }
                    break;
                #endregion //JoinMenu

                #region SetupLocalPlayers
                case Global.GameState.SetupLocalPlayers:
                    setupLocalPlayers();
                    break;
                #endregion //SetupLocalPlayers

                #region LevelPicking
                case Global.GameState.LevelPicking:
                    if (Global.input.isFirstPress(Buttons.B))
                    {
                        Global.gameState = Global.GameState.Menu;
                    }
                    switch (levelMenu.update())
                    {
                        case 0:
                            Global.gameState = Global.GameState.Lobby;
                            Global.levelManager.currentLevel = 1;
                            int localIndex = 1;
                            if (Global.networkManager.createSession())
                            {
                                foreach (LocalNetworkGamer gamer in Global.networkManager.networkSession.LocalGamers)
                                {
                                    gamer.Tag = new LocalPlayer(Vector3.Zero, gamer.SignedInGamer.PlayerIndex, localIndex++, gamer);
                                    Global.localPlayers.Add((LocalPlayer)gamer.Tag);
                                }
                            }
                            break;
                        case 1:
                            Global.gameState = Global.GameState.Lobby;
                            Global.levelManager.currentLevel = 2;
                            localIndex = 1;
                            if (Global.networkManager.createSession())
                            {
                                foreach (LocalNetworkGamer gamer in Global.networkManager.networkSession.LocalGamers)
                                {
                                    gamer.Tag = new LocalPlayer(Vector3.Zero, gamer.SignedInGamer.PlayerIndex, localIndex++, gamer);
                                    Global.localPlayers.Add((LocalPlayer)gamer.Tag);
                                }
                            }
                            break;
                        case 2:
                            Global.gameState = Global.GameState.Lobby;
                            Global.networkManager.createSession();
                            Global.levelManager.currentLevel = 3;
                            localIndex = 1;
                            foreach (LocalNetworkGamer gamer in Global.networkManager.networkSession.LocalGamers)
                            {
                                gamer.Tag = new LocalPlayer(Vector3.Zero, gamer.SignedInGamer.PlayerIndex, localIndex++, gamer);
                                Global.localPlayers.Add((LocalPlayer)gamer.Tag);
                            }
                            break;
                    }
                    break;
                #endregion //LevelPicking

                #region Lobby
                case Global.GameState.Lobby:
                    if (Global.networkManager.currentState == NetworkManager.CurrentState.Running)
                    {
                        if (Global.input.isFirstPress(Buttons.B))
                        {
                            Global.gameState = Global.GameState.Menu;
                            Global.networkManager.disposeNetworkSession();
                        }
                        else if(Global.networkManager.hostSessionType == NetworkManager.HostSessionType.Host &&
                            Global.networkManager.networkSession.AllGamers.Count > 1 && 
                            (Global.input.isFirstPress(Buttons.A) || Global.input.isFirstPress(Buttons.Start)))
                        {
                            int firstJugIndex = Global.rand.Next(0, Global.networkManager.networkSession.AllGamers.Count);
                            NetworkGamer firstJug = Global.networkManager.networkSession.AllGamers[firstJugIndex];
                            Global.networkManager.newJuggernaut(firstJug);
                            if (firstJug.IsLocal)
                            {
                                LocalPlayer player = firstJug.Tag as LocalPlayer;
                                player.isJuggernaut = true;
                            }
                            Global.gameState = Global.GameState.Playing;
                            Global.networkManager.networkSession.StartGame();
                        }
                    }
                    break;
                #endregion //Lobby

                #region SetupLocalPlayersHeatmap
                case Global.GameState.SetupLocalPlayersHeatmap:
                    //if (setupLocalPlayers())
                    {
                        if (Global.networkManager.hostSessionType == NetworkManager.HostSessionType.Host)
                        {
                            Global.gameState = Global.GameState.ChooseHeatmap;
                        }
                        
                    }
                    if (Global.input.isFirstPress(Keys.Back))
                    {
                        Global.gameState = Global.GameState.Menu;
                    }
                    break;
                #endregion

                #region ChooseHeatmap
                case Global.GameState.ChooseHeatmap:
                    switch (levelMenu.update())
                    {
                        case 0: //Level 1
                            levelManager.currentLevel = 1;
                            levelManager.setupLevel();
                            Global.gameState = Global.GameState.playingHeatmap;
                            break;
                        case 1: //Level 2
                            levelManager.currentLevel = 2;
                            levelManager.setupLevel();
                            Global.gameState = Global.GameState.playingHeatmap;
                            break;
                        case 2: //Level 3
                            levelManager.currentLevel = 3;
                            levelManager.setupLevel();
                            break;
                    }
                    break;
                #endregion

                #region PlayHeatmap
                case Global.GameState.playingHeatmap:
                    Global.menusong.Stop();
                    Global.actionsong.Play();
                    
                    debug = true;
                    if (!Global.debugMode)
                        Global.debugMode = true;
                    foreach (LocalPlayer player in Global.localPlayers)
                    {
                        player.update();
                        //TODO:if(player.isJuggernaught == true) then do the bugbot check else do nothing
                        foreach (BugBot bot in Global.bugBots)
                        {
                            if ((bot.position - player.Position).Length() < BugBot.ATTACK_RADIUS)
                            {
                                //shoot a bullet at the player if he is the juggernaught
                                Vector3 dir = player.Position - bot.position;
                                dir.Normalize();
                                //bot.ShootBullet(dir);
                                debug = false;
                            }
                        }
                    }

                    Global.BulletManager.update();

                    levelManager.update();
                    break;
                #endregion

                #region Playing
                case Global.GameState.Playing:
                    Global.menusong.Stop();
                    Global.actionsong.Play();
                    debug = true;
                    foreach (LocalPlayer player in Global.localPlayers)
                    {
                        player.update();
                        if (player.score >= Global.Constants.MAX_SCORE)
                        {
                            for (int i = 0; i < Global.localPlayers.Count; i++)
                            {
                                Global.localPlayers[i].score = 0;
                            }
                            Global.gameState = Global.GameState.GameOver;
                            Global.winningPlayer = player.gamer.Gamertag;
                            Global.graphics.GraphicsDevice.Viewport = new Viewport(0, 0, Global.viewPort.Width, Global.viewPort.Height);
                            Global.networkManager.announceWinner(player.gamer);
                            Global.networkManager.disposeNetworkSession();
                        }
                    }

                    foreach (RemotePlayer rPlayer in Global.remotePlayers)
                    {
                        rPlayer.update();
                    }

                    Global.BulletManager.update();

                    levelManager.update();
                    break;
                #endregion //Playing

                #region Paused
                case Global.GameState.Paused:

                    break;
                #endregion //Paused

                #region GameOver
                case Global.GameState.GameOver:
                    foreach (LocalPlayer player in Global.localPlayers)
                    {
                        if (Global.input.isFirstPress(Buttons.A, player.playerIndex))
                        {
                            Global.gameState = Global.GameState.Menu;
                        }
                    }
                    break;
                #endregion //GameOver

                #region NetworkQuit
                case Global.GameState.NetworkQuit:
                    if (Global.input.isAnyFirstPress(Buttons.A) || Global.input.isAnyFirstPress(Buttons.Start))
                        Global.gameState = Global.GameState.Menu;
                    break;
                #endregion //NetworkQuit
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Global.gameTime = gameTime;
            GraphicsDevice.Clear(Color.CornflowerBlue);

            switch (Global.gameState)
            {
                #region Intro
                case Global.GameState.Intro:
                    Global.spriteBatch.Begin();
                    Global.spriteBatch.Draw(splashTexture, Global.viewPort, Color.White);
                    Global.spriteBatch.End();
                    break;
                #endregion

                #region Menu
                case Global.GameState.Menu:
                    Global.spriteBatch.Begin();
                    mainMenu.draw();
                    Global.spriteBatch.End();
                    break;
                #endregion

                #region CreateMenu
                case Global.GameState.CreateMenu:
                    Global.spriteBatch.Begin();
                    createGameMenu.draw();
                    Global.spriteBatch.End();
                    break;
                #endregion //CreateMenu

                #region JoinMenu
                case Global.GameState.JoinMenu:
                    Global.spriteBatch.Begin();
                    joinGameMenu.draw();
                    Global.spriteBatch.End();
                    break;
                #endregion //JoinMenu

                #region SetupLocalPlayers
                case Global.GameState.SetupLocalPlayers:
                    Global.spriteBatch.Begin();
                    drawLocalPlayerSetup();
                    Global.spriteBatch.End();
                    break;
                #endregion //SetupLocalPlayers

                #region LevelPicking:
                case Global.GameState.LevelPicking:
                    Global.spriteBatch.Begin();
                    levelMenu.draw();
                    Global.spriteBatch.End();
                    break;
                #endregion //LevelPicking

                #region Lobby
                case Global.GameState.Lobby:
                    Global.spriteBatch.Begin();
                    Global.networkManager.draw();
                    if (Global.networkManager.currentState == NetworkManager.CurrentState.Running)
                    {
                        Global.spriteBatch.Draw(background, Global.viewPort, Color.White);

                        Vector2 playerTextStarting = new Vector2(Global.viewPort.Left + 30, Global.viewPort.Height / 2 - 30);
                        Vector2 promptStarting = new Vector2(Global.viewPort.Left + 30, Global.viewPort.Height - 50);
                        float textOffset = consolas.MeasureString("A").Y + 5;

                        Global.spriteBatch.DrawString(consolas, "Joined Players:", playerTextStarting, Color.Black);
                        int off = 1;
                        foreach (NetworkGamer nGamer in Global.networkManager.networkSession.AllGamers)
                        {
                            Global.spriteBatch.DrawString(consolas, nGamer.Gamertag, new Vector2(playerTextStarting.X, playerTextStarting.Y + off++ * textOffset), Color.Black);
                        }

                        if (Global.networkManager.networkSession.AllGamers.Count > 1 && Global.networkManager.hostSessionType == NetworkManager.HostSessionType.Host)
                        {
                            Global.spriteBatch.DrawString(consolas, "Press A to Start the Game", promptStarting, Color.Black);
                        }
                        Global.spriteBatch.DrawString(consolas, "Press B to Return", new Vector2(promptStarting.X, promptStarting.Y + textOffset), Color.Black);
                    }
                    Global.spriteBatch.End();
                    break;
                #endregion //Lobby

                #region SetupLocalPlayersHeatmap
                case Global.GameState.SetupLocalPlayersHeatmap:
                    Global.spriteBatch.Begin();
                    drawLocalPlayerSetup();
                    Global.spriteBatch.End();
                    break;
                #endregion

                #region ChooseHeatmap
                case Global.GameState.ChooseHeatmap:
                    Global.spriteBatch.Begin();
                    levelMenu.draw();
                    Global.spriteBatch.End();
                    break;
                #endregion

                #region PlayHeatmap
                case Global.GameState.playingHeatmap:
                    //3D Drawing Section
                    resetGraphicsDevice();
                    foreach (LocalPlayer player in Global.localPlayers)
                    {
                        Global.CurrentCamera = player.camera;

                        levelManager.drawWalls();
                        levelManager.drawPlatforms();
                        foreach (LocalPlayer drawPlayer in Global.localPlayers)
                        {
                            drawPlayer.draw();
                        }
                        Global.BulletManager.draw();
                    }

                    //draw the heatmaps when debug mode is ran.
                    Global.heatmapKills.draw();
                    Global.heatmapDeaths.draw();
                    Global.heatmapUsedJetpack.draw();

                    //SpriteBatch Drawing Section
                    Global.spriteBatch.Begin();

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
                            "\nLookAt: " + debug.ToString() +
                            "\nRight: " + Global.CurrentCamera.view.Right.ToString(),
                            new Vector2(5f, 95f), Global.debugColor);

                    }

                    foreach (LocalPlayer drawPlayer in Global.localPlayers)
                    {
                        Global.spriteBatch.End();
                        Global.CurrentCamera = drawPlayer.camera;
                        Global.spriteBatch.Begin();
                        drawPlayer.drawHUD();
                    }

                    Global.spriteBatch.End();
                    break;
                #endregion //PlayHeatmap

                #region Playing
                case Global.GameState.Playing:
                    //3D Drawing Section
                    resetGraphicsDevice();
                    foreach (LocalPlayer player in Global.localPlayers)
                    {
                        Global.CurrentCamera = player.camera;
                        
                        levelManager.drawWalls();
                        levelManager.drawPlatforms();
                        foreach (LocalPlayer drawPlayer in Global.localPlayers)
                        {
                            drawPlayer.draw();
                        }
                        Global.BulletManager.draw();
                        foreach (RemotePlayer rPlayer in Global.remotePlayers)
                        {
                            rPlayer.draw();
                        }
                    }

                    

                    //SpriteBatch Drawing Section
                    Global.spriteBatch.Begin();
                    
                    if (Global.debugMode)
                    {
                        //draw the heatmaps when debug mode is ran.
                        Global.heatmapKills.draw();
                        Global.heatmapDeaths.draw();
                        Global.heatmapUsedJetpack.draw();

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
                            "\nLookAt: " + debug.ToString() +
                            "\nRight: " + Global.CurrentCamera.view.Right.ToString(),
                            new Vector2(5f, 95f), Global.debugColor);

                    }

                    foreach (LocalPlayer drawPlayer in Global.localPlayers)
                    {
                        Global.spriteBatch.End();
                        Global.CurrentCamera = drawPlayer.camera;
                        Global.spriteBatch.Begin();
                        drawPlayer.drawHUD();
                    }

                    Global.spriteBatch.End();
                    break;
                #endregion

                #region Paused
                case Global.GameState.Paused:
                    Global.spriteBatch.Begin();

                    Global.spriteBatch.End();
                    break;
                #endregion

                #region GameOver
                case Global.GameState.GameOver:
                    Global.spriteBatch.Begin();

                    Global.spriteBatch.Draw(mainMenu.background, Global.viewPort, Color.White);
                    Global.spriteBatch.DrawString(consolas, "Player " + Global.winningPlayer + " Won!", new Vector2(20, Global.viewPort.Height / 2), Color.Black);
                    Global.spriteBatch.DrawString(consolas, "Press A To Continue", new Vector2(20, Global.viewPort.Height / 2 + 50), Color.Black);

                    Global.spriteBatch.End();
                    break;
                #endregion

                #region NetworkQuit
                case Global.GameState.NetworkQuit:
                    Global.spriteBatch.Begin();
                    
                    Global.spriteBatch.Draw(mainMenu.background, Global.viewPort, Color.White);
                    Global.spriteBatch.DrawString(consolas, "Network Session Ended", new Vector2(20, Global.viewPort.Height / 2), Color.Black);
                    Global.spriteBatch.DrawString(consolas, "Press A To Continue", new Vector2(20, Global.viewPort.Height / 2 + 50), Color.Black);

                    Global.spriteBatch.End();
                    break;
                #endregion
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

        private void setupLocalPlayers()
        {
            if (Global.input.isAnyFirstPress(Buttons.A) && !Guide.IsVisible)
            {
#if XBOX
                Guide.ShowSignIn(4, true);
#else
                Guide.ShowSignIn(1, true);
#endif
            }
            else if (Global.input.isAnyFirstPress(Buttons.Start))
            {
                Global.numLocalGamers = (byte)SignedInGamer.SignedInGamers.Count;
                if (Global.networkManager.hostSessionType == NetworkManager.HostSessionType.Host)
                {
                    Global.gameState = Global.GameState.CreateMenu;
                }
                else
                {
                    Global.gameState = Global.GameState.JoinMenu;
                }
            }
            else if (Global.input.isFirstPress(Buttons.B))
            {
                Global.gameState = Global.GameState.Menu;
            }
        }

        private void drawLocalPlayerSetup()
        {
            Global.spriteBatch.Draw(background, Global.viewPort, Color.White);
            Global.spriteBatch.DrawString(mainMenu.titleFont, "Juggernaut", mainMenu.titlePosition, Color.Black);

            Vector2 playerTextStarting = new Vector2(Global.viewPort.Left + 30, Global.viewPort.Height / 2 - 30);
            Vector2 promptStarting = new Vector2(Global.viewPort.Left + 30, Global.viewPort.Height - 50);
            float textOffset = consolas.MeasureString("A").Y + 5;

            Global.spriteBatch.DrawString(consolas, "Signed In Players:", playerTextStarting, Color.Black);
            int off = 1;
            foreach (SignedInGamer gamer in SignedInGamer.SignedInGamers)
            {
                Global.spriteBatch.DrawString(consolas, gamer.Gamertag, new Vector2(playerTextStarting.X, playerTextStarting.Y + off++ * textOffset), Color.Black);
            }
            off += 2;

            Global.spriteBatch.DrawString(consolas, "Press A to Sign In", new Vector2(playerTextStarting.X, playerTextStarting.Y + off++ * textOffset), Color.Black);
            if (SignedInGamer.SignedInGamers.Count > 0)
            {
                Global.spriteBatch.DrawString(consolas, "Press Start to Continue", new Vector2(playerTextStarting.X, playerTextStarting.Y + off * textOffset), Color.Black);
            }
        }

        private void resetGraphicsDevice()
        {
            Global.graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            Global.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Global.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }
    }
}
