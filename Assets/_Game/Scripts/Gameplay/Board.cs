using Qutility.CustomEditor;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class Board : MonoBehaviour
    {
        [Header("Board")]
        public int cols = 7;
        public int rows = 9;
        public float cellSize = 1f;

        Cell[,] _cells;
        [SerializeField] List<Hole> _holes = new List<Hole>();
        [SerializeField] List<Sheep> _sheeps = new List<Sheep>();

        [SerializeField] GameObject _cellPrefab;

        Transform _boardRoot;

        Hole _selectedHole;

        private void Start()
        {
            BuildGrid();
            LoadBoard();
        }

        private void BuildGrid()
        {
            _cells = new Cell[cols, rows];

            _boardRoot = new GameObject("Board").transform;
            _boardRoot.SetParent(transform);
            _boardRoot.localPosition = Vector3.zero;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    var go = Instantiate(_cellPrefab);
                    go.name = $"Cell_{c}_{r}";
                    go.transform.localPosition = GridToWorld(c, r);
                    var cell = go.GetComponent<Cell>();
                    cell.Init(c, r);
                    cell.transform.SetParent(_boardRoot);
                    _cells[c, r] = cell;
                }
            }
        }

        private void LoadBoard()
        {
            for (int i = 0; i < _holes.Count; i++)
            {
                Hole hole = _holes[i];
                hole.transform.localPosition = GridToWorld(hole.Pivot.x, hole.Pivot.y);

                List<Vector2Int> shape = hole.GetShapeCells();
                for (int j = 0; j < shape.Count; j++)
                {
                    Cell cell = _cells[shape[j].x, shape[j].y];
                    cell.Type = CellType.Hole;
                }
            }

            for (int i = 0; i < _sheeps.Count; i++)
            {
                Vector2Int cellPos = _sheeps[i].CellPos;
                Cell cell = _cells[cellPos.x, cellPos.y];
                cell.Type = CellType.Sheep;
            }
        }

        public Vector3 GridToWorld(int c, int r)
        {
            float ox = -(cols - 1) * cellSize * 0.5f;
            float oy = -(rows - 1) * cellSize * 0.5f;
            return new Vector3(ox + c * cellSize, oy + r * cellSize, 0f);
        }

        Vector2Int FromLocalToGrid(Vector3 localPos)
        {
            float ox = -(cols - 1) * cellSize * 0.5f;
            float oy = -(rows - 1) * cellSize * 0.5f;

            int c = Mathf.RoundToInt((localPos.x - ox) / cellSize);
            int r = Mathf.RoundToInt((localPos.y - oy) / cellSize);

            return new Vector2Int(c, r);
        }

        public void OnTouchBegan(Vector3 pos)
        {
            Log($"OnTouchBegan {pos}");

            Vector3 localPos = transform.InverseTransformPoint(pos);
            Vector2Int cellPos = FromLocalToGrid(localPos);

            Log($"OnTouchBegan {cellPos}");
            Hole hole = GetHoleAtCell(cellPos);
            if (hole != null)
            {
                _selectedHole = hole;
                _selectedHole.Select(cellPos);
                Log($"Selected hole at cell {cellPos}");
            }
        }

        public void OnTouchMove(Vector3 pos)
        {
            if (_selectedHole == null) return;

            Vector3 localPos = transform.InverseTransformPoint(pos);
            Vector2Int cellPos = FromLocalToGrid(localPos);

            Log($"OnTouchMove cellPos {cellPos}");
            MoveHoleToCell(_selectedHole, cellPos);
        }

        public void MoveHoleToCell(Hole hole, Vector2Int cellPos)
        {
            if (cellPos.x < 0 || cellPos.x >= cols || cellPos.y < 0 || cellPos.y >= rows)
                return;

            if (cellPos != _selectedHole.CellPos)
            {
                var direction = cellPos - _selectedHole.CellPos;
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    direction.y = 0;
                    direction.x = direction.x > 0 ? 1 : -1;
                }
                else
                {
                    direction.x = 0;
                    direction.y = direction.y > 0 ? 1 : -1;
                }
                cellPos = _selectedHole.CellPos + direction;

                if (CheckMoveValid(hole, cellPos) == false)
                    return;

                _selectedHole.SetCell(cellPos);
                _selectedHole.transform.localPosition = GridToWorld(hole.Pivot.x, hole.Pivot.y);
                CheckSheepJumpToHole(_selectedHole);

                bool isHoleFull = _selectedHole.IsFull();
                if (isHoleFull)
                {
                    _selectedHole.OnFullHole();
                    OnHoleFull(_selectedHole);
                    EndMovement();

                }
                else
                {
                    _selectedHole.GetShapeCells().ForEach(pos => _cells[pos.x, pos.y].Type = CellType.Hole);
                }
            }
        }

        private void EndMovement()
        {
            _selectedHole = null;
        }

        public void CheckSheepJumpToHole(Hole hole)
        {
            hole.GetShapeCells().ForEach(pos =>
            {
                Cell cell = _cells[pos.x, pos.y];
                if (cell.Type == CellType.Sheep)
                {
                    var sheep = GetSheepAtCell(pos);
                    if (sheep != null && sheep.Color == hole.Color
                    && !hole.IsFull())
                    {
                        sheep.JumpToHole(hole);
                        cell.Type = CellType.Hole;
                    }
                }
            });
        }

        public void OnHoleFull(Hole hole)
        {
            hole.GetShapeCells().ForEach(pos =>
            {
                Cell cell = _cells[pos.x, pos.y];
                if (cell.Type == CellType.Hole)
                {
                    cell.Type = CellType.Empty;
                }
            });
        }

        private bool CheckMoveValid(Hole hole, Vector2Int cellPos)
        {
            Vector2Int direction = cellPos - _selectedHole.CellPos;
            // check direction
            List<Vector2Int> newShape = _selectedHole.GetShapeCells();
            for (int i = 0; i < newShape.Count; i++)
            {
                newShape[i] += direction;
            }

            for (int i = 0; i < newShape.Count; i++)
            {
                if (newShape[i].x < 0 || newShape[i].x >= cols || newShape[i].y < 0 || newShape[i].y >= rows)
                    return false;
                Cell cell = _cells[newShape[i].x, newShape[i].y];
                if (cell.Type == CellType.Block)
                    return false;
                if (cell.Type == CellType.Hole)
                {
                    var conflictHole = GetHoleAtCell(cell.X, cell.Y);
                    if (conflictHole != null && conflictHole != hole)
                        return false;
                }

                if (cell.Type == CellType.Sheep)
                {
                    var conflictSheep = GetSheepAtCell(cell.X, cell.Y);
                    if (conflictSheep != null && conflictSheep.Color != hole.Color)
                        return false;
                }
            }
            return true;
        }

        public void OnTouchEnd(Vector3 pos)
        {
            if (_selectedHole == null)
                return;

            Vector3 localPos = transform.InverseTransformPoint(pos);
            Vector2Int cellPos = FromLocalToGrid(localPos);

            MoveHoleToCell(_selectedHole, cellPos);
            _selectedHole = null;
        }

        Hole GetHoleAtCell(int c, int r)
        {
            return GetHoleAtCell(new Vector2Int(c, r));
        }

        Hole GetHoleAtCell(Vector2Int pos)
        {
            for (int i = 0; i < _holes.Count; i++)
            {
                if (_holes[i].IsAtCell(pos) && _holes[i].IsActive())
                {
                    return _holes[i];
                }
            }
            return null;
        }

        Sheep GetSheepAtCell(Vector2Int pos)
        {
            for (int i = 0; i < _sheeps.Count; i++)
            {
                if (_sheeps[i].IsAtCell(pos))
                {
                    return _sheeps[i];
                }
            }
            return null;
        }

        Sheep GetSheepAtCell(int c, int r)
        {
            return GetSheepAtCell(new Vector2Int(c, r));
        }

        [ButtonMethod]
        public void LoadBoardState()
        {
            Sheep[] sheeps = GetComponentsInChildren<Sheep>();
            _sheeps.Clear();
            _sheeps.AddRange(sheeps);

            Hole[] holes = GetComponentsInChildren<Hole>();
            _holes.Clear();
            _holes.AddRange(holes);
        }

        private static void Log(string msg)
        {
            Debug.Log($"[Board] - {msg}");
        }
    }
}
