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
    class Collidable : Microsoft.Xna.Framework.GameComponent
    {
        public Vector3 position;
        public Vector3 velocity;
        public float radius;

        public Collidable(Game game, Vector3 pos, Vector3 vel, float radius_):
            base(game)
        {
            position = pos;
            velocity = vel;
            radius = radius_;
        }
    }
}
