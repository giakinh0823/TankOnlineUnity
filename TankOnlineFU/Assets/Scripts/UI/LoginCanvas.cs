namespace UI
{

    using NetworkPUN;
    using Photon.Pun;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class LoginCanvas : MonoBehaviour
    {
        [field: SerializeField]
        public TMP_Text InputNickname { get; private set; }

        [field: SerializeField]
        public Button ButtonLogin { get; private set; }

        private void Awake()
        {
            this.ButtonLogin.onClick.AddListener(this.OnLoginButtonClicked);
        }

        private void Update()
        {
            this.ButtonLogin.interactable =
                !string.IsNullOrEmpty(this.InputNickname.text) &&
                PhotonNetwork.IsConnectedAndReady;
        }

        public async void OnLoginButtonClicked()
        {
            this.ButtonLogin.interactable = false;
            await FindObjectOfType<NetworkPUN>().JoinRoom(this.InputNickname.text);
            this.ButtonLogin.interactable = true;
        }
    }

}