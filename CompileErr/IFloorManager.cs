using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackYard
{
    public interface IStageTreeNode
    {
        List<IStageTreeNode> Children { get; }
        IStage thisStage { get; }
        void AddChild(IStageTreeNode node);
    }

    //所有数据都放在静态类GameManager中，酌情使用。尽管理论上讲有访问权限限制，但可能写错，
    //所以无论如何不要试图更改从其中读来的数据（除非它属于你管理）
    public interface IFloorManager
    {
        List<IStage> Dictionary { get; }
        //在这里获得可用关卡
        abstract static IFloorManager CreateFloor(List<IStage> input);
        //构造新地图。输入是一系列关卡，需要把他们用完。参考stage的tag，以一定的随机方法构造。参见IStage
        //如何生成地图是很需要商讨的问题。。先按照顺序写吧
        public void EndBattle(IStage stage);
        //关卡结束时回调。在这里标记关卡完成。对于final关卡，回调EnterNextFloor。更新可用关卡
        public void IniBattle(ref IBattle thisBattle);
        //传入一个sbattle，向里面加入合适的奖励。从默认奖励中抽取，但注意要求卡包！，先准备好一些基础方案吧。在开始战斗时调用。
    }
}

