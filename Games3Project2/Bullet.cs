using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Net;

using BoundingVolumeRendering;
using Games3Project2.Globals;
using Geometry;

namespace Games3Project2
{
    public class Bullet : Collidable
    {
        BoundingSphere bs;
        public int timeLived;
        /// <summary>
        /// // The NetworkGamer who shot this bullet.
        /// </summary>
        public NetworkGamer shooter; 
        public Vector3 startPosition;
        public int damage;
        private Sphere sphere;

        public enum State { Active, Idle };
        public State state;

        public Bullet() :
            base(Global.game, Vector3.Zero, Vector3.Zero, Global.Constants.BULLET_RADIUS)
        {
            sphere = new Sphere(Global.game, Global.Constants.BULLET_COLOR, startPosition);
            sphere.localScale = Matrix.CreateScale(0.3f);
            sphere.SetWireframe(1);
            bs = new BoundingSphere(startPosition, Global.Constants.BULLET_RADIUS);
            timeLived = 0;
            state = State.Idle;
        }

        public void fire(Vector3 startPosition, Vector3 velocity, NetworkGamer shooter, int damage)
        {
            this.startPosition = startPosition;
            position = startPosition;
            //need a brand new velocity vector so it doesn't take on player's residual velocity
            this.velocity = new Vector3(velocity.X, velocity.Y, velocity.Z);
            this.shooter = shooter;
            this.damage = damage;
            timeLived = 0;
            state = State.Active;
            Global.shot.Play();
        }

        public void disable()
        {
            startPosition = Vector3.Zero;
            position = Vector3.Zero;
            velocity = Vector3.Zero;
            shooter = null;
            damage = 0;
            timeLived = 0;
            state = State.Idle;
        }

        public void update(GameTime gametime)
        {
            switch (state)
            {
                case State.Active:
                    position = position + velocity * gametime.ElapsedGameTime.Milliseconds;
                    bs.Center = position;
                    timeLived += gametime.ElapsedGameTime.Milliseconds;
                    sphere.Position = position;
                    sphere.Update(Global.gameTime);
                    break;
                case State.Idle:
                    //hehe
                    break;
            }
        }

        public void draw()
        {
            switch (state)
            {
                case State.Active:
                    sphere.Draw(Global.CurrentCamera);
                    break;
                case State.Idle:

                    break;
            }
        }
            
    }
}
