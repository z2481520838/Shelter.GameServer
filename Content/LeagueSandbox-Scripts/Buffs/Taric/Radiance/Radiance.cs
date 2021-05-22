using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;

namespace Radiance
{
    internal class Radiance : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IChampion owner;
        IAttackableUnit Unit;
        ISpell spell;
        IParticle p;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            owner = ownerSpell.CastInfo.Owner as IChampion;
            Unit = unit;
            spell = ownerSpell;
            var ADbuff = 10f + ownerSpell.CastInfo.SpellLevel * 20;
            var APbuff = 10f + ownerSpell.CastInfo.SpellLevel * 20;

            if (unit != owner)
            {
                ADbuff *= 0.5f;
                APbuff *= 0.5f;
            }
            else
            {
               p = AddParticleTarget(unit, "taricgemstorm.troy", unit, 1.4f, lifetime: buff.Duration);
            }

            StatsModifier.AttackDamage.FlatBonus += ADbuff;
            StatsModifier.AbilityPower.FlatBonus += APbuff;
            unit.AddStatModifier(StatsModifier);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            RemoveParticle(p);
        }

        public void OnUpdate(float diff)
        {
            if (Unit == owner)
            {
                var units = GetUnitsInRange(owner.Position, 400, true);
                for (int i = units.Count - 1; i >= 0; i--)
                {
                    if (units[i].Team == owner.Team && units[i] != owner && !(units[i] is IBaseTurret || units[i] is IObjBuilding || units[i] is IInhibitor || units[i] is IMinion)) //TODO: Fix self apply and not applying to nearby allies, and double check if buff are still applied to minions
                    {
                        AddBuff("Radiance", 1f, 1, spell, units[i], spell.CastInfo.Owner);
                    }

                }

            }


        }
    }
}
