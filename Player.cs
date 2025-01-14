/*
 * I started the project with the code build you provided. After adding some features to the game,
 * I started getting some errors and I used AI to solve them. (Claude Ai)
 */

using IslandAdventure.Structs;

namespace IslandAdventure;

public class Player
{
    private readonly Game _game;
    public string Name { get; private set; }
    public Vector2Int Position { get; private set; }
    public Inventory Inventory { get; set; }  

    public Player(Game game)
    {
        _game = game;
        Position = Game.DefaultStartingCoordinates;
        Name = "Player";
    }

    public Player()
    {
        Position = Game.DefaultStartingCoordinates;
        Inventory = new Inventory();
        Name = "Player";
    }

    public void SetUp(string name)
    {
        Name = name;
    }

    public void Move(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                Position = new Vector2Int { X = Position.X, Y = Position.Y + 1 };
                break;
            case Direction.South:
                Position = new Vector2Int { X = Position.X, Y = Position.Y - 1 };
                break;
            case Direction.East:
                Position = new Vector2Int { X = Position.X + 1, Y = Position.Y };
                break;
            case Direction.West:
                Position = new Vector2Int { X = Position.X - 1, Y = Position.Y };
                break;
        }
    }

    public void SetPosition(Vector2Int newPosition)
    {
        Position = newPosition;
    }
}
