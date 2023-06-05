namespace Entity
{

    using System;
    using UnityEngine;

    [Serializable]
    public class ControlKeymap
    {
        [field: SerializeField]
        public KeyCode Up { get; set; }

        [field: SerializeField]
        public KeyCode Down { get; set; }

        [field: SerializeField]
        public KeyCode Left { get; set; }

        [field: SerializeField]
        public KeyCode Right { get; set; }

        [field: SerializeField]
        public KeyCode Fire { get; set; }
    }

}