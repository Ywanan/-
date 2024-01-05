using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMovementSystem
{
    [Serializable]
    public class CapsulecolliderUtility
    {
        public CapsuleColliderData CapsuleColliderData { get; private set; }
        [field: SerializeField]public DefaultColliderData DefaultColliderData { get; private set; }
        [field: SerializeField]public SlopeData SlopeData { get; private set; }

        public void Initialize(GameObject gameObject)
        {
            if (CapsuleColliderData != null) return;

            CapsuleColliderData = new CapsuleColliderData(); 

            CapsuleColliderData.Initialize(gameObject);

            OnInitialize();
        }

        protected virtual void OnInitialize()
        {

        }

        public void CalculateCapsulecolliderDimensions()
        {
            SetCapsulecolliderRadius(DefaultColliderData.Radius);

            SetCapsulecolliderHeight(DefaultColliderData.Height * (1f - SlopeData.StepHightPercentage));

            RecalculateCapsuleColliderCenter();

            float halfColliderHeigjht = CapsuleColliderData.Collider.height / 2f;
            if (halfColliderHeigjht < CapsuleColliderData.Collider.radius)
            {
                SetCapsulecolliderRadius(halfColliderHeigjht);
            }

            CapsuleColliderData.UpdateColliderData();
        }


        public void SetCapsulecolliderRadius(float radius)
        {
            CapsuleColliderData.Collider.radius = radius;
        }

        public void SetCapsulecolliderHeight(float height)
        {
            CapsuleColliderData.Collider.height = height;
        }

        public void RecalculateCapsuleColliderCenter()
        {
            float colliderHeightDifference = DefaultColliderData.Height - CapsuleColliderData.Collider.height;

            Vector3 newColliderCenter = new Vector3(0f, DefaultColliderData.CenterY + (colliderHeightDifference / 2f), 0f);

            CapsuleColliderData.Collider.center = newColliderCenter;
        }
    }
}
