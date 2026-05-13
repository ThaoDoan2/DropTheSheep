using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class OnBoardObject : MonoBehaviour
    {
        [SerializeField] protected Vector2Int _cellPos;
        [SerializeField] protected Board _board;

        public Vector2Int CellPos => _cellPos;

        public Vector2Int Pivot => _cellPos;

        public void Init(Vector2Int pos, Board board)
        {
            _cellPos = pos;
            _board = board;
        }

        protected void LoadPositionFromCellPos()
        {
            if (_board == null)
                _board = transform.GetComponentInParent<Board>();
            if (_board == null)
                return;
            Vector3 pos = _board.GridToWorld(_cellPos.x, _cellPos.y);
            transform.localPosition = pos;
        }

        public void SetCell(Vector2Int pos)
        {
            _cellPos = pos;
        }

        public virtual void Select(Vector2Int pos)
        {
        }

        public virtual List<Vector2Int> GetShapeCells()
        {
            List<Vector2Int> cells = new List<Vector2Int>();
            cells.Add(_cellPos);
            return cells;
        }


#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            LoadPositionFromCellPos();
        }
#endif
    }
}
