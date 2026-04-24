using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    [Header("Board")]
    public int cols     = 7;
    public int rows     = 9;
    public float cellSize = 1f;

    [Header("Generation")]
    public int numColors = 4;
    public int numHoles  = 3;
    [Range(0f, 1f)] public float sheepDensity = 0.60f;

    public List<HolePiece> Holes => _holes;

    private GridCell[,]   _cells;
    private SheepPiece[,] _sheep;
    private List<HolePiece> _holes = new List<HolePiece>();

    private Transform _boardRoot;
    private Transform _sheepRoot;
    private Transform _holesRoot;

    void Awake() => Instance = this;

    // ============================================================ Generation

    public void GenerateBoard()
    {
        ClearBoard();
        numColors = Mathf.Min(numColors, GameColors.Count);
        BuildGrid();
        PopulateSheep();
        SpawnHoles();
    }

    private void ClearBoard()
    {
        if (_boardRoot) Destroy(_boardRoot.gameObject);
        if (_sheepRoot) Destroy(_sheepRoot.gameObject);
        if (_holesRoot) Destroy(_holesRoot.gameObject);
        _cells = null;
        _sheep = null;
        _holes.Clear();
    }

    private void BuildGrid()
    {
        _cells = new GridCell[cols, rows];
        _boardRoot = new GameObject("Board").transform;
        _boardRoot.SetParent(transform);

        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
        {
            var go   = new GameObject($"Cell_{c}_{r}");
            go.transform.SetParent(_boardRoot);
            go.transform.position = GridToWorld(c, r);
            var cell = go.AddComponent<GridCell>();
            cell.Init(c, r);
            _cells[c, r] = cell;
        }
    }

    private void PopulateSheep()
    {
        _sheep = new SheepPiece[cols, rows];
        _sheepRoot = new GameObject("Sheep").transform;
        _sheepRoot.SetParent(transform);

        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
        {
            if (Random.value < sheepDensity)
                SpawnSheep(c, r, Random.Range(0, numColors));
        }
    }

    private void SpawnSheep(int c, int r, int ci)
    {
        var go = new GameObject($"Sheep_{c}_{r}");
        go.transform.SetParent(_sheepRoot);
        go.transform.position = GridToWorld(c, r);
        var s = go.AddComponent<SheepPiece>();
        s.Init(c, r, ci);
        _sheep[c, r] = s;
    }

    private void SpawnHoles()
    {
        _holesRoot = new GameObject("Holes").transform;
        _holesRoot.SetParent(transform);

        for (int h = 0; h < numHoles; h++)
            TrySpawnHole(50);
    }

    private bool TrySpawnHole(int maxTries)
    {
        for (int t = 0; t < maxTries; t++)
        {
            int si = Random.Range(1, Mathf.Min(8, GameColors.Shapes.Length));
            Vector2Int[] shape = GameColors.Shapes[si];
            int ci = Random.Range(0, numColors);

            int pc = Random.Range(0, cols);
            int pr = Random.Range(0, rows);

            bool ok = true;
            foreach (var off in shape)
            {
                int tc = pc + off.x, tr = pr + off.y;
                if (tc < 0 || tc >= cols || tr < 0 || tr >= rows) { ok = false; break; }
                if (HoleOccupies(tc, tr)) { ok = false; break; }
            }
            if (!ok) continue;

            var go = new GameObject($"Hole_{_holes.Count}");
            go.transform.SetParent(_holesRoot);
            var hole = go.AddComponent<HolePiece>();
            hole.Init(ci, shape, new Vector2Int(pc, pr), cellSize);
            hole.MoveTo(GridToWorld(pc, pr));
            _holes.Add(hole);

            foreach (var off in shape)
            {
                int tc = pc + off.x, tr = pr + off.y;
                if (_sheep[tc, tr] != null) { _sheep[tc, tr].Disappear(); _sheep[tc, tr] = null; }
            }
            return true;
        }
        return false;
    }

    // ============================================================ Queries

    private bool HoleOccupies(int c, int r)
    {
        foreach (var h in _holes)
        foreach (var off in h.shape)
            if (h.pivot.x + off.x == c && h.pivot.y + off.y == r) return true;
        return false;
    }

    public HolePiece GetHoleAt(Vector2Int grid)
    {
        foreach (var h in _holes)
        foreach (var off in h.shape)
            if (h.pivot + off == grid) return h;
        return null;
    }

    public bool IsValidPlacement(HolePiece hole, Vector2Int newPivot)
    {
        for (int i = 0; i < hole.shape.Length; i++)
        {
            int tc = newPivot.x + hole.shape[i].x;
            int tr = newPivot.y + hole.shape[i].y;

            if (tc < 0 || tc >= cols || tr < 0 || tr >= rows) return false;

            foreach (var other in _holes)
            {
                if (other == hole) continue;
                foreach (var off in other.shape)
                    if (other.pivot.x + off.x == tc && other.pivot.y + off.y == tr) return false;
            }

            if (!hole.IsCellFilled(i))
            {
                var s = _sheep[tc, tr];
                if (s != null && s.colorIndex != hole.colorIndex) return false;
            }
        }
        return true;
    }

    // ============================================================ Place hole

    public void PlaceHole(HolePiece hole, Vector2Int newPivot)
    {
        if (hole == null || !IsValidPlacement(hole, newPivot)) return;

        hole.pivot = newPivot;
        hole.MoveTo(GridToWorld(newPivot.x, newPivot.y));

        for (int i = 0; i < hole.shape.Length; i++)
        {
            if (hole.IsCellFilled(i)) continue;
            int tc = newPivot.x + hole.shape[i].x;
            int tr = newPivot.y + hole.shape[i].y;
            var s = _sheep[tc, tr];
            if (s != null && s.colorIndex == hole.colorIndex)
            {
                s.Disappear();
                _sheep[tc, tr] = null;
                hole.FillCell(i);
            }
        }

        if (hole.IsFull)
        {
            _holes.Remove(hole);
            Destroy(hole.gameObject);
            GameManager.Instance?.CheckWinCondition();
        }
    }

    // ============================================================ Highlighting

    public void HighlightPreview(HolePiece hole, Vector2Int previewPivot)
    {
        ResetHighlights();
        bool valid = IsValidPlacement(hole, previewPivot);
        foreach (var off in hole.shape)
        {
            int tc = previewPivot.x + off.x, tr = previewPivot.y + off.y;
            if (tc < 0 || tc >= cols || tr < 0 || tr >= rows) continue;
            if (valid) _cells[tc, tr].SetValid();
            else       _cells[tc, tr].SetInvalid();
        }
    }

    public void ResetHighlights()
    {
        if (_cells == null) return;
        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
            _cells[c, r]?.SetNormal();
    }

    // ============================================================ Coordinate helpers

    public Vector3 GridToWorld(int c, int r)
    {
        float ox = -(cols - 1) * cellSize * 0.5f;
        float oy = -(rows - 1) * cellSize * 0.5f;
        return new Vector3(ox + c * cellSize, oy + r * cellSize, 0f);
    }

    public Vector2Int WorldToGrid(Vector3 world)
    {
        float ox = -(cols - 1) * cellSize * 0.5f;
        float oy = -(rows - 1) * cellSize * 0.5f;
        int c = Mathf.RoundToInt((world.x - ox) / cellSize);
        int r = Mathf.RoundToInt((world.y - oy) / cellSize);
        return new Vector2Int(c, r);
    }
}
