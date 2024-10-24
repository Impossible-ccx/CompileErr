using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackYard
{
    //所有数据都放在静态类GameManager中，酌情使用。尽管理论上讲有访问权限限制，但可能写错，
    //所以无论如何不要试图更改从其中读来的数据（除非它属于你管理）

    public interface ICardPile
    {
        List<ICard> CardList { get; }
        //
        ICardPile Copy();
        //返回一个深拷贝！
    }
    public interface IBattleManager
    {
        public string PresentBackGround { get; }
        void FoldCard(int index = -1);
        Queue<int> HandOutCardIndex { get; set; }
        Queue<IAction> WaitingActions { get; set; }
        //这是敌人要进行的行动，我只管输入，取出来时直接弹出。iaction是有发起者的，就是sender。
        ICardPile HandPile { get; }
        ICardPile DiscardPile { get; }
        ICardPile ExiledPile {  get; }
        //这个是放逐牌堆。已经放逐的牌不会在对局中出现，但下一次对战仍可用。摧毁的牌则彻底消失
        ICardPile CardPile { get; }
        ICardPile WatingPile { get; }
        //等待牌堆，暂时留空即可
        List<IEnemy> Enemis { get; }
        //在线的敌人
        int cost { get; set; }
        //也许应该从哪里定义初始值？
        int Pace {  get; }
        //目前的回合数
        bool Discard(int item,IPlayer sender, IPlayer target);
        //出牌，输入为第几张卡，发动者，目标。
        //调用对应卡的执行方法即可
        void Dull();
        //抽卡
        void StartBattle(IBattle thisStage);
        //初始化，这是入口。调用humanplayer取得对战开始时的牌堆
        void EndBattle();
        //结束战斗，将所有牌堆叠到一起并saveCardPile。计算奖励。
        //回调floormanager的结束战斗！
        List<ICard> GetRewardCards();
        void SelectReward(ICard card);

        //由于牌堆全是public的，弃牌，改变手牌cost等操作均可由action实现。不要再添加到这里了。
        void NextPace();
        //下一回合
        //游戏更新顺序需要注意！先更新敌人，再更新玩家cost、抽牌，再进入出牌阶段（暂时控制台输入，或者标记断点之类的）
        void RollBack();
        //用于“时间倒流”，主程的想法。具体处理可先搁置
    }
}
