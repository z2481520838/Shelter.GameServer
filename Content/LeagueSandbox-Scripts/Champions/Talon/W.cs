using System.Collections.Generic;
using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;

namespace Spells
{
    public class TalonRake : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            //PlayAnimation(owner, "Spell1");
        }

        public void OnSpellCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner as IChampion;

             for (int bladeCount = 0; bladeCount <= 2; bladeCount++)
             {
                 var targetPos = GetPointFromUnit(owner, 700f, (-13f + (bladeCount * 13f)));
                 SpellCast(owner, 1, SpellSlotType.ExtraSlots, targetPos, targetPos, true, Vector2.Zero);
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
        }
    }

    public class TalonRakeMissileOne : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Circle
            },
            IsDamagingSpell = true,
            TriggersSpellCasts = true

            // TODO
        };
        public List<IAttackableUnit> UnitsHit = new List<IAttackableUnit>();

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellMissileHit.AddListener(this, new KeyValuePair<ISpell, IObjAiBase>(spell, owner), TargetExecute, false);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            UnitsHit.Clear();
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile)
        {
            var owner = spell.CastInfo.Owner;
            var spellLevel = owner.GetSpell("TalonRake").CastInfo.SpellLevel;
            var ADratio = owner.Stats.AttackDamage.TotalBonus * 0.6f;
            var damage = 30 + 25f*(spellLevel - 1) + ADratio;

            if (!UnitsHit.Contains(target))
            {
                UnitsHit.Add(target);
                target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
                AddBuff("TalonWSlow", 2f, 1, spell, target, owner); //TODO: Find Proper Name
                AddParticleTarget(owner, target, "talon_w_tar.troy", target, 1f);
            }
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
