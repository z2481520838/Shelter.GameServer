using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Scripting.CSharp;


namespace Spells
{
    public class FioraQ : ISpellScript
    {
        IAttackableUnit Target;
        IObjAiBase Owner;
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            Target = target;
            Owner = owner;
        }

        public void OnSpellCast(ISpell spell)
        {
        }

        public void OnSpellPostCast(ISpell spell)
        {
            var current = new Vector2(spell.CastInfo.Owner.Position.X, spell.CastInfo.Owner.Position.Y);
            var to = Vector2.Normalize(new Vector2(Target.Position.X, Target.Position.Y) - current);
            var range = to * 800;

            var trueCoords = current + range;

            //TODO: Dash to the correct location (in front of the enemy IChampion) instead of far behind or inside them
            ForceMovement(spell.CastInfo.Owner, "Spell1", Target.Position, 1200, 0, 0, 0);
            //ForceMovement(spell.CastInfo.Owner, "Spell4", trueCoords, 2200, 0, 0, 0);
            AddParticleTarget(spell.CastInfo.Owner, "FioraQLunge_tar.troy", Target, 1, "");
            var bonusAd = (Owner.Stats.AttackDamage.Total - Owner.Stats.AttackDamage.BaseValue) * 0.6f;
            var damage = 40 + ((spell.CastInfo.SpellLevel - 1) * 25) + bonusAd;
            Target.TakeDamage(Owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
        }

        public void ApplyEffects(IObjAiBase owner, IAttackableUnit target, ISpell spell, ISpellMissile missile)
        {

        }

        public void OnSpellChannel(ISpell spell)
        {
        }

        public void OnSpellChannelCancel(ISpell spell)
        {
        }

        public void OnSpellPostChannel(ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
