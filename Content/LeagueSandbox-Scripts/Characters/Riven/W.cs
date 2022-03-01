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
    public class RivenMartyr : ISpellScript //Fix this shit not working at all
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
            spell.CreateSpellSector(new SectorParameters
            {
                Length = 260f,
                SingleTick = true,
                Type = SectorType.Area,
                OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes
            });
        }

        public void OnSpellCast(ISpell spell)
        {
        }

        public void OnSpellPostCast(ISpell spell)
        {
			var owner = spell.CastInfo.Owner;
			if (owner.HasBuff("RivenFengShuiEngine"))
            {
				AddParticleTarget(owner, owner, "exile_W_weapon_cas.troy", owner, bone: "weapon");
				AddParticleTarget(owner, owner, "Riven_Base_W_Ult_Cas.troy", owner);
				AddParticle(owner, null, "Riven_Base_W_Ult_Cas_Ground.troy.troy", owner.Position);
			    AddParticleTarget(owner, owner, "exile_W_weapon_cas.troy", owner, bone: "weapon");
			}
			else
			{
                AddParticleTarget(owner, owner, "Riven_Base_W_Cast.troy", owner);
			    AddParticleTarget(owner, owner, "exile_W_weapon_cas.troy", owner, bone: "weapon");
			}
        }
        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
            var AP = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.25f;
            var AD = spell.CastInfo.Owner.Stats.AttackDamage.Total * 0.6f;
            float damage = 5f + spell.CastInfo.SpellLevel * 35f + AP + AD;
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
            AddParticleTarget(owner, target, "exile_W_tar_02.troy", target, 1f);
            
        }


        public void OnSpellChannel(ISpell spell)
        {
        }

        public void OnSpellChannelCancel(ISpell spell, ChannelingStopSource reason)
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
