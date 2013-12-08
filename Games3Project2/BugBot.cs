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
using Games3Project2.Globals;
using Camera3D;
using ReticuleCursor;
using InputHandler;
using Geometry;

namespace Games3Project2
{
    
    public class BugBot
    {
        public Vector3 position;
        public Vector3 direction;
        public float speed;
        public Vector3[] points;
        public Boolean Alive;
        public int pointCount = 0;
        public int lastFiringTime;
        public const int ATTACK_RADIUS = 100;
        public readonly int npcID;

        Sphere body;

        public BugBot(Vector3 position, float speed, Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4)
        {
            this.position = position;
            body = new Sphere(Global.game, Color.GhostWhite, this.position);
            body.localScale = Matrix.CreateScale(2);
            body.SetWireframe(1);
            this.speed = speed;
            points = new Vector3[4];
            points[0] = point1;
            points[1] = point2;
            points[2] = point3;
            points[3] = point4;
            Alive = true;
            lastFiringTime = 0;
            npcID = -(new Random().Next(100));

        }
        
        public void update()
        {
            //move in a square pattern
            if (Alive)
            {
                direction = position - points[pointCount];
                lastFiringTime += Global.gameTime.ElapsedGameTime.Milliseconds;

                if ((position - points[pointCount]).Length() < 2)
                {
                    pointCount++;
                    if (pointCount >= 4)
                        pointCount = 0;
                }

                direction.Normalize();

                position -= speed * direction * Global.gameTime.ElapsedGameTime.Milliseconds;

                body.Position = position;
                body.Update(Global.gameTime);

                if (lastFiringTime > Global.Constants.BOT_FIRING_COOLDOWN)
                {
                    foreach (LocalPlayer player in Global.localPlayers)
                    {
                        if (player.isJuggernaut && (position - player.Position).Length() < BugBot.ATTACK_RADIUS)
                        {
                            //shoot a bullet at the player if he is the juggernaught
                            Vector3 dir = player.Position - position;
                            dir.Normalize();
                            ShootBullet(dir);
                            lastFiringTime = 0;
                        }
                    }
                }
            }

        }

        public void draw()
        {
            body.Draw(Global.CurrentCamera);
        }

        public void ShootBullet(Vector3 dir)
        {
            Global.BulletManager.fireBullet(position, dir * Global.Constants.BULLET_SPEED, null, Global.Constants.BULLET_DAMAGE);
        }

    }
}
