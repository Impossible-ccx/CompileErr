﻿using BackYard;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Effects
{
    public class Poison : IEffect
    {
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
}
