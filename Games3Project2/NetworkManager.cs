using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Input;

using Games3Project2;
using Games3Project2.Globals;

namespace Networking
{
    public class NetworkManager
    {
        public NetworkSessionType sessionType;
        public NetworkSessionProperties sessionProperties = null;
        /// <summary>
        /// Host is the "server" in the client-server network architecture.
        /// </summary>
        public enum HostSessionType { Host, Client };
        public HostSessionType hostSessionType;

        public NetworkSession networkSession = null;
        public PacketReader reader;
        public PacketWriter writer;
        public SendDataOptions dataOptions = SendDataOptions.Reliable;

        /// <summary>
        /// Progress is a message to announce to the other player how much progress has been made
        /// Winning message is sent to announce that the other player has won and to end the play state.
        /// </summary>
        public enum MessageType { Level, FireBullet, PlayerUpdate, NewJuggernaut, JuggernautKill, AnnounceWinner };
        string lastErrorMessage;

        public enum CurrentState { Idle, Joining, Creating, Running, JoinFailed, CreateFailed }
        CurrentState cs = CurrentState.Idle;
        public CurrentState currentState
        {
            get { return cs; }
            set
            {
                cs = value;
                timeInState = 0;
            }
        }
        int timeInState = 0;
        const int JOINING_TIMEOUT = 10000;
        const int CREATING_TIMEOUT = 10000;
        const int FAILED_TIMEOUT = 4000;

        Texture2D background;
        SpriteFont consolas;

        public NetworkManager()
        {
            sessionType = NetworkSessionType.SystemLink;
            hostSessionType = HostSessionType.Client;
            reader = new PacketReader();
            writer = new PacketWriter();
            background = Global.game.Content.Load<Texture2D>(@"Textures\menubackground");
            consolas = Global.game.Content.Load<SpriteFont>(@"Fonts\Consolas");
        }

        public void update()
        {
            timeInState += Global.gameTime.ElapsedGameTime.Milliseconds;
            switch (currentState)
            {
                case CurrentState.Idle:

                    break;
                case CurrentState.Joining:
                    if (timeInState > JOINING_TIMEOUT)
                    {
                        currentState = CurrentState.JoinFailed;
                    }
                    break;
                case CurrentState.Creating:
                    if (timeInState > CREATING_TIMEOUT)
                    {
                        currentState = CurrentState.CreateFailed;
                    }
                    break;
                case CurrentState.Running:
                    if (networkSession == null)
                        break;
                    readIncomingPackets();
                    writeOutgoingPackets();
                    break;
                case CurrentState.JoinFailed:
                case CurrentState.CreateFailed:
                    if (timeInState > FAILED_TIMEOUT || Global.input.isFirstPress(Buttons.B))
                    {
                        Global.gameState = Global.GameState.Menu;
                        currentState = CurrentState.Idle;
                    }
                    break;
            }

            if (networkSession != null)
            {
                networkSession.Update(); //Pump the packets out to the network.
            }
        }

        public void draw()
        {
            switch (currentState)
            {
                case CurrentState.Idle:
                case CurrentState.Running:
                    break;
                case CurrentState.Joining:
                    Global.spriteBatch.Draw(background, Global.viewPort, Color.White);
                    Global.spriteBatch.DrawString(consolas, "Attempting to join a game...", new Vector2(Global.viewPort.Left + 15, Global.viewPort.Height / 2 - 5), Color.Black);
                    break;
                case CurrentState.Creating:
                    Global.spriteBatch.Draw(background, Global.viewPort, Color.White);
                    Global.spriteBatch.DrawString(consolas, "Creating a game...", new Vector2(Global.viewPort.Left + 15, Global.viewPort.Height / 2 - 5), Color.Black);
                    break;
                case CurrentState.JoinFailed:
                    Global.spriteBatch.Draw(background, Global.viewPort, Color.White);
                    Global.spriteBatch.DrawString(consolas, "Failed to join a game.", new Vector2(Global.viewPort.Left + 15, Global.viewPort.Height / 2 - 5), Color.Black);
                    break;
                case CurrentState.CreateFailed:
                    Global.spriteBatch.Draw(background, Global.viewPort, Color.White);
                    Global.spriteBatch.DrawString(consolas, "Failed to create a game.", new Vector2(Global.viewPort.Left + 15, Global.viewPort.Height / 2 - 5), Color.Black);
                    break;
            }
        }

        public bool joinSession() 
        {
            currentState = CurrentState.Joining;
            try
            {
                using(AvailableNetworkSessionCollection availableSessions = NetworkSession.Find(sessionType, Global.Constants.MAX_PLAYERS_LOCAL, sessionProperties))
                {
                    if (availableSessions.Count == 0)
                    {
                        throw new System.Exception();
                    }
                    else
                    {
                        networkSession = NetworkSession.Join(availableSessions[0]);
                        hookEvents();
                    }
                }
            }
            catch (Exception e)
            {
                currentState = CurrentState.JoinFailed;
                lastErrorMessage = e.Message;
                return false;
            }

            currentState = CurrentState.Running;
            return true;
        }

        public bool createSession()
        {
            currentState = CurrentState.Creating;
            try
            {
                networkSession = NetworkSession.Create(sessionType, Global.Constants.MAX_PLAYERS_LOCAL, Global.Constants.MAX_PLAYERS_TOTAL, 0, sessionProperties);
                hookEvents();
            }
            catch (Exception e)
            {
                currentState = CurrentState.CreateFailed;
                lastErrorMessage = e.Message;
                return false;
            }

            currentState = CurrentState.Running;
            return true;
        }

        public void disposeNetworkSession()
        {
            if (networkSession != null)
            {
                networkSession.Dispose();
                networkSession = null;
            }

            Global.localPlayers.Clear();
            Global.remotePlayers.Clear();
            currentState = CurrentState.Idle;
        }

        #region Hook Events
        private void hookEvents()
        {
            networkSession.GameStarted += GameStartedEventHandler;
            networkSession.GamerJoined += GamerJoinedEventHandler;
            networkSession.GamerLeft += GamerLeftEventHandler;
            networkSession.SessionEnded += SessionEndedEventHandler;
        }

        void GameStartedEventHandler(object sender, GameStartedEventArgs e)
        {
            Global.levelManager.setupLevel();
            Global.gameState = Global.GameState.Playing;
        }

        void GamerJoinedEventHandler(object sender, GamerJoinedEventArgs e)
        {
            if (!e.Gamer.IsLocal)
            {
                e.Gamer.Tag = new RemotePlayer(new Vector3(-5000, -5000, -5000), e.Gamer);
                Global.remotePlayers.Add((RemotePlayer)e.Gamer.Tag);
            }
        }

        void SessionEndedEventHandler(object sender, NetworkSessionEndedEventArgs e)
        {
            disposeNetworkSession();
            Global.gameState = Global.GameState.NetworkQuit;
        }

        void GamerLeftEventHandler(object sender, GamerLeftEventArgs e)
        {
            if (e.Gamer.IsLocal)
            {
                LocalPlayer player = e.Gamer.Tag as LocalPlayer;
                Global.localPlayers.Remove(player);
            }
            else
            {
                RemotePlayer player = e.Gamer.Tag as RemotePlayer;
                Global.remotePlayers.Remove(player);
            }
        }
        #endregion

        private void writeOutgoingPackets()
        {
            foreach (LocalNetworkGamer lnGamer in networkSession.LocalGamers)
            {
                if (writer.Length > 0)
                {
                    lnGamer.SendData(writer, dataOptions);
                }
            }
        }

        private void readIncomingPackets()
        {
            foreach (LocalNetworkGamer lnGamer in networkSession.LocalGamers)
            {
                while (lnGamer.IsDataAvailable)
                {
                    //read data
                    NetworkGamer sender;
                    lnGamer.ReceiveData(reader, out sender);

                    // Discard packets sent by local gamers: we already know their state!
                    if (sender.IsLocal)
                        continue;

                    MessageType incomingPacketType = (MessageType)reader.ReadByte(); //Read the packet type.
                    switch (incomingPacketType)
                    {
                        case MessageType.FireBullet:
                            readBullet();
                            break;
                        case MessageType.Level:
                            readLevel();
                            break;
                        case MessageType.NewJuggernaut:
                            readNewJuggernaut();
                            break;
                        case MessageType.PlayerUpdate:
                            readPlayerUpdate();
                            break;
                        case MessageType.JuggernautKill:
                            readJuggernautKill();
                            break;
                        default: 
                            break;
                    }

                    // Estimate how long this packet took to arrive.
                    /*TimeSpan latency = networkSession.SimulatedLatency +
                                       TimeSpan.FromTicks(sender.RoundtripTime.Ticks / 2);*/
                }
            }
        }

        private NetworkGamer findGamerWithTag(string tag)
        {
            foreach (NetworkGamer gamer in networkSession.AllGamers)
            {
                if (gamer.Gamertag == tag)
                    return gamer;
            }

            return null;
        }

        public void fireBullet(Bullet bullet)
        {
            writer.Write((byte)MessageType.FireBullet);
            writer.Write(bullet.startPosition);
            writer.Write(bullet.Velocity);
            writer.Write(bullet.shooter.Gamertag);
            writer.Write(bullet.damage);
        }

        private void readBullet()
        {
            Vector3 startPos = reader.ReadVector3();
            Vector3 vel = reader.ReadVector3();
            string tag = reader.ReadString();
            NetworkGamer shooter = findGamerWithTag(tag);
            int damage = reader.ReadInt32();

            Global.BulletManager.fireBullet(startPos, vel, shooter, damage);
        }

        public void announceLevel(int levelNumber)
        {
            writer.Write((byte)MessageType.Level);
            writer.Write(levelNumber);
        }

        private void readLevel()
        {
            int levelNum = reader.ReadInt32();
            Global.levelManager.currentLevel = levelNum;
            Global.levelManager.setupLevel();
        }

        public void playerUpdate(LocalPlayer player)
        {
            writer.Write((byte)MessageType.PlayerUpdate);
            writer.Write(player.gamer.Gamertag);
            writer.Write(player.Position);
            writer.Write(player.Velocity);
            writer.Write(player.camera.pitch);
            writer.Write(player.camera.yaw);
        }

        private void readPlayerUpdate()
        {
            string tag = reader.ReadString();
            NetworkGamer gamer = findGamerWithTag(tag);
            Vector3 newPos = reader.ReadVector3();
            Vector3 newVel = reader.ReadVector3();
            float pitch = reader.ReadSingle();
            float yaw = reader.ReadSingle();
            RemotePlayer player = gamer.Tag as RemotePlayer;
            player.receiveNewPacketUpdate(newPos, newVel, pitch, yaw);
        }

        public void newJuggernaut(NetworkGamer gamer) //Announces who shall be the new juggernaut.
        {
            writer.Write((byte)MessageType.NewJuggernaut);
            writer.Write(gamer.Gamertag);
            Global.msgNewJuggernaut(gamer.Gamertag); //Fires off a message to the message displayer unit.
        }

        private void readNewJuggernaut() //Reads in who should be the new juggernaut.
        {
            string tag = reader.ReadString();
            Global.msgNewJuggernaut(tag); //Fires off a message to the message displayer unit.
            NetworkGamer gamer = findGamerWithTag(tag);
            if (gamer.IsLocal)
            {
                LocalPlayer player = gamer.Tag as LocalPlayer;
                player.setAsJuggernaut();
                for (int i = 0; i < Global.localPlayers.Count; i++)
                {
                    if (Global.localPlayers[i] != player)
                    {
                        Global.localPlayers[i].setAsNotJuggernaut();
                    }
                }
                for (int i = 0; i < Global.remotePlayers.Count; i++)
                {
                    Global.remotePlayers[i].setAsNotJuggernaut();
                }
            }
            else
            {
                RemotePlayer rPlayer = gamer.Tag as RemotePlayer;
                rPlayer.setAsJuggernaut();
                for (int i = 0; i < Global.localPlayers.Count; i++)
                {
                    Global.localPlayers[i].setAsNotJuggernaut();
                }
                for (int i = 0; i < Global.remotePlayers.Count; i++) //Loop for all remotes except the juggernaut
                {
                    if (Global.remotePlayers[i] != rPlayer)
                    {
                        Global.remotePlayers[i].setAsNotJuggernaut();
                    }
                }
            }

            //TODO: Announce msg "Who is Juggernaut" ,tag so that remote players know who is the new juggernaut is.
        }

        public void juggernautKill() //Juggernaut earns a kill.
        {
            writer.Write((byte)MessageType.JuggernautKill);
        }

        private void readJuggernautKill() //Juggernaut earns a kill.
        {
            foreach (LocalPlayer player in Global.localPlayers)
            {
                if (player.isJuggernaut)
                {
                    player.score++;
                    break;
                }
            }
        }

        public void announceWinner(NetworkGamer gamer)
        {
            writer.Write((byte)MessageType.AnnounceWinner);
            writer.Write(gamer.Gamertag);
        }

        private void readAnnounceWinner()
        {
            Global.winningPlayer = reader.ReadString();
            Global.gameState = Global.GameState.GameOver;
            disposeNetworkSession();
        }
    }
}
