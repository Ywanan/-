using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuanshenMovementSystem
{
    public class PlayerSprintingSate : PlayerMoveingState
    {
        private PlayerSprintData sprintData;

        private bool keepSprinting;

        private bool shouldResetSprintState;

        private float startTime;

        public PlayerSprintingSate(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            sprintData = movementData.SprintData;
        }

        #region IState Methods
        public override void Enter()
        {
            stateMachine.ReusableData.MovementSpeedModifier = sprintData.SpeedModifier;

            base.Enter();

            StartAnimation(stateMachine.Player.AnimationData.SprintParameterHash);

            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StrongForce;

            shouldResetSprintState = true;

            startTime = Time.time;
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.Player.AnimationData.SprintParameterHash);

            if (shouldResetSprintState)
            {
                keepSprinting = false;

                stateMachine.ReusableData.ShouldSprint = false;
            }
        }

        public override void Update()
        {
            base.Update();

            if (keepSprinting) return;

            if (Time.time < startTime + sprintData.SprintToRunTime) return;

            StopSprinting();
        }

        #endregion

        #region Main Methods
        private void StopSprinting()
        {
            if(stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.IdlingState);

                return;
            }
            stateMachine.ChangeState(stateMachine.RunningState);
        }
        #endregion

        #region Reusable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();

            stateMachine.Player.Input.PlayerActions.Sprint.performed += OnSprintPerformed;
        }

        protected override void RemoveInputActionsCallBacks()
        {
            base.RemoveInputActionsCallBacks();
        }

        protected override void OnFall()
        {
            shouldResetSprintState = false;

            base.OnFall();
        }
        #endregion

        #region Input Methods
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.HardStoppingState);

            base.OnMovementCanceled(context);
        }

        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            keepSprinting = true;

            stateMachine.ReusableData.ShouldSprint = true;
        }
        protected override void OnJumpStarted(InputAction.CallbackContext context)
        {

            shouldResetSprintState = false;

            base.OnJumpStarted(context);
        }
        #endregion
    }
}