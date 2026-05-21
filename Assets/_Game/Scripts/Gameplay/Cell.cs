using Gameplay;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] SpriteRenderer _center;
    [SerializeField] SpriteRenderer _tf, _tr, _bl, _br;
    [SerializeField] SpriteRenderer _top, _bottom, _left, _right;
    [SerializeField] SpriteRenderer _horizontalCell, _verticalCell;
    [SerializeField] SpriteRenderer _penisulaTop, _peninsulaBottom, _peninsulaLeft, _peninsulaRight;

    SpriteRenderer[] _allCorners;
    int _x; // col
    int _y; // row

    public CellType Type;

    public void Init(int x, int y)
    {
        _x = x;
        _y = y;

        _allCorners = new SpriteRenderer[] { _tf, _tr, _bl, _br , _top, _left, _left, _right};
    }

    public int X => _x;
    public int Y => _y;

    public void UpdateCell(bool left, bool top, bool right, bool bottom)
    {
        _center.gameObject.SetActive(true);
        int t = (left ? 1 <<  3 : 0) ^ (top ? 1 << 2 : 0) ^ (right ? 1 << 1 : 0) ^ (bottom ? 1 : 0);
        switch (t)
        {
            case 0:
                UpdateIsolatedCell();
                break;
            case 1:
                UpdateTopPeninsula();
                break;
            case 2:
                UpdateLeftPeninsula();
                break;
            case 3:
                UpdateTopLeftCorner();
                break;
            case 4:
                UpdateBottomPeninsula();
                break;
            case 5:
                UpdateVerticalCell();
                break;
            case 6:
                UpdateBottomLeftCorner();
                break;
            case 7:
                UpdateLeftEdge();
                break;
            case 8:
                UpdateRightPenisula();
                break;
            case 9:
                UpdateTopRightCorner();
                break;
            case 10:
                UpdateHorizontalCell();
                break;
            case 11:
                UpdateTopEdge();
                break;
            case 12:
                UpdateBottomRightCorner();
                break;
            case 13:
                UpdateRightEdge();
                break;
            case 14:
                UpdateBottomEdge();
                break;
            case 15:
                UpdateCenterCell();
                break;
        }
    }

    void HideAllElement()
    {
        foreach (var item in _allCorners)
        {
            item.gameObject.SetActive(false);
        }
    }

    void UpdateTopLeftCorner()
    {
        HideAllElement();
        _tf.gameObject.SetActive(true);

    }

    void UpdateTopRightCorner()
    {
        HideAllElement();
        _tr.gameObject.SetActive(true);
    }

    void UpdateBottomLeftCorner()
    {
        HideAllElement();
        _bl.gameObject.SetActive(true);
    }

    void UpdateBottomRightCorner()
    {
        HideAllElement();
        _br.gameObject.SetActive(true);
    }

    void UpdateTopEdge()
    {
        HideAllElement();
        _top.gameObject.SetActive(true);
    }

    void UpdateBottomEdge()
    {
        HideAllElement();
        _bottom.gameObject.SetActive(true);
    }

    void UpdateLeftEdge()
    {
        HideAllElement();
        _left.gameObject.SetActive(true);
    }

    void UpdateRightEdge()
    {
        HideAllElement();
        _right.gameObject.SetActive(true);
    }

    void UpdateCenterCell()
    {
        HideAllElement();
    }

    void UpdateIsolatedCell()
    {
        HideAllElement();
    }

    void UpdateTopPeninsula()
    {
        HideAllElement();
        _penisulaTop.gameObject.SetActive(true);
    }

    void UpdateBottomPeninsula()
    {
        HideAllElement();
        _peninsulaBottom.gameObject.SetActive(true);
    }

    void UpdateLeftPeninsula()
    {
        HideAllElement();
        _peninsulaLeft.gameObject.SetActive(true);
    }

    void UpdateRightPenisula()
    {
        HideAllElement();
        _peninsulaRight.gameObject.SetActive(true);
    }

    void UpdateHorizontalCell()
    {
        _horizontalCell.gameObject.SetActive(true);
    }

    void UpdateVerticalCell()
    {
        _verticalCell.gameObject.SetActive(true);
    }

    public void OnRemoved()
    {
        _center.gameObject.SetActive(false);
        HideAllElement();
    }
}

