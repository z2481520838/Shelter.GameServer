using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;


namespace Buffs
{
    internal class NetherBlade : IBuffGameScript
    {
        public BuffType BuffType => BuffType.SLOW;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IBuff thisBuff;
        IObjAiBase Unit;
        IParticle p;
        IParticle p2;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            thisBuff = buff;
            if(unit is IObjAiBase ai)
            {
                Unit = ai;

                ApiEventManager.OnHitUnit.AddListener(this, ai, TargetExecute, true);
                p = AddParticleTarget(ownerSpell.CastInfo.Owner, null, "Kassadin_Base_W_buf.troy", unit, buff.Duration, 1, "R_hand", "R_hand");
                p2 = AddParticleTarget(ownerSpell.CastInfo.Owner, null, "Kassadin_Netherblade.troy", unit, buff.Duration, 1, "R_hand", "R_hand");
                ai.SkipNextAutoAttack();
            }

            StatsModifier.Range.FlatBonus += 50;
            unit.AddStatModifier(StatsModifier);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            //ApiEventManager.OnHitUnit.RemoveListener(this);

            RemoveParticle(p);
            RemoveParticle(p2);
        }
        public void TargetExecute(IAttackableUnit target, bool Iscrit)
        {
            if (!thisBuff.Elapsed() && thisBuff != null && Unit != null)
            {
                float ap = Unit.Stats.AbilityPower.Total * 0.6f;
                float damage = 15 + 25 * Unit.GetSpell(1).CastInfo.SpellLevel + ap;
                float manaHeal = (Unit.Stats.ManaPoints.Total - Unit.Stats.CurrentMana) * 0.03f + 0.01f * Unit.GetSpell(1).CastInfo.SpellLevel;
                if (target is IChampion)
                {
                    manaHeal *= 5;
                }
                target.TakeDamage(Unit, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
                Unit.Stats.CurrentMana += manaHeal;
                thisBuff.DeactivateBuff();
            }
        }
        public void OnUpdate(float diff)
        {

        }
    }
}

