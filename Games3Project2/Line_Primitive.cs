using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Primatives
{
    /// <summary>
    /// Author: Andrew Claudy
    /// </summary>
    class Line_Primitive
    {
        #region Members
        GraphicsDevice graphicsDevice;
        BasicEffect effect;

        // Vertex data
        private VertexPositionColor[] verts;
        private VertexBuffer vertexBuffer;
        private short[] vertexIndicies;

        public Line_Primitive(GraphicsDevice graphicsDevice,
            Vector3 point1, Color point1Color,
            Vector3 point2, Color point2Color)
        {
            this.graphicsDevice = graphicsDevice;
            this.effect = new BasicEffect(this.graphicsDevice);
            
            // Initialize vertices
            verts = new VertexPositionColor[2];
            verts[0] = new VertexPositionColor(point1, point1Color);
            verts[1] = new VertexPositionColor(point2, point2Color);
            
            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor),
                verts.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionColor>(verts);

            // List them in an array.
            vertexIndicies = new short[2];
            vertexIndicies[0] = 0;
            vertexIndicies[1] = 1;
        }

        /// <summary>
        /// Change the color of a vertex in this primitive.
        /// </summary>
        /// <param name="index">This value can only be 0 or 1 since this is a line. 
        /// All other values result in a IndexOutOfRangeException.</param>
        /// <param name="color"></param>
        public void ChangeVertexColor(int index, Color color)
        {
            if (index == 0 || index == 1)
            {
                verts[index].Color = color;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
        /// <summary>
        /// Change the 3D coordinate of a vertex in this primitive.
        /// </summary>
        /// <param name="index">This value can only be 0 or 1 since this is a line. 
        /// All other values result in a IndexOutOfRangeException.</param>
        public void ChangeVertexLocation(int index, Vector3 newPosition)
        {
            if (index == 0 || index == 1)
            {
                verts[index].Position = newPosition;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cullMode">Default is CullMode.None</param>
        /// <param name="fillMode">Default is FillMode.WireFrame</param>
        /// <param name="vertexColorsEnabled">Default is true.</param>
        public void Draw(Matrix world,
            Matrix cameraView,
            Matrix cameraProjection,
            CullMode cullMode,
            FillMode fillMode,
            bool vertexColorsEnabled )
        {
            // I had to make a localRS because assigning directly to the graphicsDevice.RasterizerState.properties 
            // didn't work.
            RasterizerState localRS = new RasterizerState();
            localRS.CullMode = cullMode;
            localRS.FillMode = fillMode;
            graphicsDevice.RasterizerState = localRS;

            // Effect configuration
            effect.World = world;
            effect.View = cameraView;
            effect.Projection = cameraProjection;
            effect.VertexColorEnabled = vertexColorsEnabled;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.LineList,
                    verts, 0, 2,
                    vertexIndicies, 0, 1);
            }
        }
        #endregion

        #region Static Functions
        /// <summary>
        /// Draw dynamically at runtime. This function will have poor-performance 
        /// due to loading two vertices into the graphicsDevice memory 
        /// and then throwing them away. For academic uses only, very expensive and slow.
        /// Use the non-static stuff in this class. An instance of this class is quicker than
        /// a bunch of arbitrary lines via this function.
        /// </summary>
        /// <param name="cullMode">Default is CullMode.None</param>
        /// <param name="fillMode">Default is FillMode.WireFrame</param>
        /// <param name="vertexColorsEnabled">Default is true.</param>
        static public void DrawLine(GraphicsDevice graphicsDevice,
            Vector3 point1, Color point1Color,
            Vector3 point2, Color point2Color,
            Matrix world,
            Matrix cameraView,
            Matrix cameraProjection,
            CullMode cullMode,
            FillMode fillMode,
            bool vertexColorsEnabled)
        {
            // Vertex data
            VertexPositionColor[] verts_static = new VertexPositionColor[2];
            verts_static[0] = new VertexPositionColor(point1, point1Color);
            verts_static[1] = new VertexPositionColor(point2, point2Color);
            VertexBuffer vertexBuffer_static = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor),
                2, BufferUsage.None);
            vertexBuffer_static.SetData<VertexPositionColor>(verts_static); // This line is why there is bad performance. Puts stuff into GPU memory.
            short[] vertexIndicies_static = new short[2];
            vertexIndicies_static[0] = 0;
            vertexIndicies_static[1] = 1;

            BasicEffect effect_static = new BasicEffect(graphicsDevice);
            // I had to make a localRS because assigning directly to the graphicsDevice.RasterizerState.properties 
            // didn't work.
            RasterizerState localRS = new RasterizerState();
            localRS.CullMode = cullMode;
            localRS.FillMode = fillMode;
            graphicsDevice.RasterizerState = localRS;

            // Effect configuration
            effect_static.World = world;
            effect_static.View = cameraView;
            effect_static.Projection = cameraProjection;
            effect_static.VertexColorEnabled = vertexColorsEnabled;


            foreach (EffectPass pass in effect_static.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.LineList, 
                    verts_static, 0, 2,
                    vertexIndicies_static, 0, 1);
            }
        }
        #endregion
    }
}
