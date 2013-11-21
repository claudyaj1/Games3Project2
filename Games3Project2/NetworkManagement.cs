using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

using Games3Project2.Globals;

namespace Games3Project2
{
    public class NetworkManagement : Microsoft.Xna.Framework.GameComponent
    {
        public bool isHost;
        public bool isNetworked;
        //Objects
        public NetworkSession networkSession;
        private PacketReader packetReader;
        private PacketWriter packetWriter;
        private NetworkSessionProperties networkSessionProperties;

        private string errorMessage;
        private string[] gamertags; // TODO: is this even a good idea?

        //Statistics
        private int numBytesSent = 0;
        private int numBytesReceived = 0;

        //Message types
        enum MessageTypes { Shoot, PositionAndVelocity, ScoreUpdate, NewJuggernaut, PlayerKilledByJuggernaut, GameSetup };
        MessageTypes messageType;

        //CTOR
        public NetworkManagement()
            : base(Global.game)
        {
            Initialize();
        }


        public override void Initialize()
        {
            isHost = false;
            isNetworked = false;
            gamertags = new string[Global.Constants.MAX_PLAYERS_TOTAL];
            packetReader = new PacketReader();
            packetWriter = new PacketWriter();
            networkSessionProperties = null;
            messageType = MessageTypes.ScoreUpdate;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (networkSession == null)
            {
                //TODO: Implement a menu screen in some other class.
                // If we are not in a network session, update the
                // menu screen that will let us create or join one.
                //UpdateMenuScreen();
            }


            if (networkSession != null)
            {
                //TODO: Implement some sort of message type determination system.


                UpdateNetworkSession();
                numBytesSent = networkSession.BytesPerSecondSent;
                numBytesReceived = networkSession.BytesPerSecondReceived;
                string msg = "Bytes sent: " + numBytesSent.ToString() + " Bytes received: " + numBytesReceived.ToString();
                Console.WriteLine(msg);
            }
            base.Update(gameTime);
        }

        #region P2P Packet Information Architecture
        void UpdateNetworkSession()
        {
            // Write packets.
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                WriteOutgoingPackets(gamer);
            }

            // Pump the underlying session object.
            networkSession.Update();
            // Make sure the session has not ended.
            if (networkSession == null)
                return;

            // Read any packets
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                ReadIncomingPackets(gamer);
            }
        } //UpdateNetworkSession-- write data to network

        public void WriteOutgoingPackets(LocalNetworkGamer gamer)
        {
            //packetWriter.Write((byte)messageType); // ALWAYS WRITE AT BEGINNING OF PACKET!

            switch (messageType)
	        {
                case MessageTypes.Shoot:
                    {

                        //TODO: Bullet shot announcement packet.
                        // Initial bullet position
                        // Bullet velocity (which will include direction)
                        // Time of bullet fire
                        // Who fired the bullet
                    }
                    break;
                case MessageTypes.GameSetup:

                    break;
                case MessageTypes.PositionAndVelocity:
                    {
                        //TODO: Announce player position packet.
                        //Who
                        //Where
                        //Health
                        //Other stats?

                        //if (isHost)
                        //{
                        //packetWriter.Write((Int32)ball.centerLocation.X);
                        //packetWriter.Write((Int32)ball.centerLocation.Y);
                        //}
                    }
                    break;
                case MessageTypes.ScoreUpdate:
                    {
                        //TODO: Announce score packet.
                        //Who?
                        //packetWriter.Write((byte)score);
                    }
                    break;
                default: break;
            }

            //Always finish this function with this line:
            if (packetWriter.Length > 0)
            {
                gamer.SendData(packetWriter, SendDataOptions.Reliable);
            }
        }//WriteOutgoingPackets
        public void ReadIncomingPackets(LocalNetworkGamer gamer)
        {
            MessageTypes mt;
            while (gamer.IsDataAvailable)
            {
                NetworkGamer sender;
                // Read a single packet from the network.
                gamer.ReceiveData(packetReader, out sender);
                // Discard packets sent by local gamers: we already know their state!
                if (sender.IsLocal)
                    continue;
                mt = (MessageTypes)packetReader.ReadByte();
                switch (mt)
                {
                    case MessageTypes.Shoot:
                        {
                            // TODO: Read in bullet shot announcement packet:
                            // Initial bullet position
                            // Bullet velocity (which will include direction)
                            // Time of bullet fire
                            // Who fired the bullet
                            //someFloatVariable = (float)packetReader.ReadInt32();
                        }
                        break;
                    case MessageTypes.GameSetup:
                        int levelNum = packetReader.ReadInt32();
                        int playerCount = packetReader.ReadInt32();
                        byte[] IDs = new byte[playerCount];
                        for (int i = 0; i < playerCount; ++i)
                        {
                            IDs[i] = packetReader.ReadByte();
                        }
                        for (int i = 0; i < playerCount; ++i)
                        {
                            Global.remotePlayers.Add(new RemotePlayer(new Vector3(0, 20, 0), (int)IDs[i]));
                        }
                        break;
                    case MessageTypes.PositionAndVelocity:
                        {
                            byte ID = packetReader.ReadByte();
                            RemotePlayer thePlayer = null;
                            foreach (RemotePlayer rPlayer in Global.remotePlayers)
                            {
                                if (rPlayer.networkPlayerID == ID)
                                {
                                    thePlayer = rPlayer;
                                    break;
                                }
                            }

                            thePlayer.Position = packetReader.ReadVector3();
                            thePlayer.Velocity = packetReader.ReadVector3();
                            packetReader.ReadBoolean();
                            packetReader.ReadSingle();
                        }
                        break;
                    case MessageTypes.ScoreUpdate:
                        {
                            //TODO: Read score packets
                        }
                        break;
                    default: break;
                }

            }
        }//ReadIncomingPackets
        #endregion //P2P Packet Information Architecture

        #region Session Creation/Joining
        public void CreateSession()
        {
            Console.WriteLine("Creating a network session...");
            try
            {
                networkSession = NetworkSession.Create(NetworkSessionType.PlayerMatch,
                    Global.Constants.MAX_PLAYERS_LOCAL,
                    Global.Constants.MAX_PLAYERS_TOTAL);
                    //0, // No private slots.
                    //null);
                HookSessionEvents();
                isHost = true;
                Console.WriteLine("Success");
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                Console.WriteLine("Failure");
                Console.WriteLine(errorMessage);
            }

            //Code for network latency testing
            //if (networkSession != null)
            //{
            //    networkSession.SimulatedLatency = TimeSpan.FromMilliseconds(200);
            //    networkSession.SimulatedPacketLoss = 0.9f;
            //}
        }
        public void JoinSession()
        {
            Console.WriteLine("Joining session...");
            try
            {
                // Search for sessions. The using keyword ensures availableSessions' disposal.
                // Learn more: http://msdn.microsoft.com/en-us/library/yh598w02(v=vs.110).aspx
                using (AvailableNetworkSessionCollection availableSessions =
                            NetworkSession.Find(NetworkSessionType.PlayerMatch,
                                                Global.Constants.MAX_PLAYERS_LOCAL,
                                                null))
                {
                    if (availableSessions.Count == 0)
                    {
                        errorMessage = "No network sessions found.";
                        Console.WriteLine(errorMessage);
                        return;
                    }

                    // Join the first session we found.
                    networkSession = NetworkSession.Join(availableSessions[0]);
                    HookSessionEvents();
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                Console.WriteLine(errorMessage);
            }
        }
        #endregion //Session Creating and Joining

        #region Hook Session Events
        void HookSessionEvents()
        {
            networkSession.GamerJoined += GamerJoinedEventHandler;
            networkSession.GamerLeft += GamerLeftEventHandler;
            networkSession.SessionEnded += SessionEndedEventHandler;
            networkSession.GameStarted += GameStarted;
        }

        void GameStarted(object sender, GameStartedEventArgs e)
        {
            string foo = e.ToString();
            Console.WriteLine(foo);
            if(!isHost)
            {
                Global.numLocalGamers = 1;
                Global.localPlayers.Add(new LocalPlayer(new Vector3(0, 20, 0), PlayerIndex.One, 1, 2));
                Global.levelManager.setupLevelOne();
            }
            Global.gameState = Global.GameState.Playing;
        }
        void GamerJoinedEventHandler(object sender, GamerJoinedEventArgs e)
        {
            string foo = e.Gamer.Gamertag.ToString();
            // for now, we'll just assign a number to the game
            Global.numTotalGamers++;
            Console.WriteLine(foo + Global.Constants.MSG_JOINED);
        }
        void GamerLeftEventHandler(object sender, GamerLeftEventArgs e)
        {
            string foo = e.Gamer.Gamertag.ToString();
            // for now, we'll just assign a number to the game
            Global.numTotalGamers--;
            Console.WriteLine(foo + Global.Constants.MSG_DISCONNECTED);
        }
        void SessionEndedEventHandler(object sender, NetworkSessionEndedEventArgs e)
        {
            errorMessage = e.EndReason.ToString();

            networkSession.Dispose();
            networkSession = null;
        }
        #endregion //Hook Session Events

        #region Pre-defined functions for creating packets for game events like bullets being fired
        public void AnnounceBulletShootEventOnNetwork(Bullet bullet)
        {
            messageType = MessageTypes.Shoot;
            packetWriter.Write((byte)messageType); // ALWAYS WRITE AT BEGINNING OF PACKET!
            packetWriter.Write((short)bullet.shooterID);    //Who fired the bullet.
            packetWriter.Write(bullet.startPosition);       //Where the bullet started.
            packetWriter.Write(bullet.Velocity);            //Direction/Speed of bullet.
            packetWriter.Write((Int32)bullet.timeLived);           //Milliseconds of time bullet has lived.
            //TODO: uh...gamer.SendData doesn't work in here.
            //TODO: use SendDataOptions.Reliable
            //TODO: This issue applies to all of the pre-defined functions for creating packets.
        }
        ////
        public void AnnounceJuggernautKilled(LocalPlayer deadJuggernaut, LocalPlayer newJuggernaut)
        {
            messageType = MessageTypes.NewJuggernaut;
            packetWriter.Write((byte)messageType); // ALWAYS WRITE AT BEGINNING OF PACKET!
            packetWriter.Write((byte)deadJuggernaut.networkPlayerID);
            packetWriter.Write((byte)newJuggernaut.networkPlayerID);
            packetWriter.Write(newJuggernaut.Position); //?
            packetWriter.Write(newJuggernaut.Velocity); //?
        }
        public void AnnounceJuggernautKilled(LocalPlayer deadJuggernaut, RemotePlayer newJuggernaut)
        {
            messageType = MessageTypes.NewJuggernaut;
            packetWriter.Write((byte)messageType); // ALWAYS WRITE AT BEGINNING OF PACKET!
            packetWriter.Write((byte)deadJuggernaut.networkPlayerID);
            packetWriter.Write((byte)newJuggernaut.networkPlayerID);
            packetWriter.Write(newJuggernaut.Position); //?
            packetWriter.Write(newJuggernaut.Velocity); //?
            packetWriter.Write(deadJuggernaut.Position); //Maybe for explosions of dead bodies.
        }
        ////
        public void AnnouncePlayerKilledByJuggernaut(LocalPlayer deadPlayer)
        {
            messageType = MessageTypes.PlayerKilledByJuggernaut;
            packetWriter.Write((byte)messageType); // ALWAYS WRITE AT BEGINNING OF PACKET!
            packetWriter.Write((byte)deadPlayer.networkPlayerID);
            packetWriter.Write(deadPlayer.Position);
        }
        ////
        public void MovementNetworkMessage(LocalPlayer localPlayer)
        {
            messageType = MessageTypes.PositionAndVelocity;
            packetWriter.Write((byte)messageType); // ALWAYS WRITE AT BEGINNING OF PACKET!
            packetWriter.Write((byte)localPlayer.networkPlayerID);
            packetWriter.Write(localPlayer.Position);
            packetWriter.Write(localPlayer.Velocity);
            packetWriter.Write((bool)localPlayer.jetpackDisabled);  //Is this useful?
            packetWriter.Write(localPlayer.jetPackThrust);          //Is this useful?
        }
        public void MovementNetworkMessage(BugBot bot)
        {
            messageType = MessageTypes.PositionAndVelocity;
            packetWriter.Write((byte)messageType); // ALWAYS WRITE AT BEGINNING OF PACKET!
            packetWriter.Write((byte)bot.npcID);
            packetWriter.Write(bot.position);
            packetWriter.Write(bot.direction);
            packetWriter.Write(bot.speed);
        }
        ////
        public void ScoreUpdate()
        {
            messageType = MessageTypes.ScoreUpdate;
            packetWriter.Write((byte)messageType); // ALWAYS WRITE AT BEGINNING OF PACKET!
            foreach(LocalPlayer localPlayer in Global.localPlayers)
            {
                packetWriter.Write((byte)localPlayer.score);
                packetWriter.Write((byte)localPlayer.health);
            }
        }

        public void gameSetupPacket(int levelNumber)
        {
            messageType = MessageTypes.GameSetup;
            packetWriter.Write((byte)messageType);
            packetWriter.Write(levelNumber);
            List<Byte> idList = new List<Byte>();
            foreach (NetworkGamer nGamer in networkSession.RemoteGamers)
            {
                idList.Add(nGamer.Id);
            }
            packetWriter.Write(idList.Count);
            foreach (Byte leByte in idList)
            {
                packetWriter.Write((byte)leByte);
            }
        }

        #endregion
    }
}
