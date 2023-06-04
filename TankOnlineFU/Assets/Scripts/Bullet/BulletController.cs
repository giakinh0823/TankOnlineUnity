namespace Bullet
{

    using Entity;
    using Tank;
    using UnityEngine;
    using UnityObjectPool;

    public class BulletController : MonoBehaviour, IPooledObject<BulletController>
    {
        public int       MaxRange  { get; set; }
        public float     Speed     { get; set; }
        public Direction Direction { get; set; }
        public Vector3   InitPos   { get; set; }


        public TankController TankController { get; set; }

        public SpriteRenderer SpriteRenderer { get; private set; }

        public Rigidbody2D Rigidbody2D { get; private set; }

        private void Awake()
        {
            this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
            this.Rigidbody2D    = this.GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            this.DestroyAfterRange();

            switch (this.Direction)
            {
                case Direction.Down:
                    this.transform.rotation = Quaternion.Euler(0, 0, 180);
                    this.Rigidbody2D.AddForce(Vector2.down * this.Speed, ForceMode2D.Impulse);
                    break;
                case Direction.Up:
                    this.transform.rotation = Quaternion.Euler(0, 0, 0);
                    this.Rigidbody2D.AddForce(Vector2.up * this.Speed, ForceMode2D.Impulse);
                    break;
                case Direction.Right:
                    this.transform.rotation = Quaternion.Euler(0, 0, -90);
                    this.Rigidbody2D.AddForce(Vector2.right * this.Speed, ForceMode2D.Impulse);
                    break;
                case Direction.Left:
                    this.transform.rotation = Quaternion.Euler(0, 0, 90);
                    this.Rigidbody2D.AddForce(Vector2.left * this.Speed, ForceMode2D.Impulse);
                    break;
                case Direction.None:
                default:
                    break;
            }

            this.SpriteRenderer.color = Color.white;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other is not { gameObject: { layer: (int)Layers.Tank } }) return;

            if (this.TankController.gameObject == other.gameObject) return;

            other.gameObject.GetComponent<TankController>().Health -= 10;
            this.ObjectPool.Release(this);
        }

        public IObjectPool<BulletController> ObjectPool { get; set; }

        private void DestroyAfterRange()
        {
            var currentPos = this.gameObject.transform.position;
            var initPos    = this.InitPos;

            switch (this.Direction)
            {
                case Direction.Down:
                    if (initPos.y - this.MaxRange >= currentPos.y) this.ObjectPool.Release(this);
                    break;
                case Direction.Up:
                    if (initPos.y + this.MaxRange <= currentPos.y) this.ObjectPool.Release(this);
                    break;
                case Direction.Left:
                    if (initPos.x - this.MaxRange >= currentPos.x) this.ObjectPool.Release(this);
                    break;
                case Direction.Right:
                    if (initPos.x + this.MaxRange <= currentPos.x) this.ObjectPool.Release(this);
                    break;
                case Direction.None:
                default:
                    this.ObjectPool.Release(this);
                    break;
            }
        }
    }

}