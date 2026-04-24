using UnityEngine;

public class GridCell : MonoBehaviour
{
    public int col;
    public int row;

    private SpriteRenderer _bg;

    static readonly Color Normal  = new Color(0.78f, 0.80f, 0.83f);
    static readonly Color Valid   = new Color(0.40f, 0.95f, 0.40f);
    static readonly Color Invalid = new Color(0.95f, 0.35f, 0.35f);

    public void Init(int col, int row)
    {
        this.col = col;
        this.row = row;

        _bg = gameObject.GetComponent<SpriteRenderer>();
        if (_bg == null) _bg = gameObject.AddComponent<SpriteRenderer>();

        SpriteHelper.ApplyUnlit(_bg);
        _bg.sprite = SpriteHelper.Square;
        _bg.color  = Normal;
        _bg.sortingOrder = 0;

        transform.localScale = Vector3.one * 0.96f;
    }

    public void SetNormal()  => _bg.color = Normal;
    public void SetValid()   => _bg.color = Valid;
    public void SetInvalid() => _bg.color = Invalid;
}
