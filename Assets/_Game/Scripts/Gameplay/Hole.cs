

using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gameplay
{
    public class Hole: OnBoardObject, IHole
    {
        [SerializeField] SpriteRenderer[] _renderers;
        [SerializeField] TextMeshPro _remainText;

        [SerializeField] SheepColor _color;
        [SerializeField] Vector2Int[] _shape;

        Vector2Int _touchCell = Vector2Int.zero;

        int remain;

        public Vector2Int Pivot => _cellPos;

        public Vector2Int CellPos => _cellPos + _touchCell;

        public SheepColor Color => _color;

        private void Start()
        {
            remain = _shape.Length;
            _remainText.text = remain.ToString();
        }

        public bool IsAtCell(Vector2Int pos)
        {
            for (int i = 0; i < _shape.Length; i++)
            {
                if (_shape[i] + _cellPos == pos)
                    return true;

            }
            return false;
        }

        public void SetCell(Vector2Int pos)
        {
            _cellPos = pos - _touchCell;
        }

        public void Select(Vector2Int pos)
        {
            for (int i = 0; i < _shape.Length; i++)
            {
                if (_shape[i] + _cellPos == pos)
                {
                    _touchCell = _shape[i];
                    break;
                }
            }
        }

        public List<Vector2Int> GetShapeCells()
        {
            List<Vector2Int> cells = new List<Vector2Int>();
            for (int i = 0; i < _shape.Length; i++)
            {
                cells.Add(_shape[i] + _cellPos);
            }
            return cells;
        }

        public void OnSheepEnterHole()
        {
            remain--;

            _remainText.text = remain.ToString();
            if (remain == 0)
                OnFullHole();
        }

        public void OnFullHole()
        {
            gameObject.SetActive(false);
        }

        public bool IsFull()
        {
            return remain == 0;
        }

        public bool IsActive()
        {
            return remain > 0;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            remain = _shape.Length;
            if (_remainText != null)
                _remainText.text = remain.ToString();

            Color c = GameplaySO.instance.GetColor(_color);
            foreach (var renderer in _renderers)
            {
                renderer.color = c;
            }
        }
#endif
    }
}
