using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Claudy.Input;

namespace Broad.Camera
{
    public class BroadCamera : Microsoft.Xna.Framework.GameComponent
    {
        public const float MOVEMENT_VELOCITY = 5f;
        public const float SPIN_RATE = 100f;

        public Matrix view { get;  set; }
        public Matrix projection { get;  set; }
        public Vector3 cameraPos { get; set; }
        public Vector3 cameraPrevPos { get; set; }

        private Vector3 lookAt;
        private Vector3 cameraTarget = Vector3.Zero;
        private Vector3 cameraUpVector = Vector3.Up;
        private Vector3 cameraReference = new Vector3(0.0f, 0.0f, 1.0f);

        private float yaw;
        private float pitch;
        private float roll; //To be used later

        public BoundingSphere BoundingSphere;
        public Matrix lookRotation = Matrix.Identity;

        public Vector3 lastPosition { get; protected set; }

        private ClaudyInput input;

        public BroadCamera(Game game, Vector3 pos, Vector3 target, Vector3 up)
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
            input = ClaudyInput.Instance;
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
        public void Update(GameTime gameTime, bool debugMode, PlayerIndex p_)
        {
            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector3 dir;
            dir = timeDelta * input.get3DMovement14Directions(true, PlayerIndex.One);
            yaw -= SPIN_RATE * timeDelta * input.GamepadByID[1].ThumbSticks.Right.X;
            pitch -= SPIN_RATE * timeDelta * input.GamepadByID[1].ThumbSticks.Right.Y;

            #region If DEBUG && WINDOWS && (No Controller) Then Keyboard-n-Mouse does control
#if DEBUG && WINDOWS
            if (!input.isConnected(PlayerIndex.One))
            {
                const float MOUSE_SENSITIVITY = 20f;
                const float KEYBOARD_SENSITIVITY = 0.04f;
                dir -= input.get3DMovement14Directions(false);
                dir *= KEYBOARD_SENSITIVITY;
                Vector2 mouseDelta = input.getMouseDelta();

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
            }
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
            
            //// cameraPos update ////
            if (dir != Vector3.Zero)
            {
                Vector3 dt = MOVEMENT_VELOCITY * dir * gameTime.ElapsedGameTime.Milliseconds;
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
            base.Update(gameTime);
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
