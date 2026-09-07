<<<<<<< HEAD
using Content.Shared.Camera;
using Robust.Shared.Prototypes;
=======
>>>>>>> cmu/master
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Camera;

[Serializable, NetSerializable]
public enum RMCCameraNetworkEditorOrigin : byte
{
    Seeded,
    Owned,
}

[Serializable, NetSerializable]
public enum RMCCameraNetworkEditorError : byte
{
    None,
    AccessDenied,
    StaleRevision,
    InvalidName,
    DuplicateName,
    MissingCamera,
    InvalidNetwork,
    SeededNetworkCannotBeDeleted,
}

[Serializable, NetSerializable]
public sealed class RMCCameraNetworkEditorNetworkUiData(
<<<<<<< HEAD
    ProtoId<CameraNetworkPrototype> id,
=======
    NetEntity id,
>>>>>>> cmu/master
    string name,
    RMCCameraNetworkEditorOrigin origin,
    bool hidden)
{
<<<<<<< HEAD
    public ProtoId<CameraNetworkPrototype> Id { get; } = id;
=======
    public NetEntity Id { get; } = id;
>>>>>>> cmu/master
    public string Name { get; } = name;
    public RMCCameraNetworkEditorOrigin Origin { get; } = origin;
    public bool Hidden { get; } = hidden;
}

[Serializable, NetSerializable]
public sealed class RMCCameraNetworkEditorCameraUiData(
    NetEntity camera,
    string name,
<<<<<<< HEAD
    List<ProtoId<CameraNetworkPrototype>> networks)
{
    public NetEntity Camera { get; } = camera;
    public string Name { get; } = name;
    public List<ProtoId<CameraNetworkPrototype>> Networks { get; } = networks;
=======
    List<NetEntity> networks)
{
    public NetEntity Camera { get; } = camera;
    public string Name { get; } = name;
    public List<NetEntity> Networks { get; } = networks;
>>>>>>> cmu/master
}

[Serializable, NetSerializable]
public sealed class RMCCameraNetworkEditorUiState(
    uint revision,
    List<RMCCameraNetworkEditorNetworkUiData> networks,
    List<RMCCameraNetworkEditorCameraUiData> cameras)
{
    public uint Revision { get; } = revision;
    public List<RMCCameraNetworkEditorNetworkUiData> Networks { get; } = networks;
    public List<RMCCameraNetworkEditorCameraUiData> Cameras { get; } = cameras;
}

[Serializable, NetSerializable]
public sealed class RMCCameraNetworkEditorCreateBuiMsg(uint revision, string name) : BoundUserInterfaceMessage
{
    public uint Revision { get; } = revision;
    public string Name { get; } = name;
}

[Serializable, NetSerializable]
public sealed class RMCCameraNetworkEditorRenameBuiMsg(
    uint revision,
<<<<<<< HEAD
    ProtoId<CameraNetworkPrototype> network,
    string name) : BoundUserInterfaceMessage
{
    public uint Revision { get; } = revision;
    public ProtoId<CameraNetworkPrototype> Network { get; } = network;
=======
    NetEntity network,
    string name) : BoundUserInterfaceMessage
{
    public uint Revision { get; } = revision;
    public NetEntity Network { get; } = network;
>>>>>>> cmu/master
    public string Name { get; } = name;
}

[Serializable, NetSerializable]
public sealed class RMCCameraNetworkEditorDeleteBuiMsg(
    uint revision,
<<<<<<< HEAD
    ProtoId<CameraNetworkPrototype> network) : BoundUserInterfaceMessage
{
    public uint Revision { get; } = revision;
    public ProtoId<CameraNetworkPrototype> Network { get; } = network;
=======
    NetEntity network) : BoundUserInterfaceMessage
{
    public uint Revision { get; } = revision;
    public NetEntity Network { get; } = network;
>>>>>>> cmu/master
}

[Serializable, NetSerializable]
public sealed class RMCCameraNetworkEditorSetHiddenBuiMsg(
    uint revision,
<<<<<<< HEAD
    ProtoId<CameraNetworkPrototype> network,
    bool hidden) : BoundUserInterfaceMessage
{
    public uint Revision { get; } = revision;
    public ProtoId<CameraNetworkPrototype> Network { get; } = network;
=======
    NetEntity network,
    bool hidden) : BoundUserInterfaceMessage
{
    public uint Revision { get; } = revision;
    public NetEntity Network { get; } = network;
>>>>>>> cmu/master
    public bool Hidden { get; } = hidden;
}

[Serializable, NetSerializable]
public sealed class RMCCameraNetworkEditorSaveCameraBuiMsg(
    uint revision,
    NetEntity camera,
    string name,
<<<<<<< HEAD
    List<ProtoId<CameraNetworkPrototype>> networks) : BoundUserInterfaceMessage
=======
    List<NetEntity> networks) : BoundUserInterfaceMessage
>>>>>>> cmu/master
{
    public uint Revision { get; } = revision;
    public NetEntity Camera { get; } = camera;
    public string Name { get; } = name;
<<<<<<< HEAD
    public List<ProtoId<CameraNetworkPrototype>> Networks { get; } = networks;
=======
    public List<NetEntity> Networks { get; } = networks;
>>>>>>> cmu/master
}

[Serializable, NetSerializable]
public sealed class RMCCameraNetworkEditorResultBuiMsg(
    RMCCameraNetworkEditorError error,
    uint revision) : BoundUserInterfaceMessage
{
    public RMCCameraNetworkEditorError Error { get; } = error;
    public uint Revision { get; } = revision;
}
