//main，用于测试
using BackYard;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Program
{
    static class Program
    {
        static void Main(string[] args)
        {
            GameManager.IniNewGame("BasicCardPack");
            //IEvent testEvent = (FloorFactoy.Map[2] as IEvent)!;
            IBattle testBattle = (FloorFactoy.Map[0] as IBattle)!;
            GameManager.PresentPlayer!.Money = 30;
            GameManager.PresentPlayer!.MaxCost = 1000;
            GameManager.EnterStage(testBattle);
            PrintBattle();
            GameManager.battleManager.Dull();
            GameManager.battleManager.Dull();
            GameManager.battleManager.Dull();
            PrintBattle();
            GameManager.battleManager.Discard(0, GameManager.PresentPlayer, GameManager.battleManager.Enemis[0]);
            PrintBattle();
            GameManager.battleManager.Discard(0, GameManager.PresentPlayer, GameManager.battleManager.Enemis[0]);
            PrintBattle();
            GameManager.battleManager.Discard(0, GameManager.PresentPlayer, GameManager.battleManager.Enemis[0]);
            PrintBattle();
            GameManager.battleManager.Discard(0, GameManager.PresentPlayer, GameManager.battleManager.Enemis[0]);
            PrintBattle();
            GameManager.battleManager.Discard(0, GameManager.PresentPlayer, GameManager.battleManager.Enemis[0]);
            PrintBattle();
            GameManager.battleManager.Discard(0, GameManager.PresentPlayer, GameManager.battleManager.Enemis[0]);
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
                Console.Write("Effects: ");
                foreach(IEffect aEffect in enemy.EffectBox)
                {
                    Console.Write($"{aEffect.IDName} {aEffect.Level} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.Write("Player Effects: ");
            foreach (IEffect aEffect in GameManager.PresentPlayer!.EffectBox)
            {
                Console.Write($"{aEffect.IDName} {aEffect.Level} ");
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
        static void PrintEvent(IEvent targetEvent)
        {
            Console.Write(targetEvent.Description);
            Console.WriteLine();
            Console.WriteLine("现有金钱" + GameManager.PresentPlayer!.Money);
            string? order = null;
            while (true)
            {
                while (true)
                {
                    order = Console.ReadLine();
                    if (order == null)
                    {
                        continue;
                    }
                    if (order.All(char.IsDigit))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("请输入数字");
                    }
                }
                if (targetEvent.Choose(int.Parse(order)))
                {
                    Console.WriteLine("选择成功");
                    break;
                }
                else
                {
                    Console.WriteLine("选择失败");
                }
            }

        }
    }
}