using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace AatroxWONHLifeBuff
{
    class AatroxWONHLifeBuff : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IParticle pbuff;
        IBuff Buff;
        IAttackableUnit Unit;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            Buff = buff;
            Unit = unit;
            //if (unit is IObjAiBase ai)
            //{
                //ApiEventManager.OnPreAttack.AddListener(this, ai, OnPreAttack, false);
            //}
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            //if (buff.TimeElapsed >= buff.Duration)
            //{
                //ApiEventManager.OnPreAttack.RemoveListener(this, unit as IObjAiBase);
            //}
        }

        public void OnPreAttack(ISpell spell)
        {
            //var owner = spell.CastInfo.Owner;

            //var counter = owner.GetBuffWithName("AatroxWONHBuff");
            //if (counter.StackCount >= 3)
            //{
            //    SpellCast(owner, 3, SpellSlotType.ExtraSlots, false, spell.CastInfo.Owner.TargetUnit, Vector2.Zero);
            //    Buff.DeactivateBuff();
            //    counter.DeactivateBuff();
            //}
            //AddBuff("AatroxWONHBuff", float.MaxValue, 1, spell, owner, owner, true);
        }

        public void OnUpdate(float diff)
        {
            //if (Unit != null && Unit.HasBuff("AatroxWONHPowerBuff"))
            //{
            //   RemoveBuff(Unit, "AatroxWPower");
            //    RemoveBuff(Unit, "AatroxWONHPowerBuff");
            //}
        }
    }
}
