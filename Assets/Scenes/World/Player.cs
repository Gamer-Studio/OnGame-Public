using System.Collections.Generic;
using Cinemachine;
using OnGame.Prefabs.Entities;
using OnGame.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace OnGame.Scenes.World
{
    public class Player : MonoBehaviour
    {
        // Character Field
        [Header("Character Entity")]
        [SerializeField] private Character character;

        // Component Fields
        [Header("Components")] 
        [SerializeField] private CinemachineVirtualCamera cam;

        public Inventory inventory;
        
        [Header("Camera Settings")]
        [Range(0.1f, 3f)] [SerializeField] private float zoomRate = 0.5f;
        [Range(0.1f, 5f)] [SerializeField] private float zoomSpeed = 3f;
        [Range(1f, 10f)] [SerializeField] private float minZoom = 1f;
        [Range(1f, 20f)] [SerializeField] private float maxZoom = 5f;
        
        private Rigidbody2D rigidBody;
        private float speed;
        private float drag;
        private float moveForce;
        private float currentZoom;
        private float newZoom;
        private float scroll;

        // Properties
        public Character Character => character;

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
            currentZoom = cam.m_Lens.OrthographicSize;
            newZoom = currentZoom;
        }

        /// <summary>
        /// Update is called every frame if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            if (!character.IsAlive) return;
            
            Rotate(character.LookAtDirection);
            CalculateCamZoom();
        }

        private void FixedUpdate()
        {
            if (!character.IsAlive) return;
            
            Movement(character.MovementDirection);
        }

        /// <summary>
        /// Change camera zoom with lerp
        /// </summary>
        private void LateUpdate()
        {
            if (!character.IsAlive) return;
            
            currentZoom = Mathf.Lerp(currentZoom, newZoom, Time.deltaTime * zoomSpeed);
            cam.m_Lens.OrthographicSize = currentZoom;
        }

        /// <summary>
        /// Character Movement Action
        /// </summary>
        /// <param name="direction"></param>
        private void Movement(Vector2 direction)
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
        /// <param name="direction"></param>
        private void Rotate(Vector2 direction)
        {
            var rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            var confDirection = rotZ switch
            {
                > 45f and < 135f => Direction.North,
                < -45f and > -135f => Direction.South,
                >= 135f or <= -135f => Direction.West,
                _ => Direction.East
            };

            // Weapon Pivot을 Character로 설정했기에 character의 rotation을 변경
            character.WeaponPivot.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
            character.OnRotateWeapon(rotZ);
            
            // Movement Animation
            character.Animator.SetInteger(Character.Angle, (int)confDirection);
        }

        /// <summary>
        /// Calculate Camera Zoom Action
        /// </summary>
        private void CalculateCamZoom()
        {
            if (!(Mathf.Abs(scroll) > 0.01f)) return;
            newZoom = currentZoom - Mathf.Clamp(scroll * zoomRate, -1f, 1f);
            newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
            scroll = 0;
        }

        #region Input Actions

        private void OnMove(InputValue value)
        {
            if (character.IsDashing) return;
            character.MovementDirection = value.Get<Vector2>();
        }

        private void OnLook(InputValue value)
        {
            var mousePosition = value.Get<Vector2>();
            var activeCamera = CinemachineCore.Instance.GetActiveBrain(0).OutputCamera;
            Vector2 worldPos = activeCamera.ScreenToWorldPoint(mousePosition);

            character.LookAtDirection = (worldPos - (Vector2)transform.position).normalized;
        }

        private void OnZoom(InputValue value)
        {
            scroll = value.Get<float>();
            CalculateCamZoom();
        }

        private void OnDash(InputValue value)
        {
            if (!character.IsDashAvailable) return;
            
            var val = value.Get<float>();
            if (val <= 0f) return;
            character.OnDash();
            if(character.MovementDirection == Vector2.zero) 
                rigidBody.AddForce(character.LookAtDirection * character.Speed * 10f, ForceMode2D.Impulse);
            else rigidBody.AddForce(character.MovementDirection * character.Speed * 10f, ForceMode2D.Impulse);
        }

        private void OnFire(InputValue value)
        {
            if(!character.IsAlive || IsPointerOverUI()) { character.IsAttacking = false; return;}
            character.IsAttacking = value.Get<float>() > 0;
        }

        private void OnGuard(InputValue value)
        {
            if(!character.IsAlive) { return; }
            
            var val = value.Get<float>();
            if(val > 0) character.OnGuard();
            else character.ChangeState(PlayerStates.Idle);
        }

        private void OnInteract(InputValue value)
        {
            if(!character.IsAlive) { character.IsInteracting = false; return; }

            var val = value.Get<float>();
            character.IsInteracting = val > 0;
        }
        
        private bool IsPointerOverUI()
        {
            var eventData = new PointerEventData(EventSystem.current)
            {
                position = Mouse.current.position.ReadValue()
            };
            var results = new List<RaycastResult>();
            
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }
        
        #endregion
    }
}