namespace Gameplay
{
    public enum SheepColor
    {
        None = -1,
        White = 0,
        Red = 1,
        Yellow = 2,
        Blue = 3,
        Orange = 4,
        Purple = 5,
        Pink = 6,
        Green = 7,
        Sky = 8,
        Lime = 9, 
        Black = 10
    }

    public enum CellType
    {
        Empty = 0,
        Sheep = 1,
        Hole = 2, 
        Block = 3,
    }

    public enum HoleShape
    {
        Square1 = 0,
        Horizontal2,
        Vertical2,

        Horizontal3,
        Vertical3,
        Corner31, //|_
        Corner32, 
        Corner33,
        Corner34,

        Square4,
        Horizontal4,
        Vertical4,
        Corner41, // L
        Corner42,
        Corner43,
        Corner44,
        Corner45,
        Corner46,
        Corner47,
        Corner48,



    }

    public class GameConst
    {
    }
}
