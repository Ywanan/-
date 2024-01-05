using System;
using UnityEngine;

namespace YuanshenMovementSystem
{
    [Serializable]
    public class PlayerRunData 
    {
        [field: SerializeField][field: Range(0f, 4f)] public float SpeedModifier { get; private set; } = 1f;

    }
}
