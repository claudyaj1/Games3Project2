using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;

using Games3Project2.Globals;
using Camera3D;
using ReticuleCursor;
using InputHandler;
using Geometry;
using HUDUtility;
using Microsoft.Xna.Framework.Audio;
using Networking;

namespace Games3Project2
{
    public class LocalPlayer : Collidable
    {
        public Camera camera;
        public PlayerIndex playerIndex;
        HUD hud;
        public int localPlayerIndex; // 1, 2, 3, or 4
        int lastFiringTime;
        Sphere sphere;
        Cube cube;
        Matrix cubeTransformation;
        const int gunLength = 3;
        public float jetPackThrust = 0;
        public int score;
        public int health;
        public bool isJuggernaut;
        public float jetFuel;
        public bool jetpackDisabled;
        public LocalNetworkGamer gamer;

        int timeSinceLastPacketSent = 0;
        const int PACKET_INTERVAL = 10;

        public override Vector3 Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;
                camera.cameraPos = base.Position;
            }
        }

        public LocalPlayer(Vector3 pos, PlayerIndex index, int localIndex, LocalNetworkGamer associatedGamer)
            : base(Global.game, pos, Vector3.Zero, Global.Constants.PLAYER_RADIUS)
        {
            
            playerIndex = index;
            localPlayerIndex = localIndex;
            score = 0;
            health = Global.Constants.MAX_HEALTH;
            isJuggernaut = false;
            jetpackDisabled = false;
            lastFiringTime = 0;
            jetFuel = Global.Constants.MAX_JET_FUEL;

            Color sphereColor = Color.Blue;
            sphere = new Sphere(Global.game, sphereColor, pos);
            sphere.localScale = Matrix.CreateScale(5);
            sphere.SetWireframe(1);
            Texture2D blankTexture = Global.game.Content.Load<Texture2D>(@"Textures\blankTexture");
            cube = new Cube(blankTexture, Color.Black);
            cubeTransformation = Matrix.CreateScale(1, 1, gunLength) * Matrix.CreateTranslation(new Vector3(radius, 0, gunLength));
            cube.wireFrame = false;
            cube.textured = false;

            gamer = associatedGamer;
            setupViewport();
        }

        public void setupViewport()
        {
            Viewport viewport = new Viewport();
            switch (Global.numLocalGamers)
            {
                case 1:
                    viewport = new Viewport(Global.viewPort.Left, Global.viewPort.Top, Global.viewPort.Width, Global.viewPort.Height);
                    break;
                case 2:
                    switch (localPlayerIndex)
                    {
                        case 1:
                            viewport = new Viewport(Global.viewPort.Left, Global.viewPort.Top, Global.viewPort.Width / 2, Global.viewPort.Height);
                            break;
                        case 2:
                            viewport = new Viewport(Global.viewPort.Left + Global.viewPort.Width / 2, Global.viewPort.Top, Global.viewPort.Width / 2, Global.viewPort.Height);
                            break;
                    }
                    break;
                case 3:
                    switch (localPlayerIndex)
                    {
                        case 1:
                            viewport = new Viewport(Global.viewPort.Left, Global.viewPort.Top, Global.viewPort.Width / 2, Global.viewPort.Height / 2);
                            break;
                        case 2:
                            viewport = new Viewport(Global.viewPort.Left + Global.viewPort.Width / 2, Global.viewPort.Top, Global.viewPort.Width / 2, Global.viewPort.Height / 2);
                            break;
                        case 3:
                            viewport = new Viewport(Global.viewPort.Left, Global.viewPort.Top + Global.viewPort.Height / 2, Global.viewPort.Width / 2, Global.viewPort.Height / 2);
                            break;
                    }
                    break;
                case 4:
                    switch (localPlayerIndex)
                    {
                        case 1:
                            viewport = new Viewport(Global.viewPort.Left, Global.viewPort.Top, Global.viewPort.Width / 2, Global.viewPort.Height / 2);
                            break;
                        case 2:
                            viewport = new Viewport(Global.viewPort.Left + Global.viewPort.Width / 2, Global.viewPort.Top, Global.viewPort.Width / 2, Global.viewPort.Height / 2);
                            break;
                        case 3:
                            viewport = new Viewport(Global.viewPort.Left, Global.viewPort.Top + Global.viewPort.Height / 2, Global.viewPort.Width / 2, Global.viewPort.Height / 2);
                            break;
                        case 4:
                            viewport = new Viewport(Global.viewPort.Left + Global.viewPort.Width / 2, Global.viewPort.Top + Global.viewPort.Height / 2, Global.viewPort.Width / 2, Global.viewPort.Height / 2);
                            break;
                    }
                    break;
            }
            camera = new Camera(position, Vector3.Zero, Vector3.Up, viewport);
            hud = new HUD(this);
        }

        public override void platformCollision()
        {
            base.platformCollision();
            camera.cameraPos = position;
        }

        public void respawn(Vector3 newPosition)
        {
            Position = newPosition;
            lastFiringTime = 0;
            jetPackThrust = 0;
            jetpackDisabled = false;
            health = Global.Constants.MAX_HEALTH;
            jetFuel = Global.Constants.MAX_JET_FUEL;
            //score = 0;
        }

        public void update()
        {
            #region Input
            float timeDelta = (float)Global.gameTime.ElapsedGameTime.TotalSeconds;
            velocity = timeDelta * Global.input.get3DMovement14Directions(true, playerIndex);
            float yawChange = Global.Constants.SPIN_RATE * timeDelta * Global.input.GamepadByID[Input.indexAsInt(playerIndex)].ThumbSticks.Right.X;
            float pitchChange = Global.Constants.SPIN_RATE * timeDelta * Global.input.GamepadByID[Input.indexAsInt(playerIndex)].ThumbSticks.Right.Y;
            lastFiringTime += Global.gameTime.ElapsedGameTime.Milliseconds;

            if (jetFuel <= 0)
            {
                jetFuel = 0;
                jetpackDisabled = true;
            }
            if (jetFuel > Global.Constants.MAX_JET_FUEL)
            {
                jetFuel = Global.Constants.MAX_JET_FUEL;
            }

            if (jetpackDisabled && jetFuel > Global.Constants.MAX_JET_FUEL / 4)
            {
                jetpackDisabled = false;
            }

            //Cheating attacks will target the jetpacking logic in this if statement.
            if ((Global.input.isPressed(Buttons.RightShoulder, playerIndex) ||
                Global.input.isPressed(Buttons.LeftShoulder, playerIndex) ||
                Global.input.GamepadByID[Input.indexAsInt(playerIndex)].Triggers.Left > 0f) && !jetpackDisabled)
            {
                jetPackThrust += Global.Constants.JET_PACK_INCREMENT;
                jetFuel -= Global.Constants.JET_FUEL_DECREMENT;
                if(!Global.debugMode)
                    Global.heatmapUsedJetpack.addPoint(position);
                Global.jetpack.Play();
            }
            else
            {
                //Cole: "We could do jet fuel addition only if the controller is plugged in"
                if (isJuggernaut)
                {
                    jetPackThrust -= Global.Constants.JET_PACK_DECREMENT / 2;
                    jetFuel += Global.Constants.JET_FUEL_INCREMENT * 1.2f;
                }
                else
                {
                    jetPackThrust -= Global.Constants.JET_PACK_DECREMENT;
                    jetFuel += Global.Constants.JET_FUEL_INCREMENT;
                }
                Global.jetpack.Stop();
            }

            if (jetPackThrust > Global.Constants.JET_PACK_Y_VELOCITY_CAP)
            {
                jetPackThrust = Global.Constants.JET_PACK_Y_VELOCITY_CAP;
            }
            if (jetPackThrust < 0)
            {
                jetPackThrust = 0;
            }
            velocity.Y += jetPackThrust * Global.gameTime.ElapsedGameTime.Milliseconds;
            velocity.Y -= Global.Constants.GRAVITY * Global.gameTime.ElapsedGameTime.Milliseconds;
            if (Global.input.GamepadByID[Input.indexAsInt(playerIndex)].Triggers.Right > 0f && 
                lastFiringTime > Global.Constants.FIRING_COOLDOWN)
            {
                lastFiringTime = 0;
                ShootBullet();
            }
            #endregion

            #region Camera/Geometry/HUD
            camera.Update(velocity, yawChange, pitchChange);
            prevPosition = position;
            position = camera.cameraPos;
            sphere.Position = position;
            sphere.Update(Global.gameTime);
            hud.Update();
            #endregion
            #region Collision
            foreach (LocalPlayer collidePlayer in Global.localPlayers)
            {
                if (collidePlayer == this)
                    continue;
                Global.Collision.bounceCollidables(this, collidePlayer);
            }
            foreach (RemotePlayer collidePlayer in Global.remotePlayers)
            {
                if (Global.Collision.didCollide(this, collidePlayer))
                {
                    collidePlayer.Velocity = Vector3.Zero;
                }
            }

            for (int i = 0; i < Global.BulletManager.bullets.Count; ++i)
            {
                if (Global.BulletManager.bullets[i].shooter != gamer && Global.Collision.didCollide(Global.BulletManager.bullets[i], this))
                {
                    health -= Global.BulletManager.bullets[i].damage;
                    if (health < 0)
                    {
                        killed(Global.BulletManager.bullets[i].shooter);
                    }
                    Global.BulletManager.bullets[i].disable();
                }
            }

            #endregion
            #region Networking
            timeSinceLastPacketSent++;
            if (timeSinceLastPacketSent > PACKET_INTERVAL)
            {
                Global.networkManager.playerUpdate(this);
                timeSinceLastPacketSent = 0;
            }
            #endregion

            base.Update(Global.gameTime);
        }

        public void draw()
        {
            if (Global.CurrentCamera != camera)
            {
                sphere.Draw(Global.CurrentCamera);
                cube.Draw(Global.CurrentCamera, cubeTransformation * 
                    Matrix.CreateRotationX(MathHelper.ToRadians(camera.pitch)) * Matrix.CreateRotationY(MathHelper.ToRadians(camera.yaw)) 
                    * Matrix.CreateTranslation(position));
            }
        }


        public void setAsJuggernaut()
        {
            isJuggernaut = true;
            //TODO: Play "New Juggernaut"
            //TODO: Announce "Who is Juggernaut" , networkID
        }

        public void killed(NetworkGamer killer)
        {
            //TODO: Play "Die" noise
            //TODO: maybe trigger some message?
            if (!Global.debugMode)
                Global.heatmapDeaths.addPoint(position);

            if (isJuggernaut)
            {
                isJuggernaut = false;
                if (killer == null)
                {
                    int nextJuggernaut = Global.rand.Next(0, Global.networkManager.networkSession.AllGamers.Count);
                    killer = Global.networkManager.networkSession.AllGamers[nextJuggernaut];
                    while (killer == gamer)
                    {
                        nextJuggernaut = Global.rand.Next(0, Global.networkManager.networkSession.AllGamers.Count);
                        killer = Global.networkManager.networkSession.AllGamers[nextJuggernaut];
                    }
                }

                if (killer.IsLocal)
                {
                    LocalPlayer player = killer.Tag as LocalPlayer;
                    player.setAsJuggernaut();
                }
                else
                {
                    RemotePlayer player = killer.Tag as RemotePlayer;
                    player.isJuggernaut = true;
                }

                Global.networkManager.newJuggernaut(killer);
            }
            else if(killer != null)
            {
                if (killer.IsLocal)
                {
                    LocalPlayer player = killer.Tag as LocalPlayer;
                    if (player.isJuggernaut)
                    {
                        player.score++;
                    }
                }
                else
                {
                    Global.networkManager.juggernautKill();
                }
            }

            Global.levelManager.respawnPlayer(this);
        }

        public void drawHUD()
        {
            hud.Draw();
        }

        ///// <summary>
        ///// Fires a solid, non-projectile laser beam blast.
        ///// </summary>
        //public void ShootLaserBurstWeapon()
        //{
            
        //    //Step one, drawWalls a line from just a smidge to the right of the avatar.
        //    //TODO: Oh baby, Line_Primative...but when?
        //    //Step two calculate collisions that might have occurred.
        //    //TODO: Ray intersection
        //    //Step three, Send message to the network to announce the event of the laser firing.
        //    //TODO: step 3
        //    //Step four, Play sound fx.
        //    //TODO: Upht.wav
        //}

        public void ShootBullet()
        {
            if (isJuggernaut)
            {
                Global.networkManager.fireBullet(Global.BulletManager.fireBullet(position + camera.view.Right * Global.Constants.RIGHT_HANDED_WEAPON_OFFSET,
                    -camera.lookRotation.Forward * Global.Constants.BULLET_SPEED, gamer, Global.Constants.JUG_BULLET_DAMAGE));
            }
            else
            {
                Global.networkManager.fireBullet(Global.BulletManager.fireBullet(position + camera.view.Right * Global.Constants.RIGHT_HANDED_WEAPON_OFFSET,
                    -camera.lookRotation.Forward * Global.Constants.BULLET_SPEED, gamer, Global.Constants.BULLET_DAMAGE));
            }
        }

        
    }
}
