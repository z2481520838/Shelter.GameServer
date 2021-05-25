using System.Linq;
using GameServerCore;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Scripting.CSharp;

namespace Spells
{
    public class AkaliShadowSwipe : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
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

        }

        public void OnSpellCast(ISpell spell)
        {
        }

        public void OnSpellPostCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;
            var APratio = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.3f;
            var ADratio = spell.CastInfo.Owner.Stats.AttackDamage.Total * 0.6f;
            var damage = 40 + spell.CastInfo.SpellLevel * 30 + APratio + ADratio;
            var MarkAPratio = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.5f;
            var MarkDamage = 45 + 25 * (owner.GetSpell("AkaliMota").CastInfo.SpellLevel - 1) + MarkAPratio;

            AddParticleTarget(owner, owner, "akali_shadowSwipe_cas.troy", owner, 1f);
            var units = GetUnitsInRange(owner.Position, 300f, true);
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i] != owner && !(units[i] is IObjBuilding) && !(units[i] is ILaneTurret) && !(units[i] is IBaseTurret))
                {
                    if (units[i].HasBuff("AkaliMota"))
                    {
                        units[i].TakeDamage(owner, MarkDamage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_PROC, false);
                        RemoveBuff(units[i], "AkaliMota");
                    }
                        units[i].TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
                        AddParticleTarget(owner, units[i], "akali_shadowSwipe_tar.troy", units[i], 1f);
                }
                //AddParticleTarget(owner, "akali_shadowSwipe_heal.troy", owner, lifetime: 1f);
                //"E" has a heal Particles but there's no healing in the abilitie's description, so i'm not sure if i should add it or not
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
