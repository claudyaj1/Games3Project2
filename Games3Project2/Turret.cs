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
using Games3Project2;
using Games3Project2.Globals;
using Camera3D;
using Geometry;

namespace AI
{
    public class Turret : Collidable
    {
        Sphere body;
        int lastFiringTime;
        const int ATTACK_RADIUS = 200;
        const int VERTICAL_DAMPER = 20;
        const int COOLDOWN = 500;

        public Turret(Vector3 pos) :
            base(Global.game, pos, Vector3.Zero, Global.Constants.BOT_RADIUS)
        {
            body = new Sphere(Global.game, Color.Red, pos);
            body.localScale = Matrix.CreateScale(Radius);
            body.SetWireframe(1);
            lastFiringTime = 0;
        }

        public void update()
        {
            lastFiringTime += Global.gameTime.ElapsedGameTime.Milliseconds;
            if (lastFiringTime > COOLDOWN)
            {
                foreach (LocalPlayer player in Global.localPlayers)
                {
                    if (player.Position.Y - position.Y < VERTICAL_DAMPER && (player.Position - position).Length() < ATTACK_RADIUS)
                    {
                        Vector3 dir = player.Position - position;
                        dir.Normalize();
                        shootBullet(dir);
                        lastFiringTime = 0;
                    }
                }
            }
        }

        public void draw()
        {
            body.Draw(Global.CurrentCamera);
        }

        public void shootBullet(Vector3 dir)
        {
            dir = randomizeFiringVector(dir);
            Global.networkManager.fireBullet(Global.BulletManager.fireBullet(position, dir * Global.Constants.BULLET_SPEED, null, Global.Constants.BULLET_DAMAGE));
        }

        private Vector3 randomizeFiringVector(Vector3 dir)
        {
            Vector3 newDir = new Vector3(dir.X, dir.Y, dir.Z);
            newDir.X += Global.rand.Next(-3, 3) / 10;
            newDir.Y += Global.rand.Next(-3, 3) / 10;
            newDir.Z += Global.rand.Next(-3, 3) / 10;

            return newDir;
        }
    }
}
