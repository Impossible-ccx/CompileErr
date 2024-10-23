using Actions;
using BackYard;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Effects
{
    public class Poison : IEffect
    {
        public override string ImagePath { get; set; } = ModManager.ModFolderPath + "Core/Images/PoisonEffect.png";
        public override string IDName { get; } = "Poison";
        public override string Description { get; } = "每回合失去等于层数的HP";
        public override int EnableTime { get; } = 2;
        public override void Excute(IPlayer sender, IAction? triAction = null, IPlayer? trigger = null, ICard? R = null)
        {
            if (trigger == PreSetObj.ExcStart)
            {
                sender.HP -= Level;
                Level -= 1;
            }
        }
        public override IEffect Copy()
        {
            IEffect effect = new Poison();
            effect.Level = Level;
            return effect;
        }
    }
    public class Defense : IEffect
    {
        public override string ImagePath { get; set; } = ModManager.ModFolderPath + "Core/Images/DefenseEffect.png";
        public override string IDName { get; } = "Defense";
        public override string Description { get; } = "抵御等于层数的攻击伤害，抵御一点降低一层";
        public override int EnableTime { get; } = 2;
        public override void Excute(IPlayer sender, IAction? triAction = null, IPlayer? trigger = null, ICard? R = null)
        {
            if(triAction is Attack)
            {
                if(Level <= triAction.Value)
                {
                    triAction.Value -= Level;
                    Level = 0;
                }
                else
                {
                    Level -= (int)triAction.Value;
                    triAction.Value = 0;
                }
            }
        }
        public override IEffect Copy()
        {
            IEffect effect = new Defense();
            effect.Level = Level;
            return effect;
        }
    }
}
