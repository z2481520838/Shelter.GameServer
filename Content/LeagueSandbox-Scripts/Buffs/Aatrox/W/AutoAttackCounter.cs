using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace AatroxWONHBuff
{
    class AatroxWONHBuff : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.STACKS_AND_RENEWS;
        public int MaxStacks => 3;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IBuff Buff;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            Buff = buff;
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
            //AddBuff("AatroxWONHBuff", float.MaxValue, 1, spell, spell.CastInfo.Owner, spell.CastInfo.Owner, true);
            //LogInfo("aa " + Buff.StackCount);
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
