using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Games3Project2.Globals;
using InputHandler;

namespace Camera3D
{
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        public Matrix view;
        public Matrix projection;
        public Vector3 cameraPos;
        public Vector3 cameraPrevPos;
        public Viewport viewport;

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

        public Camera(Vector3 pos, Vector3 target, Vector3 up, Viewport viewport_)
            : base(Global.game)
        {
            //Initialize view matrix
            cameraPos = pos;
            lastPosition = pos;
            cameraPrevPos = cameraPos;
            lookAt = target;
            view = Matrix.CreateLookAt(pos, target, up);
            viewport = viewport_;

            //Initialize projection matrix
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float)viewport.Width /
                (float)viewport.Height,
                1, 1000);

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
        public void Update(Vector3 dir, float yawChange, float pitchChange)
        {
            yaw -= yawChange;
            pitch -= pitchChange;
            if (pitch < -90 || pitch > 90)
            {
                pitch = Math.Sign(pitch) * 89.99f;
            }
            
            Matrix yawR = Matrix.CreateRotationY(MathHelper.ToRadians(yaw));
            Matrix pitchR = Matrix.CreateRotationX(MathHelper.ToRadians(pitch));
            Matrix rollr = Matrix.CreateRotationZ(MathHelper.ToRadians(roll));

            //Matrix dirRotation = Matrix.Identity;
            // I don't think you want to have pitch here because otherwise the player will fly.
            Matrix dirRotation = yawR; 

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
