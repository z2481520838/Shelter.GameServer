using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System;
using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;

namespace Spells
{
    public class JavelinToss : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Circle
            },
            TriggersSpellCasts = true,
            IsDamagingSpell = true

            // TODO
        };

        public Vector2 castcoords;
        float finaldamage;

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellMissileHit.AddListener(this, new System.Collections.Generic.KeyValuePair<ISpell, IObjAiBase>(spell, owner), TargetExecute, false);

        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(ISpell spell)
        {
            castcoords = spell.CastInfo.Owner.Position;
        }

        public void OnSpellPostCast(ISpell spell)
        {
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile)
        {
            var owner = spell.CastInfo.Owner as IChampion;
            var APratio = owner.Stats.AbilityPower.Total * 0.4f;
            var basedamage = 50 + 25 * (spell.CastInfo.SpellLevel - 1) + APratio;
            var hitcoords = new Vector2(missile.Position.X, missile.Position.Y);
            var distance = Math.Sqrt(Math.Pow(castcoords.X - hitcoords.X, 2) + Math.Pow(castcoords.Y - hitcoords.Y, 2));

            if (Math.Abs(distance) <= 525f)
            {
                finaldamage = basedamage;
            }
            else if (distance > 525f && !(distance >= 1300f))
            {
                var damagerampup = (basedamage * (0.02f * (float)Math.Round(Math.Abs(distance - 525f) / 7.75f)));
                if(damagerampup >= basedamage)
                {
                    damagerampup = basedamage;
                }
                finaldamage = basedamage + damagerampup;
            }
            else if (distance >= 1300f)
            {
                finaldamage = basedamage * 3f;
            }

            target.TakeDamage(owner, finaldamage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            
            
            if (!target.IsDead)
            {
                AddParticleTarget(target, "Nidalee_Base_Q_Tar.troy", target, 1, "chest", lifetime: 1f);
                //TODO: Fix particles not working angainst minions for some reason
            }

            missile.SetToRemove();
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

