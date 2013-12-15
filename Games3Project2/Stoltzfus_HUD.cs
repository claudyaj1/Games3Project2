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

using Games3Project2;
using Games3Project2.Globals;
using ReticuleCursor;

namespace HUDUtility
{
    class HUD
    {
		LocalPlayer player;
        Cursor cursor;

        Color healthColor;

        Texture2D fuelTexture;
        Texture2D healthTexture;
        Texture2D blankTexture;

        int healthBarLength = 400;
        int healthBarHeight = 40;
        int jetFuelWidth = 40;
        int jetFuelHeight = 200;
        Point healthBarPosition = new Point(20, 33);
        Point jetFuelPosition;
        Vector2 healthTextPostion, fuelTextPosition;
        Vector2 scorePosition;

        Rectangle goodRect, badRect;
        Rectangle outlineLeft, outlineRight;
        Rectangle fuelFilledRect, fuelEmptyRect;
        Rectangle fuelOutlineTop, fuelOutlineBottom;

        public HUD(LocalPlayer player)
        {
			this.player = player;
            cursor = new Cursor(new Vector2(player.camera.viewport.Width / 2, player.camera.viewport.Height / 2));

            healthBarLength = (int)(.3125 * player.camera.viewport.Width);
            healthBarHeight = (int)(.055555 * player.camera.viewport.Height);
            jetFuelWidth = healthBarHeight;
            jetFuelHeight = (int)(.277777 * player.camera.viewport.Height);

            goodRect = new Rectangle(healthBarPosition.X, healthBarPosition.Y, healthBarLength, healthBarHeight);
            badRect = new Rectangle(healthBarPosition.X, healthBarPosition.Y, 0, healthBarHeight);
            outlineLeft = new Rectangle(healthBarPosition.X - 1, healthBarPosition.Y, 1, healthBarHeight);
            outlineRight = new Rectangle(healthBarPosition.X + healthBarLength, healthBarPosition.Y, 1, healthBarHeight);
            healthTextPostion = new Vector2(healthBarPosition.X, healthBarPosition.Y - 30f);

            jetFuelPosition = new Point(20, player.camera.viewport.Height - 40 - jetFuelHeight);
            fuelFilledRect = new Rectangle(jetFuelPosition.X, jetFuelPosition.Y, jetFuelWidth, jetFuelHeight);
            fuelEmptyRect = new Rectangle(jetFuelPosition.X, jetFuelPosition.Y + jetFuelHeight, jetFuelWidth, 0);
            fuelTextPosition = new Vector2(jetFuelPosition.X, jetFuelPosition.Y - 30f);

            fuelOutlineTop = new Rectangle(jetFuelPosition.X, jetFuelPosition.Y + jetFuelHeight + 1, jetFuelWidth, 1);
            fuelOutlineBottom = new Rectangle(jetFuelPosition.X, jetFuelPosition.Y - 1, jetFuelWidth, 1);

            scorePosition = new Vector2(player.camera.viewport.Width - 160, 30);

            fuelTexture = Global.game.Content.Load<Texture2D>(@"Textures\fuelTexture");
            healthTexture = Global.game.Content.Load<Texture2D>(@"Textures\healthTexture");
            blankTexture = Global.game.Content.Load<Texture2D>(@"Textures\blankTexture");
        }

        public void Update()
        {
            if (player.isJuggernaut)
            {
                healthColor = Color.Turquoise;
            }
            else
            {
                healthColor = Color.Green;
            }

            float healthFraction = (float)player.health / Global.Constants.MAX_HEALTH;
            int goodHealthLength = (int)(healthFraction * healthBarLength);
            int badHealthLength = healthBarLength - goodHealthLength;

            goodRect = new Rectangle(healthBarPosition.X, healthBarPosition.Y, goodHealthLength, healthBarHeight);
            badRect = new Rectangle(healthBarPosition.X + goodHealthLength, healthBarPosition.Y, badHealthLength, healthBarHeight);

            float fuelFraction = (float)player.jetFuel / Global.Constants.MAX_JET_FUEL;
            int filledFuelHeight = (int)(fuelFraction * jetFuelHeight);
            int emptyFuelHeight = jetFuelHeight - filledFuelHeight;

            fuelFilledRect = new Rectangle(jetFuelPosition.X, jetFuelPosition.Y + emptyFuelHeight, jetFuelWidth, filledFuelHeight);
            fuelEmptyRect = new Rectangle(jetFuelPosition.X, jetFuelPosition.Y, jetFuelWidth, emptyFuelHeight);
        }

        public void Draw()
        {
            Global.spriteBatch.DrawString(Global.consolas, "Health:", healthTextPostion, Global.HUD_COLOR);
            Global.spriteBatch.Draw(healthTexture, goodRect, healthColor);
            Global.spriteBatch.Draw(healthTexture, badRect, Color.Red);
            Global.spriteBatch.Draw(blankTexture, outlineLeft, Color.Black);
            Global.spriteBatch.Draw(blankTexture, outlineRight, Color.Black);

            Global.spriteBatch.DrawString(Global.consolas, "Jet:", fuelTextPosition, Global.HUD_COLOR);
            Global.spriteBatch.Draw(fuelTexture, fuelFilledRect, Color.Orange);
            Global.spriteBatch.Draw(fuelTexture, fuelEmptyRect, Color.Gray);
            Global.spriteBatch.Draw(blankTexture, fuelOutlineTop, Color.Black);
            Global.spriteBatch.Draw(blankTexture, fuelOutlineBottom, Color.Black);

            Global.spriteBatch.DrawString(Global.consolas, "Score: " + player.score.ToString(), scorePosition, Global.HUD_COLOR);

            cursor.Draw();
        }
    }
}
