using Gameplay;
using UnityEngine;

public class Cell : MonoBehaviour
{
    int _x; // col
    int _y; // row

    public CellType Type;

    public void Init(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public int X => _x;
    public int Y => _y;
}

