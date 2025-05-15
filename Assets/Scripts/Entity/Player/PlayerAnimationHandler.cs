using System;
using System.Collections;
using System.Collections.Generic;
using OnGame.Prefabs.Entities;
using UnityEngine;

namespace OnGame
{
    public class PlayerAnimationHandler : MonoBehaviour
    {
        private Animator animator;
        private bool isMoving = false;
        private bool isDamaged = false;
        private bool isUsingSkill = false;
        private Direction currentDirection = Direction.South;
        
        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Setmovement(Vector2 input)
        {
            isMoving = input != Vector2.zero;
            
            if (isMoving && !isUsingSkill)
            {
                if (Mathf.Abs(input.x) >= Mathf.Abs(input.y))
                {
                    currentDirection = input.x > 0 ? Direction.East : Direction.West;
                }
                else
                {
                    currentDirection = input.y > 0 ? Direction.North : Direction.South;
                }
            }

            ApplyToAnimator();
        }

        public void SetDamaged(bool damaged)
        {
            isDamaged = damaged;
            ApplyToAnimator();
        }


        public void SetSkillDirection(Vector2 mousePosition, Vector2 playerPosition)
        {
            isUsingSkill = true;
            
            Vector2 direction = mousePosition - playerPosition;

            if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
            {
                currentDirection = direction.x > 0 ? Direction.East : Direction.West;
            }
            else
            {
                currentDirection = direction.y > 0 ? Direction.North : Direction.South;
            }
            ApplyToAnimator();
        }

        public void SetSkillEnd()
        {
            isUsingSkill = false;
        }
        private void ApplyToAnimator()
        {
            animator.SetBool("IsMove", isMoving);
            animator.SetBool("IsDamage", isDamaged);
            animator.SetInteger("Direction", (int)currentDirection);
        }
    }
}
