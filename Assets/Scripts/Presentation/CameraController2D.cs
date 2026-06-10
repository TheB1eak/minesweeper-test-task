using Minesweeper.Presentation.Config;
using UnityEngine;

namespace Minesweeper.Presentation
{
    [RequireComponent(typeof(Camera))]
    public sealed class CameraController2D : MonoBehaviour
    {
        [SerializeField] private CameraSettings _settings;

        private Camera _camera;
        private Vector3 _targetPosition;
        private Vector3 _currentVelocity;
        private float _targetZoom;
        private float _zoomVelocity;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _targetPosition = transform.position;
            _targetZoom = _camera.orthographicSize;
        }

        private void Update()
        {
            if (_settings == null)
                return;

            HandleMovementInput();
            HandleZoomInput();
            SmoothMove();
            SmoothZoom();
        }

        private void HandleMovementInput()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            Vector3 moveDirection = new Vector3(moveX, moveY, 0f).normalized;

            _targetPosition += moveDirection * _settings.MoveSpeed * Time.deltaTime;
            _targetPosition.x = Mathf.Clamp(_targetPosition.x, _settings.MinBounds.x, _settings.MaxBounds.x);
            _targetPosition.y = Mathf.Clamp(_targetPosition.y, _settings.MinBounds.y, _settings.MaxBounds.y);
        }

        private void HandleZoomInput()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) <= 0.01f)
                return;

            _targetZoom -= scroll * _settings.ZoomSpeed;
            _targetZoom = Mathf.Clamp(_targetZoom, _settings.MinZoom, _settings.MaxZoom);
        }

        private void SmoothMove()
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                _targetPosition,
                ref _currentVelocity,
                _settings.SmoothTime);
        }

        private void SmoothZoom()
        {
            _camera.orthographicSize = Mathf.SmoothDamp(
                _camera.orthographicSize,
                _targetZoom,
                ref _zoomVelocity,
                _settings.ZoomSmoothTime);
        }
    }
}
