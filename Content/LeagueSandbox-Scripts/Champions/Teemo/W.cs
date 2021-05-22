using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Scripting.CSharp;

namespace Spells
{
    public class MoveQuick : ISpellScript
    {
        bool takeDamage = true;
        float timer = 5000.0f;
        IAttackableUnit Unit;
        ISpell Spell;
        IObjAiBase Owner;
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            Spell = spell;
            Owner = owner;
            ApiEventManager.OnTakeDamage.AddListener(this, owner, TakeDamage, false);
        }
        public void TakeDamage(IAttackableUnit unit, IAttackableUnit source)
        {
            Unit = unit;
            if (Owner != null)
            {
                RemoveBuff(Owner, "Move Quick");
                takeDamage = true;
            }
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

        public void OnSpellChannelCancel(ISpell spell)
        {
        }

        public void OnSpellPostChannel(ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
            if (takeDamage == true && Unit != null && Spell.CastInfo.SpellLevel >= 1)
            {
                timer += diff;
                if (timer >= 5000.0)
                {
                    AddBuff("Move Quick", 1, 1, Spell, Unit, Unit, true);
                    takeDamage = false;
                    timer = 0;
                }
            }
        }
    }
}
