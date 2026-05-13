using UnityEngine;

namespace Gameplay
{
    public class OnBoardObject: MonoBehaviour
    {
        [SerializeField] protected Vector2Int _cellPos;
        [SerializeField] protected Board _board;

        public void Init(Vector2Int pos, Board board)
        {
            _cellPos = pos;
            _board = board;
        }


        #if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            LoadPositionFromCellPos();
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
#endif
    }
}
