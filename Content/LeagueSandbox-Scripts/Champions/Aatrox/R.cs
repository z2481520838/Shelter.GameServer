using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Scripting.CSharp;

namespace Spells
{
    public class AatroxR : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
            // TODO
        };

        string pCastName;
        string pHitName;
        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            if (owner is IChampion c)
            {
                switch (c.Skin)
                {
                    case 1:
                        pCastName = "Aatrox_Skin01_R_Activate.troy";
                        pHitName = "Aatrox_Skin01_R_active_hit_tar.troy";
                        break;
                    case 2:
                        pCastName = "Aatrox_Skin02_R_Activate.troy";
                        pHitName = "Aatrox_Skin02_R_active_hit_tar.troy";
                        break;
                    default:
                        pCastName = "Aatrox_Base_R_Activate.troy";
                        pHitName = "Aatrox_Base_R_active_hit_tar.troy";
                        break;
                }
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
            AddParticleTarget(spell.CastInfo.Owner, spell.CastInfo.Owner, pCastName, spell.CastInfo.Owner, 1f);
        }

        public void OnSpellPostCast(ISpell spell)
        {
            if (spell.CastInfo.Owner is IChampion owner)
            {
                var damage = 200 + (100 * (spell.CastInfo.SpellLevel - 1)) + (owner.Stats.AbilityPower.Total*0.9f);
                var units = GetUnitsInRange(owner.Position, 550f, true);

                for (int i = 0; i < units.Count; i++)
                {
                    if (units[i].Team != owner.Team && !(units[i] is IObjBuilding || units[i] is IBaseTurret))
                    {
                        units[i].TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                        AddParticleTarget(owner, units[i], pHitName, units[i], lifetime: 1f);
                    }
                }
                AddBuff("AatroxR", 12f, 1, spell, spell.CastInfo.Owner, spell.CastInfo.Owner);
            }
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
