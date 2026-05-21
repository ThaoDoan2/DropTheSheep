using Gameplay;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    public class BoardEditor : Board
    {
        OnBoardObject _selectedObject;
        OnBoardObject _draggingObject;
        SheepColor _currentColor = SheepColor.White;
        Transform _boardRoot;

        MapEditorMenu _menu;

        [SerializeField] MapEditorState _state;

        bool _removeDraggingObject = false;

        public SheepColor CurrentColor { get => _currentColor; set => _currentColor = value; }

        private void Start()
        {
            BuildGrid();
            LoadBoard();

            _menu = FindAnyObjectByType<MapEditorMenu>();
        }

        private void BuildGrid()
        {
            if (_boardRoot != null)
            {
                for (int i = 0; i < _boardRoot.childCount; i++)
                {
                    Destroy(_boardRoot.GetChild(i).gameObject);
                }
            }
            else
            {
                _boardRoot = new GameObject("Board").transform;
                _boardRoot.SetParent(transform);
                _boardRoot.localPosition = Vector3.zero;
            }

            _cells = new Cell[cols, rows];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    var go = Instantiate(_cellPrefab);
                    go.name = $"Cell_{c}_{r}";
                    var cell = go.GetComponent<Cell>();
                    cell.Init(c, r);
                    go.transform.SetParent(_boardRoot);
                    go.transform.localPosition = GridToWorld(c, r);
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
            Vector2Int direction = cellPos - _selectedObject.CellPos;
            // check direction
            List<Vector2Int> newShape = _selectedObject.GetShapeCells();
            for (int i = 0; i < newShape.Count; i++)
            {
                newShape[i] += direction;
            }

            for (int i = 0; i < newShape.Count; i++)
            {
                if (newShape[i].x < 0 || newShape[i].x >= cols || newShape[i].y < 0 || newShape[i].y >= rows)
                    return false;
                Cell cell = _cells[newShape[i].x, newShape[i].y];
                if (cell.Type == CellType.Block || cell.Type == CellType.Void)
                    return false;
                if (cell.Type == CellType.Hole || cell.Type == CellType.Sheep)
                {
                    var conflictHole = GetObjectAtCell(new Vector2Int(cell.X, cell.Y));
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
            if (obj is Sheep)
            {
                _sheeps.Remove(obj as Sheep);
            }
        }

        public void OnTouchBegan(Vector3 pos)
        {
            Log($"OnTouchBegan {pos}");

            Vector3 localPos = transform.InverseTransformPoint(pos);
            Vector2Int cellPos = FromLocalToGrid(localPos);

            Log($"OnTouchBegan {cellPos}");
            OnBoardObject obj = GetObjectAtCell(cellPos);
            if (obj != null)
            {
                _draggingObject = obj;
                _selectedObject = obj;
                _draggingObject.Select(cellPos);
                Log($"Selected hole at cell {cellPos}");
            }
        }

        public void OnTouchMove(Vector3 pos)
        {
            if (_draggingObject == null)
                return;

            if (!_removeDraggingObject)
            {
                _removeDraggingObject = true;
                RemoveSelectedObject(_draggingObject);
            }

            Vector3 localPos = transform.InverseTransformPoint(pos);
            Vector2Int cellPos = FromLocalToGrid(localPos);

            Log($"OnTouchMove cellPos {cellPos}");
            MoveHoleToCell(_draggingObject, cellPos);
        }

        public void OnTouchEnd(Vector3 pos)
        {
            if (_draggingObject == null)
                return;

            Vector3 localPos = transform.InverseTransformPoint(pos);
            Vector2Int cellPos = FromLocalToGrid(localPos);

            MoveAndAddHoleToCell(_draggingObject, cellPos);
            _draggingObject = null;
            _removeDraggingObject = false;
        }

        public void OnClick(Vector3 pos)
        {
            Vector3 localPos = transform.InverseTransformPoint(pos);
            Vector2Int cellPos = FromLocalToGrid(localPos);

            if (_state == MapEditorState.InsertSheep)
            {
                InsertSheep(cellPos);
                return;
            }
            else if (_state == MapEditorState.RemoveSheep)
            {
                var obj = GetObjectAtCell(cellPos);
                if (obj != null)
                {
                    RemoveSelectedObject(obj);
                    Destroy(obj.gameObject);
                }
                return;
            }
            else if (_state == MapEditorState.RemoveCell)
            {
                if (cellPos.x < 0 || cellPos.x >= cols || cellPos.y < 0 || cellPos.y >= rows)
                    return;
                OnRemoveCell(cellPos);
            }
            else if (_state == MapEditorState.InsertCell && _cells[cellPos.x, cellPos.y].Type == CellType.Void)
            {
                if (cellPos.x < 0 || cellPos.x >= cols || cellPos.y < 0 || cellPos.y >= rows)
                    return;
                _cells[cellPos.x, cellPos.y].Type = CellType.Empty;
            }

            OnBoardObject onBoardObject = GetObjectAtCell(cellPos);
            if (_menu != null)
            {
                if (onBoardObject is Hole)
                    _menu.SelectHole(onBoardObject as Hole);
                else if (onBoardObject is Sheep)
                    _menu.SelectSheep(onBoardObject as Sheep);
            }
        }

        void OnRemoveCell(Vector2Int cellPos)
        {
            Cell cell = _cells[cellPos.x, cellPos.y];
            cell.Type = CellType.Void;
            cell.OnRemoved();

            if (cellPos.x > 0)
                RenderCell(new Vector2Int(cellPos.x - 1, cellPos.y));
            if (cellPos.x < cols - 1)
                RenderCell(new Vector2Int(cellPos.x + 1, cellPos.y));
            if (cellPos.y > 0)
                RenderCell(new Vector2Int(cellPos.x, cellPos.y - 1));
            if (cellPos.y < rows - 1)
                RenderCell(new Vector2Int(cellPos.x, cellPos.y + 1));
        }

        public void MoveHoleToCell(OnBoardObject hole, Vector2Int cellPos)
        {
            if (cellPos.x < 0 || cellPos.x >= cols || cellPos.y < 0 || cellPos.y >= rows)
                return;

            if (cellPos != _selectedObject.CellPos)
            {
                var direction = cellPos - _selectedObject.CellPos;
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
                cellPos = _selectedObject.CellPos + direction;

                _selectedObject.SetCell(cellPos);
                _selectedObject.transform.localPosition = GridToWorld(hole.Pivot.x, hole.Pivot.y);
            }
        }

        public void MoveAndAddHoleToCell(OnBoardObject obj, Vector2Int cellPos)
        {
            if (cellPos != _selectedObject.CellPos)
            {
                var direction = cellPos - _selectedObject.CellPos;
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
                cellPos = _selectedObject.CellPos + direction;

                if (CheckMoveValid(obj, cellPos) == false)
                    return;

                _selectedObject.SetCell(cellPos);
                _selectedObject.transform.localPosition = GridToWorld(obj.Pivot.x, obj.Pivot.y);
            }
            if (obj is Hole)
                _holes.Add((Hole)obj);
            else if (obj is Sheep)
                _sheeps.Add((Sheep)obj);
        }

        private void EndMovement()
        {
            _selectedObject = null;
        }

        public void InsertObject(Sheep s, Vector3 pos)
        {
            Debug.Log($"InsertObject sheep {pos}");
            GameObject go = UnityEditor.PrefabUtility.InstantiatePrefab(_sheepPrefab) as GameObject;
            go.transform.SetParent(transform);
            Sheep sheep = go.GetComponent<Sheep>();
            Vector3 localPos = transform.InverseTransformPoint(pos);
            Vector2Int cellPos = FromLocalToGrid(localPos);

            sheep.Init(cellPos, this, _currentColor);
            _selectedObject = sheep;
            _draggingObject = sheep;
        }

        public void InsertSheep(Vector2Int cellPos)
        {
            var obj = GetObjectAtCell(cellPos);
            if (obj != null)
                return;

            GameObject go = UnityEditor.PrefabUtility.InstantiatePrefab(_sheepPrefab) as GameObject;
            go.transform.SetParent(transform);
            Sheep sheep = go.GetComponent<Sheep>();
            sheep.Init(cellPos, this, _currentColor);
            sheep.LoadData();
            _sheeps.Add(sheep);

            _selectedObject = sheep;
        }

        public void InsertObject(Hole h, Vector3 pos)
        {
            HoleShape shape = h.ShapeType;

            GameObject go = UnityEditor.PrefabUtility.InstantiatePrefab(_holePrefabs[(int)shape]) as GameObject;
            go.transform.SetParent(transform);
            Hole hole = go.GetComponent<Hole>();
            Vector3 localPos = transform.InverseTransformPoint(pos);
            Vector2Int cellPos = FromLocalToGrid(localPos);

            hole.Init(cellPos, this, shape, _currentColor);
            hole.LoadData();
            _selectedObject = hole;
            _draggingObject = hole;
        }

        public Hole InsertObject(HoleShape shapeType, Vector2Int cellPos)
        {
            var shape = shapeType;
            GameObject go = UnityEditor.PrefabUtility.InstantiatePrefab(_holePrefabs[(int)shape]) as GameObject;
            go.transform.SetParent(transform);
            Hole hole = go.GetComponent<Hole>();

            hole.Init(cellPos, this, shape, _currentColor);
            hole.LoadData();
            _selectedObject = hole;

            return hole;
        }

        public void UpdateBoardSize(int col, int row)
        {
            cols = col;
            rows = row;

            BuildGrid();
        }

        private static void Log(string msg)
        {
            Debug.Log($"BoardEditor - {msg}");
        }

        public void NextHole(Hole h)
        {
            HoleShape shapeType = Hole.GetNextShape(h.ShapeType);

            if (shapeType == h.ShapeType)
                return;

            var newHole = ReplaceHole(h, shapeType);
            if (_menu != null)
            {
                _menu.SelectHole(newHole);
            }
        }

        public void PrevHole(Hole h)
        {
            HoleShape shapeType = Hole.GetPrevShape(h.ShapeType);

            if (shapeType == h.ShapeType)
                return;

            var newHole = ReplaceHole(h, shapeType);
            if (_menu != null)
            {
                _menu.SelectHole(newHole);
            }
        }

        private Hole ReplaceHole(Hole h, HoleShape shapeType)
        {
            _holes.Remove(h);
            var hole = InsertObject(shapeType, h.CellPos);
            Destroy(h.gameObject);

            _holes.Add(hole);
            return hole;
        }

        public void OnClickSheep()
        {
            Debug.Log("Insert Sheep");
            _state = MapEditorState.InsertSheep;
            _selectedObject = null;

            _menu.OnUnselectHole();
            _menu.OnUpdateBoardState(_state);
        }

        public void ChangeStateRemoveObject()
        {
            Log("ChangeStateRemoveObject");
            _state = MapEditorState.RemoveSheep;
            _menu.OnUpdateBoardState(_state);
        }

        public void ChangeStateRemoveCell()
        {
            Log("ChangeStateRemoveCell");
            _state = MapEditorState.RemoveCell;
            _menu.OnUpdateBoardState(_state);
        }

        public void ChangeStateAddCell()
        {
            Log("ChangeStateAddCell");
            _state = MapEditorState.InsertCell;
            _menu.OnUpdateBoardState(_state);
        }

        public void ChangeStateNormal()
        {
            _state = MapEditorState.Normal;
            _menu.OnUpdateBoardState(_state);
        }
    }
}
