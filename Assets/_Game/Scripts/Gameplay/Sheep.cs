using UnityEngine;

namespace Gameplay
{
    public class Sheep : OnBoardObject
    {
        [SerializeField] SheepColor _color;

        [SerializeField] SpriteRenderer _renderer;
        [SerializeField] SheepColorConst _sheepColorConst;
        

        public SheepColor Color => _color;



        public void Init(Vector2Int pos, Board board, SheepColor color)
        {
            base.Init(pos, board);
            _color = color;
        }

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

        public void LoadData()
        {
            LoadPositionFromCellPos();
            Color color = _sheepColorConst.GetColor(_color);

            _renderer.color = color;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            LoadData();
        }
#endif
    }
}
