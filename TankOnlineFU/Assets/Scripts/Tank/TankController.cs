namespace Tank
{

    using System;
    using Bullet;
    using Entity;
    using Photon.Pun;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(PhotonTankView))]
    public class TankController : MonoBehaviour
    {
        public Sprite tankUp;
        public Sprite tankDown;
        public Sprite tankLeft;
        public Sprite tankRight;

        [field: SerializeField]
        public float Speed { get; private set; }

        [field: SerializeField]
        public Canvas Canvas { get; private set; }

        [field: SerializeField]
        public TMP_Text TextNickname { get; private set; }

        [field: SerializeField]
        public Slider SliderHP { get; private set; }

        private float lastFire;

        public Direction Direction
        {
            get =>
                this.SpriteRenderer.sprite switch
                {
                    _ when this.SpriteRenderer.sprite == this.tankDown  => Direction.Down,
                    _ when this.SpriteRenderer.sprite == this.tankUp    => Direction.Up,
                    _ when this.SpriteRenderer.sprite == this.tankLeft  => Direction.Left,
                    _ when this.SpriteRenderer.sprite == this.tankRight => Direction.Right,
                    _                                                   => Direction.None
                };

            set
            {
                this.SpriteRenderer.sprite = value switch
                {
                    Direction.Down  => this.tankDown,
                    Direction.Up    => this.tankUp,
                    Direction.Left  => this.tankLeft,
                    Direction.Right => this.tankRight,
                    _               => this.SpriteRenderer.sprite
                };
            }
        }

        public int Health { get; set; } = 100;

        public SpriteRenderer SpriteRenderer { get; private set; }

        public Rigidbody2D Rigidbody2D { get; private set; }

        public BulletPool BulletPool { get; private set; }

        public PhotonView PhotonView { get; private set; }

        public void EarnDamage(int damage)
        {
            this.Health -= damage;
        }
        
        private void Awake()
        {
            this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
            this.Rigidbody2D    = this.GetComponent<Rigidbody2D>();
            this.PhotonView     = this.GetComponent<PhotonView>();
            this.BulletPool     = FindObjectOfType<BulletPool>();

            this.Move(Direction.Down);
        }

        private void Update()
        {
            this.TextNickname.text = PhotonNetwork.NickName;
            this.SliderHP.value    = this.Health / 100f;
        }

        private void FixedUpdate()
        {
            if (!this.PhotonView.IsMine) return;

            var direction = Direction.None;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                direction |= Direction.Left;

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                direction |= Direction.Right;

            if (direction == Direction.None)
            {
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                    direction |= Direction.Down;

                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                    direction |= Direction.Up;
            }

            this.Move(direction);

            if (Input.GetKey(KeyCode.Space))
            {
                this.PhotonView.RPC(nameof(this.Fire), RpcTarget.All);
            }
        }

        private void Move(Direction direction)
        {
            var currentPos = this.gameObject.transform.position;

            if (direction == Direction.None)
                return;

            if (direction.HasFlag(Direction.Left))
                currentPos.x -= this.Speed;

            if (direction.HasFlag(Direction.Right))
                currentPos.x += this.Speed;

            if (direction.HasFlag(Direction.Up))
                currentPos.y += this.Speed;

            if (direction.HasFlag(Direction.Down))
                currentPos.y -= this.Speed;

            this.Rigidbody2D.MovePosition(currentPos);

            this.Direction = direction;
        }

        [PunRPC]
        private void Fire()
        {
            var currentDirection = this.Direction;

            if (this.lastFire + 1 > Time.time) return;

            var bullet = this.BulletPool.Instantiate();
            bullet.transform.position = this.transform.position;

            bullet.Direction      = currentDirection;
            bullet.Speed          = .5f;
            bullet.TankController = this;
            bullet.InitPos        = this.transform.position;
            bullet.MaxRange       = 10;

            this.lastFire = Time.time;
        }
    }

}