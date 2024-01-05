using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuanshenMovementSystem
{
    public class PlayerDashingState : PlayerGroundedState
    {
        private PlayerDashData dashData;

        private float startTime;

        private int consecutinuousDashesUsed;

        private bool shouldKeepRotating;

        public PlayerDashingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            dashData = movementData.DashData;
        }

        #region IState Methods
        public override void Enter()
        {
            stateMachine.ReusableData.MovementSpeedModifier = dashData.SpeedModifier;

            base.Enter();

            StartAnimation(stateMachine.Player.AnimationData.DashParameterHash);

            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StrongForce;

            stateMachine.ReusableData.RotationData = dashData.RotationData;

            Dash();

            shouldKeepRotating = stateMachine.ReusableData.MovementInput != Vector2.zero;

            UpdateConsecutiveDashes();

            startTime = Time.time;
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.Player.AnimationData.DashParameterHash);

            SetBaseRotationData();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (!shouldKeepRotating) return;

            RotateTowardTargetRotation();
        }

        public override void OnAnimationTransitionEvent()
        {
            if (stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.HardStoppingState);

                return;
            }
            stateMachine.ChangeState(stateMachine.SprintingState);
        }
        
        #endregion

        #region Main Method
        private void Dash()
        {
            Vector3 characterRotationDirection = stateMachine.Player.transform.forward;

            characterRotationDirection.y = 0f;

            UpdateTargetRotation(characterRotationDirection, false);

            if (stateMachine.ReusableData.MovementInput != Vector2.zero)
            {
                UpdateTargetRotation(GetMovementInputDirection());

                characterRotationDirection = GetTargetRotationDirection(stateMachine.ReusableData.CurrentTargetRotation.y);
            }
            
            stateMachine.Player.Rigidbody.velocity = characterRotationDirection * GetMovementSpeed(false);
        }

        private void UpdateConsecutiveDashes()
        {
            if (!IsConsecutive())
            {
                consecutinuousDashesUsed = 0;
            }

            ++consecutinuousDashesUsed;

            if(consecutinuousDashesUsed == dashData.ConsecutiveDashesLimitAmount)
            {
                consecutinuousDashesUsed = 0;

                stateMachine.Player.Input.DisableActionFor(stateMachine.Player.Input.PlayerActions.Dash, dashData.DashLimtReachedCooldown);
            }
        }

        private bool IsConsecutive()
        {
            return Time.time < startTime + dashData.TimeToBeConsideredConsecutive;
        }
        #endregion

        #region Resuable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();

            stateMachine.Player.Input.PlayerActions.Movement.performed += OnMovementPerformed;
        }


        protected override void RemoveInputActionsCallBacks()
        {
            base.RemoveInputActionsCallBacks();

            stateMachine.Player.Input.PlayerActions.Movement.performed -= OnMovementPerformed;
        }
        #endregion

        #region Input Methods
        protected override void OnDashStarted(InputAction.CallbackContext context)
        {
            
        }

        private void OnMovementPerformed(InputAction.CallbackContext context)
        {
            base.OnMovementPerformed(context);

            shouldKeepRotating = true;
        }
        #endregion
    }
}
