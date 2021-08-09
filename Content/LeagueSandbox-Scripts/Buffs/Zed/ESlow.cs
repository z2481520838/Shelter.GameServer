using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using LeagueSandbox.GameServer.GameObjects.Stats;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;


namespace Buffs
{
    internal class ZedSlow : IBuffGameScript //Find proper name
    {
        public BuffType BuffType => BuffType.COMBAT_DEHANCER;
        public BuffAddType BuffAddType => BuffAddType.STACKS_AND_RENEWS;
        public int MaxStacks => 2;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public IStatsModifier StatsModifier2 { get; private set; } = new StatsModifier();

        IParticle p;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            if (buff.StackCount == 1)
            {
                StatsModifier.MoveSpeed.PercentBonus -= 0.15f + 0.05f * (ownerSpell.CastInfo.SpellLevel - 1);
                unit.AddStatModifier(StatsModifier);
            }
            else if(buff.StackCount == 2)
            {
                StatsModifier2.MoveSpeed.PercentBonus -= 0.1f + 0.075f * (ownerSpell.CastInfo.SpellLevel - 1);
                unit.AddStatModifier(StatsModifier2);
            }

        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            if(buff.StackCount == 2)
            {
                unit.RemoveStatModifier(StatsModifier2);
            }
        }

        public void OnUpdate(float diff)
        {
        }
    }
}

