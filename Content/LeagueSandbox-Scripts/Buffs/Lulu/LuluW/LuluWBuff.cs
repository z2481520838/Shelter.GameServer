using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;

namespace LuluWBuff
{
    internal class LuluWBuff : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IParticle p;
        IParticle p2;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            var APratio = ownerSpell.CastInfo.Owner.Stats.AbilityPower.Total * 0.001f;

            p = AddParticleTarget(ownerSpell.CastInfo.Owner, "Lulu_W_buf_01.troy", unit, 1, lifetime: buff.Duration);
            p2 = AddParticleTarget(ownerSpell.CastInfo.Owner, "Lulu_W_buf_02.troy", unit, 1, lifetime: buff.Duration);

            StatsModifier.MoveSpeed.PercentBonus += 0.3f + APratio;
            unit.AddStatModifier(StatsModifier);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            RemoveParticle(p);
            RemoveParticle(p2);
        }

        public void OnUpdate(float diff)
        {

        }
    }
}
