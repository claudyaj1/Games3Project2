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
        enum MessageTypes { Shoot, Position, ScoreUpdate, NewJuggernaut, PlayerKilledByJuggernaut };
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
            gamertags = new string[Global.Constants.MAX_PLAYERS_TOTAL];
            packetReader = new PacketReader();
            packetWriter = new PacketWriter();
            networkSessionProperties = new NetworkSessionProperties();
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

        private void WriteOutgoingPackets(LocalNetworkGamer gamer)
        {
            packetWriter.Write((byte)messageType); // ALWAYS WRITE AT BEGINNING OF PACKET!

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
                case MessageTypes.Position:
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
            gamer.SendData(packetWriter, SendDataOptions.Reliable);
        }//WriteOutgoingPackets
        private void ReadIncomingPackets(LocalNetworkGamer gamer)
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
                    case MessageTypes.Position:
                        {
                            //TODO: Read position packets.
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
        #endregion

        #region Session Creation/Joining
        /// <summary>
        /// Starts hosting a new network session.
        /// </summary>
        private void CreateSession()
        {
            Console.WriteLine("Creating a network session...");
            try
            {
                networkSession = NetworkSession.Create(NetworkSessionType.PlayerMatch,
                    Global.Constants.MAX_PLAYERS_LOCAL,
                    Global.Constants.MAX_PLAYERS_TOTAL,
                    0, // No private slots.
                    networkSessionProperties);
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
        /// <summary>
        /// Searches for existing session and attempts to join one.
        /// </summary>
        private void JoinSession()
        {
            Console.WriteLine("Joining session...");
            try
            {
                // Search for sessions. The using keyword ensures availableSessions' disposal.
                // Learn more: http://msdn.microsoft.com/en-us/library/yh598w02(v=vs.110).aspx
                using (AvailableNetworkSessionCollection availableSessions =
                            NetworkSession.Find(NetworkSessionType.PlayerMatch,
                                                Global.Constants.MAX_PLAYERS_LOCAL,
                                                networkSessionProperties))
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
        #endregion

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
        #endregion

        #region Pre-defined functions for creating packets for game events like bullets being fired
        public void AnnounceBulletShortOnNetwork(Bullet bullet)
        {
            messageType = MessageTypes.Shoot;
            packetWriter.Write((byte)messageType); // ALWAYS WRITE AT BEGINNING OF PACKET!
            packetWriter.Write((short)bullet.shooterID);    //Who fired the bullet.
            packetWriter.Write(bullet.startPosition);       //Where the bullet started.
            packetWriter.Write(bullet.Velocity);            //Direction/Speed of bullet.
            packetWriter.Write((Int32)bullet.timeLived);           //Milliseconds of time bullet has lived.
            //TODO: uh...gamer.SendData doesn't work in here.
            //TODO: use SendDataOptions.Reliable
        }

        public void AnnounceJuggernautKilled(LocalPlayer deadJuggernaut, LocalPlayer newJuggernaut)
        {
            messageType = MessageTypes.NewJuggernaut;
            packetWriter.Write((byte)messageType); // ALWAYS WRITE AT BEGINNING OF PACKET!
            packetWriter.Write((short)deadJuggernaut.networkPlayerID);
            packetWriter.Write((short)newJuggernaut.networkPlayerID);
            packetWriter.Write(newJuggernaut.Position); //?
            packetWriter.Write(newJuggernaut.Velocity); //?
        }
        public void AnnounceJuggernautKilled(LocalPlayer deadJuggernaut, RemotePlayer newJuggernaut)
        {
            messageType = MessageTypes.NewJuggernaut;
            packetWriter.Write((byte)messageType); // ALWAYS WRITE AT BEGINNING OF PACKET!
            packetWriter.Write((short)deadJuggernaut.networkPlayerID);
            packetWriter.Write((short)newJuggernaut.networkPlayerID);
            packetWriter.Write(newJuggernaut.Position); //?
            packetWriter.Write(newJuggernaut.Velocity); //?
            packetWriter.Write(deadJuggernaut.Position); //Maybe for explosions of dead bodies.
        }

        public void AnnouncePlayerKilledByJuggernaut(LocalPlayer deadPlayer)
        {
            messageType = MessageTypes.PlayerKilledByJuggernaut;
            packetWriter.Write((byte)messageType); // ALWAYS WRITE AT BEGINNING OF PACKET!
            packetWriter.Write((short)deadPlayer.networkPlayerID);
            packetWriter.Write(deadPlayer.Position);
        }
        #endregion
    }
}
