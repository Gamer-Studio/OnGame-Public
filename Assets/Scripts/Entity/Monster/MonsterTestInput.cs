using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnGame
{
    public class MonsterTestInput : MonoBehaviour
    {
        private MonsterAnimatorController animator;

        private void Start()
        {
            animator = GetComponent<MonsterAnimatorController>();
            animator.SetState("Idle");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                animator.SetState("Idle");
            if (Input.GetKeyDown(KeyCode.Alpha2))
                animator.SetState("Move");
            if (Input.GetKeyDown(KeyCode.Alpha3))
                animator.SetState("Damage");
        }
    }
}