using System;
using UnityEngine;

namespace YuanshenMovementSystem
{
    [Serializable]
    public class PlayerRotationData
    {
        [field: SerializeField][Tooltip("基础旋转到达时间")] public Vector3 TargetRotationReachTime { get; private set; }
    }
}
