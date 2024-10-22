using BackYard;

namespace Actions
{
    public class DestoryCard : IAction
    {
        public override string IDName { get; } = "DestoryCard";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            ICard? targetCard = null;
            foreach(ICard card in GameManager.PresentPlayer!.PresentCardPile.CardList)
            {
                if(card.ID == Args)
                {
                    targetCard = card;
                }
            }
            if (targetCard != null)
            {
                GameManager.PresentPlayer.PresentCardPile.CardList.Remove(targetCard);
            }
            else
            {
                throw new Exception("Event failed");
            }
        }
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            if(sender != PreSetObj.EventEng || target != GameManager.PresentPlayer)
            {
                throw new Exception("this method is for event!");
            }
            return true;
        }
        public override IAction Copy()
        {
            return new DestoryCard();
        }
    }
    public class BuyCard : IAction
    {
        public override string IDName { get; } = "BuyCard";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            ICard? targetCard = CardPacksFactory.CardDict[Args];
            if(targetCard != null)
            {
                targetCard = targetCard.CopyCard();
                GameManager.PresentPlayer!.PresentCardPile.CardList.Add(targetCard);
            }
            else
            {
                throw new Exception("Event failed");
            }
        }
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            if (sender != PreSetObj.EventEng || target != GameManager.PresentPlayer)
            {
                throw new Exception("this method is for event!");
            }
            if(GameManager.PresentPlayer!.Money < Value)
            {
                return false;
            }
            return true;
        }
        public override IAction Copy()
        {
            return new BuyCard();
        }
    }
    public class Attack : IAction
    {
        public override string IDName { get; } = "Attack";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {

            target!.HP -= Convert.ToInt32(Value);
        }
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
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
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
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
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
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
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            for (int i = 0; i < Value; i++)
            {
                GameManager.battleManager!.FoldCard();
            }
        }
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
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
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
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
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
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
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
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
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
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
