using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

using Games3Project2.Globals;
using Camera3D;
using Geometry;

namespace Games3Project2
{
    public class RemotePlayer : Collidable
    {
        public int score;
        Sphere sphere;
        Cube cube;
        Matrix cubeTransformation;
        const int gunLength = 3;
        public bool isJuggernaut;
        public float yaw;
        public float pitch;
        public NetworkGamer gamer;
        const int PACKET_INTERVAL = 10;
        int framesSinceLastPacket;
        float yawInterpolate;
        float pitchInterpolate;

        struct RemotePlayerState
        {
            public Vector3 position;
            public Vector3 velocity;
            public float pitch;
            public float yaw;
        }

        RemotePlayerState targetState;
        RemotePlayerState lastState;

        public RemotePlayer(Vector3 pos, NetworkGamer associatedGamer) :
            base(Global.game, pos, Vector3.Zero, Global.Constants.PLAYER_RADIUS)
        {
            score = 0;
            Color sphereColor = Color.Blue; 
            Texture2D blankTex = Global.game.Content.Load<Texture2D>(@"Textures\blankTexture");
            sphere = new Sphere(Global.game, sphereColor, pos);
            cube = new Cube(blankTex, sphereColor); 
            sphere.localScale = Matrix.CreateScale(5);
            sphere.SetWireframe(1);
            cube.wireFrame = false;
            cube.textured = false;
            cubeTransformation = Matrix.CreateScale(1, 1, gunLength) * Matrix.CreateTranslation(new Vector3(radius, 0, gunLength));
            isJuggernaut = false;
            yaw = 0;
            pitch = 0;

            targetState = new RemotePlayerState();
            targetState.position = position;
            targetState.velocity = velocity;
            targetState.pitch = pitch;
            targetState.yaw = yaw;
            lastState = new RemotePlayerState();
            lastState.position = position;
            lastState.velocity = velocity;
            lastState.pitch = pitch;
            lastState.yaw = yaw;
            framesSinceLastPacket = 0;
            yawInterpolate = 0;
            pitchInterpolate = 0;
        }

        public void update()
        {
            pitch += pitchInterpolate;
            yaw += yawInterpolate;
            position += velocity * Global.Constants.MOVEMENT_VELOCITY * Global.gameTime.ElapsedGameTime.Milliseconds;
            sphere.Position = position;
            sphere.Update(Global.gameTime);
        }

        public void draw()
        {
            sphere.Draw(Global.CurrentCamera);
            cube.Draw(Global.CurrentCamera, cubeTransformation *
                Matrix.CreateRotationX(MathHelper.ToRadians(pitch)) * Matrix.CreateRotationY(MathHelper.ToRadians(yaw))
                * Matrix.CreateTranslation(position));
        }

        public void receiveNewPacketUpdate(Vector3 newPos, Vector3 newVel, float newPitch, float newYaw)
        {
            lastState.position = position;
            lastState.velocity = velocity;
            lastState.pitch = pitch;
            lastState.yaw = yaw;

            targetState.position = newPos;
            targetState.velocity = newVel;
            targetState.pitch = newPitch;
            targetState.yaw = newYaw;

            yawInterpolate = targetState.yaw - lastState.yaw;
            pitchInterpolate = targetState.pitch - lastState.pitch;
            Velocity = targetState.position - lastState.position;
            Velocity /= PACKET_INTERVAL;
            Velocity.Normalize();

            framesSinceLastPacket = 0;
        }
    }
}
