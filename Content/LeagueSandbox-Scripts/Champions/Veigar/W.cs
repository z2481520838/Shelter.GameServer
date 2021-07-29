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
using System.Collections.Generic;
using GameServerCore.Domain.GameObjects.Spell.Sector;

namespace Spells
{
    public class VeigarDarkMatter : ISpellScript
    {
        IObjAiBase Owner;
        ISpell Spell;
        IStatsModifier statsModifier = new StatsModifier();
        ISpellSector DamageSector;
        bool limiter = false;
        bool limiter2 = false;
        float counter = 0f;
        string particles2;

        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellSectorHit.AddListener(this, new KeyValuePair<ISpell, IObjAiBase>(spell, owner), TargetExecute, false);
            Owner = owner;
            Spell = spell;
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile)
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
            var owner = spell.CastInfo.Owner;
            var ownerSkinID = owner.SkinID;
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


            AddBuff("VeigarW", 1.25f, 1, spell, owner, owner);
        }

        public void OnSpellPostCast(ISpell spell)
        {
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
            var ownerSkinID = owner.SkinID;
            var APratio = owner.Stats.AbilityPower.Total;
            var damage = 120f + ((spell.CastInfo.SpellLevel - 1) * 50) + APratio;
            var StacksPerLevel = spell.CastInfo.SpellLevel;

            target.TakeDamage(spell.CastInfo.Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            if(target is IChampion && target.IsDead)
            {
                var buffer = owner.Stats.AbilityPower.FlatBonus;

                statsModifier.AbilityPower.FlatBonus = owner.Stats.AbilityPower.FlatBonus + StacksPerLevel - buffer;
                owner.AddStatModifier(statsModifier);
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
