using Content.Client.Eye;
using Content.Shared.Camera;
using Content.Shared.SurveillanceCamera;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;

namespace Content.Client.SurveillanceCamera.UI;

public sealed class SurveillanceCameraMonitorBoundUserInterface : BoundUserInterface
{
    private readonly EyeLerpingSystem _eyeLerpingSystem;
    private readonly SurveillanceCameraMonitorSystem _surveillanceCameraMonitorSystem;

    [ViewVariables] private SurveillanceCameraMonitorWindow? _window;
    [ViewVariables] private EntityUid? _currentCamera;

    private uint? _sessionId;
    private ulong _revision;
    private ulong _markerRevision;
    private CameraSessionDirectoryUiData? _directory;
    private CameraMapUiState _geometry = new(default, []);

    public SurveillanceCameraMonitorBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        _eyeLerpingSystem = EntMan.System<EyeLerpingSystem>();
        _surveillanceCameraMonitorSystem = EntMan.System<SurveillanceCameraMonitorSystem>();
    }

    protected override void Open()
    {
        base.Open();
        _window = this.CreateWindow<SurveillanceCameraMonitorWindow>();
<<<<<<< HEAD

        _window.CameraSelected += OnCameraSelected;
        _window.NetworkOpened += OnNetworkRequest;
        _window.CameraRefresh += OnCameraRefresh;
        _window.SubnetRefresh += OnSubnetRefresh;
        _window.CameraSwitchTimer += OnCameraSwitchTimer;
        _window.CameraDisconnect += OnCameraDisconnect;
    }

    private void OnCameraSelected(NetEntity camera)
    {
        SendMessage(new SurveillanceCameraMonitorSwitchMessage(camera));
    }

    private void OnNetworkRequest(ProtoId<CameraNetworkPrototype> network)
    {
        SendMessage(new SurveillanceCameraMonitorSubnetRequestMessage(network));
    }

    private void OnCameraSwitchTimer()
    {
        _surveillanceCameraMonitorSystem.AddTimer(Owner, _window!.OnSwitchTimerComplete);
    }

    private void OnCameraRefresh()
    {
        SendMessage(new SurveillanceCameraRefreshCamerasMessage());
    }

    private void OnSubnetRefresh()
    {
        SendMessage(new SurveillanceCameraRefreshSubnetsMessage());
    }

    private void OnCameraDisconnect()
    {
        SendMessage(new SurveillanceCameraDisconnectMessage());
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        if (_window == null || state is not SurveillanceCameraMonitorUiState cast)
=======
        _window.CameraSelected += camera => SendMessage(new CameraSessionSelectMessage(camera));
        _window.NetworkOpened += network => SendMessage(new CameraSessionSelectNetworkMessage(network));
        _window.CameraRefresh += RequestResync;
        _window.SubnetRefresh += RequestResync;
        _window.CameraSwitchTimer += () =>
            _surveillanceCameraMonitorSystem.AddTimer(Owner, _window!.OnSwitchTimerComplete);
        _window.CameraDisconnect += () => SendMessage(new CameraSessionDisconnectMessage());
    }

    protected override void ReceiveMessage(BoundUserInterfaceMessage message)
    {
        switch (message)
>>>>>>> cmu/master
        {
            case CameraSessionSnapshotMessage snapshot:
                if (_sessionId != snapshot.SessionId
                    || _directory?.ActiveNetwork != snapshot.Directory.ActiveNetwork)
                    ResetGeometry();

                _sessionId = snapshot.SessionId;
                _revision = snapshot.Revision;
                _directory = snapshot.Directory;
                ApplyState();
                break;
            case CameraSessionDeltaMessage delta when _sessionId == delta.SessionId:
                if (delta.BaseRevision != _revision)
                {
                    RequestResync();
                    return;
                }

                if (_directory?.ActiveNetwork != delta.Directory.ActiveNetwork)
                    ResetGeometry();

                _revision = delta.Revision;
                _directory = delta.Directory;
                ApplyState();
                break;
            case CameraSessionGeometryMessage geometry when
                _sessionId == geometry.SessionId &&
                _directory?.ActiveNetwork == geometry.Network:
                if (geometry.MarkerRevision < _markerRevision)
                    return;

                _markerRevision = geometry.MarkerRevision;
                _geometry = geometry.Geometry;
                ApplyState();
                break;
            case CameraSessionResetMessage reset when _sessionId == reset.SessionId:
                ResetState();
                break;
        }
    }

    private void RequestResync()
    {
        if (_sessionId is { } sessionId)
            SendMessage(new CameraSessionResyncMessage(sessionId));
    }

    private void ResetGeometry()
    {
        _markerRevision = 0;
        _geometry = new CameraMapUiState(default, []);
    }

    private void ApplyState()
    {
        if (_window == null || _directory == null)
            return;

        var active = EntMan.GetEntity(_directory.ActiveCamera);
        UpdateEye(active);
        _window.UpdateState(
            active is { } camera && EntMan.TryGetComponent<EyeComponent>(camera, out var eye) ? eye.Eye : null,
            _directory,
            _geometry);
    }

    private void UpdateEye(EntityUid? active)
    {
        if (_currentCamera == active)
            return;

        if (_currentCamera is { } previous)
        {
<<<<<<< HEAD
            _window.UpdateState(null, cast);

            if (_currentCamera != null)
            {
                _surveillanceCameraMonitorSystem.RemoveTimer(Owner);
                _eyeLerpingSystem.RemoveEye(_currentCamera.Value);
                _currentCamera = null;
            }
=======
            _surveillanceCameraMonitorSystem.RemoveTimer(Owner);
            _eyeLerpingSystem.RemoveEye(previous);
>>>>>>> cmu/master
        }

<<<<<<< HEAD
            _window.UpdateState(EntMan.TryGetComponent<EyeComponent>(active, out var eye) ? eye.Eye : null, cast);
        }
=======
        if (active is { } selected)
            _eyeLerpingSystem.AddEye(selected);
        _currentCamera = active;
    }

    private void ResetState()
    {
        UpdateEye(null);
        _sessionId = null;
        _revision = 0;
        _directory = null;
        ResetGeometry();
>>>>>>> cmu/master
    }

    protected override void Dispose(bool disposing)
    {
        ResetState();
        base.Dispose(disposing);
        if (disposing)
            _window?.Close();
    }
}
