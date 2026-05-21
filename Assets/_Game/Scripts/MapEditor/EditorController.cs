
using Gameplay;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

namespace MapEditor
{
    public class EditorController : MonoBehaviour
    {
        Camera _camera;

        private bool _enableTouch;

        OnBoardObject _selectedHole;
        BoardEditor _board;
        OnBoardObject _insertObject;
        OnBoardObject _touchingObj;
        long _touchTime;
        const int DRAG_THRESHOLD = 150; // milisecond


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

        private void Start()
        {
            Enable = true;
            _camera = Camera.main;
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
            _touchTime = DateTime.Now.Ticks;

            var pos = _camera.ScreenToWorldPoint(screenPos);
            pos.z = 0;

            var hit = Physics2D.Raycast(pos, Vector2.zero);
            if ((hit.collider != null && hit.collider.TryGetComponent<Hole>(out var hole)))
            {
                if (_board == null)
                {
                    _board = FindAnyObjectByType<BoardEditor>();
                }
                _board.ChangeStateNormal();
                _touchingObj = hole;
            }
            else if (hit.collider != null && hit.collider.TryGetComponent<Sheep>(out var sheep))
            {
                if (_board == null)
                {
                    _board = FindAnyObjectByType<BoardEditor>();
                }
                _touchingObj = sheep;
            }
            else if (hit.collider != null && hit.collider.TryGetComponent<BoardEditor>(out var board))
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

            long touchDuration = GetTouchMilisecond();
            var hit = Physics2D.Raycast(pos, Vector2.zero);
            if (hit.collider != null && hit.collider.TryGetComponent<Hole>(out var hole))
            {
                if (_board == null)
                {
                    _board = FindAnyObjectByType<BoardEditor>();
                }
                if (_touchingObj == hole && touchDuration > DRAG_THRESHOLD)
                {
                    _touchingObj = null;
                    _board.InsertObject(hole, pos);
                }
            }
            else if (hit.collider != null && hit.collider.TryGetComponent<Sheep>(out var sheep))
            {
                if (_board == null)
                {
                    _board = FindAnyObjectByType<BoardEditor>();
                }

                if (_touchingObj == sheep && touchDuration > DRAG_THRESHOLD)
                {
                    _touchingObj = null;
                    _board.InsertObject(sheep, pos);
                }
            }

            if (touchDuration > DRAG_THRESHOLD)
            {
                _board?.OnTouchMove(pos);
            }

        }

        private void HandleTouchEnded(Vector3 screenPos)
        {
            var pos = _camera.ScreenToWorldPoint(screenPos);
            pos.z = 0;

            long miliseconds = GetTouchMilisecond();
            if (miliseconds <= DRAG_THRESHOLD)
            {
                OnClick(pos);
            }
            else
            {
                _board?.OnTouchEnd(pos);
            }

            _board = null;
            _touchingObj = null;
        }

        private long GetTouchMilisecond()
        {
            long touchDuration = DateTime.Now.Ticks - _touchTime;
            long miliseconds = touchDuration / TimeSpan.TicksPerMillisecond;
            return miliseconds;
        }

        private void OnClick(Vector3 pos)
        {
            var hit = Physics2D.Raycast(pos, Vector2.zero);
            if (hit.collider != null && hit.collider.TryGetComponent<Sheep>(out var sheep))
            {
                if (_board == null)
                {
                    _board = FindAnyObjectByType<BoardEditor>();
                }
                _board.OnClickSheep();
            }
            else if (hit.collider != null && hit.collider.TryGetComponent<BoardEditor>(out var board))
            {
                Debug.Log($"OnClick Board");
                board.OnClick(pos);
                _board = board;
            }
            else if (hit.collider != null)
            {
                if (_board == null)
                {
                    _board = FindAnyObjectByType<BoardEditor>();
                }
                if (hit.collider.gameObject.CompareTag("RemoveObject"))
                {
                    _board.ChangeStateRemoveObject();
                }
                else if (hit.collider.gameObject.CompareTag("RemoveCell"))
                {
                    _board.ChangeStateRemoveCell();
                }
                else if (hit.collider.gameObject.CompareTag("InsertCell"))
                {
                    _board.ChangeStateAddCell();
                }

            }
        }

        static void Log(string msg)
        {
            Debug.Log($"InputController - {msg}");
        }
    }
}
