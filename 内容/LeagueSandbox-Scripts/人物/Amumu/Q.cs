using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Spells
{
    public class BandageToss : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        private IObjAiBase _owner;
        private ISpell _spell;

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            _owner = owner;
            _spell = spell;
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
        }

        public void OnSpellPostCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner as IChampion;
            var ownerSkinID = owner.SkinID;
            var targetPos = GetPointFromUnit(owner, 1150.0f);      
            FaceDirection(targetPos, owner);     
            SpellCast(owner, 0, SpellSlotType.ExtraSlots, targetPos, targetPos, false, Vector2.Zero);
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
            //SetSpellToolTipVar(_owner, 2, _owner.Stats.AttackDamage.Total * _spell.SpellData.AttackDamageCoefficient, SpellbookType.SPELLBOOK_CHAMPION, 0, SpellSlotType.SpellSlots);
        }
    }

    public class SadMummyBandageToss : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Circle
            },
            IsDamagingSpell = true
            // TODO
        };

        //Vector2 direction;

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
			var dist = System.Math.Abs(Vector2.Distance(target.Position, owner.Position));
			var time = dist/1400f;
			var time2 = time + 1f;
			PlayAnimation(owner, "Spell2");
			AddBuff("Ghosted", time2, 1, spell, target, owner);
            //var ad = owner.Stats.AttackDamage.Total * spell.SpellData.AttackDamageCoefficient;
            var ap = owner.Stats.AbilityPower.Total * 0.7f;
            var damage = 30 + owner.GetSpell(0).CastInfo.SpellLevel * 50 + ap;
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);       
            AddParticleTarget(owner, target, "BandageToss_tar", target);
			AddBuff("Stun", 1f, 1, spell, target, owner);
			ForceMovement(owner, null, target.Position, 1400, 0, 0, 0);
			CreateTimer((float) time , () =>
            {  
			StopAnimation(owner, "Spell2");
			});
            missile.SetToRemove();

            // SpellBuffAdd EzrealRisingSpellForce
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
