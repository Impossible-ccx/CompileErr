//main，用于测试
using BackYard;
namespace Program
{
    static class Program
    {
        static void Main(string[] args)
        {
            GameManager.IniNewGame("BasicCardPack");
            Console.WriteLine(FloorFactoy.Lay1Group[0][1].type);
            Random random = new Random();
        }
    }
}