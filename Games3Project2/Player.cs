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

namespace Games3Project2
{
    public class LocalPlayer : Collidable
    {
        public Camera camera;
        public Cursor cursor;
        public Vector3 lastPosition;
        public PlayerIndex playerIndex;
        public int localPlayerIndex;
        public float jetPackThrust = 0;

        public LocalPlayer(Vector3 pos, PlayerIndex index, int localIndex)
            : base(Global.game, pos, Vector3.Zero, Global.Constants.PLAYER_RADIUS)
        {
            playerIndex = index;
            localPlayerIndex = localIndex;
            lastPosition = pos;
            Viewport viewport = new Viewport();

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

        public void update()
        {
            float timeDelta = (float)Global.gameTime.ElapsedGameTime.TotalSeconds;
            Vector3 dir = timeDelta * Global.input.get3DMovement14Directions(true, PlayerIndex.One);
            float yawChange = Global.Constants.SPIN_RATE * timeDelta * Global.input.GamepadByID[Global.input.toInt(playerIndex)].ThumbSticks.Right.X;
            float pitchChange = Global.Constants.SPIN_RATE * timeDelta * Global.input.GamepadByID[Global.input.toInt(playerIndex)].ThumbSticks.Right.Y;

            //Jetpack related calculations
            jetPackThrust -= Global.Constants.JET_PACK_DECREMENT;

            if (Global.input.isPressed(Buttons.RightShoulder) ||
                Global.input.isPressed(Buttons.LeftShoulder) ||
                Global.input.GamepadByID[Global.input.toInt(playerIndex)].Triggers.Left > 0f)
            {
                jetPackThrust += Global.Constants.JET_PACK_INCREMENT;
                //TODO: Jet Fuel subtraction.
            }
            else if (Global.input.GamepadByID[Global.input.toInt(playerIndex)].IsConnected)
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
            dir.Y += jetPackThrust * Global.gameTime.ElapsedGameTime.Milliseconds;
            dir.Y -= Global.Constants.GRAVITY * Global.gameTime.ElapsedGameTime.Milliseconds;
            /*
            #region If DEBUG && WINDOWS && (No Controller) Then Keyboard-and-Mouse does control
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
#endif
            #endregion
            */
            camera.Update(dir, yawChange, pitchChange);

            base.Update(Global.gameTime);
        }

        public void draw()
        {
            if (Global.CurrentCamera == camera)
            {
                cursor.Draw();
            }
        }
    }
}
