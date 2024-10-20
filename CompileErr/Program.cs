//main，用于测试
using BackYard;
namespace Program
{
    static class Program
    {
        static void Main(string[] args)
        {
            GameManager.IniNewGame("BasicCardPack");
            Console.WriteLine();
            Random random = new Random();
        }
    }
}