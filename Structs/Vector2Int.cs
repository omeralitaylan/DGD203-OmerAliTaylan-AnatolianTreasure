namespace IslandAdventure.Structs;

public struct Vector2Int
{
    public int X;
    public int Y;
    
    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }
    
    public Vector2Int()
    {
        X = 0;
        Y = 0;
    }
    
    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}
