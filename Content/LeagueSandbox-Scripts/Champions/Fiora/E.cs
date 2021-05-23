using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Scripting.CSharp;



namespace Spells
{
    public class FioraFlurry : ISpellScript
    {
        IMinion minion;
        IObjAiBase Owner;
        IBuff Buff;
        ISpell Spell;
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = false,
            NotSingleTargetSpell = true
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
            Owner = owner;
            Spell = spell;
            spell.CastInfo.Owner.CancelAutoAttack(true);

            Buff = AddBuff("FioraFlurry", 3.0f, 1, spell, owner, owner);
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
        public void TargetExecute(IAttackableUnit unit, bool isCrit)
        {
            if (Owner.HasBuff("FioraFlurry"))
            {

                AddBuff("FioraFlurryDummy", 3f, 1, Spell, Owner, Owner, false);

            }
        }
        public void OnUpdate(float diff)
        {
            if (Owner != null && !Owner.IsDead && Owner.HasBuff(Buff))
            {
                ApiEventManager.OnHitUnit.AddListener(this, Spell.CastInfo.Owner, TargetExecute, false);
            }
        }

    }
}