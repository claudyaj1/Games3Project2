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
        //TODO: CreateTranslation matrix
        public const int TTL = 2000; //Milliseconds
        public int timeLived;

        public Bullet(Vector3 startPosition, Vector3 velocity) :
            base(Global.game, startPosition, velocity, Global.Constants.BULLET_RADIUS)
        {
            bs = new BoundingSphere(startPosition, Global.Constants.BULLET_RADIUS);
            timeLived = 0;
        }

        public void update(GameTime gametime)
        {
            position = position + velocity * gametime.ElapsedGameTime.Milliseconds;
            bs.Center = position;
            timeLived += gametime.ElapsedGameTime.Milliseconds;
        }

        public void draw()
        {
            BoundingSphereRenderer.Draw(bs,
                Global.CurrentCamera.view,
                Global.CurrentCamera.projection);
        }
            
    }
}
