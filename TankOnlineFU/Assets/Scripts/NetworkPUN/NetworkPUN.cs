namespace NetworkPUN
{

    using System.Threading.Tasks;
    using Photon.Pun;
    using Photon.Realtime;
    using UnityEngine;

    public class NetworkPUN : MonoBehaviourPunCallbacks
    {
        private void Awake()
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to master");
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Joined lobby");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Joined room");
            PhotonNetwork.LoadLevel("GameplayScene");
        }

        public async Task JoinRoom(string nickname)
        {
            if (!PhotonNetwork.InLobby) return;

            PhotonNetwork.NickName = nickname;

            PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: new RoomOptions
            {
                IsOpen     = true,
                IsVisible  = true,
                MaxPlayers = 10
            });

            while (true)
            {
                await Task.Yield();

                if (PhotonNetwork.InRoom) break;
            }
        }
    }

}