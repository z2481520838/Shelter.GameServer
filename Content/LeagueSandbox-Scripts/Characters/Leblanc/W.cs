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
    public class LeblancSlide: ISpellScript
    {

        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

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
			var owner = spell.CastInfo.Owner;
            AddBuff("LeblancSlide", 4.0f, 1, spell, owner, owner);
			AddBuff("LeblancSlideMove", 3.5f, 1, spell, owner, owner);
			IMinion Leblanc = AddMinion(owner, "TestCube", "TestCube", owner.Position, owner.Team, owner.SkinID, true, false);
			AddBuff("LeblancSlideReturn", 4.0f, 1, spell, Leblanc, owner);			
        }

        public void OnSpellPostCast(ISpell spell)
        {
			spell.SetCooldown(0.5f, true);
			var owner = spell.CastInfo.Owner;
			//AddBuff("LeblancSlideAOE", 0.5f, 1, spell, owner, owner);
			//if (!owner.HasBuff("LeblancSlideM") && owner.GetSpell("LeblancMimic").CastInfo.SpellLevel >= 1 )
			if (!owner.HasBuff("LeblancSlideM")&&owner.HasBuff("LeblancMimic"))
             {
             owner.SetSpell("LeblancSlideM", 3, true);
			 }
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
			var dist = System.Math.Abs(Vector2.Distance(truecoords, owner.Position));
            var time = dist / 1450f;
			//PlayAnimation(owner, "Spell2",time);
			owner.Stats.SetActionState(ActionState.CAN_MOVE, false);	 
			AddParticleTarget(owner, owner, "LeBlanc_Base_W_mis.troy", owner, 0.5f);
			AddParticle(owner, null, "LeBlanc_Base_W_cas.troy", owner.Position,time);
			FaceDirection(truecoords, spell.CastInfo.Owner, true);
            ForceMovement(owner, null, truecoords, 1450, 0, 0, 0);
            PlayAnimation(owner, "Spell2",time);
			//var time1 = dist + 0.2f;
			CreateTimer((float) time , () =>
            {
            if (spell.CastInfo.Owner is IChampion c)
            {
				//c.GetSpell(1).LowerCooldown(20);
				StopAnimation(owner, "Spell2");
			    AddParticle(c, null, "LeBlanc_Base_W_aoe_impact_02.troy", c.Position);

                var units = GetUnitsInRange(c.Position, 260f, true);
                for (int i = 0; i < units.Count; i++)
                {
                    if (units[i].Team != c.Team && !(units[i] is IObjBuilding || units[i] is IBaseTurret))
                    {
						var AP = c.Stats.AbilityPower.Total * 0.65f;
						var QLevel = c.GetSpell(0).CastInfo.SpellLevel;
						var WLevel = c.GetSpell(1).CastInfo.SpellLevel;
						var RLevel = c.GetSpell(3).CastInfo.SpellLevel;
						var damage = 85 + (40 * (WLevel - 1)) + AP;
						var Qdamage = 55 + 25f*(QLevel - 1) + AP;
			            var QMarkdamage = Qdamage + damage;
						var RQdamage = 100 + 100f*(QLevel - 1) + AP;
			            var RQMarkdamage = Qdamage + damage;
						if (units[i].HasBuff("LeblancChaosOrb"))
                            {
							units[i].TakeDamage(c, QMarkdamage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, true);
							AddParticleTarget(c, units[i], "LeBlanc_Base_W_tar.troy", units[i], 1f);
				            AddParticleTarget(c, units[i], "LeBlanc_Base_W_tar_02.troy", units[i], 1f);
					        units[i].RemoveBuffsWithName("LeblancChaosOrb");
                            }
						if (units[i].HasBuff("LeblancChaosOrbM"))
                            {
							units[i].TakeDamage(c, RQMarkdamage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, true);
							AddParticleTarget(c, units[i], "LeBlanc_Base_W_tar.troy", units[i], 1f);
				            AddParticleTarget(c, units[i], "LeBlanc_Base_W_tar_02.troy", units[i], 1f);
					        units[i].RemoveBuffsWithName("LeblancChaosOrbM");
                            }
						else
							{
                            units[i].TakeDamage(c, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
						    AddParticleTarget(c, units[i], "LeBlanc_Base_W_tar.troy", units[i], 1f);
				            AddParticleTarget(c, units[i], "LeBlanc_Base_W_tar_02.troy", units[i], 1f);
						    }
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
	public class LeblancSlideReturn: ISpellScript
    {

       public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

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