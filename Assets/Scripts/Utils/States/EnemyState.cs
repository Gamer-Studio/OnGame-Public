using OnGame.Prefabs.Entities;
using UnityEngine;

namespace OnGame.Utils.States.EnemyState
{
    public class PatrolState : State<EnemyCharacter>
    {
        private float lastSearch = float.NegativeInfinity;
        
        public override void Enter(EnemyCharacter source)
        {
            source.MonsterAnimator.SetState("Idle");
        }

        public override void Execute(EnemyCharacter source)
        {
            if (source.Target == null || !source.Target.IsAlive) return;
            
            if (!(Time.time > lastSearch + source.searchRate)) return;
            lastSearch = Time.time;
            var hit = Physics2D.Raycast(source.transform.position + DirectionToTarget(source) * 0.45f,
                DirectionToTarget(source), source.MaxDistanceToTarget);
            if ((1 << source.Target.gameObject.layer & (1 << hit.collider.gameObject.layer)) != 0)
            {
                source.ChangeState(EnemyStates.Chase);
            }
        }

        public override void Exit(EnemyCharacter source)
        {
        }

        private Vector3 DirectionToTarget(EnemyCharacter source)
        {
            return (source.Target.transform.position - source.transform.position).normalized;
        }
    }

    public class ChaseState : State<EnemyCharacter>
    {
        public override void Enter(EnemyCharacter source)
        {
            source.MonsterAnimator.SetState("Move");
        }

        public override void Execute(EnemyCharacter source)
        {
            if (source.Target == null || !source.Target.IsAlive) source.ChangeState(EnemyStates.Patrol);
            
            source.MoveWithPath();
            if(source.path == null) source.ChangeState(EnemyStates.Patrol);
            
            var hit = Physics2D.Raycast(source.transform.position + DirectionToTarget(source) * 0.45f,
                DirectionToTarget(source), source.MaxDistanceToTarget);
            if ((1 << source.Target.gameObject.layer & (1 << hit.collider.gameObject.layer)) != 0 && DistanceToTarget(source.transform.position, source.Target.transform.position) <= source.AttackRange)
            {
                source.ReachedEndOfPath = true;
                source.ChangeState(EnemyStates.Attack);
            }
        }

        public override void Exit(EnemyCharacter source)
        {
        }
        
        private Vector3 DirectionToTarget(EnemyCharacter source)
        {
            return (source.Target.transform.position - source.transform.position).normalized;
        }
        
        private float DistanceToTarget(Vector3 origin, Vector3 target)
        {
            return Vector3.Distance(origin, target);
        }
    }

    public class AttackState : State<EnemyCharacter>
    {
        public override void Enter(EnemyCharacter source)
        {
            source.IsAttacking = true;
            source.MonsterAnimator.SetState("Idle");
        }

        public override void Execute(EnemyCharacter source)
        {
            if (source.Target == null || !source.Target.IsAlive) source.ChangeState(EnemyStates.Patrol);
            var hit = Physics2D.Raycast(source.transform.position + DirectionToTarget(source) * 0.45f, DirectionToTarget(source), source.MaxDistanceToTarget);
            if (hit.collider == null) source.ChangeState(EnemyStates.Chase);
            if ((1 << source.Target.gameObject.layer & (1 << hit.collider.gameObject.layer)) == 0 || DistanceToTarget(source.transform.position, source.Target.transform.position) > source.AttackRange * 1.2f)
            {
                source.ChangeState(EnemyStates.Chase);
            }
        }

        public override void Exit(EnemyCharacter source)
        {
            source.IsAttacking = false;
        }
        private Vector3 DirectionToTarget(EnemyCharacter source)
        {
            return (source.Target.transform.position - source.transform.position).normalized;
        }
        
        private float DistanceToTarget(Vector3 origin, Vector3 target)
        {
            return Vector3.Distance(origin, target);
        }
    }

    public class DeadState : State<EnemyCharacter>
    {
        public override void Enter(EnemyCharacter source)
        {
            Debug.Log("Changed to dead state");
            source.MonsterAnimator.SetState("Idle");
            source.WeaponHandler?.StopAllCoroutines();
        }

        public override void Execute(EnemyCharacter source)
        {
        }

        public override void Exit(EnemyCharacter source)
        {
            source.IsAlive = true;
        }
    }
}