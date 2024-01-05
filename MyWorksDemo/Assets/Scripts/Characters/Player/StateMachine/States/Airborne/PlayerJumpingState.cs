using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuanshenMovementSystem
{
    public class PlayerJumpingState : PlayerAirborneState
    {
        private PlayerJumpData jumpData;

        private bool shouldKeepRotating;

        private bool canStartFalling;
        
        public PlayerJumpingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            jumpData = airborneData.JumpData;
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            stateMachine.ReusableData.MovementSpeedModifier = 0f;

            stateMachine.ReusableData.MovementDecelerationForce = jumpData.DecelerationForce;

            stateMachine.ReusableData.RotationData = jumpData.RotationData;

            shouldKeepRotating = stateMachine.ReusableData.MovementInput != Vector2.zero;

            Jump();
        }

        public override void Update()
        {
            base.Update();

            if (!canStartFalling && IsMoveingUp(0f)) canStartFalling = true;

            if (!canStartFalling || IsMoveingUp(0f)) return;

            stateMachine.ChangeState(stateMachine.FallingState);
        }

        public override void Exit()
        {
            base.Exit();

            SetBaseRotationData();

            canStartFalling = false;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (shouldKeepRotating)
            {
                RotateTowardTargetRotation();
            }

            if (IsMoveingUp())
            {
                DecelerateVertically();
            }
        }
        #endregion

        #region Reusable Methods
        protected override void ResetSprintState()
        {

        }

        #endregion

        #region Main Methods

        private void Jump()
        {
            Vector3 jumpForce = stateMachine.ReusableData.CurrentJumpForce;

            Vector3 jumpDirection = stateMachine.Player.transform.forward;

            if (shouldKeepRotating)
            {
                UpdateTargetRotation(GetMovementInputDirection());

                jumpDirection = GetTargetRotationDirection(stateMachine.ReusableData.CurrentTargetRotation.y);
            }

            jumpForce.x *= jumpDirection.x;
            jumpForce.z *= jumpDirection.z;

            jumpForce = GetJumpForceOnSlope(jumpForce);

            ResetVelocity();

            stateMachine.Player.Rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
        }
        private Vector3 GetJumpForceOnSlope(Vector3 jumpForce)
        {
            Vector3 capsuleColliderCenterInWorldSpace = stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

            Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

            if (Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit, airborneData.JumpData.JumpToGroundRayDistance, stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

                if (IsMoveingUp())
                {
                    float forceModifier = airborneData.JumpData.JumpForceModifierOnSlopeUpwards.Evaluate(groundAngle);

                    jumpForce.x *= forceModifier;
                    jumpForce.z *= forceModifier;
                }

                if (IsMoveingDown())
                {
                    float forceModifier = airborneData.JumpData.JumpForceModifierOnSlopeDownwards.Evaluate(groundAngle);

                    jumpForce.y *= forceModifier;
                }
            }

            return jumpForce;
        }
        #endregion

        #region Input Methods
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            
        }
        #endregion
    }
}
