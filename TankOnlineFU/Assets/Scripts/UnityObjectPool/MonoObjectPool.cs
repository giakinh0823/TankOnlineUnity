namespace UnityObjectPool
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public abstract class MonoObjectPool<TObject> : MonoBehaviour, IObjectPool<TObject>
    where TObject : Object, IPooledObject<TObject>
    {
        private HashSet<TObject> InUseObject     { get; set; } = new ();
        private HashSet<TObject> AvailableObject { get; set; } = new ();

        public TObject Instantiate()
        {
            var obj = this.AvailableObject.FirstOrDefault();

            if (obj == null)
            {
                obj            = this.DoCreateNew();
                obj.ObjectPool = this;
            }

            this.AvailableObject.Remove(obj);
            this.InUseObject.Add(obj);

            this.OnInstantiate(obj);
            return obj;
        }

        public void Release(TObject obj)
        {
            if (this.InUseObject.Contains(obj))
            {
                this.OnRelease(obj);
                this.InUseObject.Remove(obj);
                this.AvailableObject.Add(obj);
                return;
            }

            Destroy(obj);
        }

        protected abstract TObject DoCreateNew();
        protected abstract void    OnRelease(TObject     obj);
        protected abstract void    OnInstantiate(TObject obj);
    }

}