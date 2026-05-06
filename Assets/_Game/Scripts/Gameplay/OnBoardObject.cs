using UnityEngine;

namespace Gameplay
{
    public class OnBoardObject: MonoBehaviour
    {
        [SerializeField] protected Vector2Int _cellPos;
        [SerializeField] protected Board _board;


#if UNITY_EDITOR
        protected virtual void OnValidate()
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
