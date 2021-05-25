using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace TalonBleedDebuff
{
    class TalonBleedDebuff : IBuffGameScript
    {
        public BuffType BuffType => BuffType.POISON;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        float damage;
        float timeSinceLastTick = 900f;
        IAttackableUnit Unit;
        IObjAiBase owner;
        IParticle p;
        IParticle p2;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            owner = ownerSpell.CastInfo.Owner as IChampion;
            Unit = unit;
            var ADratio = owner.Stats.AttackDamage.TotalBonus;
            damage = (10 * ownerSpell.CastInfo.SpellLevel + ADratio) / 6f;

            p = AddParticleTarget(owner, unit, "talon_Q_bleed_indicator.troy", unit, 1, buff.Duration);
            p2 = AddParticle(owner, unit, "talon_Q_bleed.troy", unit.Position, 1, buff.Duration);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            RemoveParticle(p);
            RemoveParticle(p2);
        }
        public void OnUpdate(float diff)
        {
            timeSinceLastTick += diff;

            if (timeSinceLastTick >= 1000.0f && Unit != null)
            {
                Unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_PERIODIC, false);
                timeSinceLastTick = 0f;
            }
        }
    }
}
