/*
Author: Clayton Sandham
Description: A simple 8-sided cylinder
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Camera3D;

namespace Geometry
{
    public class Cylinder : Microsoft.Xna.Framework.GameComponent
    {
        public Vector3 Position;
        public Matrix cubeScale = Matrix.CreateScale(1f, 1f, 1f);
        Matrix locationMatrix = Matrix.Identity;
        Matrix localRotation = Matrix.Identity;
        Matrix localScale = Matrix.CreateScale(4f, 4f, 4f);
        /// <summary>
        /// This variable should be set to true when a collision occurs.
        /// It will be turned off after one frame of drawing of this bastion.
        /// </summary>
        public bool collisionColoringActive = false;
        // Vertex data
        short[] indexBuffer;
        VertexPositionColor[] verts;
        VertexBuffer vertexBuffer;

        // Effect
        BasicEffect effect;

        // Color
        Color normalColor;
        readonly Color collisionFlashColor = Color.OrangeRed;

        int wireFrame;
        int cullMode;

        public Cylinder(Game game, Color color, Vector3 centerPoint)
            : base(game)
        {
            Position = centerPoint;
            locationMatrix *= Matrix.CreateTranslation(centerPoint);
            this.normalColor = color;
            SetWireframe(1);
            this.Initialize(); // Because Claudy hates manually calling initialize.
            ChangeAllVertexColors(color);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            float rt2 = (float)Math.Sqrt(2);
            verts = new VertexPositionColor[18];
            //top
            verts[0] = new VertexPositionColor(new Vector3(0, 1f/2, 0), normalColor);

            verts[1] = new VertexPositionColor(new Vector3(1 / rt2, 1f / 2, 1 / rt2), normalColor);
            verts[2] = new VertexPositionColor(new Vector3(0, 1f / 2, 1), normalColor);
            verts[3] = new VertexPositionColor(new Vector3(-1 / rt2, 1f / 2, 1 / rt2), normalColor);
            verts[4] = new VertexPositionColor(new Vector3(-1, 1f / 2, 0), normalColor);
            verts[5] = new VertexPositionColor(new Vector3(-1 / rt2, 1f / 2, -1 / rt2), normalColor);
            verts[6] = new VertexPositionColor(new Vector3(0, 1f / 2, -1), normalColor);
            verts[7] = new VertexPositionColor(new Vector3(1 / rt2, 1f / 2, -1 / rt2), normalColor);
            verts[8] = new VertexPositionColor(new Vector3(1, 1f / 2, 0), normalColor);

            //bottom
            verts[9] = new VertexPositionColor(new Vector3(0, -1f / 2, 0), normalColor);

            verts[10] = new VertexPositionColor(new Vector3(1 / rt2, -1f / 2, 1 / rt2), normalColor);
            verts[11] = new VertexPositionColor(new Vector3(0, -1f / 2, 1), normalColor);
            verts[12] = new VertexPositionColor(new Vector3(-1 / rt2, -1f / 2, 1 / rt2), normalColor);
            verts[13] = new VertexPositionColor(new Vector3(-1, -1f / 2, 0), normalColor);
            verts[14] = new VertexPositionColor(new Vector3(-1 / rt2, -1f / 2, -1 / rt2), normalColor);
            verts[15] = new VertexPositionColor(new Vector3(0, -1f / 2, -1), normalColor);
            verts[16] = new VertexPositionColor(new Vector3(1 / rt2, -1f / 2, -1 / rt2), normalColor);
            verts[17] = new VertexPositionColor(new Vector3(1, -1f / 2, 0), normalColor);


            indexBuffer = new short[96]
                {
                    //top circle: 8
                    0, 1, 2,
                    0, 2, 3,
                    0, 3, 4,
                    0, 4, 5,
                    0, 5, 6,
                    0, 6, 7,
                    0, 7, 8,
                    0, 8, 1,

                    //bottom circle: 8
                    9, 10, 11,
                    9, 11, 12,
                    9, 12, 13,
                    9, 13, 14,
                    9, 14, 15,
                    9, 15, 16,
                    9, 16, 17,
                    9, 17, 10,

                    //sides: 16
                    1, 10, 2,
                    2, 10, 11,

                    2, 11, 3,
                    3, 11, 12,

                    3, 12, 4,
                    4, 12, 13,

                    4, 13, 5,
                    5, 13, 14,

                    5, 14, 6,
                    6, 14, 15,

                    6, 15, 7,
                    7, 15, 16,

                    7, 16, 8,
                    8, 16, 17,

                    8, 17, 1,
                    1, 17, 10
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
            if (collisionColoringActive)
            {
                ChangeAllVertexColors(collisionFlashColor);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Change the normalColor of a vertex in this primitive.
        /// </summary>
        /// <param name="index">This value can only be between 0 and the number of vertices. 
        /// All other values result in a IndexOutOfRangeException.</param>
        /// <param name="normalColor"></param>
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
        /// Change the normalColor of the entire cube to the normalColor specified.
        /// </summary>
        /// <param name="normalColor">The normalColor specified by the function caller.</param>
        public void ChangeAllVertexColors(Color color)
        {
            // Must use for. Foreach would have worked if no assignment ops were used.
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i].Color = color; // Assignment op.
            }
            this.normalColor = color;
        }

        public void Move(Vector3 position)
        {
            locationMatrix *= Matrix.CreateTranslation(position);
            this.Position += position;
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
            effect.World = localRotation * localScale * locationMatrix;
            effect.View = camera.view;
            effect.Projection = camera.projection;
            effect.VertexColorEnabled = true;

            // Begin effect and drawWalls for each pass
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                Game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, verts, 0, 18, indexBuffer, 0, 32);

            }

            // Turn off the color flash and revert to normal colors.
            if (collisionColoringActive)
            {
                ChangeAllVertexColors(normalColor);
                collisionColoringActive = false;
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
