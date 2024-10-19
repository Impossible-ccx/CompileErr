
namespace BackYard
{
    public class StageTreeNode :IStageTreeNode
    {
        public List<IStageTreeNode> Parents { get; private set; } = new List<IStageTreeNode>();
        public IStage thisStage { get; private set; }
        public StageTreeNode(IStage stage)
        {
            thisStage = stage;
        }
        public void AddParent(IStageTreeNode child)
        {
            Parents.Add(child);
        }
    }
    //一棵前向树
    public partial class FloorManager : IFloorManager
    {

    }
}
