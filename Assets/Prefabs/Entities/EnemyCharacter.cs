using System;
using System.Collections.Generic;
using OnGame.Prefabs.Items.Weapon.WeaponHandlers;
using OnGame.Scenes.World;
using OnGame.Utils;
using OnGame.Utils.States;
using OnGame.Utils.States.EnemyState;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace OnGame.Prefabs.Entities
{
    public class EnemyCharacter : Entity
    {
        // Const Fields
        public readonly int Angle = Animator.StringToHash("Direction");
        public readonly int IsDamage = Animator.StringToHash("IsDamage");
        public readonly int IsMove = Animator.StringToHash("IsMove");
        private readonly Dictionary<float, bool[]> chanceCache = new();
        
        // Component Fields
        [Header("Components")]
        [SerializeField] protected Animator animator;
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected MonsterAnimatorController monsterAnimator;
        [SerializeField] protected GaugeBar healthUI;
        
        
        // State Fields
        [Header("States")] 
        [SerializeField] [GetSet("IsInvincible")] private bool isInvincible;
        [SerializeField] [GetSet("IsAlive")] private bool isAlive = true;
        [SerializeField] [GetSet("IsAttacking")] private bool isAttacking = false;
        
        // Stats Fields
        private float originalSpeed;
        
        // Cooldown Fields
        [Header("Cooldowns")]
        [SerializeField] private float timeSinceLastAttack = float.MaxValue;
        [SerializeField] private float timeSinceLastInvincible = float.MaxValue;
        [SerializeField] private float invincibleTimeDelay = 0.5f; 
        
        // Physics Fields
        [Header("Physics")] 
        [SerializeField] protected Vector2 lookAtDirection = Vector2.zero;
        [SerializeField] protected Vector2 movementDirection = Vector2.zero;
        
        // Target of Enemy
        [Header("Target")] 
        [SerializeField] protected LayerMask levelCollisionLayer;
        [SerializeField] protected Character target;
        [SerializeField] protected float maxDistanceToTarget = 30f;
        
        // Weapon Fields
        [Header("Weapons")]
        [SerializeField] private Transform weaponPivot;
        [SerializeField] private WeaponHandler weaponPrefab;
        private WeaponHandler weaponHandler;
        
        // Path Finding Fields
        [Header("Path Finding")]
        [SerializeField] protected Seeker seeker;
        [SerializeField] protected float nextWayPointDistance = 3f;
        [SerializeField] protected int currentWayPoint = 0;
        [SerializeField] protected float repathRate = 0.3f;
        [SerializeField] private float lastRepath = float.NegativeInfinity;
        [SerializeField] protected bool reachedEndOfPath;
        [SerializeField] protected float stopDistance = 0.4f;
        public Path path;
        public bool ReachedEndOfPath { get => reachedEndOfPath; set => reachedEndOfPath = value; }
        public float searchRate = 0.5f;

        // Properties
        public bool IsInvincible { get => isInvincible; set => isInvincible = value; }
        public bool IsAlive { get => isAlive; set => isAlive = value; }
        public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
        public Vector2 MovementDirection => movementDirection;
        public Vector2 LookAtDirection => lookAtDirection;
        public Animator Animator => animator;
        public MonsterAnimatorController MonsterAnimator => monsterAnimator;
        public Character Target => target;
        public LayerMask LevelCollisionLayer => levelCollisionLayer;
        public float MaxDistanceToTarget => maxDistanceToTarget;
        public float AttackRange => weaponHandler?.AttackRange ?? 1.0f;
        public float AttackDelay => weaponHandler?.TotalDelay ?? 0.5f;
        public WeaponHandler WeaponHandler => weaponHandler ?? null;
        
        // State Machine
        private State<EnemyCharacter>[] states;
        public EnemyStates CurrentState { get; private set; } = EnemyStates.Patrol;
        public StateMachine<EnemyCharacter> StateMachine { get; private set; }
        
        // Action event
        public UnityEvent onDeath;
        
        private void Awake()
        {
            if (animator == null) animator = Helper.GetComponentInChildren_Helper<Animator>(gameObject);
            if (target == null) target = FindFirstObjectByType<Player>().Character;
            seeker = Helper.GetComponentInChildren_Helper<Seeker>(gameObject);
            
            // Sets Player States
            SetUp();
        }

        private void Start()
        {
            OnChangeWeapon(weaponPrefab);
            healthUI.ChangeMax(100);
        }

        protected override void Update()
        {
            base.Update();
            
            // State Machine
            StateMachine.Execute();
        }

        protected override void HandleAction()
        {
            Rotate(lookAtDirection);
            HandleAttackDelay();
            HandleInvincibleTimeDelay();
        }

        private void FixedUpdate()
        {
            if (target == null || !isAlive) return;
            
            if (CurrentState == EnemyStates.Patrol) return;
            lookAtDirection = DirectionToTarget();
        }

        /// <summary>
        /// Move Enemy Character
        /// </summary>
        /// <param name="direction">Movement Direction</param>
        public void Move(Vector2 direction)
        {
            if(rigidBody.velocity.magnitude > speed)
            {
                rigidBody.velocity *= (speed / rigidBody.velocity.magnitude);    
            }

            rigidBody.AddForce(direction.normalized * moveForce, ForceMode2D.Force);
        }
        
        /// <summary>
        /// Character Rotation Action
        /// </summary>
        /// <param name="direction">LookAt Direction</param>
        private void Rotate(Vector2 direction)
        {
            if (!isAlive) return;
            
            // Flip Character Sprite if it's left or right
            var rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var isLeft = Mathf.Abs(rotZ) > 90f;
            spriteRenderer.flipX = isLeft;
            
            // Weapon Pivot을 Character로 설정했기에 character의 rotation을 변경
            weaponPivot.rotation = Quaternion.Euler(0f, 0f, rotZ);
            weaponHandler?.Rotate(rotZ);
        }
        
        /// <summary>
        /// Called when Player changes Weapon
        /// </summary>
        /// <param name="prefab">Weapon Prefab</param>
        public void OnChangeWeapon(WeaponHandler prefab)
        {
            if (weaponHandler != null) { Destroy(weaponHandler.gameObject); }
            weaponHandler = Instantiate(prefab, weaponPivot);
            weaponHandler.Init(this);
        }
        
        /// <summary>
        /// Called when Player attacks
        /// </summary>
        private void OnAttack()
        {
            if (lookAtDirection == Vector2.zero || weaponHandler == null) return;
            weaponHandler.Attack();
        }
        
        /// <summary>
        /// Apply Knockback Force to Player
        /// </summary>
        /// <param name="other"></param>
        /// <param name="power"></param>
        public void ApplyKnockBack(Transform other, float power) 
        { 
            rigidBody.AddForce(-(other.position - transform.position).normalized * power, ForceMode2D.Impulse);
        }

        /// <summary>
        /// Called when Enemy takes damage
        /// </summary>
        /// <param name="damage"></param>
        public void OnDamage(float damage)
        {
            if (!isAlive || isInvincible) return;

            var calculatedDamage = damage * (1f - defenseStat.Value / 100f);
            health.Value += Mathf.CeilToInt(calculatedDamage);
            healthUI.Value = health.Value;
            if(health.Value <= 0) { ChangeState(EnemyStates.Dead); Die(); return; }
            
            animator.SetBool(IsDamage, true);
            isInvincible = true;
            timeSinceLastInvincible = 0f;
        }
        
        /// <summary>
        /// Returns Calculated Damage -> (Base Attack + Weapon Power) * CriticalMultiplier
        /// </summary>
        /// <returns>Returns float value of calculated damage</returns>
        public float Return_CalculatedDamage()
        {
            var damage = attackStat.Value + weaponHandler.Power;
            if (IsCriticalHit()) damage *= CriticalMultiplier.Value;
            return damage;
        }

        /// <summary>
        /// Check if it's CriticalHit or not
        /// </summary>
        /// <returns>Returns random bool value</returns>
        private bool IsCriticalHit()
        {
            if (!chanceCache.ContainsKey(CriticalPossibility.Value))
            {
                var arr = new bool[100];
                for (var i = 0; i < CriticalPossibility.Value * 100; i++) arr[i] = true;
                ShuffleArray(arr);
                chanceCache.Add(CriticalPossibility.Value, arr);
            }
            
            var random = Random.Range(0, 100);
            return chanceCache[CriticalPossibility.Value][random];
        }
        
        public void OnHealthRecover(int coef)
        {
            if (!isAlive) return;
            health.Value += coef;
        }

        private void Die()
        {
            isAlive = false;
            rigidBody.velocity = Vector2.zero;

            onDeath.Invoke();
        }
        
        #region Path Finding
        
        /// <summary>
        /// Path Finding Method
        /// </summary>
        public void MoveWithPath()
        {
            if (Time.time > lastRepath + repathRate && seeker.IsDone())
            {
                lastRepath = Time.time;
                var hit = Physics2D.Raycast(transform.position + DirectionToTarget() * 0.5f, DirectionToTarget(), maxDistanceToTarget);
                if ((1 << target.gameObject.layer & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    seeker.StartPath(transform.position, target.transform.position, OnPathComplete);
                }
            }
            
            if (path == null) {
                // We have no path to follow yet, so don't do anything
                return;
            }

            // Check in a loop if we are close enough to the current waypoint to switch to the next one.
            // We do this in a loop because many waypoints might be close to each other and we may reach
            // several of them in the same frame.
            reachedEndOfPath = false;
            // The distance to the next waypoint in the path
            float distanceToWaypoint;
            while (true) {
                // If you want maximum performance you can check the squared distance instead to get rid of a
                // square root calculation. But that is outside the scope of this tutorial.
                distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWayPoint]);
                if (distanceToWaypoint < nextWayPointDistance) {
                    // Check if there is another waypoint or if we have reached the end of the path
                    if (currentWayPoint + 1 < path.vectorPath.Count) {
                        currentWayPoint++;
                    } else {
                        // Set a status variable to indicate that the agent has reached the end of the path.
                        // You can use this to trigger some special code if your game requires that.
                        reachedEndOfPath = true;
                        break;
                    }
                } else {
                    break;
                }
            }
            
            // Slow down smoothly upon approaching the end of the path
            // This value will smoothly go from 1 to 0 as the agent approaches the last waypoint in the path.
            var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint/nextWayPointDistance) : 1f;

            // Direction to the next waypoint
            // Normalize it so that it has a length of 1 world unit
            var dir = (path.vectorPath[currentWayPoint] - transform.position).normalized;

            // Multiply the direction by our desired speed to get a velocity
            var velocity = dir * (speed * speedFactor);
            
            transform.parent.position += velocity * Time.deltaTime;
        }
        
        public void OnPathComplete(Path p)
        {
            Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

            // Path pooling. To avoid unnecessary allocations paths are reference counted.
            // Calling Claim will increase the reference count by 1 and Release will reduce
            // it by one, when it reaches zero the path will be pooled and then it may be used
            // by other scripts. The ABPath.Construct and Seeker.StartPath methods will
            // take a path from the pool if possible. See also the documentation page about path pooling.
            p.Claim(this);
            if (!p.error) { 
                path?.Release(this);
                path = p;
                // Reset the waypoint counter so that we start to move towards the first point in the path
                currentWayPoint = 0;
            } else {
                p.Release(this);
            }
        }
        
        #endregion
        
        #region Handlers

        /// <summary>
        /// Handle Attack Delay
        /// </summary>
        private void HandleAttackDelay()
        {
            if (!isAlive) return;
            if (timeSinceLastAttack <= weaponHandler.TotalDelay)
            {
                timeSinceLastAttack += Time.deltaTime;
            }
            else
            {
                if (!isAttacking) return;
                timeSinceLastAttack = 0;
                OnAttack();
            }
        }

        /// <summary>
        ///    Handle Invincibility Delay
        /// </summary>
        private void HandleInvincibleTimeDelay()
        {
            if (!isInvincible || !isAlive) return;

            if (timeSinceLastInvincible <= invincibleTimeDelay)
            {
                timeSinceLastInvincible += Time.deltaTime;
            }
            else
            {
                isInvincible = false;
                timeSinceLastInvincible = 0;
            }
        }        
        
        #endregion
        
        #region Utils

        /// <summary>
        /// Shuffles Array Type
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        private void ShuffleArray<T>(T[] array)
        {
            for (var i = array.Length - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }

        /// <summary>
        /// Calculate Direction to target
        /// </summary>
        /// <returns>Returns Vector3 value of direction to target</returns>
        private Vector3 DirectionToTarget()
        {
            return (target.transform.position - transform.position).normalized;
        }
        
        #endregion

        #region Basic Action Rules

        /// <summary>
        /// State Machine Setup
        /// </summary>
        private void SetUp()
        {
            states = new State<EnemyCharacter>[Enum.GetValues(typeof(EnemyStates)).Length];
            for (var i = 0; i < states.Length; i++) states[i] = GetState((EnemyStates)i);
            StateMachine = new StateMachine<EnemyCharacter>();
            StateMachine.SetUp(this, states[(int)EnemyStates.Patrol]);
        }

        /// <summary>
        /// PlayerState 기준에 따라 어떤 작업을 수행할 것인지 정해주는 작업
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private State<EnemyCharacter> GetState(EnemyStates state)
        {
            return state switch
            {
                EnemyStates.Patrol => new PatrolState(),
                EnemyStates.Chase => new ChaseState(),
                EnemyStates.Attack => new AttackState(),
                EnemyStates.Dead => new DeadState(),
                _ => new PatrolState()
            };
        }

        /// <summary>
        /// State Change가 필요할 때 호출하는 함수
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(EnemyStates newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            StateMachine.ChangeState(states[(int)newState]);
        }

        #endregion
    }
}
