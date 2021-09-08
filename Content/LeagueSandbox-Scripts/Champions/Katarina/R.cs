using System.Linq;
using GameServerCore;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Sector;

namespace Spells
{
    public class KatarinaR : ISpellScript //Fix this shit not working at all
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {

            NotSingleTargetSpell = true,
            IsDamagingSpell = true,
            TriggersSpellCasts = true

            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {


        }

        public void OnSpellCast(ISpell spell)
        {
            AddBuff("KatarinaR", 2.5f, 1, spell, spell.CastInfo.Owner, spell.CastInfo.Owner, false);
            spell.CastInfo.Owner.StopMovement();
            spell.CreateSpellSector(new SectorParameters
            {
                HalfLength = 550f,
                Tickrate = 4,
                Type = SectorType.Area,
                CanHitSameTargetConsecutively = true,
                OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectHeroes,
                Lifetime = 2.5f
            });
        }

        public void OnSpellPostCast(ISpell spell)
        {
        }
        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            if (spell.CastInfo.Owner.HasBuff("KatarinaR") && target != null)
            {
                SpellCast(spell.CastInfo.Owner, 0, SpellSlotType.ExtraSlots, false, target, Vector2.Zero);
                //target.TakeDamage(spell.CastInfo.Owner, 10f, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
            }
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
        }

        public class KatarinaRMis : ISpellScript
        {
            public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
            {
                TriggersSpellCasts = true,
                IsDamagingSpell = true,
                MissileParameters = new MissileParameters
                {
                    Type = MissileType.Target
                }
                // TODO
            };

            public void OnActivate(IObjAiBase owner, ISpell spell)
            {
                ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
            }

            public void OnDeactivate(IObjAiBase owner, ISpell spell)
            {
            }

            public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
            {
            }

            public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
            {
                var owner = spell.CastInfo.Owner;

                AddParticleTarget(owner, target, "Ezreal_mysticshot_tar.troy", target, 1f);

                missile.SetToRemove();
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

            public void OnUpdate(float diff)
            {
            }
        }
    }
}
