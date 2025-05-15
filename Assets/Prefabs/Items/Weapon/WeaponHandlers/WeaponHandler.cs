using System.Collections;
using OnGame.Prefabs.Entities;
using UnityEngine;

namespace OnGame.Prefabs.Items.Weapon.WeaponHandlers
{
    public class WeaponHandler : MonoBehaviour
    {
        // Constant Fields
        private static readonly int IsAttack = Animator.StringToHash("IsAttack");
        
        // Component Fields
        [Header("Component Fields")]
        [SerializeField] protected Animator animator;
        [SerializeField] protected Transform weaponTransform;

        [Header("Weapon Stat.")] 
        [SerializeField] protected string weaponName = "";
        [SerializeField] protected int currentUpgradeLevel;
        [SerializeField] protected int maxUpgradeLevel = 4;
        
        // Weapon Settings.
        [Header("Weapon Settings")]
        [SerializeField] private float delay = 1.0f;
        [SerializeField] protected float animationDelay = 0.1f;
        [SerializeField] private float weaponSize = 1f;
        [SerializeField] private float power = 1f;
        [SerializeField] private float speed = 1f;
        [SerializeField] private float attackRange = 10f;
        [SerializeField] protected LayerMask target;
        [SerializeField] protected AudioClip attackClip;
        
        [Header("Knock Back Info")] 
        [SerializeField] private bool isOnKnockback = false; 
        [SerializeField] private float knockBackPower = 10f; 
        
        
        // Properties
        public string WeaponName => weaponName;
        public int CurrentUpgradeLevel => currentUpgradeLevel;
        public int MaxUpgradeLevel => maxUpgradeLevel;
        public float TotalDelay => delay + animationDelay;
        public float AttackRange => attackRange;
        public LayerMask Target => target; 
        public AudioClip AttackClip => attackClip;
        
        public float WeaponSize { get => weaponSize; set => weaponSize = value; }
        public float Power { get => power; set => power = value; }
        public float Speed { get => speed; set => speed = value; }
        public float Delay { get => delay; set => delay = value; }
        public bool IsOnKnockback { get => isOnKnockback; set => isOnKnockback = value; }
        public float KnockBackPower { get => knockBackPower; set => knockBackPower = value; } 
        public Entity Character { get; private set; }

        public void Init(Entity character)
        {
            Character = character;
            animator.speed = 1.0f / delay; 
            transform.localScale = Vector3.one * weaponSize;
        }

        protected virtual void Start() { }

        public virtual void Attack()
        {
            StartCoroutine(DelayedAttack());
        }

        private IEnumerator DelayedAttack()
        {
            AttackAnimation();
            yield return new WaitForSeconds(animationDelay);
            AudioManager.Instance.PlaySFX(SoundType.Attack);
            OnAttackAfterDelay();
            yield return null;
        }
        
        protected virtual void OnAttackAfterDelay() { }

        public void AttackAnimation()
        {
            animator.SetTrigger(IsAttack);
        }

        public void Rotate(float rotZ)
        {
            weaponTransform.localScale = rotZ is > 135f or < -135f ? new Vector3(1, -1, 1) : Vector3.one;
        }
    }
}