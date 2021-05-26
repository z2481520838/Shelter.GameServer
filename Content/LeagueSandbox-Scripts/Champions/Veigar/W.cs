using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.Stats;
using GameServerCore.Scripting.CSharp;


namespace Spells
{
    public class VeigarDarkMatter : ISpellScript
    {
        IObjAiBase Owner;
        IStatsModifier statsModifier = new StatsModifier();

        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            Owner = owner;
        }

        public void OnSpellCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner as IChampion;
            var ownerSkinID = owner.Skin;
            var truecoords = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var distance = Vector2.Distance(spell.CastInfo.Owner.Position, truecoords);
            if (distance > 900f)
            {
                truecoords = GetPointFromUnit(spell.CastInfo.Owner, 900f);
            }
            string particles;
            if (ownerSkinID == 8)
            {
                particles = "Veigar_Skin08_W_cas.troy";

            }
            else
            {
                particles = "Veigar_Base_W_cas.troy";
            }
            AddParticle(owner, null, particles, truecoords, lifetime: 1.25f);

            //TODO: Remove this timer and further optmize this section
            CreateTimer(1.25f, () =>
            {
                var owner = spell.CastInfo.Owner;
                var APratio = owner.Stats.AbilityPower.Total;
                var damage = 120f + ((spell.CastInfo.SpellLevel - 1) * 50) + APratio;
                var StacksPerLevel = spell.CastInfo.SpellLevel;

                var units = GetUnitsInRange(truecoords, 225f, true);
                for (int i = 0; i < units.Count; i++)
                {
                    if (units[i].Team != spell.CastInfo.Owner.Team && !(units[i] is IObjBuilding || units[i] is IBaseTurret) && units[i] is IObjAiBase ai)
                    {

                        units[i].TakeDamage(spell.CastInfo.Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

                        if (units[i].IsDead && (units[i] is IChampion))
                        {
                            var buffer = owner.Stats.AbilityPower.FlatBonus;

                            statsModifier.AbilityPower.FlatBonus = owner.Stats.AbilityPower.FlatBonus + StacksPerLevel - buffer;
                            owner.AddStatModifier(statsModifier);
                        }
                    }
                }
                string particles2;
                switch (ownerSkinID)
                {
                    case 8:
                        particles2 = "Veigar_Skin08_W_aoe_explosion.troy";
                        break;

                    case 4:
                        particles2 = "Veigar_Skin04_W_aoe_explosion.troy";
                        break;

                    default:
                        particles2 = "Veigar_Base_W_aoe_explosion.troy";
                        break;
                }
                AddParticle(owner, null, particles2, truecoords, 1f);
            });
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
