using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NetworkManager : SDSingleton<NetworkManager>, IPunCallbacks
{
    [SerializeField] private int _sendRate = 32;
    public int SendRate
    {
        get => _sendRate;
        set
        {
            _sendRate = value;
            PhotonNetwork.sendRate = _sendRate;
            PhotonNetwork.sendRateOnSerialize = _sendRate;
        }
    }

    private void Start()
    {
        // 포톤 네트워크에 Resources 폴더 내의 세팅 파일을 기반으로 연결합니다.
        PhotonNetwork.ConnectUsingSettings(Application.version);

        // 포톤 네트워크 Send Rate를 적용하기 위해 값을 재지정합니다.
        SendRate = SendRate;
    }

    /// <summary>
    /// Called when the initial connection got established but before you can use the server. OnJoinedLobby() or OnConnectedToMaster() are called when PUN is ready.
    /// </summary>
    /// <remarks>
    /// This callback is only useful to detect if the server can be reached at all (technically).
    /// Most often, it's enough to implement OnFailedToConnectToPhoton() and OnDisconnectedFromPhoton().
    ///
    /// <i>OnJoinedLobby() or OnConnectedToMaster() are called when PUN is ready.</i>
    ///
    /// When this is called, the low level connection is established and PUN will send your AppId, the user, etc in the background.
    /// This is not called for transitions from the masterserver to game servers.
    /// </remarks>
     public void OnConnectedToPhoton() { Debug.Log("Connected to Photon"); }

    /// <summary>
    /// Called when the local user/client left a room.
    /// </summary>
    /// <remarks>
    /// When leaving a room, PUN brings you back to the Master Server.
    /// Before you can use lobbies and join or create rooms, OnJoinedLobby() or OnConnectedToMaster() will get called again.
    /// </remarks>
    public void OnLeftRoom() { }

    /// <summary>
    /// Called after switching to a new MasterClient when the current one leaves.
    /// </summary>
    /// <remarks>
    /// This is not called when this client enters a room.
    /// The former MasterClient is still in the player list when this method get called.
    /// </remarks>
    public void OnMasterClientSwitched(PhotonPlayer newMasterClient) { }

    /// <summary>
    /// Called when a CreateRoom() call failed. The parameter provides ErrorCode and message (as array).
    /// </summary>
    /// <remarks>
    /// Most likely because the room name is already in use (some other client was faster than you).
    /// PUN logs some info if the PhotonNetwork.logLevel is >= PhotonLogLevel.Informational.
    /// </remarks>
    /// <param name="codeAndMsg">codeAndMsg[0] is short ErrorCode and codeAndMsg[1] is a string debug msg.</param>
    public void OnPhotonCreateRoomFailed(object[] codeAndMsg) { }

    /// <summary>
    /// Called when a JoinRoom() call failed. The parameter provides ErrorCode and message (as array).
    /// </summary>
    /// <remarks>
    /// Most likely error is that the room does not exist or the room is full (some other client was faster than you).
    /// PUN logs some info if the PhotonNetwork.logLevel is >= PhotonLogLevel.Informational.
    /// </remarks>
    /// <param name="codeAndMsg">codeAndMsg[0] is short ErrorCode and codeAndMsg[1] is string debug msg.</param>
    public void OnPhotonJoinRoomFailed(object[] codeAndMsg) { }

    /// <summary>
    /// Called when this client created a room and entered it. OnJoinedRoom() will be called as well.
    /// </summary>
    /// <remarks>
    /// This callback is only called on the client which created a room (see PhotonNetwork.CreateRoom).
    ///
    /// As any client might close (or drop connection) anytime, there is a chance that the
    /// creator of a room does not execute OnCreatedRoom.
    ///
    /// If you need specific room properties or a "start signal", it is safer to implement
    /// OnMasterClientSwitched() and to make the new MasterClient check the room's state.
    /// </remarks>
    public void OnCreatedRoom() { }

    /// <summary>
    /// Called on entering a lobby on the Master Server. The actual room-list updates will call OnReceivedRoomListUpdate().
    /// </summary>
    /// <remarks>
    /// Note: When PhotonNetwork.autoJoinLobby is false, OnConnectedToMaster() will be called and the room list won't become available.
    ///
    /// While in the lobby, the roomlist is automatically updated in fixed intervals (which you can't modify).
    /// The room list gets available when OnReceivedRoomListUpdate() gets called after OnJoinedLobby().
    /// </remarks>
    public void OnJoinedLobby()
    {
        Debug.Log("Connected to Lobby");
        RoomOptions options = new RoomOptions();
        TypedLobby typed = new TypedLobby();
        PhotonNetwork.JoinOrCreateRoom("test", options, typed);

    }

    /// <summary>
    /// Called after leaving a lobby.
    /// </summary>
    /// <remarks>
    /// When you leave a lobby, [CreateRoom](@ref PhotonNetwork.CreateRoom) and [JoinRandomRoom](@ref PhotonNetwork.JoinRandomRoom)
    /// automatically refer to the default lobby.
    /// </remarks>
    public void OnLeftLobby() { }

    /// <summary>
    /// Called if a connect call to the Photon server failed before the connection was established, followed by a call to OnDisconnectedFromPhoton().
    /// </summary>
    /// <remarks>
    /// This is called when no connection could be established at all.
    /// It differs from OnConnectionFail, which is called when an existing connection fails.
    /// </remarks>
    public void OnFailedToConnectToPhoton(DisconnectCause cause) { }

    /// <summary>
    /// Called when something causes the connection to fail (after it was established), followed by a call to OnDisconnectedFromPhoton().
    /// </summary>
    /// <remarks>
    /// If the server could not be reached in the first place, OnFailedToConnectToPhoton is called instead.
    /// The reason for the error is provided as DisconnectCause.
    /// </remarks>
    public void OnConnectionFail(DisconnectCause cause) { }

    /// <summary>
    /// Called after disconnecting from the Photon server.
    /// </summary>
    /// <remarks>
    /// In some cases, other callbacks are called before OnDisconnectedFromPhoton is called.
    /// Examples: OnConnectionFail() and OnFailedToConnectToPhoton().
    /// </remarks>
    public void OnDisconnectedFromPhoton() { }

    /// <summary>
    /// Called on all scripts on a GameObject (and children) that have been Instantiated using PhotonNetwork.Instantiate.
    /// </summary>
    /// <remarks>
    /// PhotonMessageInfo parameter provides info about who created the object and when (based off PhotonNetworking.time).
    /// </remarks>
    public void OnPhotonInstantiate(PhotonMessageInfo info) { }

    /// <summary>
    /// Called for any update of the room-listing while in a lobby (PhotonNetwork.insideLobby) on the Master Server
    /// or when a response is received for PhotonNetwork.GetCustomRoomList().
    /// </summary>
    /// <remarks>
    /// PUN provides the list of rooms by PhotonNetwork.GetRoomList().<br/>
    /// Each item is a RoomInfo which might include custom properties (provided you defined those as lobby-listed when creating a room).
    ///
    /// Not all types of lobbies provide a listing of rooms to the client. Some are silent and specialized for server-side matchmaking.
    /// </remarks>
    public void OnReceivedRoomListUpdate() { }

    /// <summary>
    /// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
    /// </summary>
    /// <remarks>
    /// This method is commonly used to instantiate player characters.
    /// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
    ///
    /// When this is called, you can usually already access the existing players in the room via PhotonNetwork.playerList.
    /// Also, all custom properties should be already available as Room.customProperties. Check Room.playerCount to find out if
    /// enough players are in the room to start playing.
    /// </remarks>
    public void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate("Character Controller Player", new Vector3(), Quaternion.identity, 0);
    }

    /// <summary>
    /// Called when a remote player entered the room. This PhotonPlayer is already added to the playerlist at this time.
    /// </summary>
    /// <remarks>
    /// If your game starts with a certain number of players, this callback can be useful to check the
    /// Room.playerCount and find out if you can start.
    /// </remarks>
    public void OnPhotonPlayerConnected(PhotonPlayer newPlayer) { }

    /// <summary>
    /// Called when a remote player left the room. This PhotonPlayer is already removed from the playerlist at this time.
    /// </summary>
    /// <remarks>
    /// When your client calls PhotonNetwork.leaveRoom, PUN will call this method on the remaining clients.
    /// When a remote client drops connection or gets closed, this callback gets executed. after a timeout
    /// of several seconds.
    /// </remarks>
    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) { }

    /// <summary>
    /// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
    /// </summary>
    /// <remarks>
    /// Most likely all rooms are full or no rooms are available. <br/>
    /// When using multiple lobbies (via JoinLobby or TypedLobby), another lobby might have more/fitting rooms.<br/>
    /// PUN logs some info if the PhotonNetwork.logLevel is >= PhotonLogLevel.Informational.
    /// </remarks>
    /// <param name="codeAndMsg">codeAndMsg[0] is short ErrorCode. codeAndMsg[1] is string debug msg.</param>
    public void OnPhotonRandomJoinFailed(object[] codeAndMsg) { }

    /// <summary>
    /// Called after the connection to the master is established and authenticated but only when PhotonNetwork.autoJoinLobby is false.
    /// </summary>
    /// <remarks>
    /// If you set PhotonNetwork.autoJoinLobby to true, OnJoinedLobby() will be called instead of this.
    ///
    /// You can join rooms and create them even without being in a lobby. The default lobby is used in that case.
    /// The list of available rooms won't become available unless you join a lobby via PhotonNetwork.joinLobby.
    /// </remarks>
    public void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// Because the concurrent user limit was (temporarily) reached, this client is rejected by the server and disconnecting.
    /// </summary>
    /// <remarks>
    /// When this happens, the user might try again later. You can't create or join rooms in OnPhotonMaxCcuReached(), cause the client will be disconnecting.
    /// You can raise the CCU limits with a new license (when you host yourself) or extended subscription (when using the Photon Cloud).
    /// The Photon Cloud will mail you when the CCU limit was reached. This is also visible in the Dashboard (webpage).
    /// </remarks>
    public void OnPhotonMaxCccuReached() { }

    /// <summary>
    /// Called when a room's custom properties changed. The propertiesThatChanged contains all that was set via Room.SetCustomProperties.
    /// </summary>
    /// <remarks>
    /// Since v1.25 this method has one parameter: Hashtable propertiesThatChanged.<br/>
    /// Changing properties must be done by Room.SetCustomProperties, which causes this callback locally, too.
    /// </remarks>
    /// <param name="propertiesThatChanged"></param>
    public void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged) { }

    /// <summary>
    /// Called when custom player-properties are changed. Player and the changed properties are passed as object[].
    /// </summary>
    /// <remarks>
    /// Since v1.25 this method has one parameter: object[] playerAndUpdatedProps, which contains two entries.<br/>
    /// [0] is the affected PhotonPlayer.<br/>
    /// [1] is the Hashtable of properties that changed.<br/>
    ///
    /// We are using a object[] due to limitations of Unity's GameObject.SendMessage (which has only one optional parameter).
    ///
    /// Changing properties must be done by PhotonPlayer.SetCustomProperties, which causes this callback locally, too.
    ///
    /// Example:<pre>
    /// void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps) {
    ///      photonPlayer player = playerAndUpdatedProps[0] as PhotonPlayer;
    ///     Hashtable props = playerAndUpdatedProps[1] as Hashtable;
    ///     //...
    /// }</pre>
    /// </remarks>
    /// <param name="playerAndUpdatedProps">Contains PhotonPlayer and the properties that changed See remarks.</param>
    public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps) { }

    /// <summary>
    /// Called when the server sent the response to a FindFriends request and updated PhotonNetwork.Friends.
    /// </summary>
    /// <remarks>
    /// The friends list is available as PhotonNetwork.Friends, listing name, online state and
    /// the room a user is in (if any).
    /// </remarks>
    public void OnUpdatedFriendList() { }

    /// <summary>
    /// Called when the custom authentication failed. Followed by disconnect!
    /// </summary>
    /// <remarks>
    /// Custom Authentication can fail due to user-input, bad tokens/secrets.
    /// If authentication is successful, this method is not called. Implement OnJoinedLobby() or OnConnectedToMaster() (as usual).
    ///
    /// During development of a game, it might also fail due to wrong configuration on the server side.
    /// In those cases, logging the debugMessage is very important.
    ///
    /// Unless you setup a custom authentication service for your app (in the [Dashboard](https://www.photonengine.com/dashboard)),
    /// this won't be called!
    /// </remarks>
    /// <param name="debugMessage">Contains a debug message why authentication failed. This has to be fixed during development time.</param>
    public void OnCustomAuthenticationFailed(string debugMessage) { }

    /// <summary>
    /// Called when your Custom Authentication service responds with additional data.
    /// </summary>
    /// <remarks>
    /// Custom Authentication services can include some custom data in their response.
    /// When present, that data is made available in this callback as Dictionary.
    /// While the keys of your data have to be strings, the values can be either string or a number (in Json).
    /// You need to make extra sure, that the value type is the one you expect. Numbers become (currently) int64.
    ///
    /// Example: void OnCustomAuthenticationResponse(Dictionary&lt;string, object&gt; data) { ... }
    /// </remarks>
    /// <see cref="https://doc.photonengine.com/en-us/pun/current/connection-and-authentication/custom-authentication"/>
    public void OnCustomAuthenticationResponse(Dictionary<string, object> data) { }

    /// <summary>
    /// Called by PUN when the response to a WebRPC is available. See PhotonNetwork.WebRPC.
    /// </summary>
    /// <remarks>
    /// Important: The response.ReturnCode is 0 if Photon was able to reach your web-service.<br/>
    /// The content of the response is what your web-service sent. You can create a WebRpcResponse from it.<br/>
    /// Example: WebRpcResponse webResponse = new WebRpcResponse(operationResponse) { }<br/>
    ///
    /// Please note: Class OperationResponse is in a namespace which needs to be "used":<br/>
    /// using ExitGames.Client.Photon;  // includes OperationResponse (and other classes)
    ///
    /// The OperationResponse.ReturnCode by Photon is:<pre>
    ///  0 for "OK"
    /// -3 for "Web-Service not configured" (see Dashboard / WebHooks)
    /// -5 for "Web-Service does now have RPC path/name" (at least for Azure)</pre>
    /// </remarks>
    public void OnWebRpcResponse(OperationResponse response) { }

    /// <summary>
    /// Called when another player requests ownership of a PhotonView from you (the current owner).
    /// </summary>
    /// <remarks>
    /// The parameter viewAndPlayer contains:
    ///
    /// PhotonView view = viewAndPlayer[0] as PhotonView;
    ///
    /// PhotonPlayer requestingPlayer = viewAndPlayer[1] as PhotonPlayer;
    /// </remarks>
    /// <param name="viewAndPlayer">The PhotonView is viewAndPlayer[0] and the requesting player is viewAndPlayer[1].</param>
    public void OnOwnershipRequest(object[] viewAndPlayer) { }

    /// <summary>
    /// Called when the Master Server sent an update for the Lobby Statistics, updating PhotonNetwork.LobbyStatistics.
    /// </summary>
    /// <remarks>
    /// This callback has two preconditions:
    /// EnableLobbyStatistics must be set to true, before this client connects.
    /// And the client has to be connected to the Master Server, which is providing the info about lobbies.
    /// </remarks>
    public void OnLobbyStatisticsUpdate() { }

    /// <summary>
    /// Called when a remote Photon Player activity changed. This will be called ONLY if PlayerTtl is greater than 0.
    /// </summary>
    /// <remarks>
    /// Use PhotonPlayer.IsInactive to check a player's current activity state.
    ///
    /// Example: void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer) {...}
    ///
    /// This callback has precondition:
    /// PlayerTtl must be greater than 0.
    /// </remarks>
    public void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer) { }

    /// <summary>
    /// Called when ownership of a PhotonView is transfered to another player.
    /// </summary>
    /// <remarks>
    /// The parameter viewAndPlayers contains:
    ///
    /// PhotonView view = viewAndPlayers[0] as PhotonView;
    ///
    /// PhotonPlayer newOwner = viewAndPlayers[1] as PhotonPlayer;
    ///
    /// PhotonPlayer oldOwner = viewAndPlayers[2] as PhotonPlayer;
    /// </remarks>
    /// <example>void OnOwnershipTransfered(object[] viewAndPlayers) {} //</example>
    public void OnOwnershipTransfered(object[] viewAndPlayers) { }
}
