namespace UnityObjectPool
{

    using UnityEngine;

    public interface IPooledObject<TObject>
    where TObject : Object, IPooledObject<TObject>
    {
        public IObjectPool<TObject> ObjectPool { get; set; }
    }

}