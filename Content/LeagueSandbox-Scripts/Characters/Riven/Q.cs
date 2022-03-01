using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using GameServerCore;
using LeagueSandbox.GameServer.GameObjects.Stats;
using System.Linq;

namespace Spells
{
    public class RivenTriCleave: ISpellScript
    {
		ISpell spell;
        int counter;
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
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
             SetStatus(owner, StatusFlags.Ghosted, true);			
        }

        public void OnSpellCast(ISpell spell)
        {	
        }

        public void OnSpellPostCast(ISpell spell)
        {
			spell.SetCooldown(0.35f, true);
			var owner = spell.CastInfo.Owner;
			var QLevel = owner.GetSpell(0).CastInfo.SpellLevel;
			var damage = 10 + (20 * (QLevel - 1)) + (owner.Stats.AttackDamage.Total * 0.6f);
			var current = new Vector2(owner.Position.X, owner.Position.Y);
            var spellPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var dist = Vector2.Distance(current, spellPos);

            if (dist > 260.0f)
            {
                dist = 260.0f;
            }
            AddBuff("RivenTriCleave", 6.0f, 1, spell, owner, owner);
            FaceDirection(spellPos, owner, true);
            var trueCoords = GetPointFromUnit(owner, dist);
			FaceDirection(trueCoords, owner, true);
				PlayAnimation(owner, "Spell1a",0.6f);
				ForceMovement(owner, null, trueCoords, 1400, 0, 0, 0);
                AddParticleTarget(owner, owner, ".troy", owner, 0.5f);
                CreateTimer((float) 0.25 , () =>
                {
                AddParticleTarget(owner, owner, "Riven_Base_Q_01_Wpn_Trail.troy", owner, 3f, bone: "C_BuffBone_Glb_Center_Loc");					
                AddParticle(owner, null, "exile_Q_01_detonate.troy", GetPointFromUnit(owner, 125f));
				var units = GetUnitsInRange(GetPointFromUnit(owner, 125f), 260f, true);
                for (int i = 0; i < units.Count; i++)
                {
                if (units[i].Team != owner.Team && !(units[i] is IObjBuilding || units[i] is IBaseTurret))
                    {					     
                         units[i].TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
						 AddParticleTarget(owner, units[i], "RivenQ_tar.troy", units[i], 10f,1,"");
				         AddParticleTarget(owner, units[i], "exile_Q_tar_01.troy", units[i], 10f,1,"");
						 AddParticleTarget(owner, units[i], "exile_Q_tar_04.troy", units[i], 10f,1,"");
                    }	
                }
			    });
				counter++;
				//AddParticle(owner, null, "RivenQStreakA.troy", owner.Position);
           if (owner.HasBuff("RivenTriCleave"))
             {			   
                if (counter == 2)
                {
				spell.SetCooldown(0.35f, true);
                PlayAnimation(owner, "Spell1b",0.6f);
				ForceMovement(owner, null, trueCoords, 1400, 0, 0, 0);
				AddParticleTarget(owner, owner, ".troy", owner, 0.5f);
				AddBuff("RivenTriCleave", 6.0f, 1, spell, owner, owner);
				CreateTimer((float) 0.25 , () =>
                {
                AddParticleTarget(owner, owner, "Riven_Base_Q_02_Wpn_Trail.troy", owner, 3f, bone: "C_BuffBone_Glb_Center_Loc");					
                AddParticle(owner, null, "exile_Q_02_detonate.troy", GetPointFromUnit(owner, 125f));
				var units = GetUnitsInRange(GetPointFromUnit(owner, 125f), 260f, true);
				for (int i = 0; i < units.Count; i++)
                {
                if (units[i].Team != owner.Team && !(units[i] is IObjBuilding || units[i] is IBaseTurret))
                    {					     
                         units[i].TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
						 AddParticleTarget(owner, units[i], "RivenQ_tar.troy", units[i], 10f,1,"");
				         AddParticleTarget(owner, units[i], "exile_Q_tar_02.troy", units[i], 10f,1,"");
						 AddParticleTarget(owner, units[i], "exile_Q_tar_04.troy", units[i], 10f,1,"");
                    }	
                }
			    });
				//AddParticle(owner, null, "RivenQStreakB.troy", owner.Position);
                }	
				if (counter == 3)
                {
				owner.Spells[0].SetCooldown(spell.SpellData.Cooldown[0]);
                PlayAnimation(owner, "Spell1c",0.6f);
				ForceMovement(owner, null, trueCoords, 1400, 0, 100, 0);
                AddParticleTarget(owner, owner, ".troy", owner, 0.5f);
				CreateTimer((float) 0.5 , () =>
                {
                AddParticleTarget(owner, owner, "Riven_Base_Q_03_Wpn_Trail.troy", owner, 3f, bone: "C_BuffBone_Glb_Center_Loc");					
                AddParticle(owner, null, "exile_Q_03_detonate.troy", GetPointFromUnit(owner, 125f));
				var units = GetUnitsInRange(GetPointFromUnit(owner, 125f), 260f, true);
				for (int i = 0; i < units.Count; i++)
                {
                if (units[i].Team != owner.Team && !(units[i] is IObjBuilding || units[i] is IBaseTurret))
                    {					     
                         units[i].TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
						 AddParticleTarget(owner, units[i], "RivenQ_tar.troy", units[i], 10f,1,"");
				         AddParticleTarget(owner, units[i], "exile_Q_tar_03.troy", units[i], 10f,1,"");
						 AddParticleTarget(owner, units[i], "exile_Q_tar_04.troy", units[i], 10f,1,"");
                    }	
                }	
			    });	
				//AddParticle(owner, null, "RivenQStreakC.troy", owner.Position);
                SetStatus(owner, StatusFlags.Ghosted, false);
                owner.RemoveBuffsWithName("RivenTriCleave");					
                counter = 0;
                }
            }				
        }
		public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        { 
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