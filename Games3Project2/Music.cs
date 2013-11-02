using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Claudy.Music
{
    public class Music
    {
        Song backgroundMusic;
        Thread thread;

        // CTOR
        public Music(Game game)
        {
            //TODO: Add real background music instead of this demo sound.
            backgroundMusic = game.Content.Load<Song>(@"Audio/nenadsimic__picked-coin-echo-2");
            MediaPlayer.Volume = 1.0f;
            MediaPlayer.Stop();
        }

        public void playBackgroundMusic()
        {
            thread = new Thread(new ThreadStart(playThread));
#if XBOX
            //DO NOT USE threads #0 or #2
            int[] hardwareThread = new int[] { 3 };
            Thread.CurrentThread.SetProcessorAffinity(hardwareThread);
#endif
            thread.Start();
        }

        public void stopBackgroundMusic()
        {
            MediaPlayer.Stop();
            thread.Abort();
        }


        private void playThread() // Must be parameterless to be called as a thread.
        {
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundMusic);
        }
    }
}
