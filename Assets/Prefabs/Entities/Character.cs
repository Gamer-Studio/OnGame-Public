#nullable enable
using System;
using OnGame.Prefabs.Items.Weapon.WeaponHandlers;
using OnGame.UI;
using OnGame.Utils;
using OnGame.Utils.States;
using OnGame.Utils.States.PlayerState;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
#pragma warning disable CS8618 // 생성자를 종료할 때, null이 가능하지 않은 필드에는 null이 아닌 값을 포함해야 합니다. 'required' 제어자를 추가하거나 null이 가능한 것으로 선언해 보세요.

namespace OnGame.Prefabs.Entities
{
    public enum Direction
    {
        South,
        East,
        North,
        West
    }

    public enum StatTypes
    {
        Health,
        Mana, 
        Attack, 
        Defense, 
        CriticalMultiplier, 
        CriticalPossibility,
    }

    public enum WeaponStatTypes
    {
        
    }

    public class Character : Entity
    {   
        // Const Fields
        public static readonly int Angle = Animator.StringToHash("Direction");
        public static readonly int IsDamage = Animator.StringToHash("IsDamage");
        public static readonly int IsMove = Animator.StringToHash("IsMove");
        private readonly SerializableDictionary<float, bool[]> chanceCache = new();
        
        // Component Fields
        [Header("Components")] [SerializeField]
        protected Animator animator = new();

        [SerializeField] protected TrailRenderer trailRenderer = new();
        [SerializeField] protected Color trailColor;
        
        // State Fields
        [Header("States")] 
        [SerializeField] [GetSet("IsInvincible")] private bool isInvincible;
        [SerializeField] [GetSet("IsAlive")] private bool isAlive = true;
        [SerializeField] [GetSet("IsAttacking")] private bool isAttacking;
        [SerializeField] [GetSet("IsInteracting")] private bool isInteracting;
        [SerializeField] [GetSet("IsDashing")] private bool isDashing;
        
        // Stats Fields
        private int availablePoint;
        
        // Cooldown Fields
        [Header("Cooldowns")]
        [SerializeField] private float timeSinceLastAttack = float.MaxValue;
        [SerializeField] private float timeSinceLastDashed = float.MaxValue;
        [SerializeField] private float timeSinceLastInvincible = float.MaxValue;
        [SerializeField] private float timeSinceLastGuard = float.MaxValue;
        [SerializeField] private float dashCoolTime = 5f;
        [SerializeField] private float invincibleTimeDelay = 0.5f;
        [SerializeField] private float enableParryTime = 5f;
        
        // Physics Fields
        [Header("Physics")] 
        [SerializeField] protected Vector2 lookAtDirection = Vector2.zero;
        [SerializeField] protected Vector2 movementDirection = Vector2.zero;

        // Weapon Fields
        [Header("Weapons")]
        [SerializeField] private Transform weaponPivot;
        [SerializeField] private WeaponHandler weaponPrefab;
        private WeaponHandler weaponHandler;
        public Transform WeaponPivot => weaponPivot;
        
        // Properties
        public bool IsInvincible { get => isInvincible; set => isInvincible = value; }
        public bool IsAlive { get => isAlive; set => isAlive = value; }
        public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
        public bool IsInteracting { get => isInteracting; set => isInteracting = value; }
        public bool IsDashing { get => isDashing; set => isDashing = value; }
        public bool IsDashTrailEnabled { get; set; }
        public bool IsDashAvailable { get; private set; } = true;
        
        public Animator Animator => animator;
        public TrailRenderer TrailRenderer => trailRenderer;
        public Color TrailColor => trailColor;
        public Vector2 MovementDirection { get => movementDirection; set => movementDirection = value; }
        public Vector2 LookAtDirection { get => lookAtDirection; set => lookAtDirection = value; }

        // State Machine
        private State<Character>[] states = Array.Empty<State<Character>>();
        public PlayerStates CurrentState { get; private set; } = PlayerStates.Idle;
        public StateMachine<Character> StateMachine { get; private set; } = new();
        
        // Action event
        public UnityEvent onDeath = new();
        
        private void Awake()
        {
            if (animator == null) animator = Helper.GetComponentInChildren_Helper<Animator>(gameObject);
            
            // Sets Player States
            SetUp();
        }

        private void Start()
        {
            OnChangeWeapon(weaponPrefab);

            var arr = new bool[100];
            for (var i = 0; i < CriticalPossibility.Value * 100; i++) arr[i] = true;
            ShuffleArray(arr);
            chanceCache[CriticalPossibility.Value] = arr;
        }

        /// <summary>
        /// Update is called every frame if the MonoBehaviour is enabled.
        /// </summary>
        protected override void Update()
        {
            base.Update();

            // State Machine
            StateMachine.Execute();
        }

        /// <summary>
        /// Handle Multiple Actions
        /// </summary>
        protected override void HandleAction()
        {
            HandleAttackDelay();
            HandleDashCoolTime();
            HandleDashTrailTime();
            HandleGuardTime();
            HandleInvincibleTimeDelay();
        }

        /// <summary>
        /// Handle Attack Delay
        /// </summary>
        private void HandleAttackDelay()
        {
            if (timeSinceLastAttack <= weaponHandler.TotalDelay)
            {
                timeSinceLastAttack += Time.deltaTime;
            }

            if (!isAttacking || timeSinceLastAttack < weaponHandler.TotalDelay) return;
            timeSinceLastAttack = 0;
            OnAttack();
        }

        /// <summary>
        /// Handle Invincibility Delay
        /// </summary>
        private void HandleInvincibleTimeDelay()
        {
            if (!isInvincible) return;
            
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

        /// <summary>
        /// Handle CoolTime of Dash Skill
        /// </summary>
        private void HandleDashCoolTime()
        {
            if (IsDashAvailable) return;

            if (timeSinceLastDashed <= dashCoolTime)
            {
                timeSinceLastDashed += Time.deltaTime;
            }
            else
            {
                IsDashAvailable = true;
                timeSinceLastDashed = 0;
            }
        }

        private void HandleDashTrailTime()
        {
            if (!IsDashTrailEnabled) return;

            if (trailRenderer.time < 0.01f)
            {
                trailRenderer.time = 0f;
                IsDashTrailEnabled = false;
            } else
            {
                trailRenderer.time = Mathf.Lerp(trailRenderer.time, 0f, Time.deltaTime * 10f);
            }
        }

        private void HandleGuardTime()
        {
            if (CurrentState != PlayerStates.Guard) { timeSinceLastGuard = float.MaxValue; return; }

            if (timeSinceLastGuard <= enableParryTime)
            {
                timeSinceLastGuard += Time.deltaTime;
            }
            else
            {
                timeSinceLastGuard = float.MaxValue;
            }
        }

        /// <summary>
        /// Called when Player changes Weapon
        /// </summary>
        /// <param name="prefab"></param>
        public void OnChangeWeapon(WeaponHandler prefab)
        {
            if (weaponHandler != null) { Destroy(weaponHandler.gameObject); }
            weaponHandler = Instantiate(prefab, weaponPivot);
            weaponHandler.Init(this);
            
            UIManager.instance.SetWeapon(weaponHandler.WeaponName, weaponHandler.CurrentUpgradeLevel);
        }

        /// <summary>
        /// Called when Weapon levels up
        /// </summary>
        /// <param name="statType"></param>
        /// <param name="coef"></param>
        public void OnLevelUp_Weapon(WeaponStatTypes statType, float coef)
        {
            
        }

        public void OnRotateWeapon(float rotZ)
        {
            weaponHandler?.Rotate(rotZ);
        }

        /// <summary>
        /// Called when Player earned exp point
        /// </summary>
        /// <param name="exp"></param>
        public void OnEarnExp(int exp)
        {
            experience.Value += exp;
            if(experience.Value >= experience.Max) OnLevelUp();
        }

        /// <summary>
        /// Called when Player level ups
        /// </summary>
        private void OnLevelUp()
        {
            availablePoint++;
            level++;
            health.Value += health.Max;
            mana.Value += mana.Max;
            experience.Value -= experience.Max;
            MaxExperienceOpers.Add(x => x + 50);
            
            // Invalidate UI
            UIManager.instance.SetHp(health.Value);
            UIManager.instance.SetMp(mana.Value);
            UIManager.instance.SetLevel(level);
        }

        /// <summary>
        /// Change Stat. By type
        /// </summary>
        /// <param name="statType"></param>
        public void OnStatusChange(StatTypes statType)
        {
            if (availablePoint <= 0) return;
            availablePoint--;
            switch (statType)
            {
                case StatTypes.Health:
                    MaxHealthOpers.Add(x => x + 50);
                    break;
                case StatTypes.Mana:
                    MaxManaOpers.Add(x => x + 25);
                    break;
                case StatTypes.Attack:
                    AttackOpers.Add(x => x + 5);
                    break;
                case StatTypes.Defense:
                    DefenseOpers.Add(x => x + 5);
                    break;
                case StatTypes.CriticalMultiplier:
                    CriticalMultiplierOpers.Add(x => x + 0.1f);
                    break;
                case StatTypes.CriticalPossibility:
                    CriticalPossibilityOpers.Add(x => x + 0.1f);
                    break;
            }
        }

        /// <summary>
        /// Called when Player attacks
        /// </summary>
        private void OnAttack()
        {
            if (!isAlive || lookAtDirection == Vector2.zero || weaponHandler == null || CurrentState == PlayerStates.Guard) return;
            weaponHandler.Attack();
        }
        
        /// <summary>
        /// Apply Knockback to Enemy
        /// </summary>
        /// <param name="other"></param>
        /// <param name="power"></param>
        public void ApplyKnockBack(Transform other, float power)
        {
            rigidBody.AddForce(-(other.position - transform.position).normalized * power, ForceMode2D.Impulse);
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
                chanceCache[CriticalPossibility.Value] = arr;
            }
            
            var random = Random.Range(0, 100);
            return chanceCache[CriticalPossibility.Value][random];
        }

        /// <summary>
        /// Called when Player used Dash Skill
        /// </summary>
        public void OnDash()
        {
            if (!isAlive || !IsDashAvailable) return;

            IsDashAvailable = false;
            isInvincible = true;
            timeSinceLastInvincible = 0;
            timeSinceLastDashed = 0;
            
            ChangeState(PlayerStates.Dash);
        }

        /// <summary>
        /// Called when Player used Guard Skill
        /// </summary>
        public void OnGuard()
        {
            if (!isAlive || mana.Value <= 0) return;

            timeSinceLastGuard = 0;
            ChangeState(PlayerStates.Guard);
        }

        /// <summary>
        /// Called when Player got damage
        /// </summary>
        /// <param name="damage"></param>
        public void OnDamage(float damage)
        {
            if (!isAlive || isInvincible) return;

            var calculatedDamage = damage * (1f - defenseStat.Value / 100f);
            
            health.Value += Mathf.CeilToInt(calculatedDamage);
            UIManager.instance.SetHp(health.Value);
            if(health.Value <= 0) { Die(); ChangeState(PlayerStates.Dead);  return; }
            if (CurrentState == PlayerStates.Guard)
            {
                if (timeSinceLastGuard <= enableParryTime)
                {
                    AudioManager.Instance.PlaySFX(SoundType.Guard);
                    ChangeState(PlayerStates.Idle); OnManaRecover(20);
                }
                else { OnManaConsume(40); }
            }
            
            AudioManager.Instance.PlaySFX(SoundType.Hit);
            animator.SetBool(IsDamage, true);
            isInvincible = true;
        }
        
        public void OnHealthRecover(int coef)
        {
            if (!isAlive) return;
            health.Value += coef;
            UIManager.instance.SetHp(health.Value);
        }

        public void OnManaConsume(int coef)
        {
            if (!isAlive) return;
            mana.Value -= coef;
            UIManager.instance.SetMp(mana.Value);
        }

        public void OnManaRecover(int coef)
        {
            if (!isAlive) return;
            mana.Value += coef;
            UIManager.instance.SetMp(mana.Value);
        }

        private void Die()
        {
            isAlive = false;
            rigidBody.velocity = Vector2.zero;
            animator.SetBool(IsMove, false);

            onDeath?.Invoke();
        }
        
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
        
        #endregion

        #region Basic Action Rules

        /// <summary>
        /// State Machine Setup
        /// </summary>
        private void SetUp()
        {
            states = new State<Character>[Enum.GetValues(typeof(PlayerStates)).Length];
            for (var i = 0; i < states.Length; i++) states[i] = GetState((PlayerStates)i);
            StateMachine = new StateMachine<Character>();
            StateMachine.SetUp(this, states[(int)PlayerStates.Idle]);
        }

        /// <summary>
        /// PlayerState 기준에 따라 어떤 작업을 수행할 것인지 정해주는 작업
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private State<Character> GetState(PlayerStates state)
        {
            return state switch
            {
                PlayerStates.Idle => new IdleState(),
                PlayerStates.Move => new MoveState(),
                PlayerStates.Dash => new DashState(),
                PlayerStates.Guard => new GuardState(),
                PlayerStates.Dead => new DeadState(),
                _ => new IdleState()
            };
        }

        /// <summary>
        /// State Change가 필요할 때 호출하는 함수
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(PlayerStates newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            StateMachine.ChangeState(states[(int)newState]);
        }

        #endregion
    }
}