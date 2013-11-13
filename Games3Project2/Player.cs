using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

using Games3Project2.Globals;
using Broad.Camera;
using DeLeone.Cursor;

namespace Games3Project2
{
    public class LocalPlayer : Collidable
    {
        public Camera camera;
        public Viewport viewport;
        public Cursor cursor;

        public LocalPlayer(Game game, Vector3 pos)
            : base(game, pos, Vector3.Zero, Global.Constants.PLAYER_RADIUS)
        {
            camera = new Camera(game, pos, Vector3.Zero, Vector3.Up);
        }

        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}
