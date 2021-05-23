using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.Stats;
using GameServerCore.Scripting.CSharp;


namespace Spells
{
    public class YorickRavenous : ISpellScript
    {
         IObjAiBase Owner;
        IMinion minion;
        ISpell Spell;
        IAttackableUnit Target;
        IStatsModifier StatsModifier = new StatsModifier();

        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,

        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            Owner = owner;
            Spell = spell;
            Target = target;
        }

        public void OnSpellCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner as IChampion;
            var ownerSkinID = owner.Skin;

            AddParticleTarget(owner, owner, "yorick_ravenousGhoul_cas_tar.troy", Target, lifetime: 1);


            var ADratio = owner.Stats.AttackDamage.FlatBonus;

            var damage = 60f + ((spell.CastInfo.SpellLevel - 1) * 35) + ADratio;

            Target.TakeDamage(spell.CastInfo.Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);


                    var MR = Target.Stats.MagicResist.Total;
            var penetration = (1 - owner.Stats.MagicPenetration.PercentBonus) * MR - owner.Stats.MagicPenetration.FlatBonus;
            var defense = 100 / (Target.Stats.MagicResist.Total + 100); //- penetration;

            var heal = damage * defense;

            owner.Stats.CurrentHealth = owner.Stats.CurrentHealth + (heal * 0.35f);


            minion = AddMinion(owner, "YorickRavenousGhoul", "YorickRavenousGhoul", Target.Position);

            AddBuff("GhoulDebuff", 99999f, 1, spell, minion, minion); //Had to set up a huge buff time cuz infinite time makes "OnUpdate" from Buff not get executed

            if (!minion.IsDead)
            {
                var units = GetUnitsInRange(minion.Position, 200f, true);
                foreach (var value in units)
                {
                    if (owner.Team != value.Team && value is IAttackableUnit && !(value is IBaseTurret) && !(value is IObjAnimatedBuilding))
                    {
                        //TODO: Change TakeDamage to activate on Jack AutoAttackHit, not use CreateTimer, and make Pets use owner spell stats
                        minion.SetTargetUnit(value);
                    }
                }
            }
        }

        public void OnSpellPostCast(ISpell spell)
        {
        }

        public void OnSpellChannel(ISpell spell)
        {
        }

        public void OnSpellChannelCancel(ISpell spell)
        {
        }

        public void OnSpellPostChannel(ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
            if (minion != null && !minion.IsDead) 
            {

                var target = GetClosestUnitInRange(minion, 700f, true);
                if (target != null && !target.IsDead && target.Team != Owner.Team)
                {
                    minion.SetTargetUnit(target);
                }
            }
        }
    }
}

