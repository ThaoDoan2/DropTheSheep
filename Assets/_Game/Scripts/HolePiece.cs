using System.Collections.Generic;
using UnityEngine;

public class HolePiece : MonoBehaviour
{
    public int colorIndex;
    public Vector2Int[] shape;
    public Vector2Int pivot;

    public int FillCount  { get; private set; }
    public int Capacity   => shape.Length;
    public bool IsFull    => FillCount >= Capacity;

    private readonly List<GameObject> _cellGOs = new List<GameObject>();
    private bool[] _filled;
    private float _cellSize;

    public void Init(int colorIndex, Vector2Int[] shape, Vector2Int pivot, float cellSize)
    {
        this.colorIndex = colorIndex;
        this.shape      = shape;
        this.pivot      = pivot;
        _cellSize       = cellSize;
        FillCount       = 0;
        _filled         = new bool[shape.Length];
        BuildCells();
    }

    private void BuildCells()
    {
        foreach (var g in _cellGOs) if (g) Destroy(g);
        _cellGOs.Clear();

        Color hc = GameColors.Palette[colorIndex];

        for (int i = 0; i < shape.Length; i++)
        {
            var cell = new GameObject($"HCell_{i}");
            cell.transform.SetParent(transform);
            cell.transform.localPosition = new Vector3(shape[i].x * _cellSize, shape[i].y * _cellSize, 0f);

            // Dark interior
            var inner = new GameObject("Inner");
            inner.transform.SetParent(cell.transform);
            inner.transform.localPosition = Vector3.zero;
            inner.transform.localScale    = Vector3.one * 0.86f;
            var inSr = inner.AddComponent<SpriteRenderer>();
            SpriteHelper.ApplyUnlit(inSr);
            inSr.sprite       = SpriteHelper.Square;
            inSr.color        = new Color(0.06f, 0.06f, 0.10f);
            inSr.sortingOrder = 3;

            // Colored border
            var bord = new GameObject("Bord");
            bord.transform.SetParent(cell.transform);
            bord.transform.localPosition = Vector3.zero;
            bord.transform.localScale    = Vector3.one * 0.96f;
            var bSr = bord.AddComponent<SpriteRenderer>();
            SpriteHelper.ApplyUnlit(bSr);
            bSr.sprite       = SpriteHelper.Border;
            bSr.color        = hc;
            bSr.sortingOrder = 4;

            _cellGOs.Add(cell);
        }
    }

    public bool IsCellFilled(int i) => i >= 0 && i < _filled.Length && _filled[i];

    public void FillCell(int i)
    {
        if (i < 0 || i >= _filled.Length || _filled[i]) return;
        _filled[i] = true;
        FillCount++;

        Color c  = GameColors.Palette[colorIndex];
        var inSr = _cellGOs[i].transform.Find("Inner")?.GetComponent<SpriteRenderer>();
        if (inSr) inSr.color = new Color(c.r * 0.55f, c.g * 0.55f, c.b * 0.55f, 0.85f);
    }

    public void SetSelected(bool on)
    {
        Color c = GameColors.Palette[colorIndex];
        Color bc = on ? Color.Lerp(c, Color.white, 0.45f) : c;
        SetAllBorderColors(bc);
        SetAllSortOrder(on ? 9 : 3);
    }

    public void ShowPreviewAt(Vector2Int previewPivot, bool isValid, BoardManager board)
    {
        Color c  = GameColors.Palette[colorIndex];
        Color ov = isValid ? new Color(0.4f, 1f, 0.4f) : new Color(1f, 0.3f, 0.3f);
        SetAllBorderColors(Color.Lerp(c, ov, 0.55f));
        transform.position = board.GridToWorld(previewPivot.x, previewPivot.y);
    }

    public void ResetVisuals()
    {
        SetAllBorderColors(GameColors.Palette[colorIndex]);
        SetAllSortOrder(3);
    }

    public void MoveTo(Vector3 worldPos) => transform.position = worldPos;

    private void SetAllBorderColors(Color c)
    {
        foreach (var cell in _cellGOs)
        {
            var sr = cell.transform.Find("Bord")?.GetComponent<SpriteRenderer>();
            if (sr) sr.color = c;
        }
    }

    private void SetAllSortOrder(int baseOrder)
    {
        foreach (var cell in _cellGOs)
        {
            var inSr = cell.transform.Find("Inner")?.GetComponent<SpriteRenderer>();
            var bSr  = cell.transform.Find("Bord" )?.GetComponent<SpriteRenderer>();
            if (inSr) inSr.sortingOrder = baseOrder;
            if (bSr)  bSr .sortingOrder = baseOrder + 1;
        }
    }
}
