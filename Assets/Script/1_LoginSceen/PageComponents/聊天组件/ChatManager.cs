using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Manager
{
    public class ChatManager : MonoBehaviour
    {
        public GameObject openButton;
        public GameObject closeButton;
        public GameObject sendButton;
        public GameObject inputFiled;
        public GameObject textArea;
        public GameObject background;

        public static ChatManager MainChat { get; set; }

        void Awake() => Init();
        public void Init()
        {
            MainChat = this;
            openButton.GetComponent<Button>().onClick.AddListener(OpenChat);
            closeButton.GetComponent<Button>().onClick.AddListener(CloseChat);
            sendButton.GetComponent<Button>().onClick.AddListener(SendMessage);
            CloseChat();
        }
        public async void SendMessage()
        {
            string text = inputFiled.GetComponent<InputField>().text;
            await Command.Network.NetCommand.ChatAsync("用户"+Info.AgainstInfo.onlineUserInfo.Name,text);
            ReceiveMessage("用户" + Info.AgainstInfo.onlineUserInfo.Name, text);
        }
        public void OpenChat()
        {
            openButton.SetActive(false);
            closeButton.SetActive(true);
            sendButton.SetActive(true);
            inputFiled.SetActive(true);
            textArea.SetActive(true);
            background.SetActive(true);
        }
        public void CloseChat()
        {
            openButton.SetActive(true);
            closeButton.SetActive(false);
            sendButton.SetActive(false);
            inputFiled.SetActive(false);
            textArea.SetActive(false);
            background.SetActive(false);
        }
        public void ReceiveMessage(string user, string text)
        {
            textArea.GetComponent<Text>().text += $"<color=yellow><b>{user}</b></color>:<color=white>{text}</color>\n";
        }
    }
}