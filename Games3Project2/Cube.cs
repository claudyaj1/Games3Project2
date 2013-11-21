/*
Author: Cole Stoltzfus
Description: A cube class that can be displayed either with a texture repeated on all sides or as wireframe.
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

using Games3Project2.Globals;
using Games3Project2;
using Camera3D;

namespace Geometry
{
    class Cube
    {
        // Vertex data
        VertexPositionColorTexture[] verts;
        VertexBuffer vertexBuffer;

        // Effect
        BasicEffect effect;
        private short[] indices;
        GraphicsDevice gd;

        public bool wireFrame = true;
        public bool textured = true;

        public Cube(Texture2D texture, Color color)
        {
            verts = new VertexPositionColorTexture[24];

            Vector3 topLeftFront = new Vector3(-1.0f, 1.0f, -1.0f);
            Vector3 bottomLeftFront = new Vector3(-1.0f, -1.0f, -1.0f);
            Vector3 topRightFront = new Vector3(1.0f, 1.0f, -1.0f);
            Vector3 bottomRightFront = new Vector3(1.0f, -1.0f, -1.0f);
            Vector3 topLeftBack = new Vector3(-1.0f, 1.0f, 1.0f);
            Vector3 topRightBack = new Vector3(1.0f, 1.0f, 1.0f);
            Vector3 bottomLeftBack = new Vector3(-1.0f, -1.0f, 1.0f);
            Vector3 bottomRightBack = new Vector3(1.0f, -1.0f, 1.0f);

            Vector2 textureTopLeft = new Vector2(0, 0);
            Vector2 textureTopRight = new Vector2(.5f, 0);
            Vector2 textureBottomLeft = new Vector2(0, .5f);
            Vector2 textureBottomRight = new Vector2(.5f, .5f);

            //front face
            verts[0] = new VertexPositionColorTexture(bottomLeftFront, color, textureBottomLeft);
            verts[1] = new VertexPositionColorTexture(topLeftFront, color, textureTopLeft);
            verts[2] = new VertexPositionColorTexture(topRightFront, color, textureTopRight);
            verts[3] = new VertexPositionColorTexture(bottomRightFront, color, textureBottomRight);

            //back face
            verts[4] = new VertexPositionColorTexture(bottomRightBack, color, textureBottomLeft);
            verts[5] = new VertexPositionColorTexture(topRightBack, color, textureTopLeft);
            verts[6] = new VertexPositionColorTexture(topLeftBack, color, textureTopRight);
            verts[7] = new VertexPositionColorTexture(bottomLeftBack, color, textureBottomRight);

            //right face
            verts[8] = new VertexPositionColorTexture(bottomRightFront, color, textureBottomLeft);
            verts[9] = new VertexPositionColorTexture(topRightFront, color, textureTopLeft);
            verts[10] = new VertexPositionColorTexture(topRightBack, color, textureTopRight);
            verts[11] = new VertexPositionColorTexture(bottomRightBack, color, textureBottomRight);

            //left face
            verts[12] = new VertexPositionColorTexture(bottomLeftBack, color, textureBottomLeft);
            verts[13] = new VertexPositionColorTexture(topLeftBack, color, textureTopLeft);
            verts[14] = new VertexPositionColorTexture(topLeftFront, color, textureTopRight);
            verts[15] = new VertexPositionColorTexture(bottomLeftFront, color, textureBottomRight);

            //top face
            verts[16] = new VertexPositionColorTexture(topLeftFront, color, textureBottomLeft);
            verts[17] = new VertexPositionColorTexture(topLeftBack, color, textureTopLeft);
            verts[18] = new VertexPositionColorTexture(topRightBack, color, textureTopRight);
            verts[19] = new VertexPositionColorTexture(topRightFront, color, textureBottomRight);

            //bottom face
            verts[20] = new VertexPositionColorTexture(bottomLeftBack, color, textureBottomLeft);
            verts[21] = new VertexPositionColorTexture(bottomLeftFront, color, textureTopLeft);
            verts[22] = new VertexPositionColorTexture(bottomRightFront, color, textureTopRight);
            verts[23] = new VertexPositionColorTexture(bottomRightBack, color, textureBottomRight);

            indices = new short[36];
            //PATTERN: bottom left, top left, bottom right, top left, top right, bottom right
            //front face
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 3;
            indices[3] = 1;
            indices[4] = 2;
            indices[5] = 3;

            //back face
            indices[6] = 4;
            indices[7] = 5;
            indices[8] = 7;
            indices[9] = 5;
            indices[10] = 6;
            indices[11] = 7;

            //right face
            indices[12] = 8;
            indices[13] = 9;
            indices[14] = 11;
            indices[15] = 9;
            indices[16] = 10;
            indices[17] = 11;

            //left face
            indices[18] = 12; 
            indices[19] = 13;
            indices[20] = 15;
            indices[21] = 13;
            indices[22] = 14;
            indices[23] = 15;

            //top face
            indices[24] = 16;
            indices[25] = 17;
            indices[26] = 19;
            indices[27] = 17;
            indices[28] = 18;
            indices[29] = 19;

            //bottom face
            indices[30] = 20;
            indices[31] = 21;
            indices[32] = 23;
            indices[33] = 21;
            indices[34] = 22;
            indices[35] = 23;

            gd = Global.graphics.GraphicsDevice;
            vertexBuffer = new VertexBuffer(gd, typeof(VertexPositionColorTexture), verts.Length, BufferUsage.None);
            vertexBuffer.SetData(verts);
            effect = new BasicEffect(gd);
            effect.Texture = texture;
            effect.TextureEnabled = true;
            effect.VertexColorEnabled = false;
        }

        public void Draw(Camera camera, Matrix world)
        {
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullClockwiseFace;
            if (wireFrame)
            {
                effect.TextureEnabled = false;
                effect.VertexColorEnabled = true;
                rs.FillMode = FillMode.WireFrame;
            }
            else
            {
                if (textured)
                {
                    effect.TextureEnabled = true;
                    effect.VertexColorEnabled = false;
                }
                else
                {
                    rs.FillMode = FillMode.Solid;
                    effect.TextureEnabled = false;
                    effect.VertexColorEnabled = true;
                }
            }
            gd.RasterizerState = rs;

            gd.SetVertexBuffer(vertexBuffer);

            effect.World = world;
            effect.View = camera.view;
            effect.Projection = camera.projection;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                gd.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, verts, 0, 24, indices, 0, 12);

            }
        }
    }
}
