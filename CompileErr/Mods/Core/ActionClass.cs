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
}
