namespace Tank
{

    using System;
    using System.Threading.Tasks;
    using Bullet;
    using Entity;
    using TMPro;
    using UI;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

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
        public Slider SliderHp { get; private set; }

        [field: SerializeField]
        public ControlKeymap Keymap { get; set; }

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

        private void Awake()
        {
            this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
            this.Rigidbody2D    = this.GetComponent<Rigidbody2D>();
            this.BulletPool     = FindObjectOfType<BulletPool>();

            this.Move(Direction.Down);
        }

        private void Update()
        {
            this.SliderHp.value = this.Health / 100f;
        }

        private void FixedUpdate()
        {
            var direction = Direction.None;

            if (Input.GetKey(this.Keymap.Left))
                direction |= Direction.Left;

            if (Input.GetKey(this.Keymap.Right))
                direction |= Direction.Right;

            if (direction == Direction.None)
            {
                if (Input.GetKey(this.Keymap.Down))
                    direction |= Direction.Down;

                if (Input.GetKey(this.Keymap.Up))
                    direction |= Direction.Up;
            }

            this.Move(direction);

            if (Input.GetKey(this.Keymap.Fire)) this.Fire();
        }

        public async void TakeDamage(int damage)
        {
            this.Health -= damage;

            if (this.Health <= 0)
            {
                Destroy(this.gameObject);
                await Task.Delay(1000);
                MapChooser.Instance.gameObject.SetActive(true);
            }
        }

        private void Move(Direction direction)
        {
            var currentPos = this.gameObject.transform.position;

            if (direction == Direction.None)
            {
                this.Rigidbody2D.velocity = Vector2.zero;
                return;
            }

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

        private void Fire()
        {
            var currentDirection = this.Direction;

            if (this.lastFire + 1 > Time.time) return;

            var bullet = this.BulletPool.Instantiate();
            bullet.transform.position = this.transform.position;

            bullet.Direction      = currentDirection;
            bullet.Speed          = 8f;
            bullet.TankController = this;
            bullet.InitPos        = this.transform.position;
            bullet.MaxRange       = 10;

            this.lastFire = Time.time;
        }
    }

}