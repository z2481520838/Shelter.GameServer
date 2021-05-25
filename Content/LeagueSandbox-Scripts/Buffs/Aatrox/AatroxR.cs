using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace AatroxR
{
    class AatroxR : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        string pAuraName;
        string pModelName;
        IParticle pModel;
        IParticle pAura;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            if (unit is IChampion owner)
            {
                // TODO: Implement Animation Overrides for spells like these
                switch (owner.Skin)
                {
                    case 1:
                        pModelName = "Aatrox_Skin01_RModel.troy";
                        pAuraName = "Aatrox_Skin01_R_Aura_Self.troy";
                        break;
                    case 2:
                        pModelName = "Aatrox_Skin02_RModel.troy";
                        pAuraName = "Aatrox_Skin02_R_Aura_Self.troy";
                        break;
                    default:
                        pModelName = "Aatrox_Base_RModel.troy";
                        pAuraName = "Aatrox_Base_R_Aura_Self.troy";
                        break;
                }
                pModel = AddParticle(owner, unit, pModelName, unit.Position, lifetime: buff.Duration);
                pAura = AddParticleTarget(owner, unit, pAuraName, unit, lifetime: buff.Duration);

                StatsModifier.AttackSpeed.PercentBonus = (0.4f + (0.1f * (ownerSpell.CastInfo.SpellLevel - 1)));
                StatsModifier.Range.FlatBonus = 175f;
                unit.AddStatModifier(StatsModifier);
            }
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            RemoveParticle(pModel);
            RemoveParticle(pAura);
        }

        public void OnUpdate(float diff)
        {

        }
    }
}
