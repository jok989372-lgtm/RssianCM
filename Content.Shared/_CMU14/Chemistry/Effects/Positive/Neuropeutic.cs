/// THIS FILE IS LICENSED UNDER THE MIT LICENSE ///
using Content.Shared._CMU14.Medical.Anatomy.Organs.Brain;
using Content.Shared._RMC14.Chemistry.Effects;
<<<<<<< HEAD
using Content.Shared._CMU14.Medical.Anatomy.Organs;
using Content.Shared._CMU14.Medical.Anatomy.Organs.Brain;
using Content.Shared._CMU14.Medical.Core;
using Content.Shared.Damage;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
=======
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Content.Shared.Stunnable;
>>>>>>> cmu/master
using Robust.Shared.Prototypes;

namespace Content.Shared._CMU14.Chemistry.Effects.Positive;

public sealed partial class Neuropeutic : OrganPeuticEffect<CMUBrainComponent>
{
<<<<<<< HEAD
    protected override void Tick(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
    {
        var medicalIndex = args.EntityManager.System<CMUMedicalBodyIndexSystem>();
        var organHealth = args.EntityManager.System<SharedOrganHealthSystem>();
        foreach (var organ in medicalIndex.GetOrgans(args.TargetEntity))
        {
            if (!args.EntityManager.HasComponent<CMUBrainComponent>(organ.Owner))
                continue;

            organHealth.HealOrgan((organ.Owner, null), args.TargetEntity, potency * 2f);
        }
    }
=======
    protected override string OrganName => "brain";
    protected override ProtoId<DamageTypePrototype> OrganDamageType => "Shock";
    protected override string PlantEffect => "Forces species mutation in plants.";
>>>>>>> cmu/master

    protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        => base.ReagentEffectGuidebookText(prototype, entSys) +
           $" Critical overdoses additionally stun for [color=red]{PotencyPerSecond * 2}[/color] seconds per tick.";

    protected override void TickCriticalOverdose(DamageableSystem damageable, FixedPoint2 potency,
        EntityEffectReagentArgs args)
    {
<<<<<<< HEAD
        return Loc.GetString("reagent-effect-guidebook-cmu-neuropeutic"); // RuMC edit
=======
        base.TickCriticalOverdose(damageable, potency, args);
        args.EntityManager.System<SharedStunSystem>().TryStun(
            args.TargetEntity,
            TimeSpan.FromSeconds((float)potency * 2f),
            true);
    }

    protected override void TickHydroTray(DamageableSystem damageable, FixedPoint2 potency, EntityEffectHydroArgs args)
    {
        var ev = new HydroTickEvent<Neuropeutic>(potency, args);
        args.EntityManager.EventBus.RaiseEvent(EventSource.Local, ev);
>>>>>>> cmu/master
    }
}
