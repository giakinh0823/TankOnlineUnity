namespace UnityObjectPool
{

    using UnityEngine;

    public interface IObjectPool<TObject>
    where TObject : Object, IPooledObject<TObject>
    {
        public TObject Instantiate();
        public void    Release(TObject obj);
    }

}