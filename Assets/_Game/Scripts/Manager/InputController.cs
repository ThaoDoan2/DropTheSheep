using Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Manager
{
    public class InputController: Singleton<InputController>
    {
        Camera _camera;
        private Hole _selectedHole;

        private Vector2Int _clickOffset;
        private Vector2Int _lastValidPivot;

        private bool _enableTouch;

        Board _board;

        bool Enable
        {
            get => _enableTouch;
            set
            {
                _enableTouch = value;
                if (!value)
                {
                    _selectedHole = null;
                }
            }
        }

        void Start()
        {
            _camera = Camera.main;
            Enable = true;
        }

        void Update()
        {
            if (!Enable) return;

            // Use the New Input System's Pointer to handle both Mouse and Touch primary input
            var pointer = Pointer.current;
            if (pointer == null) return;

            Vector2 screenPos = pointer.position.ReadValue();

            if (pointer.press.wasPressedThisFrame)
            {
                HandleTouchBegan(screenPos);
            }
            else if (pointer.press.wasReleasedThisFrame)
            {
                HandleTouchEnded(screenPos);
            }
            else if (pointer.press.isPressed)
            {
                HandleTouchMoved(screenPos);
            }
        }

        private void HandleTouchBegan(Vector3 screenPos)
        {
            Log($"HandleTouchBegan {screenPos}");

            var pos = _camera.ScreenToWorldPoint(screenPos);
            pos.z = 0;

            var hit = Physics2D.Raycast(pos, Vector2.zero);

            if (hit.collider != null && hit.collider.TryGetComponent<Board>(out var board))
            {
                Debug.Log($"HandleTouchBegan Touch Board");
                board.OnTouchBegan(pos);
                _board = board;
            }
        }

        private void HandleTouchMoved(Vector3 screenPos)
        {
            var pos = _camera.ScreenToWorldPoint(screenPos);
            pos.z = 0;

            _board?.OnTouchMove(pos);
        }

        private void HandleTouchEnded(Vector3 screenPos)
        {
            var pos = _camera.ScreenToWorldPoint(screenPos);
            pos.z = 0;

            _board?.OnTouchEnd(pos);
            _board = null;
        }

        static void Log(string msg)
        {
            Debug.Log($"InputController - {msg}");
        }
    }
}
