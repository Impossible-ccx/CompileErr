
namespace BackYard
{
    public class StageTreeNode :IStageTreeNode
    {
        public List<IStageTreeNode> Children { get; private set; } = new List<IStageTreeNode>();
        public IStage thisStage { get; private set; }
        public StageTreeNode(IStage stage)
        {
            thisStage = stage;
        }
        public void AddChild(IStageTreeNode child)
        {
            Children.Add(child);
        }
    }
    //一棵前向树
    public partial class FloorManager : IFloorManager
    {

    }
}
