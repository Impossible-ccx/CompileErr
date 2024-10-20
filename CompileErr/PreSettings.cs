using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
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
        //回合开始的调用者
        static public IPlayer ExcStart = new AbsPlayer("ExcStart");
        static public IPlayer DisCard = new AbsPlayer("DisCard");
        static public IPlayer EventEng = new AbsPlayer("EventEng");
        static public IPlayer OnActed = new AbsPlayer("OnActed");
    }
    public abstract class Player : IPlayer
    {
        public virtual string Name { get; set; } = new string(string.Empty);
        public virtual string ID { get; set; } = new string(string.Empty);
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public List<IEffect> EffectBox { get; set; } = new List<IEffect>();
        public bool OnAction(IAction action)
        {
            if (action.thisAction(action.Sender, action.Target))
            {
                foreach (IEffect effect in EffectBox)
                {
                    effect.Excute(this, action);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public abstract IPlayer Copy();
    }
    public class AbsPlayer : IPlayer
    {
        public virtual string Name { get; set; } = new string(string.Empty);
        public virtual string ID { get; set; } = new string(string.Empty);
        public int HP { get; set; } = -1;
        public int MaxHP { get; set; } = -1;
        public List<IEffect> EffectBox { get; set; } = new List<IEffect>();
        public bool OnAction(IAction action)
        {
            throw new Exception("抽象玩家不能作为动作目标");
        }
        public IPlayer Copy() { throw new Exception("Abstract Player"); }
        public AbsPlayer(string nameID)
        {
            Name = nameID;
            ID = nameID;
        }
    }
    public class HumanPlayer : Player, IHumanPlayer
    {
        public HumanPlayer(string? name, ICardPile IniPackPile)
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
        public int MaxCost { get; set; } = 3;
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
        public override IPlayer Copy()
        {
            Enemy result = new Enemy();
            result.NextDelay = NextDelay;
            int nextDelay = 100000;
            foreach (EnemyLogic logic in Logic)
            {
                result.Logic.Add(logic.Copy());
                if (logic.delay < nextDelay)
                {
                    nextDelay = logic.delay;
                }
            }
            result.Name = new string(Name);
            result.ID = new string(ID);
            result.NextDelay = nextDelay;
            result.HP = HP;
            foreach(IEffect aEffect in EffectBox)
            {
                result.EffectBox.Add(aEffect.Copy());
            }
            return result;
        }
    }
    public class Card : ICard
    {
        public string Name { get; set; } = new string(string.Empty);
        public string Description { get; set; } = new string(string.Empty);
        //卡面描述
        public string ImagePath { get; set; } = new string(string.Empty);
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
            card.WhereThis = WhereThis;
            foreach(string activeAcion in activeAcions)
            {
                card.activeAcions.Add(new string(activeAcion));
            }
            foreach(KeyValuePair<string,double> kv in actionValue)
            {
                card.actionValue[kv.Key] = kv.Value;
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
                     newAction = AEEFactory.ActionDict[aAction];
                }
                catch (KeyNotFoundException)
                {
                    throw new Exception("E003 不存在的action");
                }
                if(newAction.Excute(sender, target, actionValue[aAction]))
                {
                    return true;
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
        public int WhereThis { get; set; }
        //检测这个值指示牌应当放置于哪里。0抽牌堆，1手牌，2弃牌堆，3放逐，4销毁  在使用牌后进行检测

        public List<string> activeAcions { get; set; } = new List<string>();
        public Dictionary<string, double> actionValue { get; set; } = new Dictionary<string, double>();
        
    }
    public class BattleStage : IBattle
    {
        public int type { get; set; } = 1;
        //假定战斗为1，事件为2。准备好生成方法就行，具体逻辑还要策划决定
        public string Tag { get; set; } = new string(string.Empty);
        //现在暂时考虑两种tag，normal和final，后者一定放在最后
        public List<ICard> RewardCard { get; set; } = new List<ICard>();
        public int Reward { get; set; }
        //这是金币
        public List<IEnemy> EnemyList { get; set; } = new List<IEnemy>();
        public IStage Copy()
        {
            IBattle newBattle = new BattleStage();
            newBattle.type = type;
            newBattle.Tag += Tag;
            newBattle.Reward = Reward;
            foreach(ICard card in RewardCard)
            {
                newBattle.RewardCard.Add(card.CopyCard());
            }
            foreach(IEnemy enemy in EnemyList)
            {
                newBattle.EnemyList.Add((enemy.Copy() as IEnemy)!);
            }
            return newBattle;
        }
    }
    public abstract class Stage : IStage
    {
        public virtual int type { get; set; }
        public virtual string Tag { get; set; } = new string(String.Empty);
        public abstract IStage Copy();
    }
    public class CardPack : ICardPack
    {
        public string NameID { get; set; } = new string(String.Empty);
        public List<ICard> Cards { get; set; } = new List<ICard>();
        public List<ICard> DefaultCards { get; set; } = new List<ICard> { };
    }
    public class Event : IEvent
    {
        public int type { get; set; } = 2;
        //假定战斗为1，事件为2。准备好生成方法就行，具体逻辑还要策划决定
        public string Tag { get; set; } = string.Empty;
        //现在暂时考虑两种tag，normal和final，后者一定放在最后
        public IStage Copy()
        {
            Event newEvent = new Event();
            newEvent.type = type;
            newEvent.Tag += Tag;
            newEvent.Description += Description;
            foreach(string Choice in Choices)
            {
                newEvent.Choices.Add(new string(Choice));
            }
            return newEvent;
        }
        public List<string> Choices { get; set; } = new List<string>();
        public string Description { get; set; } = string.Empty;
        public bool Choose(string choice)
        {
            try
            {
                List<string> acts = Actions[choice];
                foreach(string aact in acts)
                {
                    if(AEEFactory.ActionDict[aact].Excute(PreSetObj.EventEng,GameManager.PresentPlayer!, keyValuePairs[choice][aact]))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch(KeyNotFoundException)
            {
                throw new Exception("事件异常！");
            }
        }
        public Dictionary<string, List<string>> Actions { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, Dictionary<string, double>> keyValuePairs { get; set; } = new Dictionary<string, Dictionary<string, double>>();
    }
}
