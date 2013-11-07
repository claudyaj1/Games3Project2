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
using Claudy.Input;

namespace Games3Project2
{
    public static class Global
    {
        public static ClaudyInput input = ClaudyInput.Instance;

        public static class Constants
        {
            public static float LevelOneWidth = 150;
            public static float LevelOneHeight = 300;
        }
    }
}
