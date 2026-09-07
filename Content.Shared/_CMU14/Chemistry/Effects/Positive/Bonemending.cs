/// THIS FILE IS LICENSED UNDER THE MIT LICENSE ///
using Content.Shared._CMU14.Medical.Anatomy.Bones;
using Content.Shared._RMC14.Chemistry.Effects;
<<<<<<< HEAD
using Content.Shared._CMU14.Medical.Injuries.Pain;
using Content.Shared.EntityEffects;
using Content.Shared.StatusEffectNew;
=======
using Content.Shared.Damage;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
>>>>>>> cmu/master
using Robust.Shared.Prototypes;

namespace Content.Shared._CMU14.Chemistry.Effects.Positive;

public sealed partial class Bonemending : RMCChemicalEffect
{
    protected override void Tick(Content.Shared.Damage.DamageableSystem damageable, Content.Shared.FixedPoint.FixedPoint2 potency, EntityEffectReagentArgs args)
    {
        var status = args.EntityManager.System<SharedStatusEffectsSystem>();
        if (!status.TryAddStatusEffectDuration(
                args.TargetEntity,
                "StatusEffectCMUBoneRegenBoost",
                out var effect,
                TimeSpan.FromSeconds(10)))
        {
            return;
        }

        var boost = args.EntityManager.EnsureComponent<BoneRegenBoostComponent>(effect.Value);
        var multiplier = MathF.Max(1f, 1f + PotencyPerSecond * 0.5f);
        if (boost.Multiplier < multiplier)
        {
            boost.Multiplier = multiplier;
            args.EntityManager.Dirty(effect.Value, boost);
        }
    }

    protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
<<<<<<< HEAD
    {
        return Loc.GetString("reagent-effect-guidebook-cmu-bonemending"); // RuMC edit
    }
=======
        => $"Restores [color=green]{PotencyPerSecond * 4}[/color] bone integrity to splinted or cast non-shattered fractures.\n" +
           "Overdoses cause malunion in an existing fracture.\n" +
           "Critical overdoses worsen an existing fracture by one severity.";

    protected override void Tick(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
        => args.EntityManager.System<SharedBoneSystem>()
            .ChemicallyMendFractures(args.TargetEntity, potency * 4f);

    protected override void TickOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
        => args.EntityManager.System<SharedBoneSystem>().ApplyChemicalMalunion(args.TargetEntity);

    protected override void TickCriticalOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
        => args.EntityManager.System<SharedBoneSystem>().WorsenChemicalFracture(args.TargetEntity);
>>>>>>> cmu/master
}
