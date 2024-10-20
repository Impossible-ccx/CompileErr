
namespace BackYard
{
    public class BattleManager : IBattleManager
    {
        public ICardPile HandPile { get; set; } = new CardPile();
        public ICardPile DiscardPile { get; set; } = new CardPile();
        public ICardPile ExiledPile { get; set; } = new CardPile();
        //这个是放逐牌堆。已经放逐的牌不会在对局中出现，但下一次对战仍可用。摧毁的牌则彻底消失
        private ICardPile _cardPile = new CardPile();
        public ICardPile CardPile { get { return _cardPile; } set { _cardPile = value;  } }
        public ICardPile WatingPile { get; set; } = new CardPile();
        //等待牌堆，暂时留空即可
        public List<IEnemy> Enemis { get; set; } = new List<IEnemy>();
        //在线的敌人
        public int cost { get; set; } = Player.MaxCost;
        //也许应该从哪里定义初始值？
        public int Pace { get; set; } = 1;
        public void Discard(int index, IPlayer sender, IPlayer target)
        {
            ICard card = HandPile.CardList[index];
            HandPile.CardList.RemoveAt(index);
            if(card.WhereThis == 2)
            {
                DiscardPile.CardList.Add(card);
            }
            else if(card.WhereThis == 3) 
            {
                ExiledPile.CardList.Add(card);
            }
            else if(card.WhereThis == 4)
            {
                ;
            }
            else
            {
                throw new Exception("Where should the card be?");
            }
            foreach (IEffect aEffect in Player.EffectBox)
            {
                aEffect.Excute(Player, null, null, card);
            }
            if (card.Delay == 2)
            {
                throw new Exception("waiting queue...");
            }
            else if(card.Delay == 0)
            {
                card.Excute(sender, target);
                foreach(IEffect aEffect in Player.EffectBox)
                {
                    aEffect.Excute(sender, null, PreSetObj.DisCard);
                }
            }
            else if (card.Delay == 1)
            {
                card.Excute(sender, target);
                foreach (IEffect aEffect in Player.EffectBox)
                {
                    aEffect.Excute(sender, null, PreSetObj.DisCard);
                }
                NextPace();
            }
            else
            {
                throw new Exception("Delay err");
            }
            //这里判断一下敌人的消灭与否。。
            for (int i = 0; i < Enemis.Count; i++)
            {
                if(Enemis[i].HP <= 0)
                {
                    Enemis.RemoveAt(i);
                    i--;
                }
            }
        }
        //出牌，输入为第几张卡，发动者，目标。
        //调用对应卡的执行方法即可
        public void Dull()
        {            
            if(CardPile.CardList.Count == 0)
            {
                _cardPile = DiscardPile;
                DiscardPile = new CardPile();
                RandomizePile(ref _cardPile);
            }
            if (CardPile.CardList.Count > 0)
            {
                HandPile.CardList.Add(CardPile.CardList[CardPile.CardList.Count - 1]);
                CardPile.CardList.RemoveAt(CardPile.CardList.Count - 1);
            }
            else
            {
                ;//如果需要，提示牌库已空
            }
        }
        //抽卡
        public void StartBattle(IBattle thisStage)
        {
            CardPile = Player.PresentCardPile.Copy();

            foreach(IEnemy enemy in thisStage.EnemyList)
            {
                Enemis.Add((enemy.Copy() as IEnemy)!);
            }
            RandomizePile(ref _cardPile);
        }
        //初始化，这是入口。调用humanplayer取得对战开始时的牌堆
        public void EndBattle()
        {
            ICardPile newCardPile = new CardPile();
            foreach(ICard p in HandPile.CardList)
            {
                newCardPile.CardList.Add(p);
            }
            HandPile.CardList.Clear();
            foreach (ICard p in DiscardPile.CardList)
            {
                newCardPile.CardList.Add(p);
            }
            DiscardPile.CardList.Clear();
            foreach (ICard p in ExiledPile.CardList)
            {
                newCardPile.CardList.Add(p);
            }
            ExiledPile.CardList.Clear();
            foreach (ICard p in CardPile.CardList)
            {
                newCardPile.CardList.Add(p);
            }
            CardPile.CardList.Clear();
            foreach (ICard p in WatingPile.CardList)
            {
                newCardPile.CardList.Add(p);
            }
            WatingPile.CardList.Clear();
            Random random = new Random();
            foreach(ICard p in thisBattle.RewardCard)
            {
                foreach (ICard q in GameManager.EnableCards)
                {
                    if (p == q)
                    {
                        if (random.Next(100) < 40)
                        {
                            newCardPile.CardList.Add(p.CopyCard());
                        }
                    }
                }

            }
            Player.Money += thisBattle.Reward;
        }
        //结束战斗，将所有牌堆叠到一起并saveCardPile。计算奖励。

        //由于牌堆全是public的，弃牌，改变手牌cost等操作均可由action实现。不要再添加到这里了。
        public void NextPace()
        {
            Pace += 1;
            cost += 2;
            foreach (IEnemy aEnemy in Enemis)
            {
                for(int i = 0; i < aEnemy.EffectBox.Count; i++)
                {
                    IEffect aEffect = aEnemy.EffectBox[i];
                    aEffect.Excute(aEnemy, null, PreSetObj.ExcStart);
                    if (aEffect.Level <= 0)
                    {
                        aEnemy.EffectBox.Remove(aEffect);
                        i--;
                    }
                }
                for (int i = 0; i < Enemis.Count; i++)
                {
                    if (Enemis[i].HP <= 0)
                    {
                        Enemis.RemoveAt(i);
                        i--;
                    }
                }
                foreach (EnemyLogic aLogic in aEnemy.Logic)
                {
                    aLogic.delay--;
                    if (aLogic.delay == 0)
                    {
                        aLogic.action!.Excute(aEnemy, Player, aLogic.value);
                    }
                    if (aLogic.IsSingle)
                    {
                        aLogic.delay = 1000000;
                    }
                }
                foreach(EnemyLogic aLogic in aEnemy.Logic)
                {
                    if (aEnemy.NextDelay > aLogic.delay)
                    {
                        aEnemy.NextDelay = aLogic.delay;
                    }
                }
                
            }
            foreach(IEffect aEffect in Player.EffectBox)
            {
                aEffect.Excute(Player, null, PreSetObj.ExcStart);
            }
            Dull();
        }
        //下一回合
        //游戏更新顺序需要注意！先更新敌人，再更新玩家cost、抽牌，再进入出牌阶段（暂时控制台输入，或者标记断点之类的）
        public void RollBack()
        {
            foreach (IEnemy aEnemy in Enemis)
            {
                foreach (EnemyLogic aLogic in aEnemy.Logic)
                {
                    aLogic.delay += 1;
                }
                aEnemy.NextDelay += 1;
                foreach (EnemyLogic aLogic in aEnemy.Logic)
                {
                    if (aEnemy.NextDelay > aLogic.delay)
                    {
                        aEnemy.NextDelay = aLogic.delay;
                    }
                }

            }
            Pace--;
        }
        //用于“时间倒流”，主程的想法。具体处理可先搁置
        private static IHumanPlayer Player = GameManager.PresentPlayer!;
        private void RandomizePile(ref ICardPile pile)
        {
            PriorityQueue<ICard, int> priorityQueue = new PriorityQueue<ICard, int>();
            Random random = new Random();
            foreach (ICard Card in pile.CardList)
            {
                priorityQueue.Enqueue(Card, random.Next(10000));
            }
            pile.CardList.Clear();
            while (priorityQueue.Count != 0)
            {
                pile.CardList.Add(priorityQueue.Dequeue());
            }
        }
        private IBattle thisBattle = new BattleStage();
    }
    public class CardPile : ICardPile
    {

        public List<ICard> CardList { get; private set; } = new List<ICard>();
        public ICardPile Copy()
        {
            CardPile result = new CardPile();
            foreach (ICard card in CardList)
            {
                result.CardList.Add(card.CopyCard());
            }
            return result;
        }
    }
}
