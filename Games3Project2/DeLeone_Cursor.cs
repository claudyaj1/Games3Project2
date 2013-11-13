using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Broad.Camera;
using Claudy.Primitive;
using Games3Project2.Globals;

// This class was derived from the Picking example code provided in class
namespace DeLeone.Cursor
{
    /// <summary>
    /// Author: Tony DeLeone
    /// Originally from GrannyCrosswalk.
    /// Modified for Juggernaut.
    /// </summary>
    public class Cursor
    {
        Vector3 nearSource, farSource;
        Camera camera;

        // This constant controls how fast the gamepad moves the cursor. this constant
        // is in pixels per second. Currently not used.
        const float CursorSpeed = 400.0f;

        // This is the sprite that is drawn at the current cursor Position.
        // textureCenter is used to center the sprite when drawing.
        Texture2D cursorTexture;
        Vector2 textureCenter;

        // Position is the cursor Position, and is in screen space. 
        public Vector2 Position;

        public Cursor(Game game, Camera c)
        {
            cursorTexture = Global.game.Content.Load<Texture2D>(@"Textures\cursor");
            textureCenter = new Vector2(cursorTexture.Width / 2, cursorTexture.Height / 2);

            camera = c;
            Position = Vector2.Zero;
        }

        public void Update()
        {
            
        }
        
        public void Draw()
        {
            Global.spriteBatch.Draw(cursorTexture, Position, null, Color.White, 0.0f, textureCenter, 1.0f, SpriteEffects.None, 0.0f);
        }
        
        // CalculateCursorRay Calculates a world space ray starting at the camera's
        // "eye" and pointing in the direction of the cursor. Viewport.Unproject is used
        // to accomplish this. see the accompanying documentation for more explanation
        // of the math behind this function.
        /*
        public Ray CalculateCursorRay(Matrix projectionMatrix, Matrix viewMatrix)
        {
            // Create 2 positions in screenspace using the cursor Position. 0 is as
            // Close as possible to the camera, 1 is as far away as possible.
            nearSource = new Vector3(Position, 0f);
            farSource = new Vector3(Position, 1f);

            // Use Viewport.Unproject to tell what those two screen space positions
            // would be in world space. we'll need the projection matrix and view
            // matrix, which we have saved as member variables. We also need a world
            // matrix, which can just be identity.
            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            // Find the direction vector that goes from the nearPoint to the farPoint
            // and normalize it....
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            // And then create a new ray using nearPoint as the source.
            return new Ray(nearPoint, direction);
        }
        */
    }
}
