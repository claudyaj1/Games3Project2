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
using ReticuleCursor;
using InputHandler;
using Geometry;

namespace AI
{

    public class BugBot : Collidable
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
        private int health;
        public int spawnerTimer;

        Sphere body;

        public BugBot(Vector3 position, float speed, Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4)
            : base(Global.game, position, Vector3.Zero, Global.Constants.PLAYER_RADIUS/2)
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
            spawnerTimer = 600;
            health = Global.Constants.MAX_HEALTH;

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
                foreach (LocalPlayer player in Global.localPlayers)
                {
                    if (player.isJuggernaut && (position - player.Position).Length() < BugBot.ATTACK_RADIUS)
                    {
                        //move point if player is in range of bot
                        movePoint(pointCount, player.Position);

                    }
                }
                for (int i = 0; i < Global.BulletManager.bullets.Count; ++i)
                {
                    if (Global.Collision.didCollide(Global.BulletManager.bullets[i], this))
                    {
                        if (Global.BulletManager.bullets[i].timeLived < Global.Constants.BULLET_POWER_DISTANCE)
                        {
                            health -= Global.BulletManager.bullets[i].damage;
                        }
                        else
                        {
                            health -= Global.BulletManager.bullets[i].damage * 2;
                        }
                        if (health < 0)
                        {
                            //GamePad.SetVibration(gamer.SignedInGamer.PlayerIndex, Global.Constants.VIBRATION_LOW, 0f);
                            killBot();
                        }
                        else
                        {
                            //GamePad.SetVibration(gamer.SignedInGamer.PlayerIndex, 0f, Global.Constants.VIBRATION_HIGH);
                        }
                        Global.BulletManager.bullets[i].disable();
                    }
                }

            }
            else
            {
                if (spawnerTimer <= 0)
                {
                    reviveBot();
                }
            }

        }

        public void killBot()
        {
            Alive = false;
        }

        public void reviveBot()
        {
            Alive = true;
            health = Global.Constants.MAX_HEALTH;
        }

        public void draw()
        {
            if (Alive)
            {
                body.Draw(Global.CurrentCamera);
            }
        }

        public void ShootBullet(Vector3 dir)
        {
            dir = randomizeFiringVector(dir);
            Global.networkManager.fireBullet(Global.BulletManager.fireBullet(position, dir * Global.Constants.BULLET_SPEED, null, Global.Constants.BULLET_DAMAGE));
        }

        private Vector3 randomizeFiringVector(Vector3 dir)
        {
            Vector3 newDir = new Vector3(dir.X, dir.Y, dir.Z);
            newDir.X += Global.rand.Next(-1500, 1500) / 1000;
            newDir.Y += Global.rand.Next(-1500, 1500) / 1000;
            newDir.Z += Global.rand.Next(-1500, 1500) / 1000;

            return newDir;
        }

        //movePoint is for the AI to change trajectory points based on 
        //player interaction.
        //int ptIndex -- passes in the index of the point to move.
        //Vector3 target -- passes in the direction from the point to the player being shot.
        public void movePoint(int ptIndex, Vector3 target)
        {
            Vector3 ptDir = target - points[ptIndex];
            ptDir.Normalize();
            points[ptIndex] += Global.Constants.ptSpeed * ptDir;
        }

        public Vector3[] getBotTrajectoryPoints()
        {
            return points;
        }
    }
}
