using System;
using UnityEngine;

namespace YuanshenMovementSystem
{
    [Serializable]
    public class DefaultColliderData
    {
        [field: SerializeField] public float Height { get; private set; } = 1.57f;
        [field: SerializeField] public float CenterY { get; private set; } = 0.78f;
        [field: SerializeField] public float Radius { get; private set; } = 0.15f;
    }
}
