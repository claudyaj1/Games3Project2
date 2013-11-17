/*
Author: Clayton Sandham
Description: A simple sphere.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

using Camera3D;

namespace Geometry
{
    public class Sphere : Microsoft.Xna.Framework.GameComponent
    {
        public const float BALL_SPEED_INITIAL = 0.08f;
        public Vector3 Position, Velocity;
        public Matrix cubeScale = Matrix.CreateScale(1f, 1f, 1f);
        Matrix locationMatrix = Matrix.Identity;
        Matrix localTranslation = Matrix.CreateTranslation(0, .25f, 0);
        Matrix localRotation = Matrix.Identity;
        public Matrix localScale = Matrix.Identity;
        // Vertex data
        short[] indexBuffer;
        VertexPositionColor[] verts;
        VertexBuffer vertexBuffer;

        // Effect
        BasicEffect effect;
        
        Color color;
        Random rand = new Random();

        int wireFrame;
        int cullMode;

        public Sphere(Game game, Color color, Vector3 centerPoint)
            : base(game)
        {
            Position = centerPoint;
            this.color = color;
            Velocity = new Vector3((float)rand.NextDouble() * 
                Math.Sign(rand.NextDouble() - 0.5f), 
                0.0f,
                (float)rand.NextDouble() * Math.Sign(rand.NextDouble() - 0.5f) / 1.3f);
            Velocity.Normalize();
            Velocity *= BALL_SPEED_INITIAL;
  

            locationMatrix = Matrix.CreateTranslation(centerPoint);
            SetWireframe(1);
            this.Initialize(); // Because Claudy hates manually calling initialize.
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            float rt2 = (float)Math.Sqrt(2);
            float twoRrt2 = 2*(float)Math.Sqrt(2);
            verts = new VertexPositionColor[26];

            float top = .9f;
            float ring1 = .9f / rt2;
            float ring2 = 0f;
            float ring3 = -ring1;
            float bottom = -top;

            //top vertex
            verts[0] = new VertexPositionColor(new Vector3(0, top, 0), color);

            //first ring1
            verts[1] = new VertexPositionColor(new Vector3(1f / 2, ring1, 1f / 2), color);
            verts[2] = new VertexPositionColor(new Vector3(0, ring1, 1f / rt2), color);
            verts[3] = new VertexPositionColor(new Vector3(-1f / 2, ring1, 1f / 2), color);
            verts[4] = new VertexPositionColor(new Vector3(-1f / rt2, ring1, 0f), color);
            verts[5] = new VertexPositionColor(new Vector3(-1f / 2, ring1, -1f / 2), color);
            verts[6] = new VertexPositionColor(new Vector3(0, ring1, -1f / rt2), color);
            verts[7] = new VertexPositionColor(new Vector3(1f / 2, ring1, -1f / 2), color);
            verts[8] = new VertexPositionColor(new Vector3(1f / rt2, ring1, 0), color);

            //middle ring 2
            verts[9] = new VertexPositionColor(new Vector3(1f / rt2, ring2, 1f / rt2), color);
            verts[10] = new VertexPositionColor(new Vector3(0f, ring2, 1f), color);
            verts[11] = new VertexPositionColor(new Vector3(-1f / rt2, ring2, 1f / rt2), color);
            verts[12] = new VertexPositionColor(new Vector3(-1f, ring2, 0f), color);
            verts[13] = new VertexPositionColor(new Vector3(-1f / rt2, ring2, -1f / rt2), color);
            verts[14] = new VertexPositionColor(new Vector3(0f, ring2, -1f), color);
            verts[15] = new VertexPositionColor(new Vector3(1f / rt2, ring2, -1f / rt2), color);
            verts[16] = new VertexPositionColor(new Vector3(1f, ring2, 0f), color);

            //lower ring 3
            verts[17] = new VertexPositionColor(new Vector3(1f / 2, ring3, 1f / 2), color);
            verts[18] = new VertexPositionColor(new Vector3(0, ring3, 1f / rt2), color);
            verts[19] = new VertexPositionColor(new Vector3(-1f / 2, ring3, 1f / 2), color);
            verts[20] = new VertexPositionColor(new Vector3(-1f / rt2, ring3, 0f), color);
            verts[21] = new VertexPositionColor(new Vector3(-1f / 2, ring3, -1f / 2), color);
            verts[22] = new VertexPositionColor(new Vector3(0, ring3, -1f / rt2), color);
            verts[23] = new VertexPositionColor(new Vector3(1f / 2, ring3, -1f / 2), color);
            verts[24] = new VertexPositionColor(new Vector3(1f / rt2, ring3, 0), color);

            verts[25] = new VertexPositionColor(new Vector3(0, bottom, 0), color);


            indexBuffer = new short[144]
                {
                    0, 1, 2,
                    0, 2, 3,
                    0, 3, 4,
                    0, 4, 5,
                    0, 5, 6,
                    0, 6, 7,
                    0, 7, 8,
                    0, 8, 1,

                    1, 9, 2,
                    2, 9, 10,

                    2, 10, 3,
                    3, 10, 11,

                    3, 11, 4,
                    4, 11, 12,

                    4, 12, 5,
                    5, 12, 13,

                    5, 13, 7,
                    6, 13, 14,

                    6, 14, 7,
                    7, 14, 15,

                    7, 15, 8,
                    8, 15, 16,

                    8, 16, 1,
                    1, 16, 9,



                    9, 17, 10,
                    10, 17, 18,

                    10, 18, 11,
                    11, 18, 19,

                    11, 19, 12,
                    12, 19, 20,

                    12, 20, 13,
                    13, 20, 21,

                    13, 21, 14,
                    14, 21, 22,

                    14, 22, 15,
                    15, 22, 23,

                    15, 23, 16,
                    16, 23, 24,

                    16, 24, 9,
                    9, 24, 17,


                    25, 18, 17,
                    25, 19, 18,

                    25, 20, 19,
                    25, 21, 20,

                    25, 22, 21,
                    25, 23, 22,

                    25, 24, 23,
                    25, 17, 24

                };

            // Set vertex data in VertexBuffer
            vertexBuffer = new VertexBuffer(Game.GraphicsDevice, typeof(VertexPositionTexture),
                verts.Length, BufferUsage.None);
            vertexBuffer.SetData(verts);

            // Initialize the BasicEffect
            effect = new BasicEffect(Game.GraphicsDevice);

            wireFrame = 0;
            cullMode = 0;

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (Math.Abs(Velocity.X) < 0.02f)
            {
                Velocity.X = Math.Sign(Velocity.X) * 0.02f;
            }
            if (Math.Abs(Velocity.Y) < 0.02f)
            {
                Velocity.Y = Math.Sign(Velocity.Y) * 0.02f;
            }
            this.Move(Velocity);
            base.Update(gameTime);
        }

        /// <summary>
        /// Change the color of a vertex in this primitive.
        /// </summary>
        /// <param name="index">This value can only be between 0 and the number of vertices. 
        /// All other values result in a IndexOutOfRangeException.</param>
        /// <param name="color"></param>
        public void ChangeVertexColor(int index, Color color)
        {
            if (index >= 0 && index < verts.Length)
            {
                verts[index].Color = color;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Change the color of the entire cube to the color specified.
        /// </summary>
        /// <param name="color">The color specified by the function caller.</param>
        public void ChangeAllVertexColors(Color color)
        {
            // Must use for. Foreach would have worked if no assignment ops were used.
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i].Color = color; // Assignment op.
            }
            this.color = color;
        }

        /// <summary>
        /// Move by a delta.
        /// </summary>
        /// <param name="newPosition"></param>
        public void Move(Vector3 moveamount)
        {
            this.Position += moveamount;  
            locationMatrix = Matrix.CreateTranslation(Position);
         
        }

        public void Draw(Camera camera)
        {
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            Game.GraphicsDevice.SamplerStates[0] = ss;


            RasterizerState rs = new RasterizerState();

            switch (cullMode)
            {
                case 0:
                    rs.CullMode = CullMode.None;
                    break;
                case 1:
                    rs.CullMode = CullMode.CullClockwiseFace;
                    break;
                case 2:
                    rs.CullMode = CullMode.CullCounterClockwiseFace;
                    break;
            }
            switch (wireFrame)
            {
                case 0:
                    rs.FillMode = FillMode.WireFrame;
                    break;
                case 1:
                    rs.FillMode = FillMode.Solid;
                    break;
            }
            Game.GraphicsDevice.RasterizerState = rs;

            // Set the vertex buffer on the GraphicsDevice
            Game.GraphicsDevice.SetVertexBuffer(vertexBuffer);


            //Set object and camera info
            effect.World = localRotation * localScale *localTranslation * locationMatrix;
            effect.View = camera.view;
            effect.Projection = camera.projection;
            effect.VertexColorEnabled = true;

            // Begin effect and draw for each pass
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                Game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, 
                    verts, 0, 26, 
                    indexBuffer, 0, 48);

            }
        }
        /// <summary>
        /// 0 = Wireframe | 1 = Filled
        /// </summary>
        public void SetWireframe(int wireBool)
        {
            wireFrame = wireBool;
        }
        /// <summary>
        /// 0 = None | 1 = Clockwise | 2 = CounterClockwise
        /// </summary>
        public void SetCullMode(int mode)
        {
            cullMode = mode;
        }
    }
}
