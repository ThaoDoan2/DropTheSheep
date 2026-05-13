using Gameplay;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    public class BoardEditor: Board
    {

        OnBoardObject _selectedHole;

        Transform _boardRoot;

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

                    bool left = c > 0;
                    bool right = c < cols - 1;
                    bool top = r < rows - 1;
                    bool bottom = r > 0;
                    cell.UpdateCell(left, top, right, bottom);
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

        Vector2Int FromLocalToGrid(Vector3 localPos)
        {
            float ox = -(cols - 1) * cellSize * 0.5f;
            float oy = -(rows - 1) * cellSize * 0.5f;

            int c = Mathf.RoundToInt((localPos.x - ox) / cellSize);
            int r = Mathf.RoundToInt((localPos.y - oy) / cellSize);

            return new Vector2Int(c, r);
        }

        public Vector3 GridToWorld(int c, int r)
        {
            float ox = -(cols - 1) * cellSize * 0.5f;
            float oy = -(rows - 1) * cellSize * 0.5f;
            return new Vector3(ox + c * cellSize, oy + r * cellSize, 0f);
        }

        OnBoardObject GetObjectAtCell(Vector2Int pos)
        {
            for (int i = 0; i < _holes.Count; i++)
            {
                if (_holes[i].IsAtCell(pos) && _holes[i].IsActive())
                {
                    return _holes[i];
                }
            }

            for (int i = 0; i < _sheeps.Count; i++)
            {
                if (_sheeps[i].IsAtCell(pos))
                {
                    return _sheeps[i];
                }
            }
            return null;
        }

        private bool CheckMoveValid(OnBoardObject hole, Vector2Int cellPos)
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
                if (cell.Type == CellType.Hole || cell.Type == CellType.Sheep)
                {
                    var conflictHole = GetObjectAtCell(new Vector2Int( cell.X, cell.Y));
                    if (conflictHole != null && conflictHole != hole)
                        return false;
                }
            }
            return true;
        }

        public void RemoveSelectedObject(OnBoardObject obj)
        {
            if (obj is Hole)
            {
                _holes.Remove(obj as Hole);
            }
        }

        public void OnTouchBegan(Vector3 pos)
        {
            Log($"OnTouchBegan {pos}");

            Vector3 localPos = transform.InverseTransformPoint(pos);
            Vector2Int cellPos = FromLocalToGrid(localPos);

            Log($"OnTouchBegan {cellPos}");
            OnBoardObject hole = GetObjectAtCell(cellPos);
            if (hole != null)
            {
                _selectedHole = hole;
                _selectedHole.Select(cellPos);
                Log($"Selected hole at cell {cellPos}");

                RemoveSelectedObject(hole);
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

        public void OnTouchEnd(Vector3 pos)
        {
            if (_selectedHole == null)
                return;

            Vector3 localPos = transform.InverseTransformPoint(pos);
            Vector2Int cellPos = FromLocalToGrid(localPos);

            MoveAndAddHoleToCell(_selectedHole, cellPos);
            _selectedHole = null;
        }

        public void MoveHoleToCell(OnBoardObject hole, Vector2Int cellPos)
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

                _selectedHole.SetCell(cellPos);
                _selectedHole.transform.localPosition = GridToWorld(hole.Pivot.x, hole.Pivot.y);
            }
        }

        public void MoveAndAddHoleToCell(OnBoardObject hole, Vector2Int cellPos)
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
            }
            if (hole is Hole)
                _holes.Add((Hole)hole);
        }

        private void EndMovement()
        {
            _selectedHole = null;
        }

        public void InsertObject(Hole h, Vector3 pos)
        {
            HoleShape shape = h.ShapeType;

            GameObject go = UnityEditor.PrefabUtility.InstantiatePrefab(_holePrefabs[(int)shape]) as GameObject;
            go.transform.SetParent(transform);
            Hole hole = go.GetComponent<Hole>();
            Vector3 localPos = transform.InverseTransformPoint(pos);
            Vector2Int cellPos = FromLocalToGrid(localPos);

            hole.Init(cellPos, this, shape, SheepColor.White);
            hole.LoadData();
            _selectedHole = hole;
        }

        private static void Log(string msg)
        {
            Debug.Log($"BoardEditor - {msg}");
        }
    }
}
