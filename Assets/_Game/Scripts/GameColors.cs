using UnityEngine;

public static class GameColors
{
    public static readonly Color[] Palette = new Color[]
    {
        new Color(0.92f, 0.18f, 0.18f), // 0 Red
        new Color(0.18f, 0.48f, 0.95f), // 1 Blue
        new Color(0.12f, 0.78f, 0.28f), // 2 Green
        new Color(0.95f, 0.80f, 0.04f), // 3 Yellow
        new Color(0.68f, 0.18f, 0.92f), // 4 Purple
        new Color(0.95f, 0.52f, 0.06f), // 5 Orange
    };

    public static int Count => Palette.Length;

    // Tetris-like shapes: Vector2Int(col, row) offsets from pivot (0,0)
    public static readonly Vector2Int[][] Shapes = new Vector2Int[][]
    {
        // 1-cell
        new[] { new Vector2Int(0, 0) },

        // 2-cell horizontal
        new[] { new Vector2Int(0, 0), new Vector2Int(1, 0) },
        // 2-cell vertical
        new[] { new Vector2Int(0, 0), new Vector2Int(0, 1) },

        // I-3 horizontal
        new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },
        // I-3 vertical
        new[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) },
        // L-3 (bottom-left pivot)
        new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1) },
        // J-3
        new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1) },

        // I-4 horizontal
        new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0) },
        // O 2x2
        new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },
        // T
        new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(1, 1) },
        // L-4
        new[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(1, 0) },
        // S
        new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(2, 1) },
        // Z
        new[] { new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },
    };
}
