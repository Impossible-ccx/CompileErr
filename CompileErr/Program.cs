//main，用于测试
using BackYard;
using System.Collections.Generic;
namespace Program
{
    static class Program
    {
        static void Main(string[] args)
        {
            GameManager.IniNewGame("BasicCardPack");
            IBattle testBattle = (FloorFactoy.Lay1Group[0][0] as IBattle)!;
            IEvent testEvent = (FloorFactoy.Lay1Group[0][1] as IEvent)!;
            //Console.WriteLine("Present HP" + GameManager.PresentPlayer.HP);
            //Console.WriteLine(testEvent.Description);
            //testEvent.Choose(testEvent.Choices[0]);
            //Console.WriteLine("Chose test 1");
            //Console.WriteLine("Present HP" + GameManager.PresentPlayer.HP);
            GameManager.EnterStage(testBattle);
            PrintBattle();
            GameManager.battleManager.Dull();
            PrintBattle();
            GameManager.battleManager.Discard(0, GameManager.PresentPlayer, GameManager.battleManager.Enemis[0]);
            PrintBattle();
            GameManager.battleManager.NextPace();
            PrintBattle();
            GameManager.battleManager.Discard(0, GameManager.PresentPlayer, GameManager.battleManager.Enemis[0]);
            PrintBattle();
            GameManager.battleManager.NextPace();
            PrintBattle();
            GameManager.battleManager.NextPace();
            PrintBattle();
            GameManager.battleManager.NextPace();
            PrintBattle();
        }
        static void PrintBattle()
        {
            Console.WriteLine("--------------------------------");
            Console.Write("Present Cost " + GameManager.battleManager.cost + "   ");
            Console.WriteLine("Present pace " + GameManager.battleManager.Pace);
            foreach (IEnemy enemy in GameManager.battleManager.Enemis)
            {

                Console.Write("Enemy" + enemy.Name);
                Console.Write(" Hp: " + enemy.HP + "  ");
                Console.Write("Next Action " + enemy.NextDelay + "  ");
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("Player HP: " + GameManager.PresentPlayer.HP);
            Console.WriteLine("HandPile:");
            foreach (ICard aCard in GameManager.battleManager.HandPile.CardList)
            {
                Console.Write(aCard.Name + "  ");
                Console.Write(aCard.Description + "  ");
                Console.WriteLine();
            }
        }
    }
}