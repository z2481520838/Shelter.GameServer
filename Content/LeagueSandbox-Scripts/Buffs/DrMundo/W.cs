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

namespace BurningAgony
{
    class BurningAgony : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IObjAiBase Owner;
        IParticle p;
        IParticle p2;
        public ISpellSector DRMundoWAOE;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            Owner = ownerSpell.CastInfo.Owner;
            ApiEventManager.OnSpellSectorHit.AddListener(this, new KeyValuePair <ISpell, IObjAiBase>(ownerSpell, Owner), TargetExecute, false);

            StatsModifier.Tenacity.FlatBonus += 5 + ownerSpell.CastInfo.SpellLevel;
            unit.AddStatModifier(StatsModifier);

            DRMundoWAOE = ownerSpell.CreateSpellSector(new SectorParameters
            {
                BindObject = ownerSpell.CastInfo.Owner,
                HalfLength = 160f,
                Tickrate = 1,
                CanHitSameTargetConsecutively = true,
                OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                Type = SectorType.Area
            });

            p = AddParticleTarget(Owner, unit, "dr_mundo_burning_agony_cas_01.troy", unit, buff.Duration);
            p2 = AddParticleTarget(Owner, unit, "dr_mundo_burning_agony_cas_02.troy", unit, buff.Duration);
        }
        public void TargetExecute(ISpell ownerSpell, IAttackableUnit target, ISpellSector sector)
        {
            float AP = Owner.Stats.AbilityPower.Total * 0.2f;
            float damage = 20f + (15 * ownerSpell.CastInfo.SpellLevel) + AP;

            target.TakeDamage(Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL,DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
        }
        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            ApiEventManager.OnSpellSectorHit.RemoveListener(this);
            DRMundoWAOE.SetToRemove();

            RemoveParticle(p);
            RemoveParticle(p2);
        }
        public void OnUpdate(float diff)
        {

        }
    }
}
