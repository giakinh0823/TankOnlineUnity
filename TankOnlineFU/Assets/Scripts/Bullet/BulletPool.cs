namespace Bullet
{
    using UnityEngine;
    using UnityObjectPool;

    public class BulletPool : MonoObjectPool<BulletController>
    {
        [field: SerializeField]
        public GameObject BulletPrefab { get; private set; }

        protected override BulletController DoCreateNew()
        {
            var bulletGameObject = Instantiate(this.BulletPrefab, this.transform);
            var bulletController = bulletGameObject.GetComponent<BulletController>();
            return bulletController;
        }

        protected override void OnRelease(BulletController obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.rotation = Quaternion.identity;
        }

        protected override void OnInstantiate(BulletController obj)
        {
            obj.gameObject.SetActive(true);
        }
    }

}