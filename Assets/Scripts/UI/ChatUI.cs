using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ChatUI : SDSingleton<ChatUI>
{
    [Tooltip("채팅 입력 인풋필드")]
    [SerializeField] private TMP_InputField m_ChatInputField;
    [SerializeField] private CanvasGroup m_ChatInputFieldCanvasGroup;
    [Space(15)]
    [Tooltip("채팅창 백그라운드 이미지")]
    [SerializeField] private Image m_ChatBackgroundImage;

    public bool isInputFieldFocus => m_ChatInputFieldCanvasGroup.alpha > 0;

    [Space(15)]
    [Tooltip("채팅 텍스트 프리팹 (혹은 객체)")]
    [SerializeField] private TextMeshProUGUI m_ChatPrefab;
    [Tooltip("채팅 텍스트 루트(부모)")]
    [SerializeField] private RectTransform m_ChatRoot;

    public void AddChat(string sender, string message)
    {
        var chatInst = Instantiate(m_ChatPrefab, m_ChatRoot);
        chatInst.SetText($"[{sender}] {message}");
        chatInst.gameObject.SetActive(true);
    }

    public void SendChat(string message)
    {
        if (message.IsNotEmpty())
            ChatManager.I.PublishMessage(message);
        UnfocusInputField();
    }

    public void ClearChat()
    {
        m_ChatRoot.DestroyChildren();
    }

    public void FocusInputField()
    {
        m_ChatInputFieldCanvasGroup.DOFade(1, 0.2f);
        m_ChatBackgroundImage.DOFade(0.5f, 0.2f);
        m_ChatInputField.ActivateInputField();
        m_ChatInputField.Select();
    }

    public void UnfocusInputField()
    {
        m_ChatInputField.text = string.Empty;
        m_ChatInputField.DeactivateInputField();
        m_ChatBackgroundImage.DOFade(0, 0.2f);
        m_ChatInputFieldCanvasGroup.DOFade(0, 0.2f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !isInputFieldFocus)
        {
            FocusInputField();
        }
    }
}
