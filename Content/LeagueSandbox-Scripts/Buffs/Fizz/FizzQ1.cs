using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Buffs
{
    internal class FizzQ1 : IBuffGameScript
    {
        public IBuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
			BuffAddType = BuffAddType.REPLACE_EXISTING
        };

        public IStatsModifier StatsModifier { get; private set; }
        IObjAiBase Unit;
		private IBuff ThisBuff;
        private readonly IAttackableUnit target = Spells.FizzPiercingStrike._target;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
			ThisBuff = buff;
            var owner = ownerSpell.CastInfo.Owner;  
			var ad = owner.Stats.AbilityPower.Total * 0.35f;
			var Wdamage = 30 + 10 * owner.GetSpell(1).CastInfo.SpellLevel + ad;
            var damage = 10f + owner.GetSpell(0).CastInfo.SpellLevel * 10f + owner.Stats.AttackDamage.Total;
			var QWdamage = Wdamage + damage;
            AddParticleTarget(owner, owner, "Fizz_PiercingStrike.troy", owner, buff.Duration);
            AddParticleTarget(owner, target, "Fizz_PiercingStrike_tar.troy", target);
            var current = new Vector2(owner.Position.X, owner.Position.Y);
            var to = Vector2.Normalize(new Vector2(target.Position.X, target.Position.Y) - current);
            var range = to * 550;
            var trueCoords = current + range;
			buff.SetStatusEffect(StatusFlags.Ghosted, true);
            ForceMovement(unit, null, trueCoords, 1400, 0, 0, 0);
			if (owner.HasBuff("FizzSeastonePassive"))
			{
			    target.TakeDamage(unit, QWdamage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
				owner.RemoveBuffsWithName("FizzSeastonePassive");
			}
			else 
			{
				target.TakeDamage(unit, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
			}         
			if (owner.HasBuff("FizzSeastoneTrident"))
			{
			AddBuff("FizzSeastoneTridentActive", 3f, 1, ownerSpell, target, owner);
			}
			else 
			{
			}
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}