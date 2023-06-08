namespace Entity
{
    using System;

    [Flags]
    [Serializable]
    public enum Direction
    {
        None  = 0, // 000000
        Up    = 1, // 000001
        Down  = 2, // 000010
        Left  = 4, // 000100
        Right = 8  // 001000
        
        // 000001 | 000010 = 000011
        // 000001 | 000100 = 000101
        
        // 000011 & 000001 = 000001
    }

}