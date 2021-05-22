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
    public class VeigarPrimordialBurst : ISpellScript
    {
        IStatsModifier statsModifier = new StatsModifier();

        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Target
            }
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellMissileHit.AddListener(this, new System.Collections.Generic.KeyValuePair<ISpell, IObjAiBase>(spell, owner), TargetExecute, false);
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile)
        {
            var owner = spell.CastInfo.Owner as IChampion;
            var ownerSkinID = owner.Skin;
            var APratio = owner.Stats.AbilityPower.Total * 1.2f;
            var targetAP = target.Stats.AbilityPower.Total * 0.8f;
            var damage = 250 + ((spell.CastInfo.SpellLevel -1) * 125) + APratio + targetAP;
            var StacksPerLevel = spell.CastInfo.SpellLevel;

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);


            if (ownerSkinID == 8) 
            {
                AddParticleTarget(owner, "Veigar_Skin08_R_cas.troy", owner, 1, lifetime: 1f);
                AddParticleTarget(target, "Veigar_Skin08_R_tar.troy", target, 1, lifetime: 1f);
            }
            else
            {
                AddParticleTarget(owner, "Veigar_Base_R_cas.troy", owner, 1, lifetime: 1f);
                AddParticleTarget(target, "Veigar_Base_R_tar.troy", target, 1, lifetime: 1f);
            }

            if (target.IsDead && (target is IChampion))
            {
                var buffer = owner.Stats.AbilityPower.FlatBonus;

                statsModifier.AbilityPower.FlatBonus = owner.Stats.AbilityPower.FlatBonus + (StacksPerLevel + 2) - buffer;
                owner.AddStatModifier(statsModifier);

                if (ownerSkinID == 8)
                {
                    AddParticle(target, "Veigar_Skin08_R_tar.troy", target.Position, 1, lifetime: 1f);

                }
                else
                {
                    AddParticle(target, "Veigar_Base_R_tar.troy", target.Position, 1, lifetime: 1f);
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
