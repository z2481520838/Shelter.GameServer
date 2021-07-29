using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using System.Collections.Generic;

namespace Buffs
{
    class VeigarW : IBuffGameScript
    {
        public BuffType BuffType => BuffType.INTERNAL;
        public BuffAddType BuffAddType => BuffAddType.STACKS_AND_OVERLAPS;
        public int MaxStacks => int.MaxValue;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();
        ISpellSector DamageSector;
        string particles2;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            ApiEventManager.OnSpellSectorHit.AddListener(this, new KeyValuePair<ISpell, IObjAiBase>(ownerSpell, ownerSpell.CastInfo.Owner), TargetExecute, true);

            DamageSector = ownerSpell.CreateSpellSector(new SectorParameters
            {
                HalfLength = 225f,
                Lifetime = -1.0f,
                Tickrate = 0,
                SingleTick = true,
                OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                Type = SectorType.Area
            });

            switch ((unit as IObjAiBase).SkinID)
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
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            DamageSector.ExecuteTick();
            AddParticle(ownerSpell.CastInfo.Owner, null, particles2, DamageSector.Position);

            /*var Unit = unit as IChampion;
            var script = Unit.GetSpell("VeigarDarkMatter").Script as Spells.VeigarDarkMatter;

            script.DamageSector.ExecuteTick();*/
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
            var ownerSkinID = owner.SkinID;
            var APratio = owner.Stats.AbilityPower.Total;
            var damage = 120f + ((spell.CastInfo.SpellLevel - 1) * 50) + APratio;
            var StacksPerLevel = spell.CastInfo.SpellLevel;

            target.TakeDamage(spell.CastInfo.Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            /*if (target is IChampion && target.IsDead)
            {
                var buffer = owner.Stats.AbilityPower.FlatBonus;

                StatsModifier.AbilityPower.FlatBonus = owner.Stats.AbilityPower.FlatBonus + StacksPerLevel - buffer;
                owner.AddStatModifier(StatsModifier);
            }*/
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
