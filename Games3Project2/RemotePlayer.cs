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
        const int PACKET_INTERVAL = 600;
        float currentSmoothing;
        int framesSinceLastPacket;

        struct RemotePlayerState
        {
            public Vector3 position;
            public Vector3 velocity;
            public float pitch;
            public float yaw;
        }

        RemotePlayerState simulationState;
        RemotePlayerState previousState;

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

            simulationState = new RemotePlayerState();
            simulationState.position = position;
            simulationState.velocity = velocity;
            simulationState.pitch = pitch;
            simulationState.yaw = yaw;
            previousState = new RemotePlayerState();
            previousState.position = position;
            previousState.velocity = velocity;
            previousState.pitch = pitch;
            previousState.yaw = yaw;
            framesSinceLastPacket = 0;
            currentSmoothing = 0;
        }

        public void update()
        {
            simulationState.position += simulationState.velocity * Global.Constants.MOVEMENT_VELOCITY * (float)Global.gameTime.ElapsedGameTime.TotalSeconds * Global.gameTime.ElapsedGameTime.Milliseconds;
            previousState.position += previousState.velocity * Global.Constants.MOVEMENT_VELOCITY * (float)Global.gameTime.ElapsedGameTime.TotalSeconds * Global.gameTime.ElapsedGameTime.Milliseconds;
            currentSmoothing -= 1.0f / (float)PACKET_INTERVAL;
            if (currentSmoothing < 0)
                currentSmoothing = 0;
            position = Vector3.Lerp(previousState.position, simulationState.position, currentSmoothing);
            velocity = Vector3.Lerp(simulationState.velocity, previousState.velocity, currentSmoothing);
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
            previousState.position = position;
            previousState.velocity = velocity;
            previousState.pitch = pitch;
            previousState.yaw = yaw;

            simulationState.position = newPos;
            simulationState.velocity = newVel;
            simulationState.pitch = newPitch;
            simulationState.yaw = newYaw;

            yaw = newYaw;
            pitch = newPitch;

            framesSinceLastPacket = 0;
        }
    }
}
