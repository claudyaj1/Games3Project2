using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Claudy.AxisReference
{
    /// <summary>
    /// Draws a single unit sized axis indicator.
    /// Axis Colors are R,G,B respectively. (i.e. X = R, Y = G, Z = B)
    /// </summary>
    /// <author>Andrew Claudy</author>
    class Axis_Reference
    {
        // Axis Colors are R,G,B respectively. (i.e. X = R, Y = G, Z = B)
        private Color xAxisColor = Color.Red;
        private Color yAxisColor = Color.Green;
        private Color zAxisColor = Color.Blue;

        GraphicsDevice graphicsDevice;
        BasicEffect effect;

        // Vertex data
        VertexPositionColor[] verts;
        VertexBuffer vertexBuffer;
        private short[] vertexIndicies;

        //No rotation, scaling, nor translation. Should be raw world matrix.

        /// <summary>
        /// Creates the axis object for purpose of showing where the
        /// three axis are in world space.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="length">Typically 1.0f</param>
        public Axis_Reference(GraphicsDevice graphicsDevice, float length)
        {
            if (length < 0.0f)
            {
                throw new ArgumentOutOfRangeException("length",
                    "The Axis_Reference constructor's parameter \"length\" requires a value greater than zero.");
            }
            this.graphicsDevice = graphicsDevice;
            effect = new BasicEffect(this.graphicsDevice);

            //Initialize list of possible vertices
            verts = new VertexPositionColor[4];
            verts[0] = new VertexPositionColor(
                new Vector3(0, 0, 0), Color.AntiqueWhite);
            verts[1] = new VertexPositionColor(
                new Vector3(length, 0, 0), xAxisColor);
            verts[2] = new VertexPositionColor(
                new Vector3(0, length, 0), yAxisColor);
            verts[3] = new VertexPositionColor(
                new Vector3(0, 0, length), zAxisColor);
            vertexBuffer = new VertexBuffer(this.graphicsDevice, typeof(VertexPositionColor),
                verts.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionColor>(verts);
            // Set up a line-list of vertices. [element] = verts index;
            vertexIndicies = new short[6];
            vertexIndicies[0] = 0;
            vertexIndicies[1] = 1;
            vertexIndicies[2] = 0;
            vertexIndicies[3] = 2;
            vertexIndicies[4] = 0;
            vertexIndicies[5] = 3;
        }

        public void Draw(Matrix world, Matrix cameraView, Matrix cameraProjection)
        {
            RasterizerState localRS = new RasterizerState();
            localRS.CullMode = CullMode.None;
            localRS.FillMode = FillMode.WireFrame;
            graphicsDevice.RasterizerState = localRS;

            effect.World = world;
            effect.View = cameraView;
            effect.Projection = cameraProjection;
            effect.VertexColorEnabled = true;

            graphicsDevice.SetVertexBuffer(vertexBuffer);

            // Draw user primitives
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.LineList, verts, 0, 4, vertexIndicies, 0, 3);
            }
        }
    }
}
