namespace Server
{
    class Program
    {
        private static Game game;
        static void Main(string[] args)
        {
            game = new Game();
            game.Init();
            game.Run();
        }
    }
}
