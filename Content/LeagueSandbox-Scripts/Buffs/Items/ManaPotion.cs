using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;


namespace FlaskOfCrystalWater
{
    internal class FlaskOfCrystalWater : IBuffGameScript
    {
        public BuffType BuffType => BuffType.HEAL;
        public BuffAddType BuffAddType => BuffAddType.STACKS_AND_CONTINUE;
        public int MaxStacks => 5;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IParticle potion;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            StatsModifier.ManaRegeneration.FlatBonus = 6.67f;
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
