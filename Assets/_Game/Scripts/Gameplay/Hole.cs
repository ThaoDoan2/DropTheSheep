

using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gameplay
{
    public class Hole: OnBoardObject, IHole
    {
        [SerializeField] TextMeshPro _remainText;

        [SerializeField] SheepColor _color;
        [SerializeField] Vector2Int[] shape;
        [SerializeField] Vector2Int _pivot;

        Vector2Int _touchCell = Vector2Int.zero;

        int remain;

        public Vector2Int Pivot => _pivot;

        public Vector2Int CellPos => _pivot + _touchCell;

        public SheepColor Color => _color;  

        public bool IsAtCell(Vector2Int pos)
        {
            for (int i = 0; i < shape.Length; i++)
            {
                if (shape[i] + _pivot == pos)
                    return true;

            }
            return false;
        }

        public void SetCell(Vector2Int pos)
        {
            _pivot = pos - _touchCell;
        }

        public void Select(Vector2Int pos)
        {
            for (int i = 0; i < shape.Length; i++)
            {
                if (shape[i] + _pivot == pos)
                {
                    _touchCell = shape[i];
                    break;
                }
            }
        }

        public List<Vector2Int> GetShapeCells()
        {
            List<Vector2Int> cells = new List<Vector2Int>();
            for (int i = 0; i < shape.Length; i++)
            {
                cells.Add(shape[i] + _pivot);
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
    }
}
