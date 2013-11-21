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
using Geometry;
using Camera3D;
using Games3Project2.Globals;

namespace Games3Project2
{
    class Platform : Microsoft.Xna.Framework.GameComponent
    {
        public enum PlatformType { Horizontal, VerticalX, VerticalZ };
        public PlatformType platformType;
        #region Get/Set
        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                translation = Matrix.CreateTranslation(position.X, position.Y, position.Z);
            }
        }
        public float Width
        {
            get { return width; }
            set
            {
                width = value;
                scale = Matrix.CreateScale(width, 1, length);
            }
        }
        public float Length
        {
            get { return length; }
            set
            {
                length = value;
                scale = Matrix.CreateScale(width, 1, length);
            }
        }
        #endregion
        //general properties
        Vector3 position;
        Vector3 normal;
        float length;
        float width;
        public Texture2D texture;
        //matrices
        Matrix scale;
        Matrix translation;
        public Matrix rotation;
        //geometry
        Quad quad;

        public Platform(Vector3 pos_, float width_, float length_, Texture2D tex, PlatformType platType) :
            base(Global.game)
        {
            Position = pos_;
            Width = width_;
            Length = length_;
            texture = tex;

            quad = new Quad(tex, Color.White);
            quad.mirrorTexture = true;
            rotation = Matrix.Identity;
            platformType = platType;
            switch (platformType)
            {
                case PlatformType.Horizontal:
                    normal = new Vector3(0, 1, 0);
                    break;
                case PlatformType.VerticalX:
                    normal = new Vector3(0, 0, 1);
                    break;
                case PlatformType.VerticalZ:
                    normal = new Vector3(1, 0, 0);
                    break;
            }
        }

        public void collide(Collidable collider)
        {
            //initially check only the plane bounds
            Vector3 diffVector = collider.Position - position;
            float distance = Math.Abs(Vector3.Dot(diffVector, normal));
            if (distance > collider.Radius)
            {
                //no collision
            }
            else //collision, so collide based on plane
            {
                switch (platformType)
                {
                    case PlatformType.Horizontal:
                        collideHorizontal(collider);
                        break;
                    case PlatformType.VerticalX:
                        collideVerticalX(collider);
                        break;
                    case PlatformType.VerticalZ:
                        collideVerticalZ(collider);
                        break;
                }
            }
        }

        public bool didCollide(Collidable collider)
        {
            Vector3 diffVector = collider.Position - position;
            float distance = Math.Abs(Vector3.Dot(diffVector, normal));
            if (distance <= collider.Radius)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void update()
        {

        }

        public void draw()
        {
            quad.Draw(Global.gameTime, scale * rotation * translation, Global.CurrentCamera.view, Global.CurrentCamera.projection);
        }

        private void collideHorizontal(Collidable collider)
        {
            if (collider.Position.X > Position.X - (Width + collider.Radius) && collider.Position.X < Position.X + (Width + collider.Radius)
                   && collider.Position.Z > Position.Z - (Length + collider.Radius) && collider.Position.Z < Position.Z + (Length + collider.Radius))
            {
                //collides, so calculate time and new position for collider
                Vector2 platformStart = new Vector2(position.X - width, position.Y);
                Vector2 platformEnd = new Vector2(position.X + width, position.Y);
                Vector2 collisionRay = platformEnd - platformStart;

                float time = ((collider.Position.X - platformStart.X) * collisionRay.X + (collider.Position.Y - platformStart.Y) * collisionRay.Y) / collisionRay.LengthSquared();
                Vector2 collisionPosition = platformStart + time * collisionRay;
                Vector2 xyPos = new Vector2(collider.Position.X, collider.Position.Y);
                Vector2 displacement = xyPos - collisionPosition;
                
                if(false)// ((xyPos - collisionPosition).LengthSquared() < collider.Radius * collider.Radius)
                    time = 0;
                else
                {
                    time = -Vector2.Dot(displacement, xyPos) / (Vector3.Dot(collider.Velocity, collider.Velocity));
                    Vector2 xyVelocity = new Vector2(collider.Velocity.X, collider.Velocity.Y);
                    time = (float)(-Vector2.Dot(displacement, xyVelocity) - Math.Sqrt(Math.Pow(Vector2.Dot(displacement, xyVelocity), 2.0) - Vector3.Dot(collider.Velocity, collider.Velocity) *
                        (Vector2.Dot(displacement, displacement) - collider.Radius * collider.Radius))) / Vector3.Dot(collider.Velocity, collider.Velocity);
                }

                Vector3 newPos = collider.PrevPosition + collider.Velocity * time;
                collider.Position = new Vector3(collider.Position.X, newPos.Y, collider.Position.Z);
            }
        }

        private void collideVerticalX(Collidable collider)
        {
            if (collider.Position.X > Position.X - (Width + collider.Radius) && collider.Position.X < Position.X + (Width + collider.Radius)
                   && collider.Position.Y > Position.Y - (Length + collider.Radius) && collider.Position.Y < Position.Y + (Length + collider.Radius))
            {
                //collides, so calculate time and new position for collider
                Vector2 platformStart = new Vector2(position.X - (width + collider.Radius), position.Z);
                Vector2 platformEnd = new Vector2(position.X + (width + collider.Radius), position.Z);
                Vector2 collisionRay = platformEnd - platformStart;

                float time = ((collider.Position.X - platformStart.X) * collisionRay.X + (collider.Position.Z - platformStart.Y) * collisionRay.Y) / collisionRay.LengthSquared();
                Vector2 collisionPosition = platformStart + time * collisionRay;
                Vector2 xzPos = new Vector2(collider.Position.X, collider.Position.Z);
                Vector2 displacement = xzPos - collisionPosition;

                if(false)// ((xzPos - collisionPosition).LengthSquared() < collider.Radius * collider.Radius)
                    time = 0;
                else
                {
                    time = -Vector2.Dot(displacement, xzPos) / (Vector3.Dot(collider.Velocity, collider.Velocity));
                    Vector2 xzVelocity = new Vector2(collider.Velocity.X, collider.Velocity.Z);
                    time = (float)(-Vector2.Dot(displacement, xzVelocity) - Math.Sqrt(Math.Pow(Vector2.Dot(displacement, xzVelocity), 2.0) - Vector3.Dot(collider.Velocity, collider.Velocity) *
                        (Vector2.Dot(displacement, displacement) - collider.Radius * collider.Radius))) / Vector3.Dot(collider.Velocity, collider.Velocity);
                }

                Vector3 newPos = collider.PrevPosition + collider.Velocity * time;
                collider.Position = new Vector3(collider.Position.X, collider.Position.Y, newPos.Z);
            }
        }

        private void collideVerticalZ(Collidable collider)
        {
            if (collider.Position.Z > Position.Z - (Length + collider.Radius) && collider.Position.Z < Position.Z + (Length + collider.Radius)
                   && collider.Position.Y > Position.Y - (Width + collider.Radius) && collider.Position.Y < Position.Y + (Width + collider.Radius))
            {
                //collides, so calculate time and new position for collider
                Vector2 platformStart = new Vector2(position.Z - (Length + collider.Radius), position.X);
                Vector2 platformEnd = new Vector2(position.Z + (Length + collider.Radius), position.X);
                Vector2 collisionRay = platformEnd - platformStart;

                float time = ((collider.Position.Z - platformStart.X) * collisionRay.X + (collider.Position.X - platformStart.Y) * collisionRay.Y) / collisionRay.LengthSquared();
                Vector2 collisionPosition = platformStart + time * collisionRay;
                Vector2 xzPos = new Vector2(collider.Position.Z, collider.Position.X);
                Vector2 displacement = xzPos - collisionPosition;

                if(false)// ((xzPos - collisionPosition).LengthSquared() < collider.Radius * collider.Radius)
                    time = 0;
                else
                {
                    time = -Vector2.Dot(displacement, xzPos) / (Vector3.Dot(collider.Velocity, collider.Velocity));
                    Vector2 xzVelocity = new Vector2(collider.Velocity.Z, collider.Velocity.X);
                    time = (float)(-Vector2.Dot(displacement, xzVelocity) - Math.Sqrt(Math.Pow(Vector2.Dot(displacement, xzVelocity), 2.0) - Vector3.Dot(collider.Velocity, collider.Velocity) *
                        (Vector2.Dot(displacement, displacement) - collider.Radius * collider.Radius))) / Vector3.Dot(collider.Velocity, collider.Velocity);
                }

                Vector3 newPos = collider.PrevPosition + collider.Velocity * time;
                collider.Position = new Vector3(newPos.X, collider.Position.Y, collider.Position.Z);
            }
        }
    }
}
