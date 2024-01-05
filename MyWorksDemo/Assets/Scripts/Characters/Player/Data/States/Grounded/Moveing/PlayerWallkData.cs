using System;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMovementSystem
{
    [Serializable]
    public class PlayerWallkData 
    {
        [field: SerializeField][field: Range(0f, 2f)] public float SpeedModifier { get; private set; } = 0.5f;
        [field: SerializeField] public List<PlayerCameraRecenteringData> BackwardsCameraRecenteringData { get; private set; }
    }
}
