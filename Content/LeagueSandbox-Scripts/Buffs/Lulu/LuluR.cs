using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.Stats;
using GameServerCore.Scripting.CSharp;

namespace LuluR
{
    internal class LuluR : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IObjAiBase owner;
        IParticle p;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            owner = ownerSpell.CastInfo.Owner as IChampion;
            var APratio = owner.Stats.AbilityPower.Total * 0.5f;
            var HealthBuff = 150f + 150f * ownerSpell.CastInfo.SpellLevel + APratio;

            p = AddParticleTarget(ownerSpell.CastInfo.Owner, "Lulu_R_cas.troy", unit, 1, lifetime: buff.Duration);

            StatsModifier.Size.PercentBonus = StatsModifier.Size.PercentBonus + 1;
            StatsModifier.HealthPoints.BaseBonus += HealthBuff;

            unit.AddStatModifier(StatsModifier);
            unit.Stats.CurrentHealth += HealthBuff;
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            RemoveParticle(p);
            AddParticleTarget(owner, "Lulu_R_expire.troy", unit, 1);
        }

        public void OnUpdate(float diff)
        { 
        }
    }
}

