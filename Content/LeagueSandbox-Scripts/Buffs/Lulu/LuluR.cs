using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.Stats;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;

namespace Buffs
{
    internal class LuluR : IBuffGameScript
    {
        public IBuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.COMBAT_ENCHANCER,
            BuffAddType = BuffAddType.REPLACE_EXISTING
        };

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IObjAiBase owner;
        IParticle p;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            owner = ownerSpell.CastInfo.Owner as IChampion;
            var APratio = owner.Stats.AbilityPower.Total * 0.5f;
            var HealthBuff = 150f + 150f * ownerSpell.CastInfo.SpellLevel + APratio;

            p = AddParticleTarget(owner, unit, "Lulu_R_cas.troy", unit, 1, buff.Duration);

            StatsModifier.Size.PercentBonus = StatsModifier.Size.PercentBonus + 1;
            StatsModifier.HealthPoints.BaseBonus += HealthBuff;

            unit.AddStatModifier(StatsModifier);
            unit.Stats.CurrentHealth += HealthBuff;
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            RemoveParticle(p);
            AddParticleTarget(owner, unit, "Lulu_R_expire.troy", unit, 1);
        }

        public void OnUpdate(float diff)
        { 
        }
    }
}

