using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;


namespace PotionOfGiantStrength
{
    internal class PotionOfGiantStrength : IBuffGameScript
    {
        public BuffType BuffType => BuffType.HEAL;
        public BuffAddType BuffAddType => BuffAddType.STACKS_AND_CONTINUE;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IParticle potion;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            var owner = ownerSpell.CastInfo.Owner;
            var extraHP = 120f + (6.39f * owner.Stats.Level);

            StatsModifier.HealthPoints.FlatBonus = extraHP;
            owner.Stats.CurrentHealth = owner.Stats.CurrentHealth + extraHP;
            StatsModifier.AttackDamage.FlatBonus = 15f;

            unit.AddStatModifier(StatsModifier);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
        }

        public void OnUpdate(float diff)
        {

        }
    }
}
