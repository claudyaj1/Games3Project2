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
        private Random Rand;
        public bool isHost;
        LocalNetworkGamer theLocalLiveAccount;

        //Objects
        public NetworkSession networkSession;
        private PacketReader packetReader;
        private PacketWriter packetWriter;
        private NetworkSessionProperties networkSessionProperties;

        private string errorMessage;
        private string[] gamertags; // TODO: is this even a good idea?
        private List<int> usedIDs;

        //Statistics
        private int numBytesSent = 0;
        private int numBytesReceived = 0;

        //Message types
        enum MessageTypes { Shoot, PositionAndVelocity, ScoreUpdate, NewJuggernaut, PlayerKilledByJuggernaut };
        MessageTypes messageType;

        //CTOR
        public NetworkManagement()
            : base(Global.game)
        {
            usedIDs = new List<int>();
            Rand = new Random();
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

        public void WriteOutgoingPackets(LocalNetworkGamer gamer)
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
            gamer.SendData(packetWriter, SendDataOptions.Reliable);
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
                            // Work out the difference between our current local time
                            // and the remote time at which this packet was sent.
                            float localTime = (float)Global.gameTime.TotalGameTime.TotalSeconds;
                            float packetSendTime = packetReader.ReadSingle(); //Read the time of sending.
                            float timeDelta = localTime - packetSendTime;

                            Bullet b = new Bullet(Vector3.Zero, Vector3.Zero, 0, Global.Constants.BULLET_DAMAGE);
                            b.shooterID     = packetReader.ReadUInt16();    //Who fired the bullet.
                            b.startPosition = packetReader.ReadVector3();   //Where the bullet started.
                            b.Velocity      = packetReader.ReadVector3();   //Direction/Speed of bullet.
                            b.timeLived     = packetReader.ReadInt32();     //Milliseconds of time bullet has lived.
                            
                            //Find if who fired the bullet was juggernaut.
                            foreach (RemotePlayer rPlayer in Global.remotePlayers)
                            {
                                if (rPlayer.networkPlayerID == b.shooterID)
                                {
                                    b.damage = (rPlayer.isJuggernaut ?
                                        Global.Constants.JUG_BULLET_DAMAGE : Global.Constants.BULLET_DAMAGE);
                                }
                            }

                            b.timeLived += (int)timeDelta; //TODO: is this value correct?
                            Global.bullets.Add(b);
                        }
                        break;
                    case MessageTypes.PositionAndVelocity:
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
        #endregion //P2P Packet Information Architecture

        #region Session Creation/Joining
        public void CreateSession()
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
                for (int i = 0; i < Global.localPlayers.Count; i++) //For each localGamer in the collection
                {
                    Global.localPlayers[i].networkPlayerID = GenerateNetworkPlayerID(); //Assign non-colliding uid.
                }
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
        public int GenerateNetworkPlayerID()
        {
            int ann = Rand.Next(65000);
            while (usedIDs.Contains(ann))
            {
                ann = Rand.Next(65000);
            }
            usedIDs.Add(ann);
            return ann;
        }
        public void GenerateNetworkPlayerID(ref LocalPlayer player)
        {
            player.networkPlayerID = GenerateNetworkPlayerID();
        }
        public void GenerateNetworkPlayerID(ref RemotePlayer player)
        {
            player.networkPlayerID = GenerateNetworkPlayerID();
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
        }
        void GamerJoinedEventHandler(object sender, GamerJoinedEventArgs e)
        {
            string foo = e.Gamer.Gamertag.ToString();
            Global.remotePlayers.Add(
                new RemotePlayer(Global.game,
                new Vector3((float)Rand.Next(-5,5), 1f, (float)Rand.Next(-5,5)),
                GenerateNetworkPlayerID()));
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
            packetWriter.Write((float)Global.gameTime.TotalGameTime.TotalSeconds); // Send our current time.
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
        ////
        public void AnnouncePlayerKilledByJuggernaut(LocalPlayer deadPlayer)
        {
            messageType = MessageTypes.PlayerKilledByJuggernaut;
            packetWriter.Write((byte)messageType); // ALWAYS WRITE AT BEGINNING OF PACKET!
            packetWriter.Write((short)deadPlayer.networkPlayerID);
            packetWriter.Write(deadPlayer.Position);
        }
        ////
        public void MovementNetworkMessage(LocalPlayer localPlayer)
        {
            messageType = MessageTypes.PositionAndVelocity;
            packetWriter.Write((byte)messageType); // ALWAYS WRITE AT BEGINNING OF PACKET!
            packetWriter.Write((short)localPlayer.networkPlayerID);
            packetWriter.Write(localPlayer.Position);
            packetWriter.Write(localPlayer.Velocity);
            packetWriter.Write(localPlayer.camera.lookRotation.Forward);
            packetWriter.Write((bool)localPlayer.jetpackDisabled);  //Is this useful?
            packetWriter.Write(localPlayer.jetPackThrust);          //Is this useful?
        }
        public void MovementNetworkMessage(BugBot bot)
        {
            messageType = MessageTypes.PositionAndVelocity;
            packetWriter.Write((byte)messageType); // ALWAYS WRITE AT BEGINNING OF PACKET!
            packetWriter.Write((short)bot.npcID);
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

        #endregion


    }
}
