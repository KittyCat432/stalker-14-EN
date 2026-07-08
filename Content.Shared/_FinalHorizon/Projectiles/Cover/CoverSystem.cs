using Robust.Shared.Random;
using Robust.Shared.Timing;
using System.Numerics;

namespace Content.Shared._FinalHorizon.Projectiles.Cover;

public sealed partial class CoverSystem : EntitySystem
{
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    private readonly RobustRandom _rand = new();
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CoverComponent, CoverHitAttemptEvent>(OnCoverHitAttempt);
    }

    private void OnCoverHitAttempt(EntityUid target, CoverComponent comp, ref CoverHitAttemptEvent args)
    {
        if (args.ProjectileComp.Shooter != null)
        {
            var targetPos = _transform.GetWorldPosition(target);
            var shooterPos = _transform.GetWorldPosition(args.ProjectileComp.Shooter.Value);
            var distance = Vector2.Distance(targetPos, shooterPos);

            if (distance < comp.CloseMissDistance)
            {
                args.Missed = true;
                return;
            }
        }

        _rand.SetSeed(args.Projectile.Id + (int)_timing.CurTick.Value);
        var roll = _rand.NextFloat();
        if (roll > comp.HitChance)
        {
            args.Missed = true;
        }
    }
};
