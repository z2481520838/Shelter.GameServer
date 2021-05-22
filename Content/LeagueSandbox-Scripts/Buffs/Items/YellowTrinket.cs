using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;


namespace YellowTriket
{
    internal class YellowTriket : IBuffGameScript
    {
        public BuffType BuffType => BuffType.INTERNAL;
        public BuffAddType BuffAddType => BuffAddType.STACKS_AND_CONTINUE;
        public int MaxStacks => 1;
        public bool IsHidden => true;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IAttackableUnit Unit;
        float timeSinceLastTick = 0f;
        float counter;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            Unit = unit;
            Unit.Stats.ManaRegeneration.PercentBonus = -1;
            Unit.Stats.CurrentMana = 60f;
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
        }

        public void OnUpdate(float diff)
        {
            timeSinceLastTick += diff;

            if(timeSinceLastTick >= 60000.0f)
            {
                Unit.Die(Unit);
            }

            //This would be used if the ward's ManaPoints were being properly read
            /*if (timeSinceLastTick >= 1000.0f)
            {
                Unit.Stats.ManaPoints.FlatBonus -= 1;
                if(Unit.Stats.CurrentMana == 0)
                {
                  Unit.Die(Unit);

                }
            }*/

        }
    }
}
