using BackYard;

namespace Actions
{
    public class Attack : IAction
    {
        public override string IDName { get; } = "Attack";
        public override bool thisAction(IPlayer? sender, IPlayer? target)
        {
            if(sender == null||target == null)
            {
                throw new ArgumentNullException(nameof(sender)+ nameof(target));
            }
            if(sender == target)
            {
                return false;
            }
            target.HP -= Convert.ToInt32(Value);
            return true;
        }
        public override IAction Copy()
        {
            return new Attack();
        }
    }
    public class AddHP : IAction
    {
        public override string IDName { get; } = "AddHP";
        public override bool thisAction(IPlayer? sender, IPlayer? target)
        {
            if (sender == null || target == null)
            {
                throw new ArgumentNullException(nameof(sender) + nameof(target));
            }

            target.HP += Convert.ToInt32(Value);
            return true;
        }
        public override IAction Copy()
        {
            return new AddHP();
        }
    }
    public class FoldCard : IAction
    {
        public override string IDName { get; } = "FoldCard";
        public override bool thisAction(IPlayer? sender, IPlayer? target)
        {
            if(GameManager.battleManager == null||GameManager.battleManager.Enemis.Count == 0)
            {
                throw new Exception("battle haven't started?");
            }
            else
            {
                for (int i = 0; i < Value; i++)
                {
                    GameManager.battleManager.FoldCard();
                }
            }
            return true;
        }
        public override IAction Copy()
        {
            return new FoldCard();
        }
    }
}
