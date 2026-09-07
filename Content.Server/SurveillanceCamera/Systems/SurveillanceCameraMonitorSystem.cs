using System.Linq;
using Content.Server.Camera;
<<<<<<< HEAD
using Content.Server.Power.Components;
using Content.Shared.Camera;
=======
using Content.Shared.Access.Systems;
using Content.Shared.Camera;
using Content.Shared.CCVar;
>>>>>>> cmu/master
using Content.Shared.Power;
using Content.Shared.SurveillanceCamera;
using Content.Shared.UserInterface;
using Robust.Server.GameObjects;
using Robust.Shared.Configuration;
using Robust.Shared.Player;
<<<<<<< HEAD
using Robust.Shared.Prototypes;
=======
using Robust.Shared.Timing;
>>>>>>> cmu/master

namespace Content.Server.SurveillanceCamera;

public sealed partial class SurveillanceCameraMonitorSystem : EntitySystem
{
<<<<<<< HEAD
    [Dependency] private SurveillanceCameraSystem _surveillanceCameras = default!;
    [Dependency] private UserInterfaceSystem _userInterface = default!;
    [Dependency] private CameraNetworkSystem _cameraNetworks = default!;
    [Dependency] private IPrototypeManager _prototypeManager = default!;
=======
    [Dependency] private readonly AccessReaderSystem _accessReader = default!;
    [Dependency] private readonly CameraNetworkSystem _cameraNetworks = default!;
    [Dependency] private readonly CameraSessionSystem _cameraSessions = default!;
    [Dependency] private readonly IConfigurationManager _configuration = default!;
    [Dependency] private readonly UserInterfaceSystem _userInterface = default!;
>>>>>>> cmu/master

    public override void Initialize()
    {
        SubscribeLocalEvent<SurveillanceCameraMonitorComponent, PowerChangedEvent>(OnPowerChanged);
        SubscribeLocalEvent<SurveillanceCameraMonitorComponent, ComponentShutdown>(OnShutdown);
<<<<<<< HEAD
        SubscribeLocalEvent<SurveillanceCameraMonitorComponent, CameraReceiverChangedEvent>(OnCameraReceiverChanged);
=======
        SubscribeLocalEvent<SurveillanceCameraMonitorComponent, CameraSessionChangedEvent>(OnCameraSessionChanged);
>>>>>>> cmu/master
        SubscribeLocalEvent<SurveillanceCameraMonitorComponent, AfterActivatableUIOpenEvent>(OnToggleInterface);
        Subs.BuiEvents<SurveillanceCameraMonitorComponent>(SurveillanceCameraMonitorUiKey.Key, subs =>
        {
            subs.Event<CameraSessionResyncMessage>(OnResyncMessage);
            subs.Event<CameraSessionSelectMessage>(OnSessionSelectMessage);
            subs.Event<CameraSessionSelectNetworkMessage>(OnSessionSelectNetworkMessage);
            subs.Event<CameraSessionDisconnectMessage>(OnSessionDisconnectMessage);
            subs.Event<BoundUIClosedEvent>(OnBoundUiClose);
        });
    }

<<<<<<< HEAD
    private void OnSubnetRequest(EntityUid uid, SurveillanceCameraMonitorComponent component,
        SurveillanceCameraMonitorSubnetRequestMessage args)
    {
        if (_cameraNetworks.GetEffectiveNetworks(uid).Contains(args.Network))
            component.ActiveNetwork = args.Network;

        UpdateUserInterface(uid, component);
=======
    private void OnSessionSelectMessage(EntityUid uid, SurveillanceCameraMonitorComponent component,
        CameraSessionSelectMessage args)
    {
        if (TryGetSession(uid, component, args.Actor, out var session)
            && TryGetEntity(args.Camera, out var camera)
            && camera is { } cameraUid)
        {
            _cameraSessions.SelectCamera(session.Id, cameraUid);
        }
    }

    private void OnSessionSelectNetworkMessage(EntityUid uid, SurveillanceCameraMonitorComponent component,
        CameraSessionSelectNetworkMessage args)
    {
        if (TryGetSession(uid, component, args.Actor, out var session)
            && TryGetEntity(args.Network, out var network)
            && network is { } networkUid)
        {
            _cameraSessions.SelectNetwork(session.Id, networkUid);
        }
    }

    private void OnSessionDisconnectMessage(EntityUid uid, SurveillanceCameraMonitorComponent component,
        CameraSessionDisconnectMessage args)
    {
        if (TryGetSession(uid, component, args.Actor, out var session))
            _cameraSessions.SelectCamera(session.Id, null);
>>>>>>> cmu/master
    }

    private void OnResyncMessage(EntityUid uid, SurveillanceCameraMonitorComponent component,
        CameraSessionResyncMessage args)
    {
<<<<<<< HEAD
        DisconnectCamera(uid, true, component);
    }

    private void OnRefreshCamerasMessage(EntityUid uid, SurveillanceCameraMonitorComponent component,
        SurveillanceCameraRefreshCamerasMessage message)
    {
        UpdateUserInterface(uid, component);
    }

    private void OnRefreshSubnetsMessage(EntityUid uid, SurveillanceCameraMonitorComponent component,
        SurveillanceCameraRefreshSubnetsMessage message)
    {
        UpdateUserInterface(uid, component);
    }

    private void OnSwitchMessage(EntityUid uid, SurveillanceCameraMonitorComponent component,
        SurveillanceCameraMonitorSwitchMessage message)
    {
        if (TryGetEntity(message.Camera, out var camera) && camera is { } cameraUid)
            TrySelectCamera((uid, component), cameraUid);

        UpdateUserInterface(uid, component);
=======
        if (TryGetSession(uid, component, args.Actor, out var session) && session.Id == args.SessionId)
            SendSnapshot(uid, args.Actor, session);
>>>>>>> cmu/master
    }

    private void OnPowerChanged(EntityUid uid, SurveillanceCameraMonitorComponent component, ref PowerChangedEvent args)
    {
        if (!args.Powered)
        {
<<<<<<< HEAD
            RemoveActiveCamera(uid, component);
            component.ActiveNetwork = null;
            UpdateUserInterface(uid, component);
=======
            _cameraSessions.ClearSelections(uid);
            UpdateMonitorVisual(uid);
>>>>>>> cmu/master
        }
    }

    private void OnShutdown(EntityUid uid, SurveillanceCameraMonitorComponent component, ComponentShutdown args)
    {
<<<<<<< HEAD
        RemoveActiveCamera(uid, component);
        _cameraNetworks.ClearMapViewSubscriptions(uid);
=======
        _cameraSessions.CloseSessions(uid);
>>>>>>> cmu/master
    }

    private void OnToggleInterface(EntityUid uid, SurveillanceCameraMonitorComponent component,
        AfterActivatableUIOpenEvent args)
    {
        AfterOpenUserInterface(uid, args.User, component);
    }

<<<<<<< HEAD
    private void OnSurveillanceCameraDeactivate(EntityUid uid, SurveillanceCameraMonitorComponent monitor,
        SurveillanceCameraDeactivateEvent args)
=======
    private void OnCameraSessionChanged(EntityUid uid, SurveillanceCameraMonitorComponent monitor,
        ref CameraSessionChangedEvent args)
>>>>>>> cmu/master
    {
        UpdateViewer(uid, args.Actor);
        UpdateMonitorVisual(uid);
    }

    private void OnCameraReceiverChanged(EntityUid uid, SurveillanceCameraMonitorComponent monitor,
        ref CameraReceiverChangedEvent args)
    {
        if (monitor.ActiveCamera is { } camera && !_cameraNetworks.CanAccess(uid, camera))
        {
            DisconnectCamera(uid, true, monitor);
            return;
        }

        UpdateUserInterface(uid, monitor);
    }

    private void OnBoundUiClose(EntityUid uid, SurveillanceCameraMonitorComponent component, BoundUIClosedEvent args)
    {
        RemoveViewer(uid, args.Actor, component);
    }

<<<<<<< HEAD
    private void DisconnectCamera(EntityUid uid, bool removeViewers, SurveillanceCameraMonitorComponent? monitor = null)
    {
        if (!Resolve(uid, ref monitor))
            return;

        if (removeViewers)
            RemoveActiveCamera(uid, monitor);

        monitor.ActiveCamera = null;
        RemComp<ActiveSurveillanceCameraMonitorComponent>(uid);
        UpdateUserInterface(uid, monitor);
    }

    private void AddViewer(EntityUid uid, EntityUid player, SurveillanceCameraMonitorComponent? monitor = null)
    {
        if (!Resolve(uid, ref monitor))
=======
    private void AddViewer(EntityUid uid, EntityUid player, SurveillanceCameraMonitorComponent monitor)
    {
        if (!TryComp(player, out ActorComponent? actor))
            return;

        var capabilities = CameraSessionCapabilities.Browse | CameraSessionCapabilities.LiveView;
        if (_configuration.GetCVar(CCVars.CMUCameraMapEnabled))
            capabilities |= CameraSessionCapabilities.Map;

        var session = _cameraSessions.OpenSession(
            actor.PlayerSession,
            player,
            uid,
            capabilities);
        if (session != null)
            SendSnapshot(uid, player, session);
    }

    private void RemoveViewer(EntityUid uid, EntityUid player, SurveillanceCameraMonitorComponent monitor)
    {
        if (!TryComp(player, out ActorComponent? actor))
            return;

        if (_cameraSessions.TryGetSession(actor.PlayerSession, uid, out var session))
        {
            _userInterface.ServerSendUiMessage(
                uid,
                SurveillanceCameraMonitorUiKey.Key,
                new CameraSessionResetMessage(session.Id),
                player);
        }

        _cameraSessions.CloseSession(actor.PlayerSession, uid);
        UpdateMonitorVisual(uid);
    }

    public void AfterOpenUserInterface(
        EntityUid uid,
        EntityUid player,
        SurveillanceCameraMonitorComponent? monitor = null,
        ActorComponent? actor = null)
    {
        if (!Resolve(uid, ref monitor) || !Resolve(player, ref actor) || !CanUseMonitor(uid, player))
            return;

        AddViewer(uid, player, monitor);
    }

    private bool TryGetSession(
        EntityUid monitorUid,
        SurveillanceCameraMonitorComponent monitor,
        EntityUid actor,
        out CameraViewerSession session)
    {
        if (CanUseMonitor(monitorUid, actor)
            && TryComp(actor, out ActorComponent? actorComponent)
            && _cameraSessions.TryGetSession(actorComponent.PlayerSession, monitorUid, out session))
        {
            return true;
        }

        RemoveViewer(monitorUid, actor, monitor);
        session = default!;
        return false;
    }

    private CameraSessionDirectoryUiData BuildDirectory(CameraViewerSession session)
    {
        var networks = _cameraNetworks.GetEffectiveNetworkEntities(session.Receiver)
            .Where(network => TryComp(network, out CameraNetworkIdentityComponent? _))
            .Select(network => new CameraSessionNetworkUiData(
                GetNetEntity(network),
                Comp<CameraNetworkIdentityComponent>(network).DisplayName))
            .OrderBy(network => network.Name, StringComparer.Ordinal)
            .ThenBy(network => network.Network.Id)
            .ToList();

        var cameras = new List<CameraSessionCameraUiData>();
        if (session.ActiveNetwork is { } activeNetwork)
        {
            cameras = session.AuthorizedCameras
                .Where(camera => _cameraNetworks.IsMemberOfNetwork(camera, activeNetwork))
                .OrderBy(camera => Name(camera), StringComparer.Ordinal)
                .ThenBy(camera => camera.Id)
                .Select(camera => new CameraSessionCameraUiData(
                    GetNetEntity(camera),
                    Name(camera),
                    TryComp(camera, out SurveillanceCameraComponent? source) && source.Active))
                .ToList();
        }

        return new CameraSessionDirectoryUiData(
            GetNetEntity(session.SelectedCamera),
            session.SelectedCamera is { } camera ? Name(camera) : null,
            networks,
            GetNetEntity(session.ActiveNetwork),
            cameras,
            (session.Capabilities & CameraSessionCapabilities.Map) != 0);
    }

    private CameraMapUiState BuildGeometry(CameraViewerSession session)
    {
        if ((session.Capabilities & CameraSessionCapabilities.Map) == 0
            || session.ActiveNetwork is not { } activeNetwork)
        {
            return new CameraMapUiState(default, []);
        }

        var map = _cameraNetworks.BuildMapState(session.Receiver);
        var grids = map.Grids
            .Select(grid => new CameraMapGridUiData(
                grid.Grid,
                grid.Name,
                grid.Markers
                    .Where(marker => TryGetEntity(marker.Camera, out var camera)
                        && camera is { } cameraUid
                        && _cameraNetworks.IsMemberOfNetwork(cameraUid, activeNetwork))
                    .ToList()))
            .Where(grid => grid.Markers.Count > 0)
            .ToList();
        return new CameraMapUiState(map.ConsoleGrid, grids);
    }

    private void SendSnapshot(EntityUid uid, EntityUid actor, CameraViewerSession session)
    {
        _userInterface.ServerSendUiMessage(
            uid,
            SurveillanceCameraMonitorUiKey.Key,
            new CameraSessionSnapshotMessage(session.Id, session.Revision, BuildDirectory(session)),
            actor);
        session.LastSentRevision = session.Revision;
        SendGeometry(uid, actor, session);
    }

    private void SendDelta(EntityUid uid, EntityUid actor, CameraViewerSession session)
    {
        if (session.LastSentRevision == 0 || session.LastSentRevision > session.Revision)
        {
            SendSnapshot(uid, actor, session);
            return;
        }

        if (session.LastSentRevision == session.Revision)
        {
            SendGeometry(uid, actor, session);
            return;
        }

        _userInterface.ServerSendUiMessage(
            uid,
            SurveillanceCameraMonitorUiKey.Key,
            new CameraSessionDeltaMessage(
                session.Id,
                session.LastSentRevision,
                session.Revision,
                BuildDirectory(session)),
            actor);
        session.LastSentRevision = session.Revision;
        SendGeometry(uid, actor, session);
    }

    private void SendGeometry(EntityUid uid, EntityUid actor, CameraViewerSession session)
    {
        if ((session.Capabilities & CameraSessionCapabilities.Map) == 0
            || session.LastSentMarkerRevision == _cameraNetworks.MarkerRevision
            && session.LastSentGeometryNetwork == session.ActiveNetwork)
        {
>>>>>>> cmu/master
            return;

<<<<<<< HEAD
        monitor.Viewers.Add(player);
        if (monitor.ActiveCamera is { } camera)
            _surveillanceCameras.AddActiveViewer(camera, player, uid);

        _cameraNetworks.SyncMapViewSubscriptions(uid, player, _cameraNetworks.BuildMapState(uid));

        UpdateUserInterface(uid, monitor);
    }

    private void RemoveViewer(EntityUid uid, EntityUid player, SurveillanceCameraMonitorComponent? monitor = null)
    {
        if (!Resolve(uid, ref monitor))
=======
        _userInterface.ServerSendUiMessage(
            uid,
            SurveillanceCameraMonitorUiKey.Key,
            new CameraSessionGeometryMessage(
                session.Id,
                GetNetEntity(session.ActiveNetwork),
                _cameraNetworks.MarkerRevision,
                BuildGeometry(session)),
            actor);
        session.LastSentMarkerRevision = _cameraNetworks.MarkerRevision;
        session.LastSentGeometryNetwork = session.ActiveNetwork;
    }

    private void UpdateViewer(EntityUid uid, EntityUid actor)
    {
        if (!TryComp(actor, out ActorComponent? actorComponent)
            || !_cameraSessions.TryGetSession(actorComponent.PlayerSession, uid, out var session)
            || !_userInterface.IsUiOpen(uid, SurveillanceCameraMonitorUiKey.Key, actor))
        {
>>>>>>> cmu/master
            return;

<<<<<<< HEAD
        monitor.Viewers.Remove(player);
        _cameraNetworks.ClearMapViewSubscriptions(uid, player);
        if (monitor.ActiveCamera is { } camera)
            _surveillanceCameras.RemoveActiveViewer(camera, player);
=======
        SendDelta(uid, actor, session);
    }

    private void UpdateMonitorVisual(EntityUid uid)
    {
        if (_cameraSessions.HasActiveSelection(uid))
            EnsureComp<ActiveSurveillanceCameraMonitorComponent>(uid);
        else
            RemComp<ActiveSurveillanceCameraMonitorComponent>(uid);
>>>>>>> cmu/master
    }

    private bool CanUseMonitor(EntityUid monitor, EntityUid actor)
    {
<<<<<<< HEAD
        if (!Resolve(uid, ref monitor) || monitor.ActiveCamera is not { } camera)
            return;

        _surveillanceCameras.RemoveActiveViewers(camera, monitor.Viewers, uid);
        monitor.ActiveCamera = null;
        RemComp<ActiveSurveillanceCameraMonitorComponent>(uid);
        UpdateUserInterface(uid, monitor);
    }

    public bool TrySelectCamera(Entity<SurveillanceCameraMonitorComponent> monitor, EntityUid camera)
    {
        if (TerminatingOrDeleted(camera) ||
            Paused(camera) ||
            !_cameraNetworks.CanAccess(monitor.Owner, camera) ||
            !TryComp(camera, out SurveillanceCameraComponent? source) ||
            !source.Active)
        {
            return false;
        }

        if (monitor.Comp.ActiveCamera is { } oldCamera)
            _surveillanceCameras.SwitchActiveViewers(oldCamera, camera, monitor.Comp.Viewers, monitor.Owner);
        else
            _surveillanceCameras.AddActiveViewers(camera, monitor.Comp.Viewers, monitor.Owner);

        monitor.Comp.ActiveCamera = camera;
        EnsureComp<ActiveSurveillanceCameraMonitorComponent>(monitor.Owner);
        return true;
    }

    public void AfterOpenUserInterface(EntityUid uid, EntityUid player,
        SurveillanceCameraMonitorComponent? monitor = null, ActorComponent? actor = null)
    {
        if (!Resolve(uid, ref monitor) || !Resolve(player, ref actor))
            return;

        AddViewer(uid, player, monitor);
    }

    public SurveillanceCameraMonitorUiState BuildUiState(Entity<SurveillanceCameraMonitorComponent> monitor)
    {
        var effectiveNetworks = _cameraNetworks.GetEffectiveNetworks(monitor.Owner);
        if (monitor.Comp.ActiveNetwork is not { } activeNetwork || !effectiveNetworks.Contains(activeNetwork))
        {
            if (effectiveNetworks.Count == 0)
            {
                monitor.Comp.ActiveNetwork = default;
            }
            else
            {
                monitor.Comp.ActiveNetwork = effectiveNetworks
                    .OrderBy(network => Loc.GetString(_prototypeManager.Index<CameraNetworkPrototype>(network).Name), StringComparer.Ordinal)
                    .ThenBy(network => network.ToString(), StringComparer.Ordinal)
                    .First();
            }
        }

        var networks = effectiveNetworks
            .Select(network => new CameraNetworkUiData(
                network,
                Loc.GetString(_prototypeManager.Index<CameraNetworkPrototype>(network).Name)))
            .OrderBy(network => network.Name, StringComparer.Ordinal)
            .ThenBy(network => network.Id.ToString(), StringComparer.Ordinal)
            .ToList();

        var cameras = new List<CameraListUiData>();
        if (monitor.Comp.ActiveNetwork is { } selectedNetwork &&
            TryComp(monitor.Owner, out CameraNetworkReceiverComponent? receiver))
        {
            cameras = _cameraNetworks.GetAccessibleCameras((monitor.Owner, receiver))
                .Where(camera => TryComp(camera, out CameraNetworkMemberComponent? member) &&
                                 member.Networks.Contains(selectedNetwork))
                .OrderBy(camera => Name(camera), StringComparer.Ordinal)
                .ThenBy(camera => camera.Id)
                .Select(camera =>
                {
                    var member = Comp<CameraNetworkMemberComponent>(camera);
                    var source = CompOrNull<SurveillanceCameraComponent>(camera);
                    return new CameraListUiData(
                        GetNetEntity(camera),
                        Name(camera),
                        source?.Active ?? false,
                        new HashSet<ProtoId<CameraNetworkPrototype>>(member.Networks));
                })
                .ToList();
        }

        var activeCamera = monitor.Comp.ActiveCamera;
        return new SurveillanceCameraMonitorUiState(
            GetNetEntity(activeCamera),
            activeCamera == null ? null : Name(activeCamera.Value),
            networks,
            monitor.Comp.ActiveNetwork,
            cameras,
            _cameraNetworks.BuildMapState(monitor.Owner));
    }

    private void UpdateUserInterface(EntityUid uid, SurveillanceCameraMonitorComponent? monitor = null)
    {
        if (!Resolve(uid, ref monitor) || !_userInterface.IsUiOpen(uid, SurveillanceCameraMonitorUiKey.Key))
            return;

        var state = BuildUiState((uid, monitor));
        foreach (var viewer in monitor.Viewers)
            _cameraNetworks.SyncMapViewSubscriptions(uid, viewer, state.CameraMap);

        _userInterface.SetUiState(uid, SurveillanceCameraMonitorUiKey.Key, state);
    }
=======
        return !TerminatingOrDeleted(actor)
            && TryComp(actor, out ActorComponent? actorComponent)
            && actorComponent.PlayerSession.AttachedEntity == actor
            && _userInterface.IsUiOpen(monitor, SurveillanceCameraMonitorUiKey.Key, actor)
            && _accessReader.IsAllowed(actor, monitor);
    }

>>>>>>> cmu/master
}
