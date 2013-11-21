using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

using Games3Project2.Globals;
using Camera3D;


namespace Games3Project2
{
    public class RemotePlayer : Collidable
    {
        public int networkPlayerID; //TODO: This needs set
        public RemotePlayer(Game game, Vector3 pos, int networkPlayerID) :
            base(game, pos, Vector3.Zero, Global.Constants.PLAYER_RADIUS)
        {
            this.networkPlayerID = networkPlayerID;
        }
    }
}
