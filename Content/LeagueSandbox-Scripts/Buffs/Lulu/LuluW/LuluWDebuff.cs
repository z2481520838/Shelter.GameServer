using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;



namespace LuluWTwo
{
    internal class LuluWTwo : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_DEHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        string buffer;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            var owner = ownerSpell.CastInfo.Owner as IChampion;
            buffer = unit.Model;

            switch (owner.Skin)
            {
                case 0:
                    unit.ChangeModel("LuluSquill");
                    break;
                case 1:
                    unit.ChangeModel("LuluCupcake");
                    break;
                case 2:
                    unit.ChangeModel("LuluKitty");
                    break;
                case 3:
                    unit.ChangeModel("LuluDragon");
                    break;
                case 4:
                    unit.ChangeModel("LuluSnowman");
                    break;
            }

            StatsModifier.MoveSpeed.BaseBonus -= 60f;
            unit.AddStatModifier(StatsModifier);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            unit.ChangeModel(buffer);
            AddParticleTarget(unit, "Lulu_W_polymorph_01.troy", unit, 1, lifetime: 1f);
        }

        public void OnUpdate(float diff)
        {

        }
    }
}
