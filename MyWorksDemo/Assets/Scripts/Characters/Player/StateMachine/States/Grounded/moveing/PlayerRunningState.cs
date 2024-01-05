using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuanshenMovementSystem
{
    public class PlayerRunningState : PlayerMoveingState
    {
        private float startTime;

        private PlayerSprintData sprintData;

        public PlayerRunningState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            sprintData = movementData.SprintData;
        }

        #region IState Methods
        public override void Enter()
        {
            stateMachine.ReusableData.MovementSpeedModifier = movementData.RunData.SpeedModifier;

            base.Enter();

            StartAnimation(stateMachine.Player.AnimationData.RunParameterHash);

            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.MediumForce;

            startTime = Time.time;
        }

        public override void Update()
        {
            base.Update();

            StopAnimation(stateMachine.Player.AnimationData.RunParameterHash);

            if (!stateMachine.ReusableData.ShouldWalk) return;

            if (Time.time < startTime + movementData.SprintData.RunToWalkTime) return;

            StopRunning();
        }
        #endregion

        #region Main Methods
        private void StopRunning()
        {
            if (stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.IdlingState);

                return;
            }
            stateMachine.ChangeState(stateMachine.WalkingState);
        }
        #endregion

        #region Input Methods
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.MediumStoppingState);

            base.OnMovementCanceled(context);
        }

        protected override void OnWalkToggleSrarted(InputAction.CallbackContext context)
        {
            base.OnWalkToggleSrarted(context);

            stateMachine.ChangeState(stateMachine.WalkingState);
        }

        #endregion
    }
}
