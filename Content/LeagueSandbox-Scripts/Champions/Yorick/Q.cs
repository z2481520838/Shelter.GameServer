using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Scripting.CSharp;



namespace Spells
{
    public class YorickSpectral : ISpellScript
    {
        IMinion minion;
        IObjAiBase Owner;
        IBuff Buff;
        ISpell Spell;
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = false,
            NotSingleTargetSpell = true
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            Owner = owner;
            Spell = spell;
            spell.CastInfo.Owner.CancelAutoAttack(true);
            ApiEventManager.OnHitUnit.AddListener(this, spell.CastInfo.Owner, TargetExecute, true);
            Buff = AddBuff("YorickSpectral", 10.0f, 1, spell, owner, owner);

        }

        public void OnSpellCast(ISpell spell)
        {
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
        public void TargetExecute(IAttackableUnit unit, bool isCrit)
        {
            if (Owner.HasBuff("YorickSpectral"))
            {
                var ADratio = Owner.Stats.AttackDamage.Total * 0.2f;
                var damage = 30f + (30 * (Spell.CastInfo.SpellLevel - 1)) + ADratio;
                LogInfo("pastel");


                unit.TakeDamage(Owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
                RemoveBuff(Buff);

                minion = AddMinion(Owner, "YorickSpectralGhoul", "YorickSpectralGhoul", unit.Position);
                LogInfo("pastel2");

                //Ghould debuff (Sets All the stats and decaying health)
                AddBuff("GhoulDebuff", 99999f, 1, Spell, minion, minion);


                //MovSpeed Buffs:
                AddBuff("Something", 5f, 1, Spell, Owner, minion);
                AddBuff("Something", 5f, 1, Spell, minion, minion);

                if (!minion.IsDead)
                {
                    var units = GetUnitsInRange(minion.Position, 200f, true);
                    foreach (var value in units)
                    {
                        if (Owner.Team != value.Team && value is IAttackableUnit && !(value is IBaseTurret) && !(value is IObjAnimatedBuilding))
                        {
                            //TODO: Change TakeDamage to activate on Jack AutoAttackHit, not use CreateTimer, and make Pets use owner spell stats

                            minion.SetTargetUnit(value);


                        }
                    }
                }
            }
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