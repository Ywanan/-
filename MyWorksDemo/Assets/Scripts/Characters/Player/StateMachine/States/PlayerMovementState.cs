using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuanshenMovementSystem
{
    public class PlayerMovementState : IState
    {
        protected PlayerGroundedData movementData;

        protected PlayerAirborneData airborneData;

        protected PlayerMovementStateMachine stateMachine;

        public PlayerMovementState(PlayerMovementStateMachine playerMovementstateMachine)
        {
            stateMachine = playerMovementstateMachine;

            movementData = stateMachine.Player.Data.GroundedData;

            airborneData = stateMachine.Player.Data.AirborneData;

            SetBaseCameraRecenteringData();

            InitializeData();
        }


        private void InitializeData()
        {
            SetBaseRotationData();
        }

        #region IState Methods
        public virtual void Enter()
        {
            Debug.Log("State:" + GetType().Name);

            AddInputActionsCallbacks();
        }

        public virtual void Exit()
        {
            RemoveInputActionsCallBacks();
        }


        public virtual void HandleInput()
        {
            ReadMovementInput();
        }

        public virtual void Update()
        {
            
        }

        public virtual void PhysicsUpdate()
        {
            Move();
        }

        public virtual void OnAnimationEnterEvent()
        {
        }

        public virtual void OnAnimationExitEvent()
        {
        }

        public virtual void OnAnimationTransitionEvent()
        {
        }

        public virtual void OnTriggerEnter(Collider collider)
        {
            if (stateMachine.Player.LayerData.IsGroundLayer(collider.gameObject.layer))
            {
                OnContactWithGround(collider);

                return;
            }
        }
        public void OnTriggerExit(Collider collider)
        {
            if (stateMachine.Player.LayerData.IsGroundLayer(collider.gameObject.layer))
            {
                OnContactWithGroundExited(collider);

                return;
            }
        }
        #endregion

        #region Main Methods
        private void ReadMovementInput()
        {
            stateMachine.ReusableData.MovementInput = stateMachine.Player.Input.PlayerActions.Movement.ReadValue<Vector2>();
        }
        
        private void Move()
        {
            if(stateMachine.ReusableData.MovementInput == Vector2.zero || stateMachine.ReusableData.MovementSpeedModifier == 0f)
            {
                return;
            }
            Vector3 movementDirection = GetMovementInputDirection();

            float taregetRotationYAngle = Rotate(movementDirection);

            Vector3 targetRotationDirection = GetTargetRotationDirection(taregetRotationYAngle);

            float movementSpeed = GetMovementSpeed();

            Vector3 currentPlayerHorizontalValocity = GetPlayerHorizontalValocity();

            stateMachine.Player.Rigidbody.AddForce(targetRotationDirection * movementSpeed - currentPlayerHorizontalValocity, ForceMode.VelocityChange);
        }


        private float Rotate(Vector3 direction)
        {
            float directionAngle = UpdateTargetRotation(direction);

            RotateTowardTargetRotation();

            return directionAngle;

        }


        private float AddCameraRotationToAngle(float directionAngle)
        {
            directionAngle += stateMachine.Player.MainCameraTransForm.eulerAngles.y;

              if (directionAngle > 360f) directionAngle -= 360f;

            return directionAngle;
        }

        private float GetDirectionAngle(Vector3 direction)
        {
            float directionAnale = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if (directionAnale < 0f) directionAnale += 360f;

            return directionAnale;
        }

        private void UptateTargetRotationData(float targetAngle)
        {
            stateMachine.ReusableData.CurrentTargetRotation.y = targetAngle;

            stateMachine.ReusableData.DampedTargetRotationPassedTime.y = 0f;
        }
        #endregion

        #region Reusable Methods
        protected void StartAnimation(int animationHash)
        {
            stateMachine.Player.Animator.SetBool(animationHash, true);
        }
        
        protected void StopAnimation(int animationHash)
        {
            stateMachine.Player.Animator.SetBool(animationHash, false);
        }

        protected float GetMovementSpeed(bool shouldConsiderSlopes = true)
        {
            float movementSpeed = movementData.BaseSpeed * stateMachine.ReusableData.MovementSpeedModifier;

            if (shouldConsiderSlopes)
                movementSpeed *= stateMachine.ReusableData.MovementOnSlopesSpeedModifier;

            return movementSpeed;
        }

        protected Vector3 GetMovementInputDirection()
        {
            return new Vector3(stateMachine.ReusableData.MovementInput.x, 0f, stateMachine.ReusableData.MovementInput.y);
        }

        protected Vector3 GetPlayerHorizontalValocity()
        {
            Vector3 playerHorizontalVelocity = stateMachine.Player.Rigidbody.velocity;

            playerHorizontalVelocity.y = 0f;

            return playerHorizontalVelocity;
        }

        protected Vector3 GetPlayerVerticalVelocity()
        {
            return new Vector3(0f, stateMachine.Player.Rigidbody.velocity.y, 0);
        }


        protected void RotateTowardTargetRotation()
        {
            float currentYAngle = stateMachine.Player.Rigidbody.rotation.eulerAngles.y;

            if (currentYAngle == stateMachine.ReusableData.CurrentTargetRotation.y) return;

            float smoothedYAngle = Mathf.SmoothDampAngle(currentYAngle, stateMachine.ReusableData.CurrentTargetRotation.y, ref stateMachine.ReusableData.DampedTargetRotationCurrentVelocity.y,
                stateMachine.ReusableData.TimeToReachTargetRotation.y - stateMachine.ReusableData.DampedTargetRotationPassedTime.y);

            stateMachine.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;

            Quaternion targetRotation = Quaternion.Euler(0f, smoothedYAngle, 0f);

            stateMachine.Player.Rigidbody.MoveRotation(targetRotation);
        }

        protected float UpdateTargetRotation(Vector3 direction, bool shouldConsiderCameraRotation = true)
        {
            float directionAngle = GetDirectionAngle(direction);

            if(shouldConsiderCameraRotation) directionAngle = AddCameraRotationToAngle(directionAngle);

            if (directionAngle != stateMachine.ReusableData.CurrentTargetRotation.y) UptateTargetRotationData(directionAngle);

            return directionAngle;
        }

        protected Vector3 GetTargetRotationDirection(float taregetAngle)
        {
            return Quaternion.Euler(0f, taregetAngle, 0f) * Vector3.forward;
        }
        
        protected void ResetVelocity()
        {
            stateMachine.Player.Rigidbody.velocity = Vector3.zero;
        }
        
        protected void ResetVerticalVelocity()
        {
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalValocity();

            stateMachine.Player.Rigidbody.velocity = playerHorizontalVelocity;
        }

        protected virtual void AddInputActionsCallbacks()
        {
            stateMachine.Player.Input.PlayerActions.WalkToggle.started += OnWalkToggleSrarted;

            stateMachine.Player.Input.PlayerActions.Look.started += OnMovementStarted;

            stateMachine.Player.Input.PlayerActions.Movement.performed += OnMovementPerformed;

            stateMachine.Player.Input.PlayerActions.Movement.canceled += OnMovementCanceled;
        }


        protected virtual void RemoveInputActionsCallBacks()
        {
            stateMachine.Player.Input.PlayerActions.WalkToggle.started -= OnWalkToggleSrarted;

            stateMachine.Player.Input.PlayerActions.Look.started -= OnMovementStarted;
            
            stateMachine.Player.Input.PlayerActions.Movement.performed -= OnMovementPerformed;

            stateMachine.Player.Input.PlayerActions.Movement.canceled -= OnMovementCanceled;            
        }

        protected void SetBaseCameraRecenteringData()
        {
            stateMachine.ReusableData.BackwardsCameraRecenteringData = movementData.BackwardsCameraRecenteringData;

            stateMachine.ReusableData.SidewaysCameraRecenteringData = movementData.SidewaysCameraRecenteringData;
        }

        protected void DecelerateHorizontally()
        {
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalValocity();

            stateMachine.Player.Rigidbody.AddForce(-playerHorizontalVelocity * stateMachine.ReusableData.MovementDecelerationForce,ForceMode.Acceleration);
        }
        
        protected void DecelerateVertically()
        {
            Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();

            stateMachine.Player.Rigidbody.AddForce(-playerVerticalVelocity * stateMachine.ReusableData.MovementDecelerationForce,ForceMode.Acceleration);
        }

        protected bool IsMoveingHorizontally(float minimumMagnitude = 0.1f)
        {
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalValocity();

            Vector2 playerHorizontalMovement = new Vector2(playerHorizontalVelocity.x, playerHorizontalVelocity.z);

            return playerHorizontalMovement.magnitude > minimumMagnitude;
        }

        protected bool IsMoveingUp(float minimunVelocity = 0.1f)
        {
            return GetPlayerVerticalVelocity().y > minimunVelocity;
        }

        protected bool IsMoveingDown(float minimunVelocity = 0.1f)
        {
            return GetPlayerVerticalVelocity().y < -minimunVelocity;
        }

        protected void UpdateCameraRecenteringState(Vector2 movementInput)
        {
            if (movementInput == Vector2.zero) return;

            if (movementInput == Vector2.up) 
            {
                DisableCameraRecentering();

                return;
            }

            float cameraVarticalAbgle = stateMachine.Player.MainCameraTransForm.eulerAngles.x;

            if (cameraVarticalAbgle >= 270f) cameraVarticalAbgle -= 360f;

            cameraVarticalAbgle = Mathf.Abs(cameraVarticalAbgle);

            if(movementInput == Vector2.down)
            {
                SetCameraRecenteringState(cameraVarticalAbgle, stateMachine.ReusableData.BackwardsCameraRecenteringData);

                return;
            }
            SetCameraRecenteringState(cameraVarticalAbgle, stateMachine.ReusableData.SidewaysCameraRecenteringData);
        }

        protected void EnableCameraRecentering(float waitTime = -1, float recenteringTime = -1f)
        {
            float movementSpeed = GetMovementSpeed();

            if(movementSpeed == 0f)
            {
                movementSpeed = movementData.BaseSpeed;
            }

            stateMachine.Player.CameraUtility.EnableRecentering(waitTime, recenteringTime, movementData.BaseSpeed, movementSpeed);
        }

        protected void DisableCameraRecentering()
        {
            stateMachine.Player.CameraUtility.DisableRecentering();
        }

        protected void SetCameraRecenteringState(float cameraVarticalAbgle, List<PlayerCameraRecenteringData> cameraRecenteringDatas)
        {
            foreach (PlayerCameraRecenteringData recenteringData in cameraRecenteringDatas)
            {
                if (!recenteringData.IswithinRange(cameraVarticalAbgle))
                {
                    continue;
                }

                EnableCameraRecentering(recenteringData.WaitTime, recenteringData.RecenteringTime);

                return;
            }
            DisableCameraRecentering();
        }
        #endregion

        #region Input Methods
        protected virtual void OnWalkToggleSrarted(InputAction.CallbackContext context)
        {
            stateMachine.ReusableData.ShouldWalk = !stateMachine.ReusableData.ShouldWalk;
        }

        protected void SetBaseRotationData()
        {
            stateMachine.ReusableData.RotationData = movementData.BaseRoationData;

            stateMachine.ReusableData.TimeToReachTargetRotation = stateMachine.ReusableData.RotationData.TargetRotationReachTime;
        }

        protected virtual void OnContactWithGround(Collider collider)
        {
             
        }

        protected virtual void OnContactWithGroundExited(Collider collider)
        {
            
        }

        protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
        {
            DisableCameraRecentering();
        }

        protected virtual void OnMovementPerformed(InputAction.CallbackContext context)
        {
            UpdateCameraRecenteringState(context.ReadValue<Vector2>());
        }

        protected virtual void OnMovementStarted(InputAction.CallbackContext context)
        {
            UpdateCameraRecenteringState(stateMachine.ReusableData.MovementInput);
        }
        #endregion
    }
}
