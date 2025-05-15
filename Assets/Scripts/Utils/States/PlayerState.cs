using System.Collections;
using OnGame.Prefabs.Entities;
using OnGame.UI;
using UnityEngine;

namespace OnGame.Utils.States.PlayerState
{
    public class IdleState : State<Character>
    {
        public override void Enter(Character source)
        {
            Debug.Log("Changed to idle state");
            source.Animator.SetBool(Character.IsMove, false);
        }

        public override void Execute(Character source)
        {
            if (source.RigidBody.velocity.magnitude >= 0.1f) source.ChangeState(PlayerStates.Move);
        }

        public override void Exit(Character source)
        {
            source.Animator.SetBool(Character.IsMove, true);
        }
    }

    public class MoveState : State<Character>
    {
        public override void Enter(Character source)
        {
            Debug.Log("Changed to move state");
            source.Animator.SetBool(Character.IsMove, true);
        }

        public override void Execute(Character source)
        {
            if (source.RigidBody.velocity.magnitude < 0.1f) source.ChangeState(PlayerStates.Idle);
        }

        public override void Exit(Character source)
        {
            source.Animator.SetBool(Character.IsMove, false);
        }
    }

    public class DashState : State<Character>
    {
        public override void Enter(Character source)
        {
            Debug.Log("Changed to dash state");
            source.IsDashing = true;
            source.IsDashTrailEnabled = true;
            source.TrailRenderer.time = 1f;
            source.TrailRenderer.startColor = new Color(source.TrailColor.r, source.TrailColor.g, source.TrailColor.b, 1f);
            source.TrailRenderer.endColor = new Color(1, 1, 1, 0);
        }

        public override void Execute(Character source)
        {
            source.StartCoroutine(WaitUntilDashEnd(source));
        }

        public override void Exit(Character source)
        {
            source.IsDashing = false;
        }

        IEnumerator WaitUntilDashEnd(Character source)
        {
            yield return new WaitForSeconds(0.2f);
            source.ChangeState(PlayerStates.Move);
        }
    }

    public class GuardState : State<Character>
    {
        private StatOperator<float> guardOperator;
        private float originalForce;
        private float originalDrag;

        public override void Enter(Character source)
        {
            Debug.Log("Changed to guard state");
            originalForce = source.MoveForce;
            originalDrag = source.Drag;

            source.RigidBody.drag = originalDrag * 2f;
            source.MoveForce *= 0.3f;
            
            guardOperator = x => x + 40f;
            source.DefenseOpers.Add(guardOperator);
            
            source.StopAllCoroutines();
            source.StartCoroutine(ReduceManaForSeconds(source));
        }

        public override void Execute(Character source)
        {
            source.Animator.SetBool(Character.IsMove, !(source.RigidBody.velocity.magnitude < 0.1f));
        }

        public override void Exit(Character source)
        {
            source.StopAllCoroutines();
            source.StartCoroutine(RecoverManaForSeconds(source));
            
            source.MoveForce = originalForce;
            source.RigidBody.drag = originalDrag;
            source.DefenseOpers.Remove(guardOperator);
        }

        private IEnumerator ReduceManaForSeconds(Character source)
        {
            while (source.CurrentState == PlayerStates.Guard)
            {
                source.OnManaConsume(5);
                Debug.Log(source.Mana.Value);
                if (source.Mana.Value <= 0) { break; }
                yield return new WaitForSeconds(1f);
            }
            if(source.IsAlive) source.ChangeState(PlayerStates.Idle);
        }

        private IEnumerator RecoverManaForSeconds(Character source)
        {
            if (!source.IsAlive) yield return null;
            
            yield return new WaitForSeconds(3f);
            while (source.Mana.Value < source.Mana.Max)
            {
                if (!source.IsAlive) break;
                source.OnManaRecover(3);
                Debug.Log(source.Mana.Value);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    public class DeadState : State<Character>
    {
        public override void Enter(Character source)
        {
            Debug.Log("Changed to dead state");
            UIManager.instance.ShowGameOverPanel();
            //Time.timeScale = 0;
        }

        public override void Execute(Character source)
        {
        }

        public override void Exit(Character source)
        {
            
        }
    }
}