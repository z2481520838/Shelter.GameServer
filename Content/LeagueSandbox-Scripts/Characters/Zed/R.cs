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
    public class ZedUlt: ISpellScript
    {
        IAttackableUnit Target;
		IBuff HandlerBuff;
        IMinion Shadow;
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
			Target = target;
			var ownerSkinID = owner.SkinID;
            IMinion Zed = AddMinion((IChampion)owner, "Zed", "Zed", owner.Position, owner.Team, owner.SkinID, true, false);
			SetStatus(Zed, StatusFlags.NoRender, true);
            PlayAnimation(owner, "Spawn");			
			var dist = System.Math.Abs(Vector2.Distance(Target.Position, owner.Position));
			var distt = dist + 1;
			var time1 = distt / 1400f;
			var time2 = time1 + 0.7f;
			SealSpellSlot(owner, SpellSlotType.SpellSlots, 3, SpellbookType.SPELLBOOK_CHAMPION, true);
			AddBuff("ZedBuffer", time2, 1, spell, owner, owner);
			AddBuff("ZedRHandler", 6.0f, 1, spell, owner, owner, false);
			AddBuff("ZedR2", 5.9f, 1, spell, owner, owner);
			CreateTimer(0.7f, () =>
            {
			SetStatus(owner, StatusFlags.NoRender, true);
			SetStatus(Zed, StatusFlags.NoRender, false);
			PlayAnimation(Zed, "spell4_strike");			
            FaceDirection(target.Position, spell.CastInfo.Owner, true);
            ForceMovement(Zed, null, target.Position, 1400f, 0, 0, 0);
			if (ownerSkinID == 1)
                {
                AddParticleTarget(owner, Zed, "Zed_Skin01_R_Dash.troy", owner, time2);
                }
				else
				{
                AddParticleTarget(owner, Zed, "Zed_R_Dash.troy", owner, time2);
				}
			});
			CreateTimer(time2, () =>
            {
			AddBuff("ZedUltExecute", 3f, 1, spell, target, owner);
			AddBuff("Ghosted", 3f, 1, spell, owner, owner);
			SetStatus(Zed, StatusFlags.NoRender, true);
			SetStatus(owner, StatusFlags.NoRender, false);
			Zed.TakeDamage(Zed, 10000f, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, DamageResultType.RESULT_NORMAL);
			SealSpellSlot(owner, SpellSlotType.SpellSlots, 3, SpellbookType.SPELLBOOK_CHAMPION, false);
			});
        }

        public void OnSpellCast(ISpell spell)
        {
			var owner = spell.CastInfo.Owner;
        }

        public void OnSpellPostCast(ISpell spell)
        {
			spell.SetCooldown(0.5f, true);
			var owner = spell.CastInfo.Owner;
			owner.StopMovement();
			var dist = System.Math.Abs(Vector2.Distance(Target.Position, owner.Position));
			var distt = dist + 1;
			var time1 = distt / 1400f;
			var time2 = time1 + 0.7f;
			var targetPos = GetPointFromUnit(owner,distt);
			var randPoint1 = new Vector2(owner.Position.X + (10.0f), owner.Position.Y + 10.0f);	
			ForceMovement(owner, null, randPoint1, 0.5f, 0, -280, 0);
			AddParticleTarget(owner, Target, "Zed_Ult_TargetMarker_tar.troy", Target, 10f);		
			IMinion Shadow = AddMinion(owner, "ZedShadow", "ZedShadow", owner.Position, owner.Team, owner.SkinID, true, false);
			AddBuff("ZedRShadowBuff", 6.0f, 1, spell, Shadow, owner);
			CreateTimer(0.7f, () =>
            {
			PlayAnimation(owner, "spell4_strike");
			owner.SetDashingState(false);	
            FaceDirection(targetPos, spell.CastInfo.Owner, true);
            ForceMovement(owner, null, targetPos, 1400f, 0, 0, 0);
            AddParticleTarget(owner, owner, "Zed_R_Dash.troy", owner, time2);
			});
			CreateTimer(time2, () =>
            {
                PlayAnimation(owner, "spell4_leadin");					
			    for (int bladeCount = 0; bladeCount <= 2; bladeCount++)
                  {	                			  
                    var targetPosReturn = GetPointFromUnit(owner, 600f, bladeCount * 120f);
					IMinion m = AddMinion((IChampion)owner, "ZedShadow", "ZedShadow", targetPosReturn, owner.Team, owner.SkinID, true, false);				
					PlayAnimation(m, "spell4_strike");
					AddParticleTarget(owner, m, "Zed_R_Dash.troy", m);
                    var targetPos = GetPointFromUnit(Target, 100f);
			        ForceMovement(m, null, targetPos, 1400, 0, 0, 0);					
					AddBuff("ZedUltDashCloneMaker", 65f, 1, spell, m, m);
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
	public class ZedUltDash: ISpellScript
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
	public class ZedR2: ISpellScript
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