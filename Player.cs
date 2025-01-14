/*
 I asked you a question about the use of AI in the classroom. Before asking that question, I had made another much more detailed game with AI and another code structure, but after I asked you the question, I changed my mind and decided to cancel this game and create something with your structure.

I built the project on the code structure you gave. After some features I added to the game, I encountered errors and edited the project with Claude AI to fix these errors.
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
