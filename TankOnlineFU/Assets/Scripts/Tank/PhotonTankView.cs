namespace Tank
{

    using System;
    using Entity;
    using Photon.Pun;
    using UnityEngine;

    [RequireComponent(typeof(TankController))]
    public class PhotonTankView : MonoBehaviour, IPunObservable
    {
        public PhotonView     PhotonView     { get; private set; }
        public TankController TankController { get; private set; }


        private Vector3 targetPosition;

        private void Awake()
        {
            this.PhotonView     = this.GetComponent<PhotonView>();
            this.TankController = this.GetComponent<TankController>();
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
                this.OnPhotonWriteView(stream, info);
            else if (stream.IsReading) this.OnPhotonReadView(stream, info);
        }

        private void OnPhotonWriteView(PhotonStream stream, PhotonMessageInfo info)
        {
            stream.SendNext(this.transform.position);
            stream.SendNext(this.TankController.Direction);
            stream.SendNext(this.TankController.Health);
        }

        private void OnPhotonReadView(PhotonStream stream, PhotonMessageInfo info)
        {
            // Sync position
            this.targetPosition = (Vector3)stream.ReceiveNext();

            // Sync direction
            this.TankController.Direction = (Direction)stream.ReceiveNext();

            // Sync health
            this.TankController.Health = (int)stream.ReceiveNext();
        }

        private void FixedUpdate()
        {
            if (this.PhotonView.IsMine) return;
            this.transform.position = Vector3.Lerp(this.transform.position, this.targetPosition, Time.fixedDeltaTime * 30f);
        }
    }

}