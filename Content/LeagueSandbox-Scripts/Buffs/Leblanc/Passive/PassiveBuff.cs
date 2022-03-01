using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using LeagueSandbox.GameServer.GameObjects.Stats;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using System.Numerics;
using GameServerCore;
using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain;

namespace Buffs
{
    internal class LeblancPassive : IBuffGameScript
    {
        public IBuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.COMBAT_ENCHANCER,
            BuffAddType = BuffAddType.REPLACE_EXISTING
        };

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IMinion Leblanc;
        ISpell Spell;
		private IBuff buff;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
			Spell = ownerSpell;
			if (ownerSpell.CastInfo.Owner is IChampion owner)
            {
			ApiEventManager.OnTakeDamage.AddListener(this, owner, OnTakeDamage, false);
			}
            
        }
		public void OnTakeDamage(IDamageData damageData)       
        {
            if (Spell.CastInfo.Owner is IChampion owner)
            {
            var currentHealth = owner.Stats.CurrentHealth;
			var limitHealth = owner.Stats.HealthPoints.Total * 0.4;
			if (limitHealth >= currentHealth)
			{
				if (owner.HasBuff("LeblancPassive"))
                {
				owner.RemoveBuffsWithName("LeblancPassive");
                }
			}
			}
        }
        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
		   if (Spell.CastInfo.Owner is IChampion owner)
           {
		   var ownerSkinID = owner.SkinID;
		   var Cursor = new Vector2(ownerSpell.CastInfo.TargetPosition.X, ownerSpell.CastInfo.TargetPosition.Z);
		   var current = new Vector2(owner.Position.X, owner.Position.Y);
            var distance = Cursor - current;
            Vector2 truecoords;
            if (distance.Length() > 25000f)
            {
                distance = Vector2.Normalize(distance);
                var range = distance * 25000f;
                truecoords = current + range;
            }
            else
            {
                truecoords = Cursor;
            }
		   AddParticleTarget(owner, owner, "LeBlanc_Base_P_poof", owner,10f);
		   AddParticleTarget(owner, owner, "", owner, buff.Duration);
		   IMinion Leblanc = AddMinion(owner, "Leblanc", "Leblanc", owner.Position, owner.Team, owner.SkinID, false, true);
		   AddBuff("", 25000f, 1, ownerSpell, Leblanc, owner,false);
		   AddBuff("", 1f, 1, ownerSpell, owner, owner,false);
		   AddBuff("", 1f, 1, ownerSpell, Leblanc, owner,false);
		   CreateTimer(0.5f, () =>
                     {
		   ForceMovement(Leblanc, "RUN", Leblanc.Owner.Position, 400, 0, 0, 0);	
                     });
           CreateTimer(1f, () =>
                     {
		   ForceMovement(Leblanc, "RUN", Leblanc.Owner.Position, 400, 0, 0, 0);	
                     });
           CreateTimer(1.5f, () =>
                     {
		   ForceMovement(Leblanc, "RUN", Leblanc.Owner.Position, 400, 0, 0, 0);	
                     });
           CreateTimer(2.5f, () =>
                     {
		   ForceMovement(Leblanc, "RUN", Leblanc.Owner.Position, 400, 0, 0, 0);	
                     });						 
		   AddParticleTarget(owner, Leblanc, "LeBlanc_Base_P_poof", owner);
		   AddParticleTarget(owner, Leblanc, "LeBlanc_Base_P_image", owner, buff.Duration);
		   CreateTimer(8f, () =>
                     {
						Leblanc.TakeDamage(Leblanc.Owner, 10000f, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, DamageResultType.RESULT_NORMAL);
						AddParticleTarget(owner, Leblanc, "LeBlanc_Base_P_imageDeath", owner, buff.Duration);
                     });
	        if (Leblanc.IsDead)
            {  
		                AddParticle(owner, null, "LeBlanc_Base_P_imageDeath.troy", Leblanc.Position);
                        AddParticleTarget(owner, Leblanc, "LeBlanc_Base_P_imageDeath", owner, buff.Duration);		
            }
		   AddParticleTarget(owner, Leblanc, "", owner, 25000f);
           AddBuff("LeblancPassiveCooldown", 3f, 1, ownerSpell, owner, owner,false); 
		   }
        }
        public void OnUpdate(float diff)
        {
        }
    }
}