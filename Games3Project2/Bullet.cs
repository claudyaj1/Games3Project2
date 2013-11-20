using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

using BoundingVolumeRendering;
using Games3Project2.Globals;

namespace Games3Project2
{
    public class Bullet : Collidable
    {
         BoundingSphere bs;

        public Bullet(Vector3 startPosition, Vector3 velocity) :
            base(Global.game, startPosition, velocity, Global.Constants.BULLET_RADIUS)
        {
            bs = new BoundingSphere(startPosition, Global.Constants.BULLET_RADIUS);
        }

        public void update(GameTime gametime)
        {
            position = position + velocity * gametime.ElapsedGameTime.Milliseconds;
            bs.Center = position;
        }

        public void draw()
        {
            BoundingSphereRenderer.Draw(bs,
                Global.CurrentCamera.view,
                Global.CurrentCamera.projection);
        }
            
    }
}
