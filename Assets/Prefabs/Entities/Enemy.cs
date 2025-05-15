using UnityEngine;

namespace OnGame.Prefabs.Entities
{
    public class Enemy : MonoBehaviour
    {
        // Character Field
        [Header("Character Entity")]
        [SerializeField] private EnemyCharacter character;
        
        private Rigidbody2D rigidBody;
        private float speed;
        private float drag;
        private float moveForce;
        
        // Properties
        public EnemyCharacter Character => character;
        
        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// </summary>
        private void Start()
        {
            character.Init();
            
            rigidBody = character.RigidBody;
            speed = character.Speed;
            drag = character.Drag;
            rigidBody.drag = drag;
            moveForce = character.MoveForce;
        }
    }
}