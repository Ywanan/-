using UnityEngine.InputSystem;

namespace YuanshenMovementSystem
{
    public class PlayerWalkingState : PlayerMoveingState
    {
        public PlayerWallkData WallkData;
        public PlayerWalkingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            WallkData = movementData.WallkData;
        }

        #region IState Methods
        public override void Enter()
        {
            stateMachine.ReusableData.MovementSpeedModifier = WallkData.SpeedModifier;

            stateMachine.ReusableData.BackwardsCameraRecenteringData = WallkData.BackwardsCameraRecenteringData;

            base.Enter();

            StartAnimation(stateMachine.Player.AnimationData.WalkParameterHash);

            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.WeakForce;
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.Player.AnimationData.WalkParameterHash);

            SetBaseCameraRecenteringData();
        }
        #endregion

        #region Input Methods
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.LightStoppingState);

            base.OnMovementCanceled(context);
        }

        protected override void OnWalkToggleSrarted(InputAction.CallbackContext context)
        {
            base.OnWalkToggleSrarted(context);

            stateMachine.ChangeState(stateMachine.RunningState);
        }

        #endregion
    }
}
