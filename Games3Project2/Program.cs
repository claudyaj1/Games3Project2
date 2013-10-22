using System;

namespace Games3Project2
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (JuggernautGame game = new JuggernautGame())
            {
                game.Run();
            }
        }
    }
#endif
}

