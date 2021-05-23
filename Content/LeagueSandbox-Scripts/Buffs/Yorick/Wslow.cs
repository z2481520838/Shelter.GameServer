using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace YorickDecayed
{
    internal class YorickDecayed : IBuffGameScript
    {
        public BuffType BuffType => BuffType.SLOW;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();
        IParticle pbuff;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            var owner = ownerSpell.CastInfo.Owner;
            StatsModifier.MoveSpeed.PercentBonus = StatsModifier.MoveSpeed.PercentBonus - (0.3f + (0.05f * (ownerSpell.CastInfo.SpellLevel-1)));
            unit.AddStatModifier(StatsModifier);
            AddParticleTarget(owner, unit, "yorick_necroExplosion_unit_tar.troy", unit, 1, buff.Duration);
            AddParticleTarget(owner, unit, "Global_Slow.troy", unit, 1, buff.Duration);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
        }

        public void OnUpdate(float diff)
        {

        }
    }
}

