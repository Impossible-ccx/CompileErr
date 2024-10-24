using BackYard;
using System;
using System.Runtime.CompilerServices;

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
            ICard? targetCard = CardPacksFactory.CardDict[Args!];
            if(targetCard != null)
            {
                targetCard = targetCard.CopyCard();
                GameManager.PresentPlayer!.PresentCardPile.CardList.Add(targetCard);
                GameManager.PresentPlayer.Money -= Convert.ToInt32(Value);
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
            for (int i = 0; i < Convert.ToInt32(Value); i++)
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
    public class Defense : IAction
    {
        public override string IDName { get; } = "Defense";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            foreach (IEffect Aeffect in sender!.EffectBox)
            {
                if (Aeffect.IDName == "Defense")
                {
                    Aeffect.Level += (int)Value;
                    return;
                }
            }
            IEffect effect = AEEFactory.EffectDict["Defense"].Copy();
            effect.Level = (int)Value;
            sender.EffectBox.Add(effect);
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
            return new Defense();
        }
    }
    public class ExtraDull : IAction
    {
        public override string IDName { get; } = "ExtraDull";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            if (GameManager.battleManager != null)
            {
                for(int i = 0; i < Convert.ToInt32(Value); i++)
                {
                    GameManager.battleManager.Dull();
                }
            }
            else
            {
                throw new Exception();
            }
        }
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            return true;
        }
        public override IAction Copy()
        {
            return new ExtraDull();
        }
    }
    public class ExtraCost : IAction
    {
        public override string IDName { get; } = "ExtraCost";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            if (GameManager.battleManager != null)
            {
                GameManager.battleManager.cost += Convert.ToInt32(Value);
            }
            else
            {
                throw new Exception();
            }
        }
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            return true;
        }
        public override IAction Copy()
        {
            return new ExtraCost();
        }
    }
    public class Restart : IAction
    {
        public override string IDName { get; } = "Restart";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            if (GameManager.battleManager != null)
            {
                int num = GameManager.battleManager.HandPile.CardList.Count;
                for (int i = 0; i < num; i++)
                {
                    GameManager.battleManager.FoldCard(0);
                }
                for (int i = 0; i < num; i++)
                {
                    GameManager.battleManager.Dull();
                }
            }
            else
            {
                throw new Exception();
            }
        }
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            return true;
        }
        public override IAction Copy()
        {
            return new Restart();
        }
    }
    public class AttackAll : IAction
    {
        public override string IDName { get; } = "AttackAll";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            if (GameManager.battleManager != null)
            {
                foreach(IEnemy enemy in GameManager.battleManager.Enemis)
                {
                    IAction aAttack = AEEFactory.ActionDict["Attack"].Copy();
                    aAttack.Excute(PreSetObj.AllEnemyAttacktor, enemy, Value, null);
                }
            }
            else
            {
                throw new Exception();
            }
        }
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            return true;
        }
        public override IAction Copy()
        {
            return new AttackAll();
        }
    }
    public class ExileFirst : IAction
    {
        public override string IDName { get; } = "ExileFirst";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            if (GameManager.battleManager != null)
            {
                if(GameManager.battleManager.HandPile.CardList.Count > 0)
                {
                    ICard track =  GameManager.battleManager.HandPile.CardList[0];
                    GameManager.battleManager.HandPile.CardList.RemoveAt(0);
                    GameManager.battleManager.ExiledPile.CardList.Add(track);
                    GameManager.battleManager.HandOutCardIndex.Enqueue(0);
                }
                else
                {
                    throw new Exception("Logic err.");
                }
            }
            else
            {
                throw new Exception();
            }
        }
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            if(GameManager.battleManager!.HandPile.CardList.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public override IAction Copy()
        {
            return new ExileFirst();
        }
    }
    public class ShallowMinded : IAction
    {
        public override string IDName { get; } = "ShallowMinded";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            if (GameManager.battleManager != null)
            {
                int num = GameManager.battleManager.HandPile.CardList.Count;
                for(int i = 0; i < num; i++)
                {
                    ICard track = GameManager.battleManager.HandPile.CardList[0];
                    GameManager.battleManager.HandPile.CardList.RemoveAt(0);
                    GameManager.battleManager.HandOutCardIndex.Enqueue(0);
                    GameManager.battleManager.ExiledPile.CardList.Add(track);
                }
                GameManager.battleManager.cost += num * 2;
            }
            else
            {
                throw new Exception();
            }
        }
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
                return true;
        }
        public override IAction Copy()
        {
            return new ShallowMinded();
        }
    }
    public class RollBack : IAction
    {
        public override string IDName { get; } = "RollBack";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            if (GameManager.battleManager != null)
            {
                GameManager.battleManager.RollBack();
            }
            else
            {
                throw new Exception();
            }
        }
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            return true;
        }
        public override IAction Copy()
        {
            return new ShallowMinded();
        }
    }
    public class SelfAttack : IAction
    {
        public override string IDName { get; } = "SelfAttack";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            IAction aAttack = AEEFactory.ActionDict["Attack"].Copy();
            aAttack.Excute(sender, sender!, Value, null);
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
            return new SelfAttack();
        }
    }
    public class MutiPoison : IAction
    {
        public override string IDName { get; } = "MutiPoison";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            foreach (IEffect Aeffect in target!.EffectBox)
            {
                if (Aeffect.IDName == "Poison")
                {
                    Aeffect.Level *= (int)Value;
                    return;
                }
            }
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
            return new MutiPoison();
        }
    }
    public class EnpoisonAll : IAction
    {
        public override string IDName { get; } = "EnpoisonAll";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            if (GameManager.battleManager != null)
            {
                foreach (IEnemy enemy in GameManager.battleManager.Enemis)
                {
                    IAction aEnpoison = AEEFactory.ActionDict["AddPoison"].Copy();
                    aEnpoison.Excute(PreSetObj.AllEnemyAttacktor, enemy, Value, null);
                }
            }
            else
            {
                throw new Exception();
            }
        }
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            return true;
        }
        public override IAction Copy()
        {
            return new EnpoisonAll();
        }
    }
    public class DefendAll : IAction
    {
        public override string IDName { get; } = "DefendAll";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            if (GameManager.battleManager != null)
            {
                foreach (IEnemy enemy in GameManager.battleManager.Enemis)
                {
                    IAction aDefense = AEEFactory.ActionDict["Defense"].Copy();
                    aDefense.Excute(enemy, enemy, Value, null);
                }
            }
            else
            {
                throw new Exception();
            }
        }
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            return true;
        }
        public override IAction Copy()
        {
            return new DefendAll();
        }
    }
    public class RestEvent : IAction
    {
        public override string IDName { get; } = "RestEvent";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            int Hp = GameManager.PresentPlayer!.HP;
            int MaxHp = GameManager.PresentPlayer!.MaxHP;
            if (Hp >= MaxHp / 2)
            {
                GameManager.PresentPlayer!.HP = MaxHp;
            }
            else
            {
                GameManager.PresentPlayer!.HP += MaxHp / 2;
            }
        }
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            if (sender == null || target == null)
            {
                throw new ArgumentNullException(nameof(sender) + nameof(target));
            }
            if (sender != PreSetObj.EventEng)
            {
                throw new Exception("this action is for event");
            }
            return true;
        }
        public override IAction Copy()
        {
            return new RestEvent();
        }
    }
    public class GetCardEvent : IAction
    {
        public override string IDName { get; } = "GetCardEvent";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            try
            {
                GameManager.PresentPlayer!.PresentCardPile.CardList.Add(CardPacksFactory.CardDict[Args!].CopyCard());
            }
            catch
            {
                throw new Exception("Err  GetCardEvent");
            }
        }
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            if (sender == null || target == null)
            {
                throw new ArgumentNullException(nameof(sender) + nameof(target));
            }
            if (sender != PreSetObj.EventEng)
            {
                throw new Exception("this action is for event");
            }
            return true;
        }
        public override IAction Copy()
        {
            return new GetCardEvent();
        }
    }
    public class AddMoney : IAction
    {
        public override string IDName { get; } = "GetCardEvent";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            try
            {
                GameManager.PresentPlayer!.Money += Convert.ToInt32(Value);
            }
            catch
            {
                throw new Exception("Err  AddMoney");
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
            return new AddMoney();
        }
    }
    public class BuyHP : IAction
    {
        public override string IDName { get; } = "BuyHP";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            int Hp;
            if(int.TryParse(Args, out Hp))
            {
                GameManager.PresentPlayer!.HP += Convert.ToInt32(Hp);
            }
            else
            {
                throw new Exception("Err Buy HP");
            }
        }
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            if (sender != PreSetObj.EventEng || target != GameManager.PresentPlayer)
            {
                throw new Exception("this method is for event!");
            }
            if (GameManager.PresentPlayer!.Money < Value)
            {
                return false;
            }
            return true;
        }
        public override IAction Copy()
        {
            return new BuyHP();
        }
    }
    public class GetCardPack : IAction
    {
        public override string IDName { get; } = "GetCardPack";
        public override void thisAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            GameManager.SelectNewCardPack("Args");
        }
        public override bool checkAction(IPlayer? sender, IPlayer? target, string? Args = null)
        {
            if (sender != PreSetObj.EventEng || target != GameManager.PresentPlayer)
            {
                throw new Exception("this method is for event!");
            }
            return true;
        }
        public override IAction Copy()
        {
            return new GetCardPack();
        }
    }
}
