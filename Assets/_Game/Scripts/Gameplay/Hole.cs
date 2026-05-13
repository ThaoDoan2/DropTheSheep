using Qutility.CustomEditor;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gameplay
{
    public class Hole : OnBoardObject, IHole
    {
        [SerializeField] SpriteRenderer[] _renderers;
        [SerializeField] TextMeshPro _remainText;

        [SerializeField] SheepColor _color;
        [SerializeField] HoleShape _shapeType;
        [SerializeField] Vector2Int[] _shape;

        Vector2Int _touchCell = Vector2Int.zero;

        int remain;

        public Vector2Int Pivot => _cellPos;

        public Vector2Int CellPos => _cellPos + _touchCell;

        public SheepColor Color => _color;

        public HoleShape ShapeType => _shapeType;

        public Vector2Int[] Shape => _shape;

        public void Init(Vector2Int pos, Board board, Vector2Int[] shape, SheepColor color)
        {
            base.Init(pos, board);
            _shape = shape;
            _color = color;
        }

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

        public void LoadData()
        {
            LoadPositionFromCellPos();

            remain = _shape.Length;
            if (_remainText != null)
                _remainText.text = remain.ToString();

            Color c = GameplaySO.instance.GetColor(_color);
            foreach (var renderer in _renderers)
            {
                renderer.color = c;
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            LoadData();
        }
#endif

        public static int GetHoleTypeFromShape(Vector2Int[] cells)
        {
            if (cells.Length == 1)
                return 0;
            if (cells.Length == 2)
            {
                if (cells[1].x == 0)
                    return 1;
                else
                    return 2;
            }

            if (cells.Length == 3)
            {
                if (cells[0] == Vector2Int.zero && cells[1] == Vector2Int.right && cells[2].x == 2)
                    return 3;
                if (cells[0] == Vector2Int.zero && cells[1] == Vector2Int.up && cells[2].y == 2)
                    return 4;
                if (cells[0] == Vector2Int.zero && cells[1] == Vector2Int.right && cells[2] == Vector2Int.up)
                    return 5;
                if (cells[0] == Vector2Int.zero && cells[1] == Vector2Int.right && cells[2] == Vector2Int.one)
                    return 6;
                if (cells[0] == Vector2Int.up && cells[1] == Vector2Int.one && cells[2] == Vector2Int.right)
                    return 7;
                if (cells[0] == Vector2Int.zero && cells[1] == Vector2Int.up && cells[2] == Vector2Int.one)
                    return 8;
            }

            return 0;
        }

        [ButtonMethod]
        public void LoadShapeFromType()
        {
            switch (_shapeType)
            {
                case 0:
                    _shape = new Vector2Int[] { Vector2Int.zero };
                    break;

                case HoleShape.Horizontal2:
                    _shape = new Vector2Int[] { Vector2Int.zero, Vector2Int.right };
                    break;
                case HoleShape.Vertical2:
                    _shape = new Vector2Int[] { Vector2Int.zero, Vector2Int.up };
                    break;

                case HoleShape.Horizontal3:
                    _shape = new Vector2Int[] { Vector2Int.zero, Vector2Int.right, Vector2Int.right * 2 };
                    break;
                case HoleShape.Vertical3:
                    _shape = new Vector2Int[] { Vector2Int.zero, Vector2Int.up, Vector2Int.up * 2 };
                    break;
                case HoleShape.Corner31:
                    _shape = new Vector2Int[] { Vector2Int.zero, Vector2Int.right, Vector2Int.up };
                    break;
                case HoleShape.Corner32:
                    _shape = new Vector2Int[] { Vector2Int.zero, Vector2Int.right, Vector2Int.one };
                    break;
                case HoleShape.Corner33:
                    _shape = new Vector2Int[] { Vector2Int.up, Vector2Int.one, Vector2Int.right };
                    break;
                case HoleShape.Corner34:
                    _shape = new Vector2Int[] { Vector2Int.zero, Vector2Int.up, Vector2Int.one };
                    break;

                case HoleShape.Square4:
                    _shape = new Vector2Int[] { Vector2Int.zero, Vector2Int.right, Vector2Int.up, Vector2Int.one };
                    break;
                case HoleShape.Horizontal4:
                    _shape = new Vector2Int[] { Vector2Int.zero, Vector2Int.right, Vector2Int.right * 2, Vector2Int.right * 3 };
                    break;
                case HoleShape.Vertical4:
                    _shape = new Vector2Int[] { Vector2Int.zero, Vector2Int.up, Vector2Int.up * 2, Vector2Int.up * 3 };
                    break;

                case HoleShape.Corner41:
                    _shape = new Vector2Int[] { Vector2Int.zero, Vector2Int.right, Vector2Int.up, Vector2Int.up * 2 };
                    break;
                case HoleShape.Corner42:
                    _shape = new Vector2Int[] { Vector2Int.zero, Vector2Int.right, Vector2Int.one, new(1, 2) };
                    break;
                case HoleShape.Corner43:
                    _shape = new Vector2Int[] { new(0, 2), new(1, 2), Vector2Int.one, new(1, 0) };
                    break;
                case HoleShape.Corner44:
                    _shape = new Vector2Int[] { Vector2Int.zero, Vector2Int.up, new(0, 2), new(1, 2) };
                    break;
                case HoleShape.Corner45:
                    _shape = new Vector2Int[] { Vector2Int.zero, Vector2Int.right, new(2, 0), Vector2Int.up };
                    break;
                case HoleShape.Corner46:
                    _shape = new Vector2Int[] { Vector2Int.zero, Vector2Int.right, new(2, 0), new(2, 1) };
                    break;
                case HoleShape.Corner47:
                    _shape = new Vector2Int[] { Vector2Int.up, new(1, 1), new(2, 1), new(2, 0) };
                    break;
                case HoleShape.Corner48:
                    _shape = new Vector2Int[] { Vector2Int.zero, Vector2Int.up, new(1, 1), new(2, 1) };
                    break;
            }
        }
    }
}
