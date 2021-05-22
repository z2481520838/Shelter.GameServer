using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using LeagueSandbox.GameServer.GameObjects.Stats;
using GameServerCore.Scripting.CSharp;

namespace Highlander
{
    internal class Highlander : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IParticle p;
        IParticle p2;
        string particle;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            switch (ownerSpell.CastInfo.SpellLevel)
            {
                case 1:
                    particle = "MasterYi_Base_R_Buf.troy";
                        break;
                case 2:
                    particle = "MasterYi_Base_R_Buf_Lvl2.troy";
                        break;
                case 3:
                    particle = "MasterYi_Base_R_Buf_Lvl3.troy";
                        break;
            }
            p = AddParticleTarget(unit, "Highlander_buf.troy", unit, 1, lifetime: buff.Duration);
            p2 = AddParticleTarget(unit, particle, unit, 1, lifetime: buff.Duration);

            StatsModifier.MoveSpeed.PercentBonus += 0.15f + (ownerSpell.CastInfo.SpellLevel * 0.10f);
            StatsModifier.AttackSpeed.PercentBonus += 0.05f + (ownerSpell.CastInfo.SpellLevel * 0.25f);
            unit.AddStatModifier(StatsModifier);
            // TODO: add immunity to slows
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            RemoveParticle(p);
            RemoveParticle(p2);
        }

        private void OnAutoAttack(IAttackableUnit target, bool isCrit)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
