using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using System.Linq;
using System.Numerics;
using GameServerCore.Scripting.CSharp;

namespace Spells
{
    public class AatroxW : ISpellScript
    {
        IObjAiBase Owner;
        ISpell Spell;
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            AddBuff("AatroxWONHBuff", float.MaxValue, 1, spell, spell.CastInfo.Owner, spell.CastInfo.Owner, true);
            //AddBuff("AatroxWLife", float.MaxValue, 1, spell, owner, owner);
            Owner = owner;
            Spell = spell;
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            if (owner.HasBuff("AatroxWLife"))
            {
                var buff = owner.GetBuffWithName("AatroxWLife");
                buff.DeactivateBuff();
            }

            AddBuff("AatroxWPower", float.MaxValue, 1, spell, owner, owner);
        }

        public void OnSpellCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;
            if (owner.HasBuff("AatroxWONHLifeBuff"))
            {
                var buff = owner.GetBuffWithName("AatroxWONHLifeBuff");
                buff.DeactivateBuff();
            }

            owner.SetSpell("AatroxW2", 1, true);
            spell.CastInfo.Owner.GetSpell("AatroxW2").SetCooldown(0.5f);
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
    public class AatroxW2 : ISpellScript
    {
        IObjAiBase Owner;
        ISpell Spell;
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            Owner = owner;
            Spell = spell;
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            if (owner.HasBuff("AatroxWPower"))
            {
                var buff = owner.GetBuffWithName("AatroxWPower");
                buff.DeactivateBuff();
            }
            AddBuff("AatroxWLife", float.MaxValue, 1, spell, owner, owner);
        }

        public void OnSpellCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;

            if (owner.HasBuff("AatroxWONHPowerBuff"))
            {
                var buff = owner.GetBuffWithName("AatroxWONHPowerBuff");
                buff.DeactivateBuff();
            }
            owner.SetSpell("AatroxW", 1, true);
            spell.CastInfo.Owner.GetSpell("AatroxW").SetCooldown(0.5f);
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

