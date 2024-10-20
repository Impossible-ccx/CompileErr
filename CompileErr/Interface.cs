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
        //�ں��ʵ�ʱ��ִ�ж�����е�����Ч��
        List<IEffect> EffectBox { get; }
        //����action����ʱҲӦ�ü�����е�effect�Ƿ�Ӧ��ִ�С�����action�Ƿ���Ч����Ч���������effect
        void OnAction(IAction action);
        IPlayer Copy();
    }
    public class EnemyLogic
    {
        public int delay;
        public bool IsSingle;
        public IAction? action;
        public double value;
        public EnemyLogic Copy()
        {
            EnemyLogic logic = new EnemyLogic();
            logic.delay = delay;
            logic.action = action;
            logic.IsSingle = IsSingle;
            return logic;
        }
    }
    public interface IEnemy : IPlayer
    {
        int NextDelay { get; set; }
        //��һ���ж���ʱ��
        List<EnemyLogic> Logic { get; }
        //�ж��߼�������ÿһ�����е��߼���ÿ����delay��ʱ��ִ��һ�Ρ�issingleΪ��ʱ��ִ�к�ȥ�������߼�
    }
    public interface IHumanPlayer : IPlayer
    {
        int Money { get; set; }
        //
        int MaxCost { get; set; }
        //���cost
        public ICardPile PresentCardPile { get; set; }
        //���п��飬�ؿ�����ʱ���¡�
    }
    //public interface IUpgrate
    //{
    //    //��ʱ����ʵ��
    //}
    public interface ICardPack
    {
        string NameID { get; }
        List<ICard> Cards { get; }
        List<ICard> DefaultCards { get; }
        //��ȡ����ʱ�Ͱ����Ŀ�
    }
    public interface ICard
    {
        string Name { get; }
        string Description { get; }
        //��������
        string ImagePath {  get; }
        //����·��
        string ID { get; }
        int Cost { get; set; }
        List<string> Tags { get; }
        //IDΨһ��Name���ǡ�
        int Delay { get; set; }
        //0˲����֣�1����pace��2����ȴ���
        ICard CopyCard();
        //����һ���µ�icardʵ��,�ӵ�ǰ�����!
        bool Excute(IPlayer sender, IPlayer target);
        //ʹ�����ſ�������Ϊ��������Ŀ�ꡣ����ֵΪ�����Ƿ�Ϸ�
        //void AttachAction(IUpgrate target);
        //Ϊ���ſ��������
        int Wherethis { get; }
        //������ֵָʾ��Ӧ�����������0���ƶѣ�1���ƣ�2���ƶѣ�3����4����  ��ʹ���ƺ���м��
    }
    public abstract class IEffect
    {
        abstract public string IDName { get; }
        abstract public string Description { get; }
        //levelΪ0Ч��������Ӧ���ٶ��󡣳���ʱ��Ҳ��levelʵ�֡�����Ϊ����
        public int Level { get; set; }
        //effect��ִ��Ч��,��action���ƹ���
        abstract public void Excute(IPlayer sender, IAction? triAction = null, IPlayer? trigger = null, ICard? trigCard = null);
        //��ʲôʱ�򴥷�Ч����Լ�� 0 OnActionʱ������1 �غϿ�ʼ�����������ж���ʼ����2 �غϽ��������������ж�������
        //3 �����Ƴ���ʱ����������ض�����Ҳʹ�������
        abstract public int EnableTime { get; }
        public abstract IEffect Copy();
    }
    public abstract class IAction
    {
        //����ԭ����֮Ҫ���ĵ�������͵��и��������������п���ִ��Ч������������������Ȼ�����ΪҲ����action
        //�κ�ʱ��actionӦ����card���á���֪����Ӧ�ð�װһ�£���û�뵽��ô����������Σ���Ҫֱ�ӵ���action
        public IPlayer? Sender { get; protected set; }
        public IPlayer? Target { get; protected set; }
        public double Value { get; protected set; }
        public bool Excute(IPlayer? sender, IPlayer target, double value)
        {
            Sender = sender;
            Target = target;
            Value = value;
            try
            {
                target.OnAction(this);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public abstract string IDName { get; }
        //action���߼�����������false
        abstract public bool thisAction(IPlayer sender, IPlayer target);
        public abstract IAction Copy();
    }
    public interface IStage
    {
        int type { get; set; }
        //�ٶ�ս��Ϊ1���¼�Ϊ2��׼�������ɷ������У������߼���Ҫ�߻�����
        string Tag { get; set; }
        //������ʱ��������tag��normal��final������һ���������
        IStage Copy();
    }
    public interface IBattle : IStage
    {
        List<ICard> RewardCard { get; set; }
        int Reward { get; set; }
        //���ǽ��
        List<IEnemy> EnemyList { get; }
    }
    public interface IEvent : IStage
    {
        List<string> Choices { get; }
        string Description { get; }
        void Choose(string choice);
    }
}