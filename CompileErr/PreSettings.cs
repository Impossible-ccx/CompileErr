using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackYard
{
    //一些特殊要素、基本元素的定义
    static public class PreSetObj
    {
        //未实现不要使用
        static public IPlayer? RandomEnemy;
        static public IPlayer? AllEnemy;
        static public IPlayer? Self;
        static public IAction? Discard;//可用来实现出牌时触发效果
    }
    public abstract class Player : IPlayer
    {
        public virtual string Name { get; protected set; } = new string(string.Empty);
        public virtual string ID { get; private set; } = new string(string.Empty);
        public int HP { get; set; }
        public List<IEffect> EffectBox { get; set; } = new List<IEffect>();
        public void OnAction(IAction action)
        {
            foreach (IEffect effect in EffectBox)
            {
                effect.Excute(this, action);
            }
        }
        public abstract IPlayer Copy();
    }
    public class HumanPlayer : Player, IHumanPlayer
    {
        public HumanPlayer(string name, ICardPile IniPackPile)
        {
            if (name == null)
            {
                base.Name += "Defualt Name";
            }
            else
            {
                base.Name += name;
            }
            PresentCardPile = IniPackPile!;
        }
        public int Money { get; set; }
        public int MaxCost { get; set; }
        public ICardPile PresentCardPile { get; set; }
        public override IPlayer Copy()
        {
            throw new Exception("E001 不要复制玩家");
        }
    }
    public class Enemy : Player, IEnemy
    {
        public int NextDelay {  get; set; }
        public List<EnemyLogic> Logic { get; set; } = new List<EnemyLogic>();
        public override IEnemy Copy()
        {
            Enemy result = new Enemy();
            result.NextDelay = NextDelay;
            foreach (EnemyLogic logic in Logic)
            {
                result.Logic.Add(logic.Copy());
            }
            return result;
        }
    }
    public class Card : ICard
    {
        public string Name { get; set; } = new string(string.Empty);
        public string Description { get; set; } = new string(string.Empty);
        //卡面描述
        public string ID { get; set; } = new string(string.Empty);
        public int Cost { get; set; }
        public List<string> Tags { get; } = new List<string>();
        //ID唯一，Name不是。
        public int Delay { get; set; }
        //0瞬间出手，1消耗pace，2进入等待区
        public ICard CopyCard()
        {
            Card card = new Card();
            card.Name += Name;
            card.Description += Description;
            card.ID += ID;
            card.Cost += Cost;
            foreach(string tag in Tags)
            {
                card.Tags.Add(new string(tag));
            }
            card.Delay = Delay;
            card.Wherethis = Wherethis;
            foreach(string activeAcion in activeAcions)
            {
                card.activeAcions.Add(new string(activeAcion));
            }
            return card;
        }
        //返回一个新的icard实例,从当前卡深拷贝!
        public bool Excute(IPlayer sender, IPlayer target)
        {
            foreach (string aAction in activeAcions)
            {
                IAction newAction;
                try
                {
                     newAction = GameManager.ActionDict[aAction].Copy();
                }
                catch (KeyNotFoundException)
                {
                    throw new Exception("E003 不存在的action");
                }
                if(newAction.Excute(sender, target, actionValue[aAction]))
                {
                    target.OnAction(newAction);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        //使用这张卡，输入为发起者与目标。返回值为操作是否合法
        //void AttachAction(IUpgrate target);
        //为这张卡添加升级
        public int Wherethis { get; set; }
        //检测这个值指示牌应当放置于哪里。0抽牌堆，1手牌，2弃牌堆，3放逐，4销毁  在使用牌后进行检测

        public List<string> activeAcions { get; set; } = new List<string>();
        public Dictionary<string, double> actionValue { get; set; } = new Dictionary<string, double>();
        
    }
    public class BattleStage : IBattle
    {
        public int type { get; } = 1;
        //假定战斗为1，事件为2。准备好生成方法就行，具体逻辑还要策划决定
        public string Tag { get; set; } = new string(string.Empty);
        //现在暂时考虑两种tag，normal和final，后者一定放在最后
        public List<ICard> RewardCard { get; set; } = new List<ICard>();
        public int Reward { get; set; }
        //这是金币
        public List<IEnemy> EnemyList { get; set; } = new List<IEnemy>();
    }
    public class CardPack : ICardPack
    {
        public string NameID { get; set; } = new string(String.Empty);
        public List<ICard> Cards { get; set; } = new List<ICard>();
        public List<ICard> DefaultCards { get; set; } = new List<ICard> { };
    }
}
