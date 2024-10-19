
namespace BackYard
{
    public class StageTreeNode : IStageTreeNode
    {
        public List<IStageTreeNode> Parents { get; private set; } = new List<IStageTreeNode>();
        //bei：我感觉还是child好一点？后续注释都用的子节点
        public IStage thisStage { get; private set; }
        public IStageTreeNode Son { get; set; }//bei：上一个结点，唯一
        public StageTreeNode(IStage stage)
        {
            thisStage = stage;
        }
        public void AddParent(IStageTreeNode child)
        {
            Parents.Add(child);
        }
        public void Say()
        {
            Console.WriteLine(Parents[0].thisStage.Tag);
        }
    }
    //一棵前向树
    public partial class FloorManager : IFloorManager
    {
        public List<IStageTreeNode> Dictionary { get; } = new List<IStageTreeNode>();

        public FloorManager()
        {
              
        }
        public IStageTreeNode rootnote { get; set; }
        public IFloorManager CreateFloor(List<IStage> stages)
        {
            if (stages.Count == 0)
                throw new ArgumentException("stage list cannot be empty.");

            foreach (IStage stage in stages)
            {
                IStageTreeNode node = new StageTreeNode(stage);
                Dictionary.Add(node);
            }//bei：将传入的地图变成结点加载进dictionary
            this.rootnote = CreateStagetree();//bei：调用局部函数，构造地图树，返回根结点
            IStageTreeNode CreateStagetree() //bei：局部函数；造地图树函数
            {
                List<IStageTreeNode> availableNodes = Dictionary.ToList();
                IStageTreeNode finalStageNode = availableNodes.Find(node => node.thisStage.Tag == "final");
                //bei：传入的地图list只能有一个final...

                if (finalStageNode != null)
                {
                    availableNodes.Remove(finalStageNode);
                }//bei：起步工作完成
                 // bei：使用队列来进行层次遍历  
                Queue<IStageTreeNode> queue = new Queue<IStageTreeNode>();

                Random random = new Random();


                IStageTreeNode root = availableNodes[random.Next(availableNodes.Count)];//bei：随机一个根结点

                availableNodes.Remove(root); // bei：从可用节点中移除根节点
                queue.Enqueue(root);


                //bei：当队列不为空时，继续构建树  
                while (queue.Count > 0 && availableNodes.Count > 0)//bei：每循环一次深度加1
                {
                    int levelSize = queue.Count; // bei：储存当前深度的节点数  

                    for (int i = 0; i < levelSize && availableNodes.Count > 0; i++)
                    {
                        IStageTreeNode currentNode = queue.Dequeue();
                        //bei：取出当前选择的父节点，并让他退出queue


                        int maxChildren = random.Next(2, 3);//2~3个子节点
                        maxChildren = Math.Min(maxChildren, availableNodes.Count);
                        // bei：确保不会超出剩余节点数  

                        // bei：为当前节点添加子节点，并把子节点加入queue，以便于给下一层的子节点添加子节点  
                        for (int j = 0; j < maxChildren && availableNodes.Count > 0; j++)
                        {
                            int childIndex = random.Next(availableNodes.Count);
                            IStageTreeNode childNode = availableNodes[childIndex];
                            availableNodes.RemoveAt(childIndex);
                            currentNode.AddParent(childNode);//bei：添加子节点
                            childNode.Son = currentNode;//bei：设置父亲结点
                            queue.Enqueue(childNode); // bei：将新添加的子节点入队以便后续处理  
                        }
                    }
                }
                int last = queue.Count;
                for (int k = 0; k < last; k++)
                {
                    IStageTreeNode leavenote = queue.Dequeue();
                    leavenote.AddParent(finalStageNode);
                }
                //bei：遍历所有叶子节点，并为它们各自添加同一个final结点。  

                return root;//bei：返回最初始的根结点
            }//bei：局部函数结束
             //bei：。。。能力有限做不到使每条路径都是一个深度，可能会有一个深度的差值,可能没有，哭
             //bei：没有交叉路径,一个父节点对应1-3个

            return this;
        }



    }
}