using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace YuanshenMovementSystem
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        [field:Header("可重用数据")]
        [field: SerializeField]public PlayerSO Data { get; private set; }

        [field: Header("碰撞器")]
        [field: SerializeField]public PlayerCapsuleColliderUtility ColliderUtility { get; private set; }

        [field: Header("摄像机")]
        [field: SerializeField]public PlayerCameraUtility CameraUtility { get; private set; }

        [field: SerializeField]public PlayerLayerData LayerData { get; private set; }

        [field: Header("动画")]
        [field: SerializeField]public PlayerAnimationData AnimationData { get; private set; }

        public Transform MainCameraTransForm { get; private set; }

        public Rigidbody Rigidbody { get; private set; }

        public Animator Animator { get; private set; }

        public PlayerInput Input { get; private set; }

        private PlayerMovementStateMachine movementStateMachine;
        private void Awake()
        {
            Input = GetComponent<PlayerInput>();

            Rigidbody = GetComponent<Rigidbody>();

            Animator = GetComponent<Animator>();

            ColliderUtility.Initialize(gameObject);

            ColliderUtility.CalculateCapsulecolliderDimensions();

            CameraUtility.Initialize();

            AnimationData.Initialzie();

            movementStateMachine = new PlayerMovementStateMachine(this);

            MainCameraTransForm = Camera.main.transform;            
        }

        private void OnValidate()
        {
            ColliderUtility.Initialize(gameObject);

            ColliderUtility.CalculateCapsulecolliderDimensions();
        }

        private void Start()
        {
            movementStateMachine.ChangeState(movementStateMachine.IdlingState);
        }
        private void OnTriggerEnter(Collider collider)
        {
            movementStateMachine.OnTriggerEnter(collider);
        }
        private void OnTriggerExit(Collider collider)
        {
            movementStateMachine.OnTriggerExit(collider);
        }

        private void Update()
        {
            movementStateMachine.HandleInput();

            movementStateMachine.Update();
        }

        private void FixedUpdate()
        {
            movementStateMachine.PhysicsUpdate();
        }

        public void OnMovementStateAnimationEnterEvent()
        {
            movementStateMachine.OnAnimationEnterEvent();
        }

        public void OnMovementStateAnimationExitEvent()
        {
            movementStateMachine.OnAnimationExitEvent();
        }

        public void OnMovementStateAnimationTransitionEvent()
        {
            movementStateMachine.OnAnimationTransitionEvent();
        }
    }
}
