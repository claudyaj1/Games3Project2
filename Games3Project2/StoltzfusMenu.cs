/*
 * Author: Cole J. Stoltzfus
 * Description: An easily reusable Menu class.
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

namespace MenuUtility
{
    class Menu : Microsoft.Xna.Framework.GameComponent
    {
        int linePointer;
        float textYoffset;

        int timeSinceLastInput;
        const int TIME_BETWEEN_INPUTS = 250;
        
        Color titleColor, optionsColor, highlightColor;
        Texture2D background;

        List<String> options;
        public String title;

        Vector2 textPosition;
        public Vector2 titlePosition;

        SpriteFont menuFont;
        SpriteFont titleFont;
        SoundEffect menuMove;

        #region Setters/Getters
        public float TextYOffset
        {
            get { return textYoffset; }
            set { textYoffset = value; }
        }
        public Color TitleColor
        {
            get { return titleColor; }
            set { titleColor = value; }
        }
        public Color OptionsColor
        {
            get { return optionsColor; }
            set { optionsColor = value; }
        }
        public Color HighlightColor
        {
            get { return highlightColor; }
            set { highlightColor = value; }
        }
        public List<String> Options
        {
            get { return options; }
            set { options = value; }
        }
        #endregion
        public Menu(List<String> options_, String title_, Vector2 firstLinePosition):
            base(Global.game)
        {
            titleColor = Color.Black;
            optionsColor = Color.Black;
            highlightColor = Color.Red;
            linePointer = 0;
            timeSinceLastInput = 500;

            background = Global.game.Content.Load<Texture2D>(@"Textures\menubackground");
            menuFont = Global.game.Content.Load<SpriteFont>(@"Fonts\Consolas");
            titleFont = Global.game.Content.Load<SpriteFont>(@"Fonts\titleFont");
            menuMove = Global.game.Content.Load<SoundEffect>(@"Audio\menuMove");

            options = options_;
            title = title_;

            textYoffset = menuFont.MeasureString(options[0]).Y;

            textPosition = firstLinePosition;
            titlePosition = new Vector2(Global.viewPort.Center.X - titleFont.MeasureString(title).X / 2, 10);
        }

        public void draw()
        {
            Global.spriteBatch.Draw(background, Global.viewPort, Color.White);
            Global.spriteBatch.DrawString(titleFont, title, titlePosition, titleColor);

            for (int i = 0; i < options.Count; ++i)
            {
                if (i == linePointer)
                    continue;
                Global.spriteBatch.DrawString(menuFont, options[i], new Vector2(textPosition.X, textPosition.Y + i * textYoffset), optionsColor);
            }

            Global.spriteBatch.DrawString(menuFont, options[linePointer], new Vector2(textPosition.X, textPosition.Y + linePointer * textYoffset), highlightColor);
        }
 
        public int update()
        {
			if(Global.input.isConnected(PlayerIndex.One)) {
				if(timeSinceLastInput > TIME_BETWEEN_INPUTS) {
					Vector2 leftStick = Global.input.GetAs8DirectionLeftThumbStick();
					if(leftStick.Y > 0) {
                        menuMove.Play();
						--linePointer;
						timeSinceLastInput = 0;
					} else if(leftStick.Y < 0) {
                        menuMove.Play();
						++linePointer;
						timeSinceLastInput = 0;
					}
					if(linePointer < 0)
						linePointer = options.Count - 1;
					if(linePointer >= options.Count)
						linePointer = 0;
				} else {
					timeSinceLastInput += Global.gameTime.ElapsedGameTime.Milliseconds;
				}

                if (Global.input.isFirstPress(Buttons.A) || Global.input.isFirstPress(Buttons.Start))
                {
                    menuMove.Play();
					return linePointer;
				} else {
					return -1;
				}
			} else {
				return -1;
			}
        }
    }
}
