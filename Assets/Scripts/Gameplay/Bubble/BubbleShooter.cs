using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Assets.Scripts.Gameplay.Bubble
{
    [RequireComponent(typeof(PlayerInput))]
    public class BubbleShooter : MonoBehaviour
    {
        [SerializeField] private GameObject _bubblePrefab;
        [SerializeField] private float _shootPower = 15f;
        [SerializeField] private AimingLaser _aimingLaser;

        private BubbleShooterControls _controls;
        private GameObject _currentBubble;
        private Camera _mainCam;
        private Vector2 _aimDirection;

        private void Awake()
        {
            _mainCam = Camera.main;
            EnhancedTouchSupport.Enable();
            _controls = new BubbleShooterControls();
        }

        private void Start()
        {
            SpawnBubble();
        }

        private void OnEnable()
        {
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += OnFingerDown;
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerMove += OnFingerMove;
            _controls.Gameplay.Aim.performed += OnFingerMove;
            _controls.Gameplay.Shoot.performed += OnFingerDown;
            _controls.Gameplay.Enable();
        }

        private void OnDisable()
        {
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= OnFingerDown;
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerMove -= OnFingerMove;
            _controls.Gameplay.Aim.performed -= OnFingerMove;
            _controls.Gameplay.Shoot.performed -= OnFingerDown;
            _controls.Gameplay.Disable();
        }

        private void OnFingerDown(Finger finger)
        {
            if (_currentBubble == null) return;

            Vector2 touchPos = finger.screenPosition;
            ShootBubble(touchPos);
        }
        private void OnFingerDown(InputAction.CallbackContext context)
        {
            if (_currentBubble == null) return;

            Vector3 worldPos = _mainCam.ScreenToWorldPoint(_aimDirection);
            Vector2 direction = (worldPos - transform.position).normalized;

            _currentBubble.GetComponent<Rigidbody>().isKinematic = false;
            _currentBubble.GetComponent<Rigidbody>().linearVelocity = direction * _shootPower;
            _currentBubble = null;
            StartCoroutine(SpawnNewBubble());
        }
        private void OnFingerMove(Finger finger)
        {
            Vector2 screenPos = finger.screenPosition;
            Vector3 screenPoint = new Vector3(screenPos.x, screenPos.y, Mathf.Abs(_mainCam.transform.position.z - transform.position.z));
            Vector3 worldPos = _mainCam.ScreenToWorldPoint(screenPoint);
            Vector3 direction = (worldPos - transform.position);
            direction.z = 0;
            direction.Normalize();
            _aimingLaser.DrawTrajectory(transform.position, direction);
        }
        private void OnFingerMove(InputAction.CallbackContext context)
        {
            _aimDirection = context.ReadValue<Vector2>();
            var worldPoint = _mainCam.ScreenPointToRay(_aimDirection);
            Vector3 direction = (worldPoint.origin + worldPoint.direction * 100f) - transform.position;
            direction.z = 0; 
            direction.Normalize();
            _aimingLaser.DrawTrajectory(transform.position, direction);
        }

        private void ShootBubble(Vector2 screenPos)
        {
            Vector3 worldPos = _mainCam.ScreenToWorldPoint(screenPos);
            _aimDirection = (worldPos - transform.position).normalized;

            var rb = _currentBubble.GetComponent<Rigidbody>();
            _currentBubble.GetComponent<Rigidbody>().isKinematic = false;
            _currentBubble.GetComponent<Rigidbody>().linearVelocity = _aimDirection * _shootPower;
            _currentBubble = null;
            Invoke(nameof(SpawnBubble), 0.5f); 
        }

        private void SpawnBubble()
        {
            _currentBubble = Instantiate(_bubblePrefab, transform.position, Quaternion.identity);
            var bubble = _currentBubble.GetComponent<Bubble>();
            bubble.SetColor();
            _currentBubble.GetComponent<Rigidbody>().isKinematic = true;
        }
        private IEnumerator SpawnNewBubble()
        {
            yield return new WaitForSeconds(0.5f);
            SpawnBubble();
        }
    }

}