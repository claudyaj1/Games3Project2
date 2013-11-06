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


namespace Games3Project2
{
    public class NetworkManagement : Microsoft.Xna.Framework.GameComponent
    {
        public const int MAX_PLAYERS_TOTAL = 4;
        public const int MAX_PLAYERS_LOCAL = 4;

        

        public NetworkManagement(Game game)
            : base(game)
        {
        }


        public override void Initialize()
        {

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
