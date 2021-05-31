using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Scripting.CSharp;


namespace Spells
{
    public class BlindMonkQOne : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Circle
            },
            TriggersSpellCasts = true
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellMissileHit.AddListener(this, new KeyValuePair<ISpell, IObjAiBase>(spell, owner), TargetExecute, false);
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

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile)
        {
            var owner = spell.CastInfo.Owner;
            var ad = owner.Stats.AttackDamage.Total * 0.9f;
            var damage = 50 + (spell.CastInfo.SpellLevel * 30) + ad;
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
            AddParticleTarget(owner, target, "blindMonk_Q_resonatingStrike_tar.troy", target, bone: "C_BuffBone_Glb_Center_Loc");
            AddParticleTarget(owner, target, "blindMonk_Q_tar.troy", target, bone: "C_BuffBone_Glb_Center_Loc");
            if (target is IObjAiBase u)
            {
                AddBuff("BlindMonkQOne", 3f, 1, spell, u, owner);
            }

            missile.SetToRemove();
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
