using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Games3Project2
{
    public class Collidable : Microsoft.Xna.Framework.GameComponent
    {
        protected Vector3 position;
        protected Vector3 prevPosition;
        protected Vector3 velocity;
        protected float radius;

        public virtual Vector3 PrevPosition
        {
            get { return prevPosition; }
        }

        public virtual Vector3 Position
        {
            get { return position; }
            set 
            {
                prevPosition = position;
                position = value; 
            }
        }
        public virtual Vector3 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        public virtual float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public Collidable(Game game, Vector3 pos, Vector3 vel, float radius):
            base(game)
        {
            position = pos;
            prevPosition = position;
            velocity = vel;
            this.radius = radius;
        }

        public virtual void platformCollision()
        {
            position.Y = prevPosition.Y;
        }
    }
}
