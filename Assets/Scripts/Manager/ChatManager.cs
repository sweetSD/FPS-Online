using Photon.Chat;
using UnityEngine;

/*
 * 
    * 챗 유저상태(ChatUserStatus)

        int 로 그 상태를 구분하고 있습니다.

        0 : offline

        1 : invisible 

        2 : online

        3 : away 방해할 수 없는 상태

        4 : DND 게임이나 그룹을 찾고 있는 상태

        5 : LFG 방에서만 사용가능하고 게임 중인 상태

        6 : Playing 

    * 메시지 카운트 (전체 채널 통틀어 500 message / sec 제한)

        RPC 호출이나 메시지 전송 횟수 등으로 카운트합니다.

        간략하게 예를 들어 보겠습니다.

        4명이 룸에 있는 경우,

        A 유저가 룸 퍼블릭 메시지를 보내면(1 send), 

        나머지 3명이 그 메시지를 받습니다(3 receive).

        그래서 4 메시지로 카운트 됩니다.
 * 
 */

public class ChatManager : SDSingleton<ChatManager>, IChatClientListener
{
    private ChatClient m_CharClient;

    #region IChatClientListener implementation

    private void Awake()
    {
        SetInstance(this);
        Initialize();
    }

    private void Initialize()
    {
        m_CharClient = new ChatClient(this);
        m_CharClient.Connect(PhotonNetwork.PhotonServerSettings.ChatAppID, Application.version, new Photon.Chat.AuthenticationValues("1"));
    }

    public void PublishMessage(string message)
    {
        m_CharClient.PublishMessage("lobbyChannel", message);
    }

    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    {
        Debug.Log($"Debug Return : {level} {message}");
    }

    public void OnDisconnected()
    {
        throw new System.NotImplementedException("OnDisconnected");
    }

    public void OnConnected()
    {
        ChatUI.I.AddChat("System", "입장하였습니다.");
        m_CharClient.Subscribe(new string[] { "lobbyChannel" });
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log($"OnChatStateChange : {state}");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length && i < messages.Length; i++)
        {
            ChatUI.I.AddChat(senders[i], messages[i] as string);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new System.NotImplementedException("OnPrivateMessage");
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        m_CharClient.PublishMessage("lobbyChannel", "hello");
    }

    public void OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException("OnUnsubscribed");
    }

    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException("OnUserSubscribed");
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException("OnUserUnsubscribed");
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException("OnStatusUpdate");
    }

    #endregion

    private void Update()
    {
        m_CharClient.Service();
    }

    private void OnDestroy()
    {
        Release();
    }

    private void OnApplicationQuit()
    {
        Release();
    }

    private void Release()
    {
        if (m_CharClient != null)
        {
            m_CharClient.Disconnect();
        }
    }
}
