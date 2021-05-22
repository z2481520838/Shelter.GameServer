using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;


namespace GhoulDebuff
{
    internal class GhoulDebuff : IBuffGameScript
    {
        IAttackableUnit Unit;
        float ticks;
        float ticks2;

        public BuffType BuffType => BuffType.SLOW;
        public BuffAddType BuffAddType => BuffAddType.STACKS_AND_RENEWS;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            Unit = unit;
            var owner = ownerSpell.CastInfo.Owner;
            StatsModifier.HealthPoints.FlatBonus = (owner.Stats.HealthPoints.Total * 0.3f) - 60;
            StatsModifier.HealthRegeneration.PercentBaseBonus = -100f;
            StatsModifier.AttackDamage.FlatBonus = owner.Stats.AttackDamage.Total * 0.3f - 10;
            StatsModifier.Armor.FlatBonus = 10f + (owner.Stats.Level * 2) - unit.Stats.Armor.BaseValue;
            StatsModifier.MagicResist.FlatBonus = 10f + (owner.Stats.Level * 2) - unit.Stats.MagicResist.BaseValue;

            unit.Stats.CurrentHealth = 1000000f;
            unit.AddStatModifier(StatsModifier);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
        }

        public void OnUpdate(float diff)
        {
            ticks++;
            ticks2++;

            if(ticks == 60f) 
            {
                //Unit.TakeDamage(Unit, Unit.Stats.HealthPoints.Total * 0.2f, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_PERIODIC, false);
                Unit.Stats.CurrentHealth = Unit.Stats.CurrentHealth - (Unit.Stats.HealthPoints.Total * 0.2f);
                ticks = 0;
            }
            if(ticks2 == 300f)
            {
                Unit.Die(Unit);
            }
        }
    }
}

