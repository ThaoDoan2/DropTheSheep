using UnityEngine;

namespace Gameplay
{
    public class Sheep : OnBoardObject
    {
        [SerializeField] SheepColor _color;

        [SerializeField] SpriteRenderer _renderer;
        

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
        protected override void OnValidate()
        {
            base.OnValidate();
            Color color = GameplaySO.instance.GetColor(_color);

            _renderer.color = color;
        }
#endif
    }
}
