namespace Entity
{

    using System;

    [Flags]
    [Serializable]
    public enum Direction
    {
        None  = 0,
        Up    = 1,
        Down  = 2,
        Left  = 4,
        Right = 8
    }

}