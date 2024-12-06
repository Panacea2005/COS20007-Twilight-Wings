using System;
using SplashKitSDK;

namespace FlappyBird
{
    public class Program
    {
        public static void Main()
        {
            SplashKit.LoadFont("Score", "Press_Start_2P/PressStart2P-Regular.ttf");
            GameController game = new GameController();
            do
            {
                SplashKit.ProcessEvents();
                game.HandleInput();
                game.Draw();
            } while (game.Quit == false);
        }
    }
}
