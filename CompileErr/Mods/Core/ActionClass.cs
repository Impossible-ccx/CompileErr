using BackYard;

namespace Actions
{
    public class Attack : IAction
    {
        public override string IDName { get; } = "Attack";
        public override void thisAction(IPlayer? sender, IPlayer? target)
        {

            target!.HP -= Convert.ToInt32(Value);
        }
        public override bool checkAction(IPlayer? sender, IPlayer? target)
        {
            if (sender == null || target == null)
            {
                throw new ArgumentNullException(nameof(sender) + nameof(target));
            }
            if (sender == target)
            {
                return false;
            }
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
        public override void thisAction(IPlayer? sender, IPlayer? target)
        {
            if (target!.HP + Convert.ToInt32(Value) > target.MaxHP)
            {
                target.HP = target.MaxHP;
            }
            else
            {
                target.HP += Convert.ToInt32(Value);
            }

        }
        public override bool checkAction(IPlayer? sender, IPlayer? target)
        {
            if (sender == null || target == null)
            {
                throw new ArgumentNullException(nameof(sender) + nameof(target));
            }
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
        public override void thisAction(IPlayer? sender, IPlayer? target)
        {
            for (int i = 0; i < Value; i++)
            {
                GameManager.battleManager!.FoldCard();
            }
        }
        public override bool checkAction(IPlayer? sender, IPlayer? target)
        {
            if (GameManager.battleManager == null||GameManager.battleManager.Enemis.Count == 0)
            {
                throw new Exception("battle haven't started?");
            }
            return true;
        }
        public override IAction Copy()
        {
            return new FoldCard();
        }
    }
}
