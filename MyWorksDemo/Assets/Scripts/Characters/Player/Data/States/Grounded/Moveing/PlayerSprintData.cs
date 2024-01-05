using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMovementSystem
{
    [Serializable]
    public class PlayerSprintData
    {
        [field: SerializeField][field: Range(0f, 4f)] public float SpeedModifier { get; private set; } = 1.7f;
        [field: SerializeField][field: Range(0f, 6f)] public float SprintToRunTime { get; private set; } = 1f;
        [field: SerializeField][field: Range(0f, 2f)] public float RunToWalkTime { get; private set; } = 0.5f;
    }
}
