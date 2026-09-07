using System.Numerics;
using Content.Server.Power.Components;
using Content.Server.SurveillanceCamera;
using Content.Shared.Camera;
using Content.Shared._RMC14.Camera;
<<<<<<< HEAD
using Content.Shared.Power;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Player;
=======
using Content.Shared.GameTicking;
using Content.Shared.Power;
using Robust.Shared.Map;
>>>>>>> cmu/master
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System.Linq;

namespace Content.Server.Camera;

public sealed class CameraNetworkSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
<<<<<<< HEAD
    [Dependency] private readonly ViewSubscriberSystem _viewSubscriber = default!;

    private readonly Dictionary<ProtoId<CameraNetworkPrototype>, HashSet<EntityUid>> _members = [];
    private readonly Dictionary<ProtoId<CameraNetworkPrototype>, HashSet<EntityUid>> _receivers = [];
    private readonly Dictionary<EntityUid,
        Dictionary<ProtoId<CameraNetworkPrototype>, HashSet<EntityUid>>> _runtimeGrants = [];
    private readonly Dictionary<EntityUid, HashSet<(EntityUid Receiver, ProtoId<CameraNetworkPrototype> Network)>>
        _grantsBySource = [];
    private readonly Dictionary<EntityUid, EntityUid> _pendingMarkerReceivers = [];
    private readonly Dictionary<EntityUid, TimeSpan> _mobileMarkerUpdates = [];
    private readonly HashSet<EntityUid> _legacyMembers = [];
    private readonly HashSet<EntityUid> _legacyReceivers = [];
    private readonly Dictionary<(EntityUid Receiver, EntityUid Viewer), MapViewSubscription> _mapViewSubscriptions = [];
    private readonly Dictionary<(EntityUid Grid, ICommonSession Session), MapGridViewSubscription>
        _mapGridViewSubscriptions = [];
=======

    private readonly Dictionary<EntityUid, HashSet<EntityUid>> _members = [];
    private readonly Dictionary<EntityUid, HashSet<EntityUid>> _receivers = [];
    private readonly Dictionary<EntityUid,
        Dictionary<EntityUid, HashSet<EntityUid>>> _runtimeGrants = [];
    private readonly Dictionary<EntityUid, HashSet<(EntityUid Receiver, EntityUid Network)>>
        _grantsBySource = [];
    private readonly Dictionary<ProtoId<CameraNetworkPrototype>, EntityUid> _seedNetworks = [];
    private readonly Dictionary<EntityUid, PendingMarkerChange> _pendingMarkerReceivers = [];
    private readonly Dictionary<EntityUid, PendingMobileMarkerChange> _mobileMarkerUpdates = [];
    private bool _seedNetworksInitialized;

    public ulong MarkerRevision { get; private set; }
>>>>>>> cmu/master

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CameraNetworkMemberComponent, ComponentStartup>(OnMemberStartup);
        SubscribeLocalEvent<CameraNetworkMemberComponent, ComponentShutdown>(OnMemberShutdown);
<<<<<<< HEAD
        SubscribeLocalEvent<CameraNetworkReceiverComponent, ComponentStartup>(OnReceiverStartup);
        SubscribeLocalEvent<CameraNetworkReceiverComponent, ComponentShutdown>(OnReceiverShutdown);
        SubscribeLocalEvent<CameraNetworkReceiverComponent, CameraNetworkGrantRequestEvent>(OnGrantRequest);
        SubscribeLocalEvent<RMCCameraComponent, RMCLegacyCameraMapInitEvent>(OnLegacyCameraMapInit);
        SubscribeLocalEvent<RMCCameraComputerComponent, RMCLegacyCameraComputerMapInitEvent>(OnLegacyComputerMapInit);
        SubscribeLocalEvent<RMCCameraComponent, RMCLegacyCameraIdChangedEvent>(OnLegacyCameraIdChanged);
        SubscribeLocalEvent<CameraMapMarkerComponent, ComponentStartup>(OnMarkerStartup);
        SubscribeLocalEvent<CameraMapMarkerComponent, ComponentShutdown>(OnMarkerShutdown);
        SubscribeLocalEvent<CameraMapMarkerComponent, MoveEvent>(OnMarkerMove);
        SubscribeLocalEvent<CameraMapMarkerComponent, EntityRenamedEvent>(OnMarkerRenamed);
        SubscribeLocalEvent<CameraMapMarkerComponent, PowerChangedEvent>(OnMarkerPowerChanged);
        SubscribeLocalEvent<CameraMapMarkerComponent, EntityPausedEvent>(OnMarkerPaused);
        SubscribeLocalEvent<CameraMapMarkerComponent, EntityUnpausedEvent>(OnMarkerUnpaused);
        SubscribeLocalEvent<EntityTerminatingEvent>(OnEntityTerminating);
=======
        SubscribeLocalEvent<CameraNetworkMemberComponent, EntityRenamedEvent>(OnMemberRenamed);
        SubscribeLocalEvent<CameraNetworkMemberComponent, PowerChangedEvent>(OnMemberPowerChanged);
        SubscribeLocalEvent<CameraNetworkMemberComponent, EntityPausedEvent>(OnMemberPaused);
        SubscribeLocalEvent<CameraNetworkMemberComponent, EntityUnpausedEvent>(OnMemberUnpaused);
        SubscribeLocalEvent<CameraNetworkReceiverComponent, ComponentStartup>(OnReceiverStartup);
        SubscribeLocalEvent<CameraNetworkReceiverComponent, ComponentShutdown>(OnReceiverShutdown);
        SubscribeLocalEvent<CameraNetworkReceiverComponent, CameraNetworkGrantRequestEvent>(OnGrantRequest);
        SubscribeLocalEvent<CameraNetworkIdentityComponent, ComponentShutdown>(OnNetworkShutdown);
        SubscribeLocalEvent<CameraMapMarkerComponent, ComponentStartup>(OnMarkerStartup);
        SubscribeLocalEvent<CameraMapMarkerComponent, ComponentShutdown>(OnMarkerShutdown);
        SubscribeLocalEvent<CameraMapMarkerComponent, MoveEvent>(OnMarkerMove);
        SubscribeLocalEvent<EntityTerminatingEvent>(OnEntityTerminating);
        SubscribeLocalEvent<PrototypesReloadedEventArgs>(OnPrototypesReloaded);
        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundRestart);
    }

    private void OnPrototypesReloaded(PrototypesReloadedEventArgs args)
    {
        if (args.WasModified<CameraNetworkPrototype>())
            EnsureSeedNetworks();
    }

    private void EnsureSeedNetworks()
    {
        foreach (var prototype in _prototypeManager.EnumeratePrototypes<CameraNetworkPrototype>())
        {
            ResolveNetwork(prototype.ID);
        }
>>>>>>> cmu/master
    }

    private void OnMemberStartup(Entity<CameraNetworkMemberComponent> ent, ref ComponentStartup args)
    {
<<<<<<< HEAD
        IndexMember(ent.Owner, ent.Comp.Networks);
        NotifyReceivers(ent.Comp.Networks, ent.Owner);
=======
        var networks = GetMemberNetworkEntities(ent.Comp);
        IndexMember(ent.Owner, networks);
        NotifyReceivers(networks, ent.Owner);
>>>>>>> cmu/master
    }

    private void OnMemberShutdown(Entity<CameraNetworkMemberComponent> ent, ref ComponentShutdown args)
    {
<<<<<<< HEAD
        UnindexMember(ent.Owner, ent.Comp.Networks);
        NotifyReceivers(ent.Comp.Networks, ent.Owner);
=======
        var networks = GetMemberNetworkEntities(ent.Comp);
        UnindexMember(ent.Owner, networks);
        NotifyReceivers(networks, ent.Owner);
    }

    private void OnMemberRenamed(Entity<CameraNetworkMemberComponent> ent, ref EntityRenamedEvent args)
    {
        QueueMemberDirectoryChange(ent);
    }

    private void OnMemberPowerChanged(Entity<CameraNetworkMemberComponent> ent, ref PowerChangedEvent args)
    {
        QueueMemberDirectoryChange(ent);
    }

    private void OnMemberPaused(Entity<CameraNetworkMemberComponent> ent, ref EntityPausedEvent args)
    {
        QueueMemberDirectoryChange(ent);
    }

    private void OnMemberUnpaused(Entity<CameraNetworkMemberComponent> ent, ref EntityUnpausedEvent args)
    {
        QueueMemberDirectoryChange(ent);
>>>>>>> cmu/master
    }

    private void OnReceiverStartup(Entity<CameraNetworkReceiverComponent> ent, ref ComponentStartup args)
    {
<<<<<<< HEAD
        IndexReceiver(ent.Owner, GetEffectiveNetworks(ent.Owner, ent.Comp));
=======
        IndexReceiver(ent.Owner, GetEffectiveNetworkEntities(ent.Owner, ent.Comp));
>>>>>>> cmu/master
    }

    private void OnReceiverShutdown(Entity<CameraNetworkReceiverComponent> ent, ref ComponentShutdown args)
    {
        _pendingMarkerReceivers.Remove(ent.Owner);
<<<<<<< HEAD
        ClearMapViewSubscriptions(ent.Owner);
        UnindexReceiver(ent.Owner, GetEffectiveNetworks(ent.Owner, ent.Comp));
        CleanupReceiverGrants(ent.Owner);
    }

=======
        UnindexReceiver(ent.Owner, GetEffectiveNetworkEntities(ent.Owner, ent.Comp));
        CleanupReceiverGrants(ent.Owner);
    }

    private void OnNetworkShutdown(Entity<CameraNetworkIdentityComponent> ent, ref ComponentShutdown args)
    {
        if (ent.Comp.Seed is { } seed && _seedNetworks.GetValueOrDefault(seed) == ent.Owner)
            _seedNetworks.Remove(seed);

        RemoveNetworkIdentity(ent.Owner);
    }

    private void OnRoundRestart(RoundRestartCleanupEvent args)
    {
        _members.Clear();
        _receivers.Clear();
        _runtimeGrants.Clear();
        _grantsBySource.Clear();
        // Seed identities are entities and remain valid until the round entity flush.
        // Keep this lookup intact until their shutdown events remove the entries so
        // other round-cleanup subscribers cannot manufacture duplicate identities.
        _pendingMarkerReceivers.Clear();
        _mobileMarkerUpdates.Clear();
        MarkerRevision = 0;
    }

>>>>>>> cmu/master
    private void OnGrantRequest(Entity<CameraNetworkReceiverComponent> ent, ref CameraNetworkGrantRequestEvent args)
    {
        if (args.Grant)
            GrantNetwork(ent.Owner, args.Network, args.Source);
        else
            RevokeNetwork(ent.Owner, args.Network, args.Source);
    }

<<<<<<< HEAD
    private void OnLegacyCameraMapInit(Entity<RMCCameraComponent> ent, ref RMCLegacyCameraMapInitEvent args)
    {
        if (HasComp<CameraNetworkMemberComponent>(ent))
            return;

        var member = EnsureComp<CameraNetworkMemberComponent>(ent);
        member.SourceKinds = CameraSourceKinds.Rmc;
        SetMemberNetworks(ent, ValidateLegacyNetworks(ent.Owner, ent.Comp.Id));
        _legacyMembers.Add(ent.Owner);
    }

    private void OnLegacyComputerMapInit(Entity<RMCCameraComputerComponent> ent, ref RMCLegacyCameraComputerMapInitEvent args)
    {
        if (HasComp<CameraNetworkReceiverComponent>(ent))
            return;

        var receiver = EnsureComp<CameraNetworkReceiverComponent>(ent);
        receiver.SupportedSources = CameraSourceKinds.Rmc;
        SetReceiverNetworks(ent, ValidateLegacyNetworks(ent.Owner, ent.Comp.ProtoIds));
        _legacyReceivers.Add(ent.Owner);
    }

    private void OnLegacyCameraIdChanged(Entity<RMCCameraComponent> ent, ref RMCLegacyCameraIdChangedEvent args)
    {
        if (!_legacyMembers.Contains(ent.Owner) || !TryComp(ent, out CameraNetworkMemberComponent? member))
            return;

        SetMemberNetworks(ent, ValidateLegacyNetworks(ent.Owner, args.NewId));
    }

    private HashSet<ProtoId<CameraNetworkPrototype>> ValidateLegacyNetworks(
        EntityUid entity,
        IEnumerable<EntProtoId> legacyIds)
    {
        var networks = new HashSet<ProtoId<CameraNetworkPrototype>>();
        foreach (var legacyId in legacyIds)
        {
            if (_prototypeManager.HasIndex<CameraNetworkPrototype>(legacyId))
            {
                networks.Add(new ProtoId<CameraNetworkPrototype>(legacyId.Id));
                continue;
            }

            Log.Warning($"RMC camera prototype '{MetaData(entity).EntityPrototype?.ID ?? "unknown"}' has invalid legacy camera network '{legacyId}'.");
        }

        return networks;
    }

    private HashSet<ProtoId<CameraNetworkPrototype>> ValidateLegacyNetworks(EntityUid entity, EntProtoId? legacyId)
    {
        return legacyId == null ? [] : ValidateLegacyNetworks(entity, [legacyId.Value]);
    }

=======
>>>>>>> cmu/master
    private void OnMarkerStartup(Entity<CameraMapMarkerComponent> ent, ref ComponentStartup args)
    {
        QueueMarkerChange(ent);
    }

    private void OnMarkerShutdown(Entity<CameraMapMarkerComponent> ent, ref ComponentShutdown args)
    {
        _mobileMarkerUpdates.Remove(ent.Owner);
<<<<<<< HEAD
        QueueMarkerChange(ent);
=======
        QueueAffectedReceivers(ent.Owner);
>>>>>>> cmu/master
    }

    private void OnMarkerMove(Entity<CameraMapMarkerComponent> ent, ref MoveEvent args)
    {
        QueueMarkerChange(ent);
    }

<<<<<<< HEAD
    private void OnMarkerRenamed(Entity<CameraMapMarkerComponent> ent, ref EntityRenamedEvent args)
    {
        QueueMarkerChange(ent);
    }

    private void OnMarkerPowerChanged(Entity<CameraMapMarkerComponent> ent, ref PowerChangedEvent args)
    {
        QueueMarkerChange(ent);
    }

    private void OnMarkerPaused(Entity<CameraMapMarkerComponent> ent, ref EntityPausedEvent args)
    {
        QueueMarkerChange(ent);
    }

    private void OnMarkerUnpaused(Entity<CameraMapMarkerComponent> ent, ref EntityUnpausedEvent args)
    {
        QueueMarkerChange(ent);
    }

=======
>>>>>>> cmu/master
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

<<<<<<< HEAD
        foreach (var (marker, due) in _mobileMarkerUpdates.ToArray())
        {
            if (due > _timing.CurTime)
                continue;

            _mobileMarkerUpdates.Remove(marker);
            QueueAffectedReceivers(marker);
=======
        if (!_seedNetworksInitialized)
        {
            EnsureSeedNetworks();
            _seedNetworksInitialized = true;
        }

        foreach (var (marker, pending) in _mobileMarkerUpdates.ToArray())
        {
            if (pending.Due > _timing.CurTime)
                continue;

            _mobileMarkerUpdates.Remove(marker);
            QueueAffectedReceivers(marker, pending.DirectoryChanged);
>>>>>>> cmu/master
        }

        var pendingMarkerReceivers = _pendingMarkerReceivers.ToArray();
        // Preserve marker changes queued synchronously by receiver event handlers for the next update.
        _pendingMarkerReceivers.Clear();

<<<<<<< HEAD
        foreach (var (receiver, marker) in pendingMarkerReceivers)
            NotifyReceiver(receiver, marker, CameraReceiverChangeKind.Marker);
=======
        foreach (var (receiver, pending) in pendingMarkerReceivers)
        {
            NotifyReceiver(
                receiver,
                pending.Marker,
                pending.DirectoryChanged
                    ? CameraReceiverChangeKind.Directory
                    : CameraReceiverChangeKind.Marker);
        }
>>>>>>> cmu/master
    }

    private void OnEntityTerminating(ref EntityTerminatingEvent args)
    {
        var entity = args.Entity.Owner;
<<<<<<< HEAD
        _legacyMembers.Remove(entity);
        _legacyReceivers.Remove(entity);
        _pendingMarkerReceivers.Remove(entity);
        ClearMapViewSubscriptions(entity);
        ClearMapViewSubscriptionsForViewer(entity);
=======
        _pendingMarkerReceivers.Remove(entity);
>>>>>>> cmu/master
        CleanupSourceGrants(entity);

        if (TryComp(entity, out CameraNetworkReceiverComponent? receiver))
        {
<<<<<<< HEAD
            UnindexReceiver(entity, GetEffectiveNetworks(entity, receiver));
=======
            UnindexReceiver(entity, GetEffectiveNetworkEntities(entity, receiver));
>>>>>>> cmu/master
            CleanupReceiverGrants(entity);
        }
    }

<<<<<<< HEAD
    public HashSet<EntityUid> GetAccessibleCameras(Entity<CameraNetworkReceiverComponent> receiver)
    {
        var cameras = new HashSet<EntityUid>();

        foreach (var network in GetEffectiveNetworks(receiver.Owner, receiver.Comp))
=======
    /// <summary>
    /// Resolves a static YAML seed to the single logical network entity for this round.
    /// </summary>
    public EntityUid ResolveNetwork(ProtoId<CameraNetworkPrototype> seed)
    {
        if (_seedNetworks.TryGetValue(seed, out var existing) && !TerminatingOrDeleted(existing))
            return existing;

        var prototype = _prototypeManager.Index<CameraNetworkPrototype>(seed);
        var network = Spawn(null, MapCoordinates.Nullspace);
        var identity = AddComp<CameraNetworkIdentityComponent>(network);
        identity.Seed = seed;
        identity.DisplayName = Loc.GetString(prototype.Name);
        identity.Runtime = false;
        _seedNetworks[seed] = network;
        return network;
    }

    public EntityUid CreateNetwork(string displayName, EntityUid? owner = null)
    {
        var network = Spawn(null, MapCoordinates.Nullspace);
        var identity = AddComp<CameraNetworkIdentityComponent>(network);
        identity.DisplayName = displayName.Trim();
        identity.CreatedBy = owner;
        identity.Runtime = true;
        return network;
    }

    public bool DeleteNetwork(EntityUid network)
    {
        if (!TryComp(network, out CameraNetworkIdentityComponent? identity) || !identity.Runtime)
            return false;

        QueueDel(network);
        return true;
    }

    public bool TryGetNetworkSeed(EntityUid network, out ProtoId<CameraNetworkPrototype> seed)
    {
        if (TryComp(network, out CameraNetworkIdentityComponent? identity) && identity.Seed is { } value)
        {
            seed = value;
            return true;
        }

        seed = default;
        return false;
    }

    public HashSet<EntityUid> GetEffectiveNetworkEntities(EntityUid receiver)
    {
        return TryComp(receiver, out CameraNetworkReceiverComponent? component)
            ? GetEffectiveNetworkEntities(receiver, component)
            : [];
    }

    private HashSet<EntityUid> GetMemberNetworkEntities(CameraNetworkMemberComponent component)
    {
        var networks = component.Networks.Select(ResolveNetwork).ToHashSet();
        networks.UnionWith(component.RuntimeNetworks.Where(Exists));
        return networks;
    }

    private HashSet<EntityUid> GetReceiverBaseNetworkEntities(CameraNetworkReceiverComponent component)
    {
        var networks = component.Networks.Select(ResolveNetwork).ToHashSet();
        networks.UnionWith(component.RuntimeNetworks.Where(Exists));
        return networks;
    }

    public HashSet<EntityUid> GetAccessibleCameras(Entity<CameraNetworkReceiverComponent> receiver)
    {
        var cameras = new HashSet<EntityUid>();
        var effectiveNetworks = GetEffectiveNetworkEntities(receiver.Owner, receiver.Comp);

        foreach (var network in effectiveNetworks)
>>>>>>> cmu/master
        {
            if (!_members.TryGetValue(network, out var members))
                continue;

            foreach (var member in members)
            {
<<<<<<< HEAD
                if (CanAccess(receiver.Owner, member))
                    cameras.Add(member);
=======
                if (!TryComp(member, out CameraNetworkMemberComponent? memberComponent) ||
                    (receiver.Comp.SupportedSources & memberComponent.SourceKinds) == CameraSourceKinds.None)
                {
                    continue;
                }

                cameras.Add(member);
>>>>>>> cmu/master
            }
        }

        return cameras;
    }

    public CameraMapUiState BuildMapState(EntityUid receiver)
    {
        EntityUid? consoleGrid = null;
        if (TryComp(receiver, out TransformComponent? receiverTransform))
            consoleGrid = receiverTransform.GridUid ?? receiverTransform.MapUid;

        if (!TryComp(receiver, out CameraNetworkReceiverComponent? receiverComponent))
            return new CameraMapUiState(GetNetEntity(consoleGrid), []);

        var grouped = new Dictionary<EntityUid, List<(EntityUid Camera, Vector2 Position, string Name, CameraMapMarkerStatus Status)>>();
        foreach (var camera in GetAccessibleCameras((receiver, receiverComponent)))
        {
            if (TerminatingOrDeleted(camera)
                || Paused(camera)
                || !TryComp(camera, out CameraMapMarkerComponent? marker)
                || !marker.Visible
                || !TryComp(camera, out TransformComponent? xform))
            {
                continue;
            }

            var grid = xform.GridUid ?? xform.MapUid;
            if (grid == null || TerminatingOrDeleted(grid.Value))
                continue;

            if (!grouped.TryGetValue(grid.Value, out var markers))
            {
                markers = [];
                grouped.Add(grid.Value, markers);
            }

            var worldPosition = _transform.GetWorldPosition(xform);
            var position = Vector2.Transform(worldPosition, _transform.GetInvWorldMatrix(grid.Value));
            var status = IsAvailable(camera)
                    ? CameraMapMarkerStatus.Active
                    : CameraMapMarkerStatus.Inactive;
<<<<<<< HEAD
            markers.Add((camera, position, Name(camera), status));
=======
            markers.Add((camera, position, GetCameraDisplayName(camera), status));
>>>>>>> cmu/master
        }

        var grids = grouped
            .OrderBy(pair => pair.Key != consoleGrid)
            .ThenBy(pair => Name(pair.Key), StringComparer.Ordinal)
            .ThenBy(pair => pair.Key.Id)
            .Select(pair => new CameraMapGridUiData(
                GetNetEntity(pair.Key),
                Name(pair.Key),
                pair.Value
                    .OrderBy(marker => marker.Name, StringComparer.Ordinal)
                    .ThenBy(marker => marker.Camera.Id)
                    .Select(marker => new CameraMapMarkerUiData(
                        GetNetEntity(marker.Camera), marker.Position, marker.Name, marker.Status))
                    .ToList()))
            .ToList();

        return new CameraMapUiState(GetNetEntity(consoleGrid), grids);
    }

<<<<<<< HEAD
    public void SyncMapViewSubscriptions(EntityUid receiver, EntityUid viewer, CameraMapUiState state)
    {
        if (!TryComp(viewer, out ActorComponent? actor))
        {
            ClearMapViewSubscriptions(receiver, viewer);
            return;
        }

        var key = (receiver, viewer);
        if (_mapViewSubscriptions.TryGetValue(key, out var existing) && existing.Session != actor.PlayerSession)
        {
            ReleaseMapViewSubscription(existing);
            existing = null;
        }

        var desired = new HashSet<EntityUid>();
        foreach (var gridData in state.Grids)
        {
            if (state.ConsoleGrid == gridData.Grid)
                continue;

            if (TryGetEntity(gridData.Grid, out var grid) && grid is { } gridUid && !TerminatingOrDeleted(gridUid))
                desired.Add(gridUid);
        }

        existing ??= new MapViewSubscription(actor.PlayerSession);

        foreach (var grid in existing.Grids.Except(desired).ToArray())
        {
            existing.Grids.Remove(grid);
            ReleaseMapGrid(grid, existing.Session);
        }

        foreach (var grid in desired.Except(existing.Grids))
        {
            existing.Grids.Add(grid);
            AcquireMapGrid(grid, existing.Session);
        }

        if (existing.Grids.Count == 0)
            _mapViewSubscriptions.Remove(key);
        else
            _mapViewSubscriptions[key] = existing;
    }

    public void ClearMapViewSubscriptions(EntityUid receiver, EntityUid viewer)
    {
        var key = (receiver, viewer);
        if (_mapViewSubscriptions.Remove(key, out var subscription))
            ReleaseMapViewSubscription(subscription);
    }

    public void ClearMapViewSubscriptions(EntityUid receiver)
    {
        foreach (var (key, subscription) in _mapViewSubscriptions.ToArray())
        {
            if (key.Receiver != receiver)
                continue;

            _mapViewSubscriptions.Remove(key);
            ReleaseMapViewSubscription(subscription);
        }
    }

    public void ClearMapViewSubscriptionsForViewer(EntityUid viewer)
    {
        foreach (var (key, subscription) in _mapViewSubscriptions.ToArray())
        {
            if (key.Viewer != viewer)
                continue;

            _mapViewSubscriptions.Remove(key);
            ReleaseMapViewSubscription(subscription);
        }
    }

    private void AcquireMapGrid(EntityUid grid, ICommonSession session)
    {
        var key = (grid, session);
        if (_mapGridViewSubscriptions.TryGetValue(key, out var subscription))
        {
            subscription.Count++;
            return;
        }

        // ViewSubscriberSystem stores a global set rather than source-aware
        // subscriptions. Use our own view entity so closing a camera UI can
        // never remove a subscription owned by another feature.
        var view = Spawn(null, new EntityCoordinates(grid, Vector2.Zero));
        _mapGridViewSubscriptions[key] = new MapGridViewSubscription(view);
        _viewSubscriber.AddViewSubscriber(view, session);
    }

    private void ReleaseMapGrid(EntityUid grid, ICommonSession session)
    {
        var key = (grid, session);
        if (!_mapGridViewSubscriptions.TryGetValue(key, out var subscription))
            return;

        if (subscription.Count > 1)
        {
            subscription.Count--;
            return;
        }

        _mapGridViewSubscriptions.Remove(key);
        _viewSubscriber.RemoveViewSubscriber(subscription.View, session);
        QueueDel(subscription.View);
    }

    private void ReleaseMapViewSubscription(MapViewSubscription subscription)
    {
        foreach (var grid in subscription.Grids)
            ReleaseMapGrid(grid, subscription.Session);
=======
    private string GetCameraDisplayName(EntityUid camera)
    {
        return TryComp(camera, out RMCCameraComponent? rmc)
            ? rmc.NameOverride ?? Name(camera)
            : Name(camera);
>>>>>>> cmu/master
    }

    public bool CanAccess(EntityUid receiver, EntityUid camera)
    {
        if (!TryComp(receiver, out CameraNetworkReceiverComponent? receiverComponent)
            || !TryComp(camera, out CameraNetworkMemberComponent? memberComponent)
            || (receiverComponent.SupportedSources & memberComponent.SourceKinds) == CameraSourceKinds.None)
        {
            return false;
        }

<<<<<<< HEAD
        foreach (var network in GetEffectiveNetworks(receiver, receiverComponent))
=======
        foreach (var network in memberComponent.Networks.Select(ResolveNetwork))
>>>>>>> cmu/master
        {
            // ComponentShutdown leaves the component readable while its lifecycle
            // callback runs. The index is therefore the authoritative indicator
            // that this source is still a live member of the logical network.
<<<<<<< HEAD
            if (memberComponent.Networks.Contains(network)
=======
            if (HasEffectiveNetwork(receiver, receiverComponent, network)
                && _members.TryGetValue(network, out var members)
                && members.Contains(camera))
                return true;
        }

        foreach (var network in memberComponent.RuntimeNetworks)
        {
            if (HasEffectiveNetwork(receiver, receiverComponent, network)
>>>>>>> cmu/master
                && _members.TryGetValue(network, out var members)
                && members.Contains(camera))
                return true;
        }

        return false;
    }

<<<<<<< HEAD
=======
    public bool IsMemberOfNetwork(EntityUid member, EntityUid network)
    {
        return _members.TryGetValue(network, out var members) && members.Contains(member);
    }

>>>>>>> cmu/master
    public bool SetMapVisibility(EntityUid camera, bool visible)
    {
        if (!TryComp(camera, out CameraMapMarkerComponent? marker)
            || marker.Visible == visible)
        {
            return false;
        }

        marker.Visible = visible;
        QueueMarkerChange((camera, marker));
        return true;
    }

    public bool IsMapVisible(EntityUid camera)
    {
        return TryComp(camera, out CameraMapMarkerComponent? marker) && marker.Visible;
    }

    public bool AddNetwork(EntityUid member, ProtoId<CameraNetworkPrototype> network)
    {
        if (!TryComp(member, out CameraNetworkMemberComponent? component)
            || !component.Networks.Add(network))
        {
            return false;
        }

<<<<<<< HEAD
        IndexMember(member, [network]);
        NotifyReceivers([network], member);
=======
        var identity = ResolveNetwork(network);
        IndexMember(member, [identity]);
        NotifyReceivers([identity], member);
>>>>>>> cmu/master
        return true;
    }

    public bool RemoveNetwork(EntityUid member, ProtoId<CameraNetworkPrototype> network)
    {
        if (!TryComp(member, out CameraNetworkMemberComponent? component)
            || !component.Networks.Remove(network))
        {
            return false;
        }

<<<<<<< HEAD
=======
        var identity = ResolveNetwork(network);
        UnindexMember(member, [identity]);
        NotifyReceivers([identity], member);
        return true;
    }

    public bool AddNetwork(EntityUid member, EntityUid network)
    {
        if (!HasComp<CameraNetworkIdentityComponent>(network)
            || !TryComp(member, out CameraNetworkMemberComponent? component)
            || !component.RuntimeNetworks.Add(network))
        {
            return false;
        }

        IndexMember(member, [network]);
        NotifyReceivers([network], member);
        return true;
    }

    public bool RemoveNetwork(EntityUid member, EntityUid network)
    {
        if (!TryComp(member, out CameraNetworkMemberComponent? component)
            || !component.RuntimeNetworks.Remove(network))
        {
            return false;
        }

>>>>>>> cmu/master
        UnindexMember(member, [network]);
        NotifyReceivers([network], member);
        return true;
    }

    public bool SetMemberNetworks(EntityUid member, IEnumerable<ProtoId<CameraNetworkPrototype>> networks)
    {
        return SetMemberNetworksBatch(
            new Dictionary<EntityUid, IReadOnlyCollection<ProtoId<CameraNetworkPrototype>>>
            {
                [member] = networks.ToHashSet(),
            });
    }

<<<<<<< HEAD
=======
    public bool SetMemberNetworkEntities(EntityUid member, IEnumerable<EntityUid> networks)
    {
        if (!TryComp(member, out CameraNetworkMemberComponent? component))
            return false;

        var identities = networks.ToHashSet();
        if (identities.Any(network => !HasComp<CameraNetworkIdentityComponent>(network)))
            return false;

        var seeded = new HashSet<ProtoId<CameraNetworkPrototype>>();
        var runtime = new HashSet<EntityUid>();
        foreach (var network in identities)
        {
            if (TryGetNetworkSeed(network, out var seed))
                seeded.Add(seed);
            else
                runtime.Add(network);
        }

        if (component.Networks.SetEquals(seeded) && component.RuntimeNetworks.SetEquals(runtime))
            return false;

        var affected = GetMemberNetworkEntities(component);
        affected.UnionWith(identities);
        UnindexMember(member, GetMemberNetworkEntities(component));
        component.Networks = seeded;
        component.RuntimeNetworks = runtime;
        IndexMember(member, identities);
        NotifyReceivers(affected, member);
        return true;
    }

>>>>>>> cmu/master
    public bool SetMemberNetworksBatch(
        IReadOnlyDictionary<EntityUid, IReadOnlyCollection<ProtoId<CameraNetworkPrototype>>> updates)
    {
        var changed = new List<(
            EntityUid Member,
            CameraNetworkMemberComponent Component,
            HashSet<ProtoId<CameraNetworkPrototype>> Updated)>();
<<<<<<< HEAD
        var affected = new HashSet<ProtoId<CameraNetworkPrototype>>();
=======
        var affected = new HashSet<EntityUid>();
>>>>>>> cmu/master

        foreach (var (member, networks) in updates)
        {
            if (!TryComp(member, out CameraNetworkMemberComponent? component))
                return false;

            var updated = networks.ToHashSet();
            if (component.Networks.SetEquals(updated))
                continue;

<<<<<<< HEAD
            affected.UnionWith(component.Networks);
            affected.UnionWith(updated);
=======
            affected.UnionWith(component.Networks.Select(ResolveNetwork));
            affected.UnionWith(updated.Select(ResolveNetwork));
>>>>>>> cmu/master
            changed.Add((member, component, updated));
        }

        if (changed.Count == 0)
            return false;

        foreach (var (member, component, _) in changed)
<<<<<<< HEAD
            UnindexMember(member, component.Networks);
=======
            UnindexMember(member, GetMemberNetworkEntities(component));
>>>>>>> cmu/master

        foreach (var (member, component, updated) in changed)
        {
            component.Networks = updated;
<<<<<<< HEAD
            IndexMember(member, component.Networks);
=======
            IndexMember(member, GetMemberNetworkEntities(component));
>>>>>>> cmu/master
        }

        NotifyReceivers(affected, changed.Count == 1 ? changed[0].Member : null);
        return true;
    }

    public IReadOnlyCollection<EntityUid> GetNetworkMembers(ProtoId<CameraNetworkPrototype> network)
    {
<<<<<<< HEAD
=======
        return _members.TryGetValue(ResolveNetwork(network), out var members)
            ? members.ToArray()
            : Array.Empty<EntityUid>();
    }

    public IReadOnlyCollection<EntityUid> GetNetworkMembers(EntityUid network)
    {
>>>>>>> cmu/master
        return _members.TryGetValue(network, out var members)
            ? members.ToArray()
            : Array.Empty<EntityUid>();
    }

    public bool SetReceiverNetworks(EntityUid receiver, IEnumerable<ProtoId<CameraNetworkPrototype>> networks)
    {
        if (!TryComp(receiver, out CameraNetworkReceiverComponent? component))
            return false;

        var updated = networks.ToHashSet();
        if (component.Networks.SetEquals(updated))
            return false;

<<<<<<< HEAD
        var oldEffective = GetEffectiveNetworks(receiver, component);
        component.Networks = updated;
        var newEffective = GetEffectiveNetworks(receiver, component);
=======
        var oldEffective = GetEffectiveNetworkEntities(receiver, component);
        component.Networks = updated;
        var newEffective = GetEffectiveNetworkEntities(receiver, component);
>>>>>>> cmu/master
        UpdateReceiverIndex(receiver, oldEffective, newEffective);
        if (!oldEffective.SetEquals(newEffective))
            NotifyAuthorizationChanged(receiver);
        return true;
    }

<<<<<<< HEAD
    public HashSet<ProtoId<CameraNetworkPrototype>> GetEffectiveNetworks(EntityUid receiver)
    {
        return TryComp(receiver, out CameraNetworkReceiverComponent? component)
            ? GetEffectiveNetworks(receiver, component)
            : [];
    }

    public bool GrantNetwork(EntityUid receiver, ProtoId<CameraNetworkPrototype> network, EntityUid source)
=======
    public bool SetReceiverNetworkEntities(EntityUid receiver, IEnumerable<EntityUid> networks)
>>>>>>> cmu/master
    {
        if (!TryComp(receiver, out CameraNetworkReceiverComponent? component))
            return false;

<<<<<<< HEAD
        var effectiveNetworks = GetEffectiveNetworks(receiver, component);
=======
        var identities = networks.ToHashSet();
        if (identities.Any(network => !HasComp<CameraNetworkIdentityComponent>(network)))
            return false;

        var seeded = new HashSet<ProtoId<CameraNetworkPrototype>>();
        var runtime = new HashSet<EntityUid>();
        foreach (var network in identities)
        {
            if (TryGetNetworkSeed(network, out var seed))
                seeded.Add(seed);
            else
                runtime.Add(network);
        }

        if (component.Networks.SetEquals(seeded) && component.RuntimeNetworks.SetEquals(runtime))
            return false;

        var oldEffective = GetEffectiveNetworkEntities(receiver, component);
        component.Networks = seeded;
        component.RuntimeNetworks = runtime;
        var newEffective = GetEffectiveNetworkEntities(receiver, component);
        UpdateReceiverIndex(receiver, oldEffective, newEffective);
        if (!oldEffective.SetEquals(newEffective))
            NotifyAuthorizationChanged(receiver);
        return true;
    }

    public bool AddReceiverNetwork(EntityUid receiver, EntityUid network)
    {
        if (!HasComp<CameraNetworkIdentityComponent>(network)
            || !TryComp(receiver, out CameraNetworkReceiverComponent? component)
            || !component.RuntimeNetworks.Add(network))
        {
            return false;
        }

        IndexReceiver(receiver, [network]);
        NotifyAuthorizationChanged(receiver);
        return true;
    }

    public bool RemoveReceiverNetwork(EntityUid receiver, EntityUid network)
    {
        if (!TryComp(receiver, out CameraNetworkReceiverComponent? component)
            || !component.RuntimeNetworks.Remove(network))
        {
            return false;
        }

        if (!_runtimeGrants.TryGetValue(receiver, out var grants) || !grants.ContainsKey(network))
            UnindexReceiver(receiver, [network]);
        NotifyAuthorizationChanged(receiver);
        return true;
    }

    public HashSet<ProtoId<CameraNetworkPrototype>> GetEffectiveNetworks(EntityUid receiver)
    {
        var networks = new HashSet<ProtoId<CameraNetworkPrototype>>();
        foreach (var network in GetEffectiveNetworkEntities(receiver))
        {
            if (TryGetNetworkSeed(network, out var seed))
                networks.Add(seed);
        }

        return networks;
    }

    public bool GrantNetwork(EntityUid receiver, ProtoId<CameraNetworkPrototype> network, EntityUid source)
    {
        return GrantNetwork(receiver, ResolveNetwork(network), source);
    }

    public bool GrantNetwork(EntityUid receiver, EntityUid network, EntityUid source)
    {
        if (!HasComp<CameraNetworkIdentityComponent>(network)
            || !TryComp(receiver, out CameraNetworkReceiverComponent? component))
            return false;

        var alreadyEffective = HasEffectiveNetwork(receiver, component, network);
>>>>>>> cmu/master
        if (!_runtimeGrants.TryGetValue(receiver, out var grants))
        {
            grants = [];
            _runtimeGrants.Add(receiver, grants);
        }

        if (!grants.TryGetValue(network, out var sources))
        {
            sources = [];
            grants.Add(network, sources);
        }

        if (!sources.Add(source))
            return false;

        AddSourceGrant(source, receiver, network);
<<<<<<< HEAD
        if (effectiveNetworks.Add(network))
=======
        if (!alreadyEffective)
>>>>>>> cmu/master
        {
            IndexReceiver(receiver, [network]);
            NotifyAuthorizationChanged(receiver);
        }

        return true;
    }

    public bool RevokeNetwork(EntityUid receiver, ProtoId<CameraNetworkPrototype> network, EntityUid source)
    {
<<<<<<< HEAD
=======
        return RevokeNetwork(receiver, ResolveNetwork(network), source);
    }

    public bool RevokeNetwork(EntityUid receiver, EntityUid network, EntityUid source)
    {
>>>>>>> cmu/master
        if (!TryComp(receiver, out CameraNetworkReceiverComponent? component))
            return false;

        return RevokeNetwork(receiver, component, network, source);
    }

    private bool RevokeNetwork(
        EntityUid receiver,
        CameraNetworkReceiverComponent component,
<<<<<<< HEAD
        ProtoId<CameraNetworkPrototype> network,
=======
        EntityUid network,
>>>>>>> cmu/master
        EntityUid source)
    {
        if (!_runtimeGrants.TryGetValue(receiver, out var grants)
            || !grants.TryGetValue(network, out var sources)
            || !sources.Remove(source))
        {
            return false;
        }

        RemoveSourceGrant(source, receiver, network);
        if (sources.Count != 0)
            return true;

        grants.Remove(network);
        if (grants.Count == 0)
            _runtimeGrants.Remove(receiver);

<<<<<<< HEAD
        if (component.Networks.Contains(network))
=======
        if (GetReceiverBaseNetworkEntities(component).Contains(network))
>>>>>>> cmu/master
            return true;

        UnindexReceiver(receiver, [network]);
        NotifyAuthorizationChanged(receiver);
        return true;
    }

<<<<<<< HEAD
    private void IndexMember(EntityUid member, IEnumerable<ProtoId<CameraNetworkPrototype>> networks)
=======
    private void IndexMember(EntityUid member, IEnumerable<EntityUid> networks)
>>>>>>> cmu/master
    {
        foreach (var network in networks)
        {
            if (!_members.TryGetValue(network, out var members))
            {
                members = [];
                _members.Add(network, members);
            }

            members.Add(member);
        }
    }

<<<<<<< HEAD
    private void UnindexMember(EntityUid member, IEnumerable<ProtoId<CameraNetworkPrototype>> networks)
=======
    private void UnindexMember(EntityUid member, IEnumerable<EntityUid> networks)
>>>>>>> cmu/master
    {
        foreach (var network in networks)
        {
            if (!_members.TryGetValue(network, out var members))
                continue;

            members.Remove(member);
            if (members.Count == 0)
                _members.Remove(network);
        }
    }

<<<<<<< HEAD
    private void IndexReceiver(EntityUid receiver, IEnumerable<ProtoId<CameraNetworkPrototype>> networks)
=======
    private void IndexReceiver(EntityUid receiver, IEnumerable<EntityUid> networks)
>>>>>>> cmu/master
    {
        foreach (var network in networks)
        {
            if (!_receivers.TryGetValue(network, out var receivers))
            {
                receivers = [];
                _receivers.Add(network, receivers);
            }

            receivers.Add(receiver);
        }
    }

<<<<<<< HEAD
    private void UnindexReceiver(EntityUid receiver, IEnumerable<ProtoId<CameraNetworkPrototype>> networks)
=======
    private void UnindexReceiver(EntityUid receiver, IEnumerable<EntityUid> networks)
>>>>>>> cmu/master
    {
        foreach (var network in networks)
        {
            if (!_receivers.TryGetValue(network, out var receivers))
                continue;

            receivers.Remove(receiver);
            if (receivers.Count == 0)
                _receivers.Remove(network);
        }
    }

    private void UpdateReceiverIndex(
        EntityUid receiver,
<<<<<<< HEAD
        IEnumerable<ProtoId<CameraNetworkPrototype>> oldNetworks,
        IEnumerable<ProtoId<CameraNetworkPrototype>> newNetworks)
=======
        IEnumerable<EntityUid> oldNetworks,
        IEnumerable<EntityUid> newNetworks)
>>>>>>> cmu/master
    {
        UnindexReceiver(receiver, oldNetworks);
        IndexReceiver(receiver, newNetworks);
    }

<<<<<<< HEAD
    private HashSet<ProtoId<CameraNetworkPrototype>> GetEffectiveNetworks(
        EntityUid receiver,
        CameraNetworkReceiverComponent component)
    {
        var networks = new HashSet<ProtoId<CameraNetworkPrototype>>(component.Networks);
=======
    private HashSet<EntityUid> GetEffectiveNetworkEntities(
        EntityUid receiver,
        CameraNetworkReceiverComponent component)
    {
        var networks = GetReceiverBaseNetworkEntities(component);
>>>>>>> cmu/master
        if (_runtimeGrants.TryGetValue(receiver, out var grants))
            networks.UnionWith(grants.Keys);

        return networks;
    }

<<<<<<< HEAD
    private void AddSourceGrant(EntityUid source, EntityUid receiver, ProtoId<CameraNetworkPrototype> network)
=======
    private bool HasEffectiveNetwork(
        EntityUid receiver,
        CameraNetworkReceiverComponent component,
        EntityUid network)
    {
        if (component.RuntimeNetworks.Contains(network)
            || _runtimeGrants.TryGetValue(receiver, out var grants) && grants.ContainsKey(network))
        {
            return true;
        }

        return component.Networks.Any(seed => ResolveNetwork(seed) == network);
    }

    private void AddSourceGrant(EntityUid source, EntityUid receiver, EntityUid network)
>>>>>>> cmu/master
    {
        if (!_grantsBySource.TryGetValue(source, out var grants))
        {
            grants = [];
            _grantsBySource.Add(source, grants);
        }

        grants.Add((receiver, network));
    }

<<<<<<< HEAD
    private void RemoveSourceGrant(EntityUid source, EntityUid receiver, ProtoId<CameraNetworkPrototype> network)
=======
    private void RemoveSourceGrant(EntityUid source, EntityUid receiver, EntityUid network)
>>>>>>> cmu/master
    {
        if (!_grantsBySource.TryGetValue(source, out var grants))
            return;

        grants.Remove((receiver, network));
        if (grants.Count == 0)
            _grantsBySource.Remove(source);
    }

    private void CleanupSourceGrants(EntityUid source)
    {
        if (!_grantsBySource.Remove(source, out var grants))
            return;

        foreach (var (receiver, network) in grants)
        {
            if (TryComp(receiver, out CameraNetworkReceiverComponent? component))
                RevokeNetwork(receiver, component, network, source);
            else
                RemoveRuntimeGrant(receiver, network, source);
        }
    }

    private void CleanupReceiverGrants(EntityUid receiver)
    {
        if (!_runtimeGrants.Remove(receiver, out var grants))
            return;

        foreach (var (network, sources) in grants)
        {
            foreach (var source in sources)
                RemoveSourceGrant(source, receiver, network);
        }
    }

<<<<<<< HEAD
    private void RemoveRuntimeGrant(EntityUid receiver, ProtoId<CameraNetworkPrototype> network, EntityUid source)
=======
    private void RemoveRuntimeGrant(EntityUid receiver, EntityUid network, EntityUid source)
>>>>>>> cmu/master
    {
        if (!_runtimeGrants.TryGetValue(receiver, out var grants)
            || !grants.TryGetValue(network, out var sources)
            || !sources.Remove(source))
        {
            return;
        }

        if (sources.Count == 0)
            grants.Remove(network);
        if (grants.Count == 0)
            _runtimeGrants.Remove(receiver);
    }

<<<<<<< HEAD
    private void NotifyReceivers(
        IEnumerable<ProtoId<CameraNetworkPrototype>> networks,
=======
    private void RemoveNetworkIdentity(EntityUid network)
    {
        if (_members.Remove(network, out var members))
        {
            foreach (var member in members)
            {
                if (TryComp(member, out CameraNetworkMemberComponent? component))
                    component.RuntimeNetworks.Remove(network);
            }
        }

        if (_receivers.Remove(network, out var receivers))
        {
            foreach (var receiver in receivers)
            {
                if (TryComp(receiver, out CameraNetworkReceiverComponent? component))
                    component.RuntimeNetworks.Remove(network);

                if (!_runtimeGrants.TryGetValue(receiver, out var grants)
                    || !grants.Remove(network, out var sources))
                    continue;

                foreach (var source in sources)
                    RemoveSourceGrant(source, receiver, network);
                if (grants.Count == 0)
                    _runtimeGrants.Remove(receiver);

                NotifyAuthorizationChanged(receiver);
            }
        }
    }

    private void NotifyReceivers(
        IEnumerable<EntityUid> networks,
>>>>>>> cmu/master
        EntityUid? member,
        CameraReceiverChangeKind kind = CameraReceiverChangeKind.MemberList)
    {
        var notified = new HashSet<EntityUid>();
        foreach (var network in networks)
        {
            if (!_receivers.TryGetValue(network, out var receivers))
                continue;

            foreach (var receiver in receivers)
            {
                if (notified.Add(receiver))
                    NotifyReceiver(receiver, member, kind);
            }
        }
    }

<<<<<<< HEAD
    private bool IsAvailable(EntityUid camera)
=======
    public bool IsAvailable(EntityUid camera)
>>>>>>> cmu/master
    {
        if (TerminatingOrDeleted(camera) || MetaData(camera).EntityPaused)
            return false;

        if (TryComp(camera, out SurveillanceCameraComponent? surveillance) && !surveillance.Active)
            return false;

        return !TryComp(camera, out ApcPowerReceiverComponent? power) || power.Powered;
    }

<<<<<<< HEAD
    private void QueueMarkerChange(Entity<CameraMapMarkerComponent> marker)
    {
        if (marker.Comp.Mobile)
        {
            _mobileMarkerUpdates.TryAdd(marker.Owner, _timing.CurTime + marker.Comp.UpdateInterval);
            return;
        }

        QueueAffectedReceivers(marker.Owner);
    }

    private void QueueAffectedReceivers(EntityUid marker)
=======
    private void QueueMarkerChange(
        Entity<CameraMapMarkerComponent> marker,
        bool directoryChanged = false)
    {
        if (marker.Comp.Mobile)
        {
            if (_mobileMarkerUpdates.TryGetValue(marker.Owner, out var pending))
            {
                _mobileMarkerUpdates[marker.Owner] = pending with
                {
                    DirectoryChanged = pending.DirectoryChanged || directoryChanged,
                };
            }
            else
            {
                _mobileMarkerUpdates.Add(
                    marker.Owner,
                    new PendingMobileMarkerChange(
                        _timing.CurTime + marker.Comp.UpdateInterval,
                        directoryChanged));
            }

            return;
        }

        QueueAffectedReceivers(marker.Owner, directoryChanged);
    }

    private void QueueMemberDirectoryChange(Entity<CameraNetworkMemberComponent> member)
    {
        if (TryComp(member.Owner, out CameraMapMarkerComponent? marker))
        {
            QueueMarkerChange((member.Owner, marker), directoryChanged: true);
            return;
        }

        NotifyReceivers(
            GetMemberNetworkEntities(member.Comp),
            member.Owner,
            CameraReceiverChangeKind.Directory);
    }

    private void QueueAffectedReceivers(EntityUid marker, bool directoryChanged = false)
>>>>>>> cmu/master
    {
        if (!TryComp(marker, out CameraNetworkMemberComponent? member))
            return;

<<<<<<< HEAD
        foreach (var network in member.Networks)
=======
        MarkerRevision++;

        foreach (var network in GetMemberNetworkEntities(member))
>>>>>>> cmu/master
        {
            if (!_receivers.TryGetValue(network, out var receivers))
                continue;

            foreach (var receiver in receivers)
<<<<<<< HEAD
                _pendingMarkerReceivers.TryAdd(receiver, marker);
=======
            {
                if (_pendingMarkerReceivers.TryGetValue(receiver, out var pending))
                {
                    _pendingMarkerReceivers[receiver] = new PendingMarkerChange(
                        marker,
                        pending.DirectoryChanged || directoryChanged);
                }
                else
                {
                    _pendingMarkerReceivers.Add(
                        receiver,
                        new PendingMarkerChange(marker, directoryChanged));
                }
            }
>>>>>>> cmu/master
        }
    }

    private void NotifyAuthorizationChanged(EntityUid receiver)
    {
        var ev = new CameraReceiverChangedEvent(CameraReceiverChangeKind.Authorization);
        RaiseLocalEvent(receiver, ref ev);
    }

    private void NotifyReceiver(EntityUid receiver, EntityUid? member, CameraReceiverChangeKind kind)
    {
        var ev = new CameraReceiverChangedEvent(kind, member);
        RaiseLocalEvent(receiver, ref ev);
    }
<<<<<<< HEAD
}

internal sealed class MapViewSubscription(ICommonSession session)
{
    public ICommonSession Session { get; } = session;
    public HashSet<EntityUid> Grids { get; } = [];
}

internal sealed class MapGridViewSubscription(EntityUid view)
{
    public EntityUid View { get; } = view;
    public int Count { get; set; } = 1;
=======

    private readonly record struct PendingMarkerChange(EntityUid Marker, bool DirectoryChanged);

    private readonly record struct PendingMobileMarkerChange(TimeSpan Due, bool DirectoryChanged);
>>>>>>> cmu/master
}
