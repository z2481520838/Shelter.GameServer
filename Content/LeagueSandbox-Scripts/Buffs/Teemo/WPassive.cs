using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;


namespace MoveQuickPassive //TODO: Find Proper Buff Name
{
    internal class MoveQuickPassive : IBuffGameScript
    {
        public BuffType BuffType => BuffType.HASTE;
        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            StatsModifier.MoveSpeed.PercentBonus += 0.06f + 0.04f * ownerSpell.CastInfo.SpellLevel;
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

