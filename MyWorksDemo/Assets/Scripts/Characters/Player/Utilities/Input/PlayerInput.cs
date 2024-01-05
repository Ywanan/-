
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuanshenMovementSystem
{
    public class PlayerInput : MonoBehaviour
    {
        public PlayerInputActions inputActions { get; private set; }

        public PlayerInputActions.PlayerActions PlayerActions { get; private set; }
        private void Awake()
        {
            inputActions = new PlayerInputActions();

            PlayerActions = inputActions.Player;
        }
        private void OnEnable()
        {
            inputActions.Enable();
        }
        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void DisableActionFor(InputAction action, float seconds)
        {
            StartCoroutine(DisableAction(action, seconds));
        }

        IEnumerator DisableAction(InputAction action, float seconds)
        {
            action.Disable();

            yield return new WaitForSeconds(seconds);

            action.Enable();
        }
    }
}
