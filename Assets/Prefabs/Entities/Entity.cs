using System.Collections.Generic;
using System.Linq;
using OnGame.Utils;
using UnityEngine;

namespace OnGame.Prefabs.Entities
{
    public class Entity : MonoBehaviour
    {
        // Config Fields
        [Header("Configs")] 
        [Range(1f, 100f)] [SerializeField] protected float speed = 20f;
        [Range(0f, 20f)] [SerializeField] protected float drag = 10f;
        [Range(1f, 100f)] [SerializeField] protected float moveForce = 30f;

        // Stat. Fields
        [Header("Stats")] 
        [SerializeField] protected Stat<float> attackStat;
        [SerializeField] protected Stat<float> defenseStat;
        [SerializeField] protected RangedStat health;
        [SerializeField] protected RangedStat mana;
        [SerializeField] protected RangedStat experience;
        [SerializeField] protected int level = 1;
        public Stat<float> CriticalMultiplier;
        public Stat<float> CriticalPossibility;

        // Buff
        [SerializeField] private List<Buff> buffs = new();
        
        // Stat Operator Fields
        public List<StatOperator<float>> AttackOpers = new();
        public List<StatOperator<float>> DefenseOpers = new();
        public List<StatOperator<int>> MaxHealthOpers = new();
        public List<StatOperator<int>> MaxManaOpers = new();
        public List<StatOperator<int>> MaxExperienceOpers = new();
        public List<StatOperator<float>> CriticalMultiplierOpers = new();
        public List<StatOperator<float>> CriticalPossibilityOpers = new();
        
        // Component Fields
        [Header("RigidBody Components")]
        [SerializeField] protected Rigidbody2D rigidBody;

        // Properties
        public Rigidbody2D RigidBody => rigidBody;
        public float Speed { get => speed; set => speed = value; }
        public float Drag => drag;
        public float MoveForce { get => moveForce; set => moveForce = value; }
        public RangedStat Mana => mana;
        public RangedStat Health => health;

        protected virtual void Update()
        {
            HandleAction();
        }

        // 스텟 연산자 List 병합
        // 실제로 합쳐지는 건 아니고, List 연산하는 과정을 분리하여 합쳐진 것처럼 보이도록 했습니다.
        // 덕분에 여전히 List가 바뀌면 스텟에도 반영되유
        public static StatOperator<T> MergeStatOperator<T>(List<StatOperator<T>> operList)
         => value => operList.Aggregate(value, (current, oper) => oper(current));
        
        /// <summary>
        /// Initialize Stat. With default values and operators.
        /// </summary>
        public virtual void Init()
        {
            attackStat = new Stat<float>(10f, MergeStatOperator(AttackOpers));
            defenseStat = new Stat<float>(5f, MergeStatOperator(DefenseOpers));
            health = new RangedStat(100, 100, MergeStatOperator(MaxHealthOpers));
            mana = new RangedStat(100, 100, MergeStatOperator(MaxManaOpers));
            experience = new RangedStat(100, 0, MergeStatOperator(MaxExperienceOpers));
            CriticalMultiplier = new Stat<float>(1.5f, MergeStatOperator(CriticalMultiplierOpers));
            CriticalPossibility = new Stat<float>(0.1f, MergeStatOperator(CriticalPossibilityOpers));
        }

        protected virtual void HandleAction() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buff">추가할 버프(상태이상)</param>
        /// <param name="update">기존에 버프가 있다면 버프의 지속시간을 갱신할 것인지</param>
        public void AddBuff(Buff buff, bool update = true)
        {
            buff.subscriber(this);
            buffs.Add(buff);
        }
        
        public void RemoveBuff(Buff buff)
        {
            if (!buffs.Contains(buff)) return;
            
            buff.unsubscriber(this);
            buffs.Remove(buff);
        }
    }
}