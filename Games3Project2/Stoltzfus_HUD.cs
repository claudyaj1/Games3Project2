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
        Color gunHeatColor;

        Texture2D fuelTexture;
        Texture2D healthTexture;
        Texture2D blankTexture;

        int healthBarLength = 400;
        int healthBarHeight = 40;
        int jetFuelWidth = 40;
        int jetFuelHeight = 200;
        Point healthBarPosition = new Point(20, 33);
        Point jetFuelPosition;
        Vector2 healthTextPostion, fuelTextPosition, juggernautMsgTextPosition, overheatTextPosition;
        Vector2 scorePosition;

        Rectangle goodRect, badRect;
        Rectangle outlineLeft, outlineRight;
        Rectangle fuelFilledRect, fuelEmptyRect;
        Rectangle fuelOutlineTop, fuelOutlineBottom;

        Point overheatPosition;
        int overheatWidth, overheatHeight;
        Rectangle overheatFilledRect, overheatEmptyRect;
        Rectangle overheatBottom, overheatTop;

        public HUD(LocalPlayer player)
        {
			this.player = player;
            cursor = new Cursor(new Vector2(player.camera.viewport.Width / 2, player.camera.viewport.Height / 2));

            healthBarLength = (int)(.3125 * player.camera.viewport.Width);
            healthBarHeight = (int)(.055555 * player.camera.viewport.Height);
            jetFuelWidth = healthBarHeight;
            jetFuelHeight = (int)(.277777 * player.camera.viewport.Height);
            overheatWidth = healthBarHeight;
            overheatHeight = (int)(.277777 * player.camera.viewport.Height);

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
            
            overheatPosition = new Point(player.camera.viewport.Width - overheatWidth - 20, player.camera.viewport.Height - 40 - overheatHeight);
            overheatBottom = new Rectangle(overheatPosition.X, overheatPosition.Y + overheatHeight + 1, overheatWidth, 1);
            overheatTop = new Rectangle(overheatPosition.X, overheatPosition.Y - 1, overheatWidth, 1);
            overheatFilledRect = new Rectangle(overheatPosition.X, overheatPosition.Y, overheatWidth, overheatHeight);
            overheatEmptyRect = new Rectangle(overheatPosition.X, overheatPosition.Y, overheatWidth, 0);
            overheatTextPosition = new Vector2(overheatPosition.X - Global.consolas.MeasureString("Gun Heat").X / 1.5f, overheatPosition.Y - 30f);

            juggernautMsgTextPosition = new Vector2((player.camera.viewport.Width / 2) - Global.consolas.MeasureString(Global.Constants.HUD_YOU_JUG).X / 2f, healthTextPostion.Y);
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

            if (!player.gunCoolDownModeNoShootingPermitted)
            {
                gunHeatColor = Color.Tomato;
            }
            else
            {
                gunHeatColor = Color.OrangeRed;
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

            float overheatFraction = (float)player.gunHeat / Global.Constants.MAX_GUN_HEAT;
            int filledOverheatHeight = (int)(overheatFraction * overheatHeight);
            int emptyOverheatHeight = overheatHeight - filledOverheatHeight;

            overheatFilledRect = new Rectangle(overheatPosition.X, overheatPosition.Y + emptyOverheatHeight, overheatWidth, filledOverheatHeight);
            overheatEmptyRect = new Rectangle(overheatPosition.X, overheatPosition.Y, overheatWidth, emptyOverheatHeight);

            //Update the timer counter down only if the timer still has time remaining on it.
            if (Global.msElaspedTimeRemainingMsgDisplayed > 0)
            {
                Global.msElaspedTimeRemainingMsgDisplayed -= Global.gameTime.ElapsedGameTime.Milliseconds;
                if (Global.msElaspedTimeRemainingMsgDisplayed <= 0)
                    Global.inGameAlerts = Global.InGameAlerts.None;
            }
        }

        public void Draw() // The hud on the screen.
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

            Global.spriteBatch.DrawString(Global.consolas, "Gun Heat:", overheatTextPosition, Global.HUD_COLOR);
            Global.spriteBatch.Draw(fuelTexture, overheatFilledRect, gunHeatColor);
            Global.spriteBatch.Draw(fuelTexture, overheatEmptyRect, Color.Gray);
            Global.spriteBatch.Draw(blankTexture, overheatBottom, Color.Black);
            Global.spriteBatch.Draw(blankTexture, overheatTop, Color.Black);

            Global.spriteBatch.DrawString(Global.consolas, "Score: " + player.score.ToString(), scorePosition, Global.HUD_COLOR);


            if(Global.inGameAlerts == Global.InGameAlerts.JuggernautKilledNewJuggernaut && Global.msElaspedTimeRemainingMsgDisplayed > 0 && !player.isJuggernaut)
            {                
                Global.spriteBatch.DrawString(Global.consolas, Global.newestJuggernautGamertag + Global.Constants.MSG_IS_JUG, juggernautMsgTextPosition, Global.HUD_COLOR);
            }//More InGameAlerts via else if chain could be added here if desired.
            else if (player.isJuggernaut)
            {
                Global.spriteBatch.DrawString(Global.consolas, Global.Constants.HUD_YOU_JUG, juggernautMsgTextPosition, Global.HUD_COLOR);
            }

            cursor.Draw();
        }
    }
}
