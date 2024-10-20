//main，用于测试
using BackYard;
namespace Program
{
    static class Program
    {
        static void Main(string[] args)
        {
            GameManager.IniNewGame("BasicCardPack");
            IEvent testEvent = (FloorFactoy.Lay1Group[0][1] as IEvent)!;
            Console.WriteLine("Present HP" + GameManager.PresentPlayer.HP);
            Console.WriteLine(testEvent.Description);
            testEvent.Choose(testEvent.Choices[0]);
            Console.WriteLine("Chose test 1");
            Console.WriteLine("Present HP" + GameManager.PresentPlayer.HP);

        }
    }
}