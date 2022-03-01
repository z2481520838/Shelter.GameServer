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
    public class AatroxQ: ISpellScript
    {

        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
            // TODO
        };
		string pcastname;
        string phitname;

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(ISpell spell)
        {
			
        }

        public void OnSpellPostCast(ISpell spell)
        {
			var owner = spell.CastInfo.Owner;
			//owner.StopMovement();
			var Blood = owner.Stats.CurrentHealth * 0.1f;
			owner.Stats.CurrentMana += Blood;
			owner.TakeDamage(owner, Blood, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_PERIODIC, false);
			//AddBuff("LeblancSlideAOE", 0.5f, 1, spell, owner, owner);
			//if (!owner.HasBuff("LeblancSlideM") && owner.GetSpell("LeblancMimic").CastInfo.SpellLevel >= 1 )
			var Cursor = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var current = new Vector2(owner.Position.X, owner.Position.Y);
            var distance = Cursor - current;
            Vector2 truecoords;
            if (distance.Length() > 600f)
            {
                distance = Vector2.Normalize(distance);
                var range = distance * 600f;
                truecoords = current + range;
            }
            else
            {
                truecoords = Cursor;
            }
			PlayAnimation(owner, "Spell1");
			AddParticleTarget(owner, owner, "Aatrox_Base_Q_Cast.troy", owner, 10f);
			AddParticle(owner, null, "Aatrox_Base_Q_Tar_Green.troy", truecoords);
			var randPoint1 = new Vector2(owner.Position.X + (40.0f), owner.Position.Y + 40.0f);	
			ForceMovement(owner, null, randPoint1, 110, 0, 80, 0);
			AddParticleTarget(owner, owner, "LeBlanc_Base_W_mis.troy", owner, 0.5f);
			AddParticle(owner, null, "LeBlanc_Base_W_cas.troy", owner.Position);
			FaceDirection(truecoords, spell.CastInfo.Owner, true);
			CreateTimer((float) 0.25 , () =>
            {
		    owner.SetDashingState(false);		
			AddParticleTarget(owner, owner, "Aatrox_Base_Q_Cast.troy", owner, 10f);
            ForceMovement(owner, null, truecoords, 2450, 0, 0, 0);
			});	
			CreateTimer((float) 0.5 , () =>
            {
            if (spell.CastInfo.Owner is IChampion c)
            {
				if (c.SkinID == 1)
                {
                    pcastname = "Aatrox_Skin01_Q_Land";
                }
                else
                {
                    pcastname = "Aatrox_Base_Q_Land";
                }
				StopAnimation(c, "Spell1");	
			    AddParticle(c, null, pcastname, c.Position);
				AddParticleTarget(c, c, "Aatrox_Base_Q_land_sound.troy", c, 10f);
                var damage = 70 + (45 * (spell.CastInfo.SpellLevel - 1)) + (c.Stats.AttackDamage.Total * 0.6f);
                var units = GetUnitsInRange(c.Position, 260f, true);
                for (int i = 0; i < units.Count; i++)
                {
                    if (units[i].Team != c.Team && !(units[i] is IObjBuilding || units[i] is IBaseTurret))
                    {
                            units[i].TakeDamage(c, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
						    AddParticleTarget(c, units[i], "Aatrox_Base_Q_Hit.troy", units[i], 1f);
				            AddParticleTarget(c, units[i], ".troy", units[i], 1f);
                    }
                }
                var unitss = GetUnitsInRange(c.Position, 100f, true);
                for (int i = 0; i < unitss.Count; i++)
                {	
                    if (unitss[i].Team != c.Team && !(unitss[i] is IObjBuilding || unitss[i] is IBaseTurret))
                    {
						ForceMovement(unitss[i], "RUN", new Vector2(unitss[i].Position.X + 5f, unitss[i].Position.Y + 5f), 13f, 0, 16.5f, 0);	    
                    }
                }					
            }
           });			
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
