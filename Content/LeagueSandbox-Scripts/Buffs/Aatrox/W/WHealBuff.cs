using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace AatroxWLife
{
    class AatroxWLife : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IBuff Buff;
        IObjAiBase owner;
        ISpell spell;
        IAttackableUnit Unit;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            Buff = buff;
            owner = ownerSpell.CastInfo.Owner;
            spell = ownerSpell;
            Unit = unit;
            if (unit is IObjAiBase ai)
            {
                ApiEventManager.OnLaunchAttack.AddListener(this, ai, OnLaunchAttack, false);
            }
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            if (buff.TimeElapsed >= buff.Duration)
            {
                ApiEventManager.OnLaunchAttack.RemoveListener(this, unit as IObjAiBase);
            }
        }

        public void OnLaunchAttack(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;

            /*var counter = owner.GetBuffWithName("AatroxWONHBuff");
            if (counter.StackCount >= 3)
            {
                SpellCast(owner, 3, SpellSlotType.ExtraSlots, false, spell.CastInfo.Owner.TargetUnit, Vector2.Zero);
                Buff.DeactivateBuff();
                counter.DeactivateBuff();
            }*/
            AddBuff("AatroxWONHBuff", float.MaxValue, 1, spell, owner, owner, true);
        }

        public void OnUpdate(float diff)
        {
            if (owner != null && spell != null && Unit != null)
            {
                var buff = Unit.GetBuffWithName("AatroxWONHBuff");
                if (buff != null && buff.StackCount == buff.MaxStacks && !Unit.HasBuff("AatroxWONHLifeBuff"))
                {
                    //RemoveBuff(Unit, "AatroxWPower");
                    //RemoveBuff(Unit, "AatroxWONHPowerBuff");
                    AddBuff("AatroxWONHLifeBuff", float.MaxValue, 1, spell, Unit, owner, false);
                    LogInfo("testeHeal");
                }
            }
        }
    }
}
