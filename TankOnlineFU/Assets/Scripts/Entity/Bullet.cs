using UnityEngine;

namespace Entity
{
    public class Bullet
    {
        public Tank Tank { get; set; }
        public Direction Direction { get; set; }

        public Vector3 InitialPosition { get; set; }
    }
}