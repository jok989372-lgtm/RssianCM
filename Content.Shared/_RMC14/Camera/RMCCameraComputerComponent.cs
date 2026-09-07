using Robust.Shared.GameStates;
<<<<<<< HEAD
using Content.Shared.Camera;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
=======
>>>>>>> cmu/master

namespace Content.Shared._RMC14.Camera;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
[Access(typeof(SharedRMCCameraSystem))]
public sealed partial class RMCCameraComputerComponent : Component
{
<<<<<<< HEAD
    [DataField(required: true), AutoNetworkedField]
    public HashSet<EntProtoId> ProtoIds = new ();

    [DataField, AutoNetworkedField]
    public EntityUid? CurrentCamera;

    [DataField, AutoNetworkedField]
    public ProtoId<CameraNetworkPrototype>? ActiveNetwork;

    [DataField, AutoNetworkedField]
    public List<NetEntity> CameraIds = new();

    [DataField, AutoNetworkedField]
    public List<string> CameraNames = new();

    [DataField, AutoNetworkedField]
    public List<EntityUid> Watchers = new();

=======
>>>>>>> cmu/master
    [DataField, AutoNetworkedField]
    public LocId? Title;

    [DataField, AutoNetworkedField]
    public Vector2i ViewportSize = new(1200, 1200);
}
