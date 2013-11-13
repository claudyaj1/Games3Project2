using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Games3Project2.Globals;
using Claudy.Input;

namespace Broad.Camera
{
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        public Matrix view;
        public Matrix projection;
        public Vector3 cameraPos;
        public Vector3 cameraPrevPos;

        public Vector3 lookAt;
        public Vector3 cameraTarget = Vector3.Zero;
        public Vector3 cameraUpVector = Vector3.Up;
        public Vector3 cameraReference = new Vector3(0.0f, 0.0f, 1.0f);

        public float yaw;
        public float pitch;
        public float roll; //To be used maybe never

        public BoundingSphere BoundingSphere;
        public Matrix lookRotation = Matrix.Identity;

        public Vector3 lastPosition { get; protected set; }

        public float jetPackThrust = 0;


        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            //Initialize view matrix
            cameraPos = pos;
            lastPosition = pos;
            cameraPrevPos = cameraPos;
            lookAt = target;
            view = Matrix.CreateLookAt(pos, target, up);

            //Initialize projection matrix
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float)Game.Window.ClientBounds.Width /
                (float)Game.Window.ClientBounds.Height,
                1, 250);

            BoundingSphere = new BoundingSphere(pos, 1.5f);
            this.Initialize();
        }

       
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Updates the BroadCamera.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="debugMode">Typically false. Consider adjusting the call
        /// depending on Debug or Release</param>
        /// <param name="p_">Typically PlayerIndex.One</param>
        public void Update(bool debugMode, PlayerIndex p_)
        {
            float timeDelta = (float)Global.gameTime.ElapsedGameTime.TotalSeconds;
            Vector3 dir;
            dir = timeDelta * Global.input.get3DMovement14Directions(true, PlayerIndex.One);
            yaw -= Global.Constants.SPIN_RATE * timeDelta * Global.input.GamepadByID[Global.input.toInt(p_)].ThumbSticks.Right.X;
            pitch -= Global.Constants.SPIN_RATE * timeDelta * Global.input.GamepadByID[Global.input.toInt(p_)].ThumbSticks.Right.Y;
            
            //Jetpack related calculations
            jetPackThrust -= Global.Constants.JET_PACK_DECREMENT;

            if (Global.input.isPressed(Buttons.RightShoulder) ||
                Global.input.isPressed(Buttons.LeftShoulder) ||
                Global.input.GamepadByID[Global.input.toInt(p_)].Triggers.Left > 0f)
            {
                jetPackThrust += Global.Constants.JET_PACK_INCREMENT;
                //TODO: Jet Fuel subtraction.
            }
            else if(Global.input.GamepadByID[Global.input.toInt(p_)].IsConnected)
            {
                //TODO: Jet Fuel addition only if the controller is plugged in.
                jetPackThrust -= Global.Constants.JET_PACK_DECREMENT;
                if (jetPackThrust < 0)
                    jetPackThrust = 0;
            }
            //End Jetpack related calculations

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

            //These lines will need uncommented if ROLL is needed for the game.
            //if (input.ScrollWheelDelta > 0)
            //{
            //    roll++;
            //}
            //else if (input.ScrollWheelDelta < 0)
            //{
            //    roll--;
            //}
            //if (roll > 360)
            //    roll -= 360;
            //else if (roll < 0)
            //    roll += 360;

            Matrix yawR = Matrix.CreateRotationY(MathHelper.ToRadians(yaw));
            Matrix pitchR = Matrix.CreateRotationX(MathHelper.ToRadians(pitch));
            Matrix rollr = Matrix.CreateRotationZ(MathHelper.ToRadians(roll));

            //Matrix dirRotation = Matrix.Identity;
            // I don't think you want to have pitch here because otherwise the player will fly.
            Matrix dirRotation = yawR; 
            
            //JetPack
            if (jetPackThrust > Global.Constants.JET_PACK_Y_VELOCITY_CAP)
            {
                jetPackThrust = Global.Constants.JET_PACK_Y_VELOCITY_CAP;
            }
            dir.Y += jetPackThrust * Global.gameTime.ElapsedGameTime.Milliseconds;
            dir.Y -= Global.Constants.GRAVITY * Global.gameTime.ElapsedGameTime.Milliseconds;

            //// cameraPos update ////
            if (dir != Vector3.Zero)
            {
                Vector3 dt = Global.Constants.MOVEMENT_VELOCITY * dir * Global.gameTime.ElapsedGameTime.Milliseconds;
                Vector3.Transform(ref dt, ref dirRotation, out dt);
                cameraPos += dt;
            }

            // Updating collision BoundingSphere Position.
            BoundingSphere.Center = cameraPos;

            //// LookAt update ////
            
            //Rotates lookat
            //lookRotation = Matrix.Identity;
            lookRotation = pitchR * yawR;

            SetCamera(lookRotation);
            
            roll = 0;
            base.Update(Global.gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookRoation"></param>
        public void SetCamera(Matrix lookRoation)
        {
            Vector3 transformedReference;
            Vector3.Transform(ref cameraReference, ref lookRoation, out transformedReference);
            cameraUpVector.Normalize();

            Vector3 foo = cameraPos;
            Vector3.Add(ref foo, ref transformedReference, out cameraTarget);
            Matrix bar = view;
            lookAt = cameraTarget;
            Matrix.CreateLookAt(ref foo, ref cameraTarget, ref cameraUpVector, out bar);
            view = bar;
        }
        public Vector3 getLookAt() { return lookAt; }
    }
}
