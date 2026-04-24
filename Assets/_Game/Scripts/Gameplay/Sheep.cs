using UnityEngine;

namespace Gameplay
{
    public class Sheep : MonoBehaviour
    {
        [SerializeField] SheepColor _color;
        [SerializeField] Vector2Int _cellPos;
        [SerializeField] Board _board;

        public SheepColor Color => _color;

        public Vector2Int CellPos => _cellPos;

        public bool IsAtCell(Vector2Int pos)
        {
            return _cellPos == pos;
        }

        public void JumpToHole(IHole hole)
        {
            //todo play animation
            gameObject.SetActive(false);
            hole.OnSheepEnterHole();
        }

#if UNITY_EDITOR
        private void OnValidate()
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
