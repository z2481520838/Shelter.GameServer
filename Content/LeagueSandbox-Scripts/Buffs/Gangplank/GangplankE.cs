using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using System.Collections.Generic;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;


namespace RaiseMoraleTeamBuff
{
    internal class RaiseMoraleTeamBuff : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        private List<IParticle> Particles => new List<IParticle>();

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            var ADbuff = 12f + 7f * (ownerSpell.CastInfo.SpellLevel - 1);
            var MSbuff = 0.08f + 0.03f * (ownerSpell.CastInfo.SpellLevel -1);

            if (unit == ownerSpell.CastInfo.Owner)
            {
                StatsModifier.MoveSpeed.PercentBonus = MSbuff;
                StatsModifier.AttackDamage.FlatBonus = ADbuff;
            }
            else
            {
                StatsModifier.MoveSpeed.PercentBonus = MSbuff / 2;
                StatsModifier.AttackDamage.FlatBonus = ADbuff / 2;
            }

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
