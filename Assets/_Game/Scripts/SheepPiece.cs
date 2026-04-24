using UnityEngine;

public class SheepPiece : MonoBehaviour
{
    public int col;
    public int row;
    public int colorIndex;

    public void Init(int col, int row, int colorIndex)
    {
        this.col = col;
        this.row = row;
        this.colorIndex = colorIndex;

        Color c = GameColors.Palette[colorIndex];

        var body = gameObject.GetComponent<SpriteRenderer>();
        if (body == null) body = gameObject.AddComponent<SpriteRenderer>();
        SpriteHelper.ApplyUnlit(body);
        body.sprite = SpriteHelper.Circle;
        body.color  = c;
        body.sortingOrder = 5;
        transform.localScale = Vector3.one * 0.72f;

        MakeCircleChild("Wool", Vector3.zero, 0.56f, Color.white, 6);
        MakeCircleChild("EyeL", new Vector3(-0.14f, 0.08f, 0), 0.18f, new Color(0.1f, 0.1f, 0.1f), 7);
        MakeCircleChild("EyeR", new Vector3( 0.14f, 0.08f, 0), 0.18f, new Color(0.1f, 0.1f, 0.1f), 7);
        MakeCircleChild("Dot",  new Vector3(0, -0.12f, 0), 0.22f, new Color(c.r * 0.7f, c.g * 0.7f, c.b * 0.7f), 8);
    }

    private void MakeCircleChild(string n, Vector3 localPos, float scale, Color color, int order)
    {
        var go = new GameObject(n);
        go.transform.SetParent(transform);
        go.transform.localPosition = localPos;
        go.transform.localScale    = Vector3.one * scale;
        var sr = go.AddComponent<SpriteRenderer>();
        SpriteHelper.ApplyUnlit(sr);
        sr.sprite = SpriteHelper.Circle;
        sr.color  = color;
        sr.sortingOrder = order;
    }

    public void Disappear() => Destroy(gameObject);
}
