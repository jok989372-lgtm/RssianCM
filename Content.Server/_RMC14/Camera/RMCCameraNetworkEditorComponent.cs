<<<<<<< HEAD
using Content.Shared.Camera;
using Robust.Shared.Prototypes;

=======
>>>>>>> cmu/master
namespace Content.Server._RMC14.Camera;

[RegisterComponent, Access(typeof(RMCCameraSystem))]
public sealed partial class RMCCameraNetworkEditorComponent : Component
{
<<<<<<< HEAD
    public readonly HashSet<ProtoId<CameraNetworkPrototype>> SeededNetworks = [];
    public readonly Dictionary<ProtoId<CameraNetworkPrototype>, string> OwnedNetworks = [];
    public readonly Dictionary<ProtoId<CameraNetworkPrototype>, string> Aliases = [];
    public readonly HashSet<ProtoId<CameraNetworkPrototype>> HiddenSeededNetworks = [];
    public uint Revision;
    public uint NextOwnedNetworkId = 1;
=======
    public readonly HashSet<EntityUid> SeededNetworks = [];
    public readonly Dictionary<EntityUid, string> OwnedNetworks = [];
    public readonly Dictionary<EntityUid, string> Aliases = [];
    public readonly HashSet<EntityUid> HiddenSeededNetworks = [];
    public uint Revision;
>>>>>>> cmu/master
}
