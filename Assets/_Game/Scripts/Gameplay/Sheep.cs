using UnityEngine;

namespace Gameplay
{
    public class Sheep: MonoBehaviour
    {
        [SerializeField] SheepColor _color;
        [SerializeField] Vector2Int _cellPos;

        public SheepColor Color => _color;

        public Vector2Int CellPos => _cellPos;

        public bool IsAtCell(Vector2Int pos)
        {
            return _cellPos == pos;
        }
    }
}
