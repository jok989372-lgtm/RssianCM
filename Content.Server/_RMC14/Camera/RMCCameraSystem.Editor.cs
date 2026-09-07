using System.Linq;
using Content.Server.Administration.Logs;
using Content.Server.SurveillanceCamera;
using Content.Shared.Access.Systems;
using Content.Shared._RMC14.Camera;
using Content.Shared._RMC14.Mortar;
using Content.Shared.Camera;
<<<<<<< HEAD
=======
using Content.Shared.CCVar;
>>>>>>> cmu/master
using Content.Shared.Database;
using Content.Shared.Item;
using Content.Shared.SurveillanceCamera;
using Robust.Shared.Prototypes;

namespace Content.Server._RMC14.Camera;

public sealed partial class RMCCameraSystem
{
    private const int MaxNetworkNameLength = 48;
    private const int MaxCameraNameLength = 64;
<<<<<<< HEAD
    private const string RuntimeNetworkPrefix = "CMURuntimeCameraNetwork";

=======
>>>>>>> cmu/master
    [Dependency] private AccessReaderSystem _accessReader = default!;
    [Dependency] private IAdminLogManager _adminLogger = default!;
    [Dependency] private MetaDataSystem _metaDataSystem = default!;

    public RMCCameraNetworkEditorUiState BuildEditorState(Entity<RMCCameraComputerComponent> computer)
    {
        var editor = EnsureEditorState(computer);
        var networks = editor.SeededNetworks
            .Select(network => new RMCCameraNetworkEditorNetworkUiData(
<<<<<<< HEAD
                network,
=======
                GetNetEntity(network),
>>>>>>> cmu/master
                ResolveNetworkName(computer.Owner, network, editor),
                RMCCameraNetworkEditorOrigin.Seeded,
                editor.HiddenSeededNetworks.Contains(network)))
            .Concat(editor.OwnedNetworks.Select(pair => new RMCCameraNetworkEditorNetworkUiData(
<<<<<<< HEAD
                pair.Key,
=======
                GetNetEntity(pair.Key),
>>>>>>> cmu/master
                pair.Value,
                RMCCameraNetworkEditorOrigin.Owned,
                false)))
            .OrderBy(network => network.Name, StringComparer.Ordinal)
            .ThenBy(network => network.Id.ToString(), StringComparer.Ordinal)
            .ToList();

        var editableNetworks = networks
            .Where(network => !network.Hidden)
<<<<<<< HEAD
            .Select(network => network.Id)
=======
            .Select(network => GetEntity(network.Id))
>>>>>>> cmu/master
            .ToList();
        var cameras = GetEditableCameras()
            .Select(camera =>
            {
                var component = Comp<RMCCameraComponent>(camera);
<<<<<<< HEAD
                var memberships = Comp<CameraNetworkMemberComponent>(camera).Networks;
                return new RMCCameraNetworkEditorCameraUiData(
                    GetNetEntity(camera),
                    GetCameraName(camera, component),
                    editableNetworks.Where(memberships.Contains).ToList());
=======
                var member = Comp<CameraNetworkMemberComponent>(camera);
                var memberships = member.Networks.Select(_cameraNetworks.ResolveNetwork)
                    .Concat(member.RuntimeNetworks)
                    .ToHashSet();
                return new RMCCameraNetworkEditorCameraUiData(
                    GetNetEntity(camera),
                    GetCameraName(camera, component),
                    editableNetworks.Where(network => network is { } id && memberships.Contains(id))
                        .Select(network => GetNetEntity(network))
                        .ToList());
>>>>>>> cmu/master
            })
            .ToList();

        return new RMCCameraNetworkEditorUiState(editor.Revision, networks, cameras);
    }

    public IReadOnlyList<EntityUid> GetEditableCameras()
    {
        var cameras = new List<(EntityUid Camera, string Name)>();
        var query = EntityQueryEnumerator<RMCCameraComponent,
            SurveillanceCameraComponent,
            CameraNetworkMemberComponent>();
        while (query.MoveNext(out var uid, out var rmc, out var surveillance, out var member))
        {
            if (!IsEditableCamera((uid, rmc, surveillance, member)))
                continue;

            cameras.Add((uid, GetCameraName(uid, rmc)));
        }

        return cameras
            .OrderBy(camera => camera.Name, StringComparer.Ordinal)
            .ThenBy(camera => camera.Camera.Id)
            .Select(camera => camera.Camera)
            .ToList();
    }

    public bool TrySaveEditorCamera(
        Entity<RMCCameraComputerComponent> computer,
        EntityUid actor,
        uint revision,
        NetEntity camera,
        string name,
<<<<<<< HEAD
        IReadOnlyCollection<ProtoId<CameraNetworkPrototype>> networks,
=======
        IReadOnlyCollection<EntityUid> networks,
>>>>>>> cmu/master
        out RMCCameraNetworkEditorError error)
    {
        var editor = EnsureEditorState(computer);
        if (!TryValidateMutation(computer, editor, actor, revision, out error))
            return false;

        var normalized = name.Trim();
        if (normalized.Length is 0 or > MaxCameraNameLength)
        {
            error = RMCCameraNetworkEditorError.InvalidName;
            return false;
        }

        if (!TryGetEntity(camera, out var cameraUid) ||
            cameraUid is not { } uid ||
            !TryComp(uid, out RMCCameraComponent? rmc) ||
            !TryComp(uid, out SurveillanceCameraComponent? surveillance) ||
            !TryComp(uid, out CameraNetworkMemberComponent? member) ||
            !IsEditableCamera((uid, rmc, surveillance, member)))
        {
            error = RMCCameraNetworkEditorError.MissingCamera;
            return false;
        }

        var editableNetworks = editor.SeededNetworks
            .Where(network => !editor.HiddenSeededNetworks.Contains(network))
            .Concat(editor.OwnedNetworks.Keys)
            .ToHashSet();
        var selectedNetworks = networks.ToHashSet();
        if (selectedNetworks.Any(network => !editableNetworks.Contains(network)))
        {
            error = RMCCameraNetworkEditorError.InvalidNetwork;
            return false;
        }

<<<<<<< HEAD
        var preservedNetworks = member.Networks
            .Where(network => !editableNetworks.Contains(network));
        var updatedNetworks = preservedNetworks.Concat(selectedNetworks).ToHashSet();
        var oldName = GetCameraName(uid, rmc);
        var oldNetworks = member.Networks.ToHashSet();
=======
        var currentNetworks = member.Networks.Select(_cameraNetworks.ResolveNetwork)
            .Concat(member.RuntimeNetworks)
            .ToHashSet();
        var preservedNetworks = currentNetworks.Where(network => !editableNetworks.Contains(network));
        var updatedNetworks = preservedNetworks.Concat(selectedNetworks).ToHashSet();
        var oldName = GetCameraName(uid, rmc);
        var oldNetworks = currentNetworks;
>>>>>>> cmu/master

        if (!string.Equals(oldName, normalized, StringComparison.Ordinal))
        {
            _metaDataSystem.SetEntityName(uid, normalized);
            SetCameraName(uid, normalized, rmc);
        }

<<<<<<< HEAD
        if (!member.Networks.SetEquals(updatedNetworks))
            _cameraNetworks.SetMemberNetworks(uid, updatedNetworks);
=======
        if (!currentNetworks.SetEquals(updatedNetworks))
            _cameraNetworks.SetMemberNetworkEntities(uid, updatedNetworks);
>>>>>>> cmu/master

        editor.Revision++;
        _adminLogger.Add(LogType.Action, LogImpact.Medium,
            $"{ToPrettyString(actor):player} edited {ToPrettyString(uid):camera} on {ToPrettyString(computer.Owner):console}: " +
            $"name '{oldName}' -> '{normalized}', networks [{string.Join(", ", oldNetworks)}] -> " +
            $"[{string.Join(", ", updatedNetworks)}]");
        RefreshAfterEditorMutation(computer);
        error = RMCCameraNetworkEditorError.None;
        return true;
    }

    public bool TryResolveNetworkName(
        EntityUid computer,
<<<<<<< HEAD
        ProtoId<CameraNetworkPrototype> network,
        out string name)
    {
        if (TryComp(computer, out RMCCameraComputerComponent? cameraComputer))
        {
            var editor = EnsureEditorState((computer, cameraComputer));
=======
        EntityUid network,
        out string name)
    {
        if (TryComp(computer, out RMCCameraNetworkEditorComponent? editor))
        {
>>>>>>> cmu/master
            if (editor.OwnedNetworks.TryGetValue(network, out name!) ||
                editor.Aliases.TryGetValue(network, out name!))
            {
                return true;
            }
        }

<<<<<<< HEAD
        if (_prototypeManager.TryIndex<CameraNetworkPrototype>(network, out var prototype))
        {
            name = Loc.GetString(prototype.Name);
=======
        if (TryComp(network, out CameraNetworkIdentityComponent? identity))
        {
            name = identity.DisplayName;
>>>>>>> cmu/master
            return true;
        }

        name = string.Empty;
        return false;
    }

    public bool TryCreateEditorNetwork(
        Entity<RMCCameraComputerComponent> computer,
        EntityUid actor,
        uint revision,
        string name,
        out RMCCameraNetworkEditorError error)
    {
        var editor = EnsureEditorState(computer);
        if (!TryValidateMutation(computer, editor, actor, revision, out error) ||
            !TryNormalizeNetworkName(computer.Owner, editor, name, null, out var normalized, out error))
        {
            return false;
        }

<<<<<<< HEAD
        var network = NextRuntimeNetwork(computer.Owner, editor);
        editor.OwnedNetworks.Add(network, normalized);
        editor.Revision++;

        var receiver = Comp<CameraNetworkReceiverComponent>(computer);
        var updated = receiver.Networks.Append(network).ToHashSet();
        if (!_cameraNetworks.SetReceiverNetworks(computer.Owner, updated))
        {
            editor.OwnedNetworks.Remove(network);
=======
        var network = _cameraNetworks.CreateNetwork(normalized, computer.Owner);
        editor.OwnedNetworks.Add(network, normalized);
        editor.Revision++;

        if (!_cameraNetworks.AddReceiverNetwork(computer.Owner, network))
        {
            editor.OwnedNetworks.Remove(network);
            _cameraNetworks.DeleteNetwork(network);
>>>>>>> cmu/master
            editor.Revision--;
            error = RMCCameraNetworkEditorError.InvalidNetwork;
            return false;
        }

        _adminLogger.Add(LogType.Action, LogImpact.Medium,
            $"{ToPrettyString(actor):player} created camera network '{normalized}' ({network}) on {ToPrettyString(computer.Owner):console}");
        RefreshAfterEditorMutation(computer);
        error = RMCCameraNetworkEditorError.None;
        return true;
    }

    public bool TryRenameEditorNetwork(
        Entity<RMCCameraComputerComponent> computer,
        EntityUid actor,
        uint revision,
<<<<<<< HEAD
        ProtoId<CameraNetworkPrototype> network,
=======
        EntityUid network,
>>>>>>> cmu/master
        string name,
        out RMCCameraNetworkEditorError error)
    {
        var editor = EnsureEditorState(computer);
        if (!TryValidateMutation(computer, editor, actor, revision, out error))
            return false;

        var seeded = editor.SeededNetworks.Contains(network);
        var owned = editor.OwnedNetworks.ContainsKey(network);
        if (!seeded && !owned)
        {
            error = RMCCameraNetworkEditorError.InvalidNetwork;
            return false;
        }

        if (!TryNormalizeNetworkName(computer.Owner, editor, name, network, out var normalized, out error))
            return false;

        var oldName = ResolveNetworkName(computer.Owner, network, editor);
        if (seeded)
            editor.Aliases[network] = normalized;
        else
            editor.OwnedNetworks[network] = normalized;
        editor.Revision++;

        _adminLogger.Add(LogType.Action, LogImpact.Medium,
            $"{ToPrettyString(actor):player} renamed camera network '{oldName}' to '{normalized}' on {ToPrettyString(computer.Owner):console}");
        RefreshAfterEditorMutation(computer);
        error = RMCCameraNetworkEditorError.None;
        return true;
    }

    public bool TryDeleteEditorNetwork(
        Entity<RMCCameraComputerComponent> computer,
        EntityUid actor,
        uint revision,
<<<<<<< HEAD
        ProtoId<CameraNetworkPrototype> network,
=======
        EntityUid network,
>>>>>>> cmu/master
        out RMCCameraNetworkEditorError error)
    {
        var editor = EnsureEditorState(computer);
        if (!TryValidateMutation(computer, editor, actor, revision, out error))
            return false;

        if (editor.SeededNetworks.Contains(network))
        {
            error = RMCCameraNetworkEditorError.SeededNetworkCannotBeDeleted;
            return false;
        }

        if (!editor.OwnedNetworks.Remove(network, out var oldName))
        {
            error = RMCCameraNetworkEditorError.InvalidNetwork;
            return false;
        }

        editor.Revision++;
<<<<<<< HEAD
        var receiver = Comp<CameraNetworkReceiverComponent>(computer);
        _cameraNetworks.SetReceiverNetworks(computer.Owner, receiver.Networks.Where(existing => existing != network));

        var updates = new Dictionary<EntityUid, IReadOnlyCollection<ProtoId<CameraNetworkPrototype>>>();
        foreach (var member in _cameraNetworks.GetNetworkMembers(network))
        {
            if (TryComp(member, out CameraNetworkMemberComponent? memberComponent))
                updates[member] = memberComponent.Networks.Where(existing => existing != network).ToHashSet();
        }

        if (updates.Count > 0)
            _cameraNetworks.SetMemberNetworksBatch(updates);
=======
        _cameraNetworks.RemoveReceiverNetwork(computer.Owner, network);

        foreach (var member in _cameraNetworks.GetNetworkMembers(network))
        {
            if (TryComp(member, out CameraNetworkMemberComponent? memberComponent))
            {
                var updated = memberComponent.Networks.Select(_cameraNetworks.ResolveNetwork)
                    .Concat(memberComponent.RuntimeNetworks)
                    .Where(existing => existing != network)
                    .ToHashSet();
                _cameraNetworks.SetMemberNetworkEntities(member, updated);
            }
        }

        _cameraNetworks.DeleteNetwork(network);
>>>>>>> cmu/master

        _adminLogger.Add(LogType.Action, LogImpact.Medium,
            $"{ToPrettyString(actor):player} deleted camera network '{oldName}' ({network}) from {ToPrettyString(computer.Owner):console}");
        RefreshAfterEditorMutation(computer);
        error = RMCCameraNetworkEditorError.None;
        return true;
    }

    public bool TrySetSeededNetworkHidden(
        Entity<RMCCameraComputerComponent> computer,
        EntityUid actor,
        uint revision,
<<<<<<< HEAD
        ProtoId<CameraNetworkPrototype> network,
=======
        EntityUid network,
>>>>>>> cmu/master
        bool hidden,
        out RMCCameraNetworkEditorError error)
    {
        var editor = EnsureEditorState(computer);
        if (!TryValidateMutation(computer, editor, actor, revision, out error))
            return false;

        if (!editor.SeededNetworks.Contains(network))
        {
            error = RMCCameraNetworkEditorError.InvalidNetwork;
            return false;
        }

        var changed = hidden
            ? editor.HiddenSeededNetworks.Add(network)
            : editor.HiddenSeededNetworks.Remove(network);
        if (!changed)
        {
            error = RMCCameraNetworkEditorError.None;
            return true;
        }

        editor.Revision++;
        _adminLogger.Add(LogType.Action, LogImpact.Medium,
            $"{ToPrettyString(actor):player} {(hidden ? "hid" : "restored")} camera network {network} on {ToPrettyString(computer.Owner):console}");
        RefreshAfterEditorMutation(computer);
        error = RMCCameraNetworkEditorError.None;
        return true;
    }

    protected override void OnEditorCreate(
        Entity<RMCCameraComputerComponent> computer,
        RMCCameraNetworkEditorCreateBuiMsg args)
    {
        if (!IsEditorUiOpen(computer, args.Actor))
            return;

        TryCreateEditorNetwork(computer, args.Actor, args.Revision, args.Name, out var error);
        FinishEditorCommand(computer, args.Actor, error);
    }

    protected override void OnEditorRename(
        Entity<RMCCameraComputerComponent> computer,
        RMCCameraNetworkEditorRenameBuiMsg args)
    {
        if (!IsEditorUiOpen(computer, args.Actor))
            return;

<<<<<<< HEAD
        TryRenameEditorNetwork(computer, args.Actor, args.Revision, args.Network, args.Name, out var error);
=======
        if (!TryGetEntity(args.Network, out var network) || network is not { } networkUid)
            return;

        TryRenameEditorNetwork(computer, args.Actor, args.Revision, networkUid, args.Name, out var error);
>>>>>>> cmu/master
        FinishEditorCommand(computer, args.Actor, error);
    }

    protected override void OnEditorDelete(
        Entity<RMCCameraComputerComponent> computer,
        RMCCameraNetworkEditorDeleteBuiMsg args)
    {
        if (!IsEditorUiOpen(computer, args.Actor))
            return;

<<<<<<< HEAD
        TryDeleteEditorNetwork(computer, args.Actor, args.Revision, args.Network, out var error);
=======
        if (!TryGetEntity(args.Network, out var network) || network is not { } networkUid)
            return;

        TryDeleteEditorNetwork(computer, args.Actor, args.Revision, networkUid, out var error);
>>>>>>> cmu/master
        FinishEditorCommand(computer, args.Actor, error);
    }

    protected override void OnEditorSetHidden(
        Entity<RMCCameraComputerComponent> computer,
        RMCCameraNetworkEditorSetHiddenBuiMsg args)
    {
        if (!IsEditorUiOpen(computer, args.Actor))
            return;

<<<<<<< HEAD
        TrySetSeededNetworkHidden(computer, args.Actor, args.Revision, args.Network, args.Hidden, out var error);
=======
        if (!TryGetEntity(args.Network, out var network) || network is not { } networkUid)
            return;

        TrySetSeededNetworkHidden(computer, args.Actor, args.Revision, networkUid, args.Hidden, out var error);
>>>>>>> cmu/master
        FinishEditorCommand(computer, args.Actor, error);
    }

    protected override void OnEditorSaveCamera(
        Entity<RMCCameraComputerComponent> computer,
        RMCCameraNetworkEditorSaveCameraBuiMsg args)
    {
        if (!IsEditorUiOpen(computer, args.Actor))
            return;

<<<<<<< HEAD
        TrySaveEditorCamera(computer, args.Actor, args.Revision, args.Camera, args.Name, args.Networks, out var error);
        FinishEditorCommand(computer, args.Actor, error);
    }

    protected override void OnCameraRemoved(Entity<RMCCameraComponent> camera)
=======
        var networks = new List<EntityUid>();
        foreach (var netNetwork in args.Networks)
        {
            if (!TryGetEntity(netNetwork, out var network) || network is not { } networkUid)
                return;
            networks.Add(networkUid);
        }

        TrySaveEditorCamera(computer, args.Actor, args.Revision, args.Camera, args.Name, networks, out var error);
        FinishEditorCommand(computer, args.Actor, error);
    }

    private void OnEditorCameraShutdown(Entity<RMCCameraComponent> camera, ref ComponentShutdown args)
>>>>>>> cmu/master
    {
        var query = EntityQueryEnumerator<RMCCameraComputerComponent>();
        while (query.MoveNext(out var computerUid, out var computer))
        {
            if (TerminatingOrDeleted(computerUid))
                continue;

<<<<<<< HEAD
            RebuildComputerCameras(computerUid, computer);
=======
>>>>>>> cmu/master
            UpdateUserInterface((computerUid, computer));
        }
    }

    protected override void OnCameraEditorRoundRestartCleanup()
    {
        var query = EntityQueryEnumerator<RMCCameraNetworkEditorComponent>();
        while (query.MoveNext(out var uid, out var editor))
        {
            CleanupEditorNetworks(uid, editor);
            editor.Aliases.Clear();
            editor.HiddenSeededNetworks.Clear();
            editor.OwnedNetworks.Clear();
            editor.Revision = 0;
<<<<<<< HEAD
            editor.NextOwnedNetworkId = 1;
=======
>>>>>>> cmu/master

            if (TryComp(uid, out RMCCameraComputerComponent? computer))
                RefreshAfterEditorMutation((uid, computer));
        }
    }

    private void OnCameraEditorShutdown(
        Entity<RMCCameraNetworkEditorComponent> editor,
        ref ComponentShutdown args)
    {
        CleanupEditorNetworks(editor.Owner, editor.Comp);
    }

    private void CleanupEditorNetworks(EntityUid computer, RMCCameraNetworkEditorComponent editor)
    {
        if (editor.OwnedNetworks.Count == 0)
            return;

        var owned = editor.OwnedNetworks.Keys.ToHashSet();
<<<<<<< HEAD
        if (TryComp(computer, out CameraNetworkReceiverComponent? receiver))
            _cameraNetworks.SetReceiverNetworks(computer, receiver.Networks.Where(network => !owned.Contains(network)));

        var updates = new Dictionary<EntityUid, IReadOnlyCollection<ProtoId<CameraNetworkPrototype>>>();
=======
        var receiverNetworks = _cameraNetworks.GetEffectiveNetworkEntities(computer)
            .Where(network => !owned.Contains(network))
            .ToHashSet();
        _cameraNetworks.SetReceiverNetworkEntities(computer, receiverNetworks);

>>>>>>> cmu/master
        foreach (var network in owned)
        {
            foreach (var member in _cameraNetworks.GetNetworkMembers(network))
            {
                if (TryComp(member, out CameraNetworkMemberComponent? component))
<<<<<<< HEAD
                    updates[member] = component.Networks.Where(existing => !owned.Contains(existing)).ToHashSet();
            }
        }

        if (updates.Count > 0)
            _cameraNetworks.SetMemberNetworksBatch(updates);
=======
                {
                    var memberNetworks = component.Networks.Select(_cameraNetworks.ResolveNetwork)
                        .Concat(component.RuntimeNetworks)
                        .Where(existing => !owned.Contains(existing))
                        .ToHashSet();
                    _cameraNetworks.SetMemberNetworkEntities(member, memberNetworks);
                }
            }

            _cameraNetworks.DeleteNetwork(network);
        }
>>>>>>> cmu/master
    }

    private bool IsEditorUiOpen(Entity<RMCCameraComputerComponent> computer, EntityUid actor)
    {
<<<<<<< HEAD
        return !TerminatingOrDeleted(actor) &&
=======
        return _configuration.GetCVar(CCVars.CMUCameraEditorEnabled) &&
            !TerminatingOrDeleted(actor) &&
>>>>>>> cmu/master
            _userInterface.IsUiOpen(computer.Owner, RMCCameraUiKey.Key, actor);
    }

    private void FinishEditorCommand(
        Entity<RMCCameraComputerComponent> computer,
        EntityUid actor,
        RMCCameraNetworkEditorError error)
    {
        if (error == RMCCameraNetworkEditorError.None)
            return;

        UpdateUserInterface(computer);
        var revision = EnsureEditorState(computer).Revision;
        _userInterface.ServerSendUiMessage(
            computer.Owner,
            RMCCameraUiKey.Key,
            new RMCCameraNetworkEditorResultBuiMsg(error, revision),
            actor);
    }

    private RMCCameraNetworkEditorComponent EnsureEditorState(Entity<RMCCameraComputerComponent> computer)
    {
        if (TryComp(computer, out RMCCameraNetworkEditorComponent? editor))
            return editor;

        editor = AddComp<RMCCameraNetworkEditorComponent>(computer);
        if (TryComp(computer, out CameraNetworkReceiverComponent? receiver))
<<<<<<< HEAD
            editor.SeededNetworks.UnionWith(receiver.Networks);
=======
            editor.SeededNetworks.UnionWith(receiver.Networks.Select(_cameraNetworks.ResolveNetwork));
>>>>>>> cmu/master
        return editor;
    }

    private bool IsEditableCamera(
        Entity<RMCCameraComponent,
            SurveillanceCameraComponent,
            CameraNetworkMemberComponent> camera)
    {
        return !TerminatingOrDeleted(camera.Owner)
            && !Paused(camera.Owner)
            && camera.Comp2.Active
            && (camera.Comp3.SourceKinds & CameraSourceKinds.Rmc) != CameraSourceKinds.None
            && !HasComp<ItemComponent>(camera.Owner)
            && !HasComp<MortarCameraComponent>(camera.Owner);
    }

    private string ResolveNetworkName(
        EntityUid computer,
<<<<<<< HEAD
        ProtoId<CameraNetworkPrototype> network,
=======
        EntityUid network,
>>>>>>> cmu/master
        RMCCameraNetworkEditorComponent editor)
    {
        if (editor.OwnedNetworks.TryGetValue(network, out var owned) ||
            editor.Aliases.TryGetValue(network, out owned))
        {
            return owned;
        }

<<<<<<< HEAD
        return _prototypeManager.TryIndex<CameraNetworkPrototype>(network, out var prototype)
            ? Loc.GetString(prototype.Name)
=======
        return TryComp(network, out CameraNetworkIdentityComponent? identity)
            ? identity.DisplayName
>>>>>>> cmu/master
            : network.ToString();
    }

    private bool TryValidateMutation(
        Entity<RMCCameraComputerComponent> computer,
        RMCCameraNetworkEditorComponent editor,
        EntityUid actor,
        uint revision,
        out RMCCameraNetworkEditorError error)
    {
<<<<<<< HEAD
        if (TerminatingOrDeleted(actor) || !_accessReader.IsAllowed(actor, computer.Owner))
=======
        if (!_configuration.GetCVar(CCVars.CMUCameraEditorEnabled) ||
            TerminatingOrDeleted(actor) ||
            !_accessReader.IsAllowed(actor, computer.Owner))
>>>>>>> cmu/master
        {
            error = RMCCameraNetworkEditorError.AccessDenied;
            return false;
        }

        if (revision != editor.Revision)
        {
            error = RMCCameraNetworkEditorError.StaleRevision;
            return false;
        }

        error = RMCCameraNetworkEditorError.None;
        return true;
    }

    private bool TryNormalizeNetworkName(
        EntityUid computer,
        RMCCameraNetworkEditorComponent editor,
        string raw,
<<<<<<< HEAD
        ProtoId<CameraNetworkPrototype>? except,
=======
        EntityUid? except,
>>>>>>> cmu/master
        out string normalized,
        out RMCCameraNetworkEditorError error)
    {
        normalized = raw.Trim();
        if (normalized.Length is 0 or > MaxNetworkNameLength)
        {
            error = RMCCameraNetworkEditorError.InvalidName;
            return false;
        }

        foreach (var network in editor.SeededNetworks.Concat(editor.OwnedNetworks.Keys))
        {
            if (except is { } exceptNetwork && network == exceptNetwork)
                continue;

            if (string.Equals(
                    ResolveNetworkName(computer, network, editor),
                    normalized,
                    StringComparison.OrdinalIgnoreCase))
            {
                error = RMCCameraNetworkEditorError.DuplicateName;
                return false;
            }
        }

        error = RMCCameraNetworkEditorError.None;
        return true;
    }

<<<<<<< HEAD
    private ProtoId<CameraNetworkPrototype> NextRuntimeNetwork(
        EntityUid computer,
        RMCCameraNetworkEditorComponent editor)
    {
        while (true)
        {
            var candidate = (ProtoId<CameraNetworkPrototype>)
                $"{RuntimeNetworkPrefix}{computer.Id}N{editor.NextOwnedNetworkId++}";
            if (_prototypeManager.HasIndex<CameraNetworkPrototype>(candidate))
                continue;

            var collision = false;
            var query = EntityQueryEnumerator<RMCCameraNetworkEditorComponent>();
            while (query.MoveNext(out _, out var other))
            {
                if (!other.OwnedNetworks.ContainsKey(candidate))
                    continue;

                collision = true;
                break;
            }

            if (!collision)
                return candidate;
        }
    }

    private void RefreshAfterEditorMutation(Entity<RMCCameraComputerComponent> computer)
    {
        RebuildComputerCameras(computer.Owner, computer.Comp);
=======
    private void RefreshAfterEditorMutation(Entity<RMCCameraComputerComponent> computer)
    {
>>>>>>> cmu/master
        UpdateUserInterface(computer);
    }
}
