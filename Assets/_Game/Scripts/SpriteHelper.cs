using UnityEngine;

public static class SpriteHelper
{
    private static Sprite _square;
    private static Sprite _circle;
    private static Sprite _border;
    private static Material _unlitMat;

    // Shared unlit material for all game sprites (bypass 2D lights)
    public static Material UnlitMat
    {
        get
        {
            if (_unlitMat == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
                if (shader == null) shader = Shader.Find("Sprites/Default");
                _unlitMat = shader != null ? new Material(shader) : null;
            }
            return _unlitMat;
        }
    }

    public static void ApplyUnlit(SpriteRenderer sr)
    {
        if (sr != null && UnlitMat != null)
            sr.sharedMaterial = UnlitMat;
    }

    public static Sprite Square
    {
        get
        {
            if (_square == null)
            {
                var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
                var px = new Color[16];
                for (int i = 0; i < 16; i++) px[i] = Color.white;
                tex.SetPixels(px);
                tex.filterMode = FilterMode.Point;
                tex.Apply();
                _square = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
                _square.name = "Square";
            }
            return _square;
        }
    }

    public static Sprite Circle
    {
        get
        {
            if (_circle == null)
            {
                int sz = 64;
                var tex = new Texture2D(sz, sz, TextureFormat.RGBA32, false);
                var px = new Color[sz * sz];
                float cx = sz * 0.5f, cy = sz * 0.5f, r = sz * 0.5f - 1.5f;
                for (int y = 0; y < sz; y++)
                for (int x = 0; x < sz; x++)
                {
                    float dx = x + 0.5f - cx, dy = y + 0.5f - cy;
                    float a = Mathf.Clamp01(r - Mathf.Sqrt(dx * dx + dy * dy) + 0.5f);
                    px[y * sz + x] = new Color(1, 1, 1, a);
                }
                tex.SetPixels(px);
                tex.filterMode = FilterMode.Bilinear;
                tex.Apply();
                _circle = Sprite.Create(tex, new Rect(0, 0, sz, sz), new Vector2(0.5f, 0.5f), (float)sz);
                _circle.name = "Circle";
            }
            return _circle;
        }
    }

    public static Sprite Border
    {
        get
        {
            if (_border == null)
            {
                int sz = 32, bw = 5;
                var tex = new Texture2D(sz, sz, TextureFormat.RGBA32, false);
                var px = new Color[sz * sz];
                for (int y = 0; y < sz; y++)
                for (int x = 0; x < sz; x++)
                {
                    bool edge = x < bw || x >= sz - bw || y < bw || y >= sz - bw;
                    px[y * sz + x] = edge ? Color.white : Color.clear;
                }
                tex.SetPixels(px);
                tex.filterMode = FilterMode.Point;
                tex.Apply();
                _border = Sprite.Create(tex, new Rect(0, 0, sz, sz), new Vector2(0.5f, 0.5f), (float)sz);
                _border.name = "Border";
            }
            return _border;
        }
    }
}
