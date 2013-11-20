/*
Author: Cole Stoltzfus
Description: A texturable Quad class that can display either a texture or wireframe.
*/
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

namespace Geometry
{
    class Quad
    {
        // Vertex data
        VertexPositionColorTexture[] verts;
        VertexBuffer vertexBuffer;

        // Effect
        BasicEffect effect;
        private short[] indices;
        GraphicsDevice gd;

        //properties
        public bool mirrorTexture;
        public bool wireFrame;

        public Quad(GraphicsDevice g, Texture2D texture, Color color)
        {
            Vector3 position = new Vector3(-1f, 0f, -1f);
            verts = new VertexPositionColorTexture[4];

            verts[0] = new VertexPositionColorTexture(position, color, new Vector2(0, 1));

            position = new Vector3(-1f, 0f, 1f);
            verts[1] = new VertexPositionColorTexture(position, color, Vector2.Zero);

            position = new Vector3(1f, 0f, 1f);
            verts[2] = new VertexPositionColorTexture(position, color, new Vector2(1, 0));

            position = new Vector3(1f, 0f, -1f);
            verts[3] = new VertexPositionColorTexture(position, color, new Vector2(1, 1));

            indices = new short[6];

            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 3;
            indices[3] = 1;
            indices[4] = 2;
            indices[5] = 3;

            vertexBuffer = new VertexBuffer(g, typeof(VertexPositionColorTexture), verts.Length, BufferUsage.None);
            vertexBuffer.SetData(verts);
            effect = new BasicEffect(g);
            gd = g;
            effect.TextureEnabled = true;
            effect.VertexColorEnabled = false;
            effect.Texture = texture;

            wireFrame = false;
            mirrorTexture = false;
        }

        public void Draw(GameTime gameTime, Matrix world, Matrix cameraView, Matrix cameraProjection)
        {
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            if (wireFrame)
            {
                effect.TextureEnabled = false;
                effect.VertexColorEnabled = true;
                rs.FillMode = FillMode.Solid;
            }
            else
            {
                effect.TextureEnabled = true;
                effect.VertexColorEnabled = false;
                rs.FillMode = FillMode.Solid;
                if (mirrorTexture)
                {
                    SamplerState ss = new SamplerState();
                    ss.AddressU = TextureAddressMode.Clamp;
                    ss.AddressV = TextureAddressMode.Clamp;
                    ss.Filter = TextureFilter.Anisotropic;
                    gd.SamplerStates[0] = ss;
                }
            }

            gd.RasterizerState = rs;

            gd.SetVertexBuffer(vertexBuffer);

            effect.World = world;
            effect.View = cameraView;
            effect.Projection = cameraProjection;
            effect.VertexColorEnabled = true;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                gd.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, verts, 0, 4, indices, 0, 2);

            }
        }
    }
}
