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
    public class AddPoison : IAction
    {
        public override string IDName { get; } = "AddPoison";
        public override void thisAction(IPlayer? sender, IPlayer? target)
        {
            foreach(IEffect Aeffect in target!.EffectBox)
            {
                if(Aeffect.IDName == "Poison")
                {
                    Aeffect.Level += (int)Value;
                    return;
                }
            }
            IEffect effect = AEEFactory.EffectDict["Poison"].Copy();
            effect.Level = (int)Value;
            target.EffectBox.Add(effect);
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
            return new AddPoison();
        }
    }
    public class AddDefense : IAction
    {
        public override string IDName { get; } = "AddDefense";
        public override void thisAction(IPlayer? sender, IPlayer? target)
        {
            foreach (IEffect Aeffect in target!.EffectBox)
            {
                if (Aeffect.IDName == "Defense")
                {
                    Aeffect.Level += (int)Value;
                    return;
                }
            }
            IEffect effect = AEEFactory.EffectDict["Defense"].Copy();
            effect.Level = (int)Value;
            target.EffectBox.Add(effect);
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
            return new AddDefense();
        }
    }
}
