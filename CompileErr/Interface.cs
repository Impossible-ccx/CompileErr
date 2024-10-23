using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace BackYard
{
    public interface IPlayer
    {
        string Name { get; }
        string ID {  get; }
        int HP { get; set; }
        int MaxHP { get; set; }
        //在合适的时候执行对象具有的所有效果
        List<IEffect> EffectBox { get; }
        //接受action，此时也应该检测所有的effect是否应该执行。返回action是否有效，无效则放弃更新effect
        bool OnAction(IAction action);
        IPlayer Copy();
    }
    public class EnemyLogic
    {
        public int delay;
        public int IniDelay;
        public bool IsSingle;
        public IAction? action;
        public double value;
        public EnemyLogic Copy()
        {
            EnemyLogic logic = new EnemyLogic();
            logic.delay = delay;
            logic.IniDelay = IniDelay;
            logic.action = action;
            logic.IsSingle = IsSingle;
            logic.value = value;
            return logic;
        }
    }
    public interface IEnemy : IPlayer
    {
        string ImagePath { get; }
        int NextDelay { get; set; }
        //下一次行动的时间
        List<EnemyLogic> Logic { get; }
        //行动逻辑，对于每一个其中的逻辑，每经过delay的时间执行一次。issingle为真时，执行后去除此条逻辑
    }
    public interface IHumanPlayer : IPlayer
    {
        int Money { get; set; }
        //
        int MaxCost { get; set; }
        //最大cost
        public ICardPile PresentCardPile { get; set; }
        //现有卡组，关卡结束时更新。
    }
    //public interface IUpgrate
    //{
    //    //有时间再实现
    //}
    public interface ICardPack
    {
        string NameID { get; }
        List<ICard> Cards { get; }
        List<ICard> DefaultCards { get; }
        //获取卡包时就包含的卡
    }
    public interface ICard
    {
        string Name { get; }
        string Description { get; }
        //卡面描述
        string ImagePath {  get; }
        //卡面路径
        string ID { get; }
        int Cost { get; set; }
        List<string> Tags { get; }
        //ID唯一，Name不是。
        int Delay { get; set; }
        //0瞬间出手，1消耗pace，2进入等待区
        ICard CopyCard();
        //返回一个新的icard实例,从当前卡深拷贝!
        bool Excute(IPlayer sender, IPlayer target);
        //使用这张卡，输入为发起者与目标。返回值为操作是否合法
        //void AttachAction(IUpgrate target);
        //为这张卡添加升级
        int WhereThis { get; }
        //2为普通牌，3放逐，4销毁，0， 1 暂时保留
    }
    public abstract class IEffect
    {
        public virtual string ImagePath { get; set; } = new string(string.Empty); 
        abstract public string IDName { get; }
        abstract public string Description { get; }
        //level为0效果结束，应销毁对象。持续时间也用level实现。负数为永续
        public int Level { get; set; }
        //effect的执行效果,与action相似管理
        abstract public void Excute(IPlayer sender, IAction? triAction = null, IPlayer? trigger = null, ICard? trigCard = null);
        //在什么时候触发效果。约定 0 OnAction时触发。1 回合开始触发（敌人行动开始）。2 回合结束触发（敌人行动结束）
        //3 任意牌出牌时触发（检测特定出牌也使用这个）
        abstract public int EnableTime { get; }//已经废弃，不要使用
        public abstract IEffect Copy();
    }
    public abstract class IAction
    {
        //种种原因，总之要从文档里读卡就得有个东西来管理所有卡的执行效果。方便起见，攻击等基本行为也当作action
        //任何时候，action应该由card调用。我知道它应该包装一下，但没想到怎么做。无论如何，不要直接调用action
        public IPlayer? Sender { get; protected set; }
        public IPlayer? Target { get; protected set; }
        public double Value { get; set; }
        public string? Args { get; set; }
        public bool Excute(IPlayer? sender, IPlayer target, double value, string? args)
        {
            Sender = sender;
            Target = target;
            Value = value;
            Args = args;
            return target.OnAction(this);
        }
        public abstract string IDName { get; }
        //action的逻辑。操作错误false
        abstract public void thisAction(IPlayer? sender, IPlayer? target, string? Args = null);
        abstract public bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null);
        public abstract IAction Copy();
    }
    public interface IStage
    {
        int type { get; set; }
        //假定战斗为1，事件为2。占位关卡为-1，载入时忽略
        string Tag { get; set; }
        //现在暂时考虑两种tag，normal和final，后者一定放在最后
        IStage Copy();
    }
    public interface IBattle : IStage
    {
        List<ICard> RewardCard { get; set; }
        int Reward { get; set; }
        //这是金币
        List<IEnemy> EnemyList { get; }
    }
    public interface IEvent : IStage
    {
        List<string> Choices { get; }
        string Description { get; }
        bool Choose(int choice);
        //返回值为选择是否有效。无效的选择应该被拒绝（比如因为金钱不足）
    }
}