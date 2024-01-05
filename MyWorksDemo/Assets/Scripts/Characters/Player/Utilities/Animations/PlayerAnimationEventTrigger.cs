using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMovementSystem
{
    public class PlayerAnimationEventTrigger : MonoBehaviour
    {
        private Player player;

        private void Awake()
        {
            player = transform.GetComponent<Player>();
        }

        public void TriggerOnMovementStateAnimationEnterEvent()
        {
            if (IsInAnimationTransition()) return;

            player.OnMovementStateAnimationEnterEvent();
        }
        
        public void TriggerOnMovementStateAnimationExitEvent()
        {
            player.OnMovementStateAnimationExitEvent();
        }
        
        public void TriggerOnMovementStateAnimationTransitionEvent()
        {
            player.OnMovementStateAnimationTransitionEvent();
        }

        private bool IsInAnimationTransition(int layerIndex = 0)
        {
            return player.Animator.IsInTransition(layerIndex);
        }
    }
}
