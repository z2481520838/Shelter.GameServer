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
        IChampion owner;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            owner = ownerSpell.CastInfo.Owner as IChampion;
            buffer = unit.Model;
            
            string model = "";
            switch (owner.Skin)
            {
                case 0:
                    model = "LuluSquill";
                    break;
                case 1:
                    model =  "LuluCupcake";
                    break;
                case 2:
                    model = "LuluKitty";
                    break;
                case 3:
                    model = "LuluDragon";
                    break;
                case 4:
                    model = ("LuluSnowman");
                    break;
            }
            unit.ChangeModel(model);

            StatsModifier.MoveSpeed.BaseBonus -= 60f;
            unit.AddStatModifier(StatsModifier);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            unit.ChangeModel(buffer);
            AddParticleTarget(owner, unit, "Lulu_W_polymorph_01.troy", unit, 1, 1f);
        }

        public void OnUpdate(float diff)
        {

        }
    }
}
