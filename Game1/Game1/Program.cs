using System;
using Newtonsoft.Json;
using System.IO;

namespace Game1
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var gameData = JsonConvert.DeserializeObject<GameData>(File.ReadAllText("./GameData.json"));
            using (var game = new Game1(gameData))
                game.Run();
        }
    }
#endif
}
