using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMovementSystem
{
    public class PlayerMovementStateMachine : StateMachine
    {
        public Player Player { get;}
        public PlayerStateReusableData ReusableData { get; }
        public PlayerDashingState DashingState { get; }
        public PlayeridlingState IdlingState { get; }
        public PlayerWalkingState WalkingState { get; }
        public PlayerRunningState RunningState { get; }
        public PlayerSprintingSate SprintingState { get; }
        public PlayerHardStoppingState HardStoppingState { get; }
        public PlayerLightStoppingState LightStoppingState { get; }
        public PlayerMediumStoppingState MediumStoppingState { get; }
        public PlayerJumpingState JumpingState { get; }
        public PlayerFallingState FallingState { get; }
        public PlayerLightLandingState LightLandingState { get; }
        public PlayerRollingState RollingState { get; }
        public PlayerHardLandingState HardLandingState { get; }

        public PlayerMovementStateMachine(Player player)
        {
            Player = player;
            ReusableData = new PlayerStateReusableData();
            DashingState = new PlayerDashingState(this);
            IdlingState = new PlayeridlingState(this);
            WalkingState = new PlayerWalkingState(this);
            RunningState = new PlayerRunningState(this);
            SprintingState = new PlayerSprintingSate(this);

            HardStoppingState = new PlayerHardStoppingState(this);
            LightStoppingState = new PlayerLightStoppingState(this);
            MediumStoppingState = new PlayerMediumStoppingState(this);
            JumpingState = new PlayerJumpingState(this);
            FallingState = new PlayerFallingState(this);
            LightLandingState = new PlayerLightLandingState(this);
            RollingState = new PlayerRollingState(this);
            HardLandingState = new PlayerHardLandingState(this);
        }
    }
}
