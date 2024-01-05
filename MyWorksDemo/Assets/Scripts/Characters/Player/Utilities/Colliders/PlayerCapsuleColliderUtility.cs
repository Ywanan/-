using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMovementSystem
{
    [Serializable]
    public class PlayerCapsuleColliderUtility : CapsulecolliderUtility
    {
        [field: SerializeField]public PlayerTriggerColliderData TriggerColliderData { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            TriggerColliderData.Initialize();
        }
    }
}
