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

        int timeSinceLastPacketSent;
        const int PACKET_INTERVAL = 10;
        public float gunHeat = 0f;
        public bool gunCoolDownModeNoShootingPermitted;

        bool isVibrating;
        int vibratingCountdown;
        const int VIBRATE_MAX = 250;

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
            localPlayerIndex = localIndex;
            timeSinceLastPacketSent = (localIndex - 1) * 2;
            score = 0;
            health = Global.Constants.MAX_HEALTH;
            isJuggernaut = false;
            jetpackDisabled = false;
            lastFiringTime = 0;
            jetFuel = Global.Constants.MAX_JET_FUEL;
            gunCoolDownModeNoShootingPermitted = false;

            sphere = new Sphere(Global.game, Global.Constants.DEFAULT_PLAYER_COLOR, pos);
            sphere.localScale = Matrix.CreateScale(Global.Constants.PLAYER_RADIUS);
            sphere.SetWireframe(1);
            Texture2D blankTexture = Global.game.Content.Load<Texture2D>(@"Textures\blankTexture");
            cube = new Cube(blankTexture, Color.Black);
            cubeTransformation = Matrix.CreateScale(1, 1, gunLength) * Matrix.CreateTranslation(new Vector3(radius, 0, gunLength));
            cube.wireFrame = false;
            cube.textured = false;

            gamer = associatedGamer;
            playerIndex = gamer.SignedInGamer.PlayerIndex;
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

            //Decrement Gun Heat Every Time
            if (!gunCoolDownModeNoShootingPermitted &&
                lastFiringTime > Global.Constants.FIRING_COOLDOWN && 
                gunHeat < Global.Constants.MAX_GUN_HEAT)
            {
                if (Global.input.GamepadByID[Input.indexAsInt(playerIndex)].Triggers.Right > 0f)
                {
                    lastFiringTime = 0;
                    ShootBullet();
                    gunHeat += 2f + gunHeat * 0.04f; //Gun Heat Increase per gun fire rate 
                    if (gunHeat > Global.Constants.MAX_GUN_HEAT) {
                        gunHeat = Global.Constants.MAX_GUN_HEAT;    //Prevent weird above max values.
                        gunCoolDownModeNoShootingPermitted = true;  //Disable gun firing.
                    }
                }
            }
            if (gunCoolDownModeNoShootingPermitted /*gunHeat > 0*/)
            {
                gunHeat -= .50f; //Steady Gun Heat Dissipation Rate.
                if (gunHeat <= 5) //If cooldown has finished.
                {
                    gunHeat = 0; //Prevent weird negative values.
                    gunCoolDownModeNoShootingPermitted = false; //Permit gun firing again.
                }
            }
            else if (lastFiringTime > 400 /*ms*/)
            {
                gunHeat -= 0.03f + (gunHeat * .05f); //A little bit every update call + a percentage (4%) of overall remaining heat.
                if (gunHeat < 0)
                    gunHeat = 0; //Prevent weird negative values.
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
                    Global.Collision.bounceCollidables(this, collidePlayer);
                    collidePlayer.Velocity = Vector3.Zero;
                }
            }

            for (int i = 0; i < Global.BulletManager.bullets.Count; ++i)
            {
                if (Global.BulletManager.bullets[i].shooter != gamer && Global.BulletManager.bullets[i].state == Bullet.State.Active && Global.Collision.didCollide(Global.BulletManager.bullets[i], this))
                {
                    if (Global.BulletManager.bullets[i].timeLived < Global.Constants.BULLET_POWER_DISTANCE)
                    {
                        health -= Global.BulletManager.bullets[i].damage;
                    }
                    else
                    {
                        health -= Global.BulletManager.bullets[i].damage*2;
                    }

                    if (health < 0)
                    {
                        startVibrating();
                        killed(Global.BulletManager.bullets[i].shooter);
                    }
                    else
                    {
                        startVibrating();
                    }
                    Global.BulletManager.bullets[i].disable();
                }
            }

            manageVibrations();

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
                    Matrix.CreateRotationX(MathHelper.ToRadians(camera.pitch)) *
                    Matrix.CreateRotationY(MathHelper.ToRadians(camera.yaw)) *
                    Matrix.CreateTranslation(position));
            }
        }

        private void startVibrating()
        {
            isVibrating = true;
            vibratingCountdown = VIBRATE_MAX;
            GamePad.SetVibration(gamer.SignedInGamer.PlayerIndex, 0f, Global.Constants.VIBRATION_LOW);
        }

        private void manageVibrations()
        {
            vibratingCountdown -= Global.gameTime.ElapsedGameTime.Milliseconds;
            if (vibratingCountdown < 0)
            {
                isVibrating = false;
                GamePad.SetVibration(gamer.SignedInGamer.PlayerIndex, 0f, 0);
                vibratingCountdown = 0;
            }
        }

        public void setAsJuggernaut()
        {
            isJuggernaut = true;
            sphere.ChangeAllVertexColors(Global.Constants.JUGGERNAUT_COLOR); //This change is not visible by the player nor the remotes.
            //TODO: Play "New Juggernaut"
        }

        public void setAsNotJuggernaut()
        {
            isJuggernaut = false;
            sphere.ChangeAllVertexColors(Global.Constants.DEFAULT_PLAYER_COLOR);
        }

        public void killed(NetworkGamer killer)
        {
            //TODO: Play "Die" noise
            //TODO: maybe trigger some message?
            if (!Global.debugMode)
                Global.heatmapDeaths.addPoint(position);

            if (isJuggernaut)
            {
                setAsNotJuggernaut();

                if (killer == null)
                {
                    if (Global.gameState == Global.GameState.SinglePlayerPlaying)
                    {
                        Global.localPlayers[0].score--;
                    }

                    if (Global.networkManager.networkSession.AllGamers.Count == 1)
                    {
                        killer = gamer;
                    }
                    else
                    {
                        int nextJuggernaut = Global.rand.Next(0, Global.networkManager.networkSession.AllGamers.Count);
                        killer = Global.networkManager.networkSession.AllGamers[nextJuggernaut];
                        while (killer == gamer)
                        {
                            nextJuggernaut = Global.rand.Next(0, Global.networkManager.networkSession.AllGamers.Count);
                            killer = Global.networkManager.networkSession.AllGamers[nextJuggernaut];
                        }
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
                    player.setAsJuggernaut();
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
