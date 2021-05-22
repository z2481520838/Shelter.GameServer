using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;



namespace Spells
{
    public class YorickDecayed : ISpellScript
    {
        IObjAiBase Owner;
        IMinion minion;
        ISpell Spell;

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
        }

        public void OnSpellCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner as IChampion;
            var ownerSkinID = owner.Skin;
            var truecoords = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var distance = Vector2.Distance(spell.CastInfo.Owner.Position, truecoords);
            if (distance > 600f)
            {
                truecoords = GetPointFromUnit(spell.CastInfo.Owner, 600f);
            }
            AddParticle(owner, "yorick_necroExplosion.troy", truecoords, lifetime: 1);


            var APratio = owner.Stats.AbilityPower.Total;
            var damage = 60f + ((spell.CastInfo.SpellLevel - 1) * 35) + APratio;
            var units = GetUnitsInRange(truecoords, 200f, true);
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].Team != spell.CastInfo.Owner.Team && !(units[i] is IObjBuilding || units[i] is IBaseTurret) && units[i] is IObjAiBase ai)
                {

                    units[i].TakeDamage(spell.CastInfo.Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    AddBuff("YorickDecayed", 1.5f, 1, spell, units[i], owner);

                }
            }

            minion = AddMinion(owner, "YorickDecayedGhoul", "YorickDecayedGhoul", truecoords);
            AddBuff("GhoulDebuff", 99999f, 1, spell, minion, minion);

            var damage2 = owner.Stats.AttackDamage.Total * 0.35f;

            if (!minion.IsDead)
            {
                var unit = GetUnitsInRange(minion.Position, 200f, true);
                foreach (var value in unit)
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
            if (minion != null && (!minion.IsDead))
            {
                var units = GetUnitsInRange(minion.Position, 130f, true);
                for (int i = units.Count - 1; i >= 0; i--)
                {
                    if (units[i].Team != Spell.CastInfo.Owner.Team && !(units[i] is IObjBuilding || units[i] is IBaseTurret) && units[i] is IObjAiBase ai)
                    {
                        if (!units[i].HasBuff("YorickDecayed"))
                        {
                            AddBuff("WSlowAura", 0.5f, 1, Spell, units[i], Owner);
                            units.RemoveAt(i);
                        }
                    }

                }
                

                /*var target = GetClosestUnitInRange(minion, 700f, true); 

                if (target != null && target.Team != Owner.Team)
                {
                    minion.SetTargetUnit(target);
                }*/

            }
        }
    }
}
