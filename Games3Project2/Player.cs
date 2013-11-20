using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Games3Project2.Globals;
using Camera3D;
using ReticuleCursor;
using InputHandler;
using Geometry;
using Primatives;

namespace Games3Project2
{
    public class LocalPlayer : Collidable
    {
        public Camera camera;
        public Cursor cursor;
        public PlayerIndex playerIndex;
        public int localPlayerIndex; // 1, 2, 3, or 4
        Sphere sphere;
        public float jetPackThrust = 0;

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

        bool firingLaserBurstWeapon;
        Vector3 justASmidgeToTheRight;
        Vector3 beamEnd;
        Line_Primitive laserBeamBurst;

        public LocalPlayer(Vector3 pos, PlayerIndex index, int localIndex)
            : base(Global.game, pos, Vector3.Zero, Global.Constants.PLAYER_RADIUS)
        {
            playerIndex = index;
            localPlayerIndex = localIndex;
            Viewport viewport = new Viewport();
            sphere = new Sphere(Global.game, Color.Red, pos);
            sphere.localScale = Matrix.CreateScale(5);
            firingLaserBurstWeapon = false;
            //split up viewport
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
            camera = new Camera(pos, Vector3.Zero, Vector3.Up, viewport);
            cursor = new Cursor(new Vector2(viewport.Width / 2, viewport.Height / 2));
        }

        public override void platformCollision()
        {
            base.platformCollision();
            camera.cameraPos = position;
        }

        public void update()
        {
            float timeDelta = (float)Global.gameTime.ElapsedGameTime.TotalSeconds;
            velocity = timeDelta * Global.input.get3DMovement14Directions(true, playerIndex);
            float yawChange = Global.Constants.SPIN_RATE * timeDelta * Global.input.GamepadByID[localPlayerIndex].ThumbSticks.Right.X;
            float pitchChange = Global.Constants.SPIN_RATE * timeDelta * Global.input.GamepadByID[localPlayerIndex].ThumbSticks.Right.Y;

            if (Global.input.isPressed(Buttons.RightShoulder, playerIndex) ||
                Global.input.isPressed(Buttons.LeftShoulder, playerIndex) ||
                Global.input.GamepadByID[localPlayerIndex].Triggers.Left > 0f)
            {
                jetPackThrust += Global.Constants.JET_PACK_INCREMENT;
                //TODO: Jet Fuel subtraction.
            }
            else if (Global.input.GamepadByID[Input.indexAsInt(playerIndex)].IsConnected)
            {
                //TODO: Jet Fuel addition only if the controller is plugged in.
                jetPackThrust -= Global.Constants.JET_PACK_DECREMENT;
                if (jetPackThrust < 0)
                    jetPackThrust = 0;
            }

            if (jetPackThrust > Global.Constants.JET_PACK_Y_VELOCITY_CAP)
            {
                jetPackThrust = Global.Constants.JET_PACK_Y_VELOCITY_CAP;
            }
            velocity.Y += jetPackThrust * Global.gameTime.ElapsedGameTime.Milliseconds;
            velocity.Y -= Global.Constants.GRAVITY * Global.gameTime.ElapsedGameTime.Milliseconds;

            if (Global.input.GamepadByID[Input.indexAsInt(playerIndex)].Triggers.Right > 0f)
            {
                ShootLaserBurstWeapon();
            }

            //TODO: User controls go here.

            #region If DEBUG && WINDOWS && (No Controller) Then Keyboard-and-Mouse does control
            /*
#if DEBUG && WINDOWS
            if (!Global.input.isConnected(PlayerIndex.One))
            {
                const float MOUSE_SENSITIVITY = 20f;
                const float KEYBOARD_SENSITIVITY = 0.04f;
                dir -= Global.input.get3DMovement14Directions(false);
                dir *= KEYBOARD_SENSITIVITY;
                Vector2 mouseDelta = Global.input.getMouseDelta();

                if (mouseDelta.X < 0)
                {
                    yaw += (MOUSE_SENSITIVITY * timeDelta * -mouseDelta.X);
                }
                else if (mouseDelta.X > 0)
                {
                    yaw -= (MOUSE_SENSITIVITY * timeDelta * mouseDelta.X);
                }
                if (yaw > 360)
                    yaw -= 360;
                else if (yaw < 0)
                    yaw += 360;

                if (mouseDelta.Y < 0)
                {
                    pitch -= (MOUSE_SENSITIVITY * timeDelta);
                }
                else if (mouseDelta.Y > 0)
                {
                    pitch += (MOUSE_SENSITIVITY * timeDelta);
                }
                if (pitch > 360)
                    pitch -= 360;
                else if (pitch < 0)
                    pitch += 360;

                if (Global.input.isPressed(Keys.Space))
                {
                    jetPackThrust += Global.Constants.JET_PACK_INCREMENT;
                }
                else
                {
                    jetPackThrust -= Global.Constants.JET_PACK_DECREMENT;
                    if (jetPackThrust < 0)
                        jetPackThrust = 0;
                }
            } //END
#endif*/
            #endregion
            
            camera.Update(velocity, yawChange, pitchChange);
            prevPosition = position;
            position = camera.cameraPos;
            sphere.Position = position;
            sphere.Update(Global.gameTime);

            base.Update(Global.gameTime);
        }

        public void draw()
        {
            if (Global.CurrentCamera == camera)
            {
                //Things that only draw in the viewport go here.
                cursor.Draw();
            }
            else
            {
                //Things that don't draw in the player's viewport (but do draw in everyone else's) go here.
                sphere.Draw(Global.CurrentCamera);
            }

            //Things that draw in everyone's viewport go here.
            if (firingLaserBurstWeapon && laserBeamBurst != null)
            {
                laserBeamBurst.Draw(Matrix.Identity, Global.CurrentCamera.view, Global.CurrentCamera.projection,
                    CullMode.CullCounterClockwiseFace, FillMode.WireFrame, true);
                firingLaserBurstWeapon = false;
            }
        }

        /// <summary>
        /// Fires a solid, non-projectile laser beam blast.
        /// </summary>
        public void ShootLaserBurstWeapon()
        {
            const float RIGHT_HANDED_WEAPON_OFFSET = 0.1f;
            //Step one, draw a line from just a smidge to the right of the avatar.
            justASmidgeToTheRight = position + (camera.getLookAt() * RIGHT_HANDED_WEAPON_OFFSET);
            beamEnd = position + camera.getLookAt() * 1f;
            laserBeamBurst = new Line_Primitive(Game.GraphicsDevice,
                camera.getLookAt(), Global.Constants.LASER_BEAM_COLOR,
                beamEnd, Global.Constants.LASER_BEAM_COLOR);
            
            firingLaserBurstWeapon = true;

            Ray r = new Ray(justASmidgeToTheRight, camera.lookAt);
            //Step two calculate collisions that might have occurred.
            //TODO: Ray intersection
            //Step three, Send message to the network to announce the event of the laser firing.
            //TODO: step 3
            //Step four, Play sound fx.
            //TODO: Upht.wav
        }
    }
}
