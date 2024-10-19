
namespace BackYard
{
    public class StageTreeNode :IStageTreeNode
    {
        public bool IsFinished {  get; set; } = false;
        public List<IStageTreeNode> Parents { get; private set; } = new List<IStageTreeNode>();
        public List<IStageTreeNode> Children { get; private set; } = new List<IStageTreeNode>();
        public IStage thisStage { get; private set; }
        public StageTreeNode(IStage stage)
        {
            thisStage = stage.Copy();
        }
        public void AddParent(IStageTreeNode parent)
        {
            Parents.Add(parent);
        }
        public void AddChild(IStageTreeNode child)
        {
            Children.Add(child);
        }
    }
    public class FloorManager : IFloorManager
    {
        public static IFloorManager CreateFloor(List<IStage> stages)
        {
            FloorManager newFloorManager = new FloorManager();

            return newFloorManager;
        }
        public void IniBattle(ref IBattle thisBattle)
        {
            //暂时只有一个包，不做调整
            throw new Exception("初始化战斗奖励方式未确定，这个err不会影响运行");
        }
        public void EndBattle(IStage thisStage)
        {
        }
    }
}
