/*
 * I started the project with the code build you provided. After adding some features to the game,
 * I started getting some errors and I used AI to solve them. (Claude Ai)
 */

using IslandAdventure.Structs;

namespace IslandAdventure;

public class GameMap
{
    #region REFERENCES
    public readonly Game _game;
    private MapLocations _locations;
    #endregion

    #region VARIABLES
    private readonly int _width;
    private readonly int _height;
    private Vector2Int _playerPosition;
    #endregion

    #region CONSTRUCTOR
    public GameMap(Game game, int width, int height, Vector2Int startPosition)
    {
        _game = game;
        _width = width;
        _height = height;
        
        // Initialize locations
        _locations = new MapLocations(this);

        // Set initial player position
        if (IsValidPosition(startPosition))
        {
            _playerPosition = startPosition;
        }
        else
        {
            _playerPosition = new Vector2Int();
        }
    }
    #endregion

    #region METHODS
    public Vector2Int GetPlayerPosition() => _playerPosition;

    public bool MovePlayer(Direction direction)
    {
        var newPosition = CalculateNewPosition(direction);
        return SetPlayerPosition(newPosition);
    }

    public bool MovePlayer(Vector2Int targetPosition)
    {
        return SetPlayerPosition(targetPosition);
    }

    public bool SetPlayerPosition(Vector2Int targetPosition)
    {
        if (IsValidPosition(targetPosition))
        {
            _playerPosition = targetPosition;
            return true;
        }
        return false;
    }

    private Vector2Int CalculateNewPosition(Direction direction)
    {
        Vector2Int newPosition = _playerPosition;
        
        switch (direction)
        {
            case Direction.North:
                newPosition.Y -= 1;  // Moving north decreases Y
                break;
            case Direction.South:
                newPosition.Y += 1;  // Moving south increases Y
                break;
            case Direction.West:
                newPosition.X -= 1;
                break;
            case Direction.East:
                newPosition.X += 1;
                break;
            default:
                Console.WriteLine("That is not a valid direction");
                break;
        }

        return newPosition;
    }

    public bool IsValidPosition(Vector2Int position)
    {
        return position.X >= 0 && position.X < _width &&
               position.Y >= 0 && position.Y < _height;
    }

    public string GetCurrentLocationName()
    {
        if (_locations.HasLocation(_playerPosition))
        {
            return _locations.GetLocationName(_playerPosition);
        }
        return "an unexplored area of the island";
    }

    public string GetCurrentLocationDescription()
    {
        return _locations.GetLocationDescription(_playerPosition);
    }

    public void InteractWithCurrentLocation()
    {
        if (_locations.TryInteract(_playerPosition, _game.Player.Inventory))
        {
            foreach (var item in _locations.Locations[_playerPosition].Items)
            {
                if (!_game.Player.Inventory.HasItem(item.Name))
                {
                    Console.WriteLine($"\nYou found: {item.Name} - {item.Description}");
                    _game.Player.Inventory.AddItem(item);
                }
            }
        }
    }

    public void Look()
    {
        var location = _locations.Locations[_playerPosition];
        Console.WriteLine($"\nLocation: {location.Name}");
        Console.WriteLine(location.Description);
    }

    public Location GetCurrentLocation()
    {
        return _locations.Locations[_playerPosition];
    }

    public void UnlockLocation(Vector2Int position)
    {
        // This method is called when a new location becomes accessible
        // For now, we just print a message as the locations are already in the dictionary
        Console.WriteLine($"A new path has been revealed! You can now travel to new areas.");
    }

    public string GetNextNPCDialogue()
    {
        return _locations.GetNextNPCDialogue(_playerPosition);
    }
    #endregion
}

public class MapLocationData
{
    public string Name { get; }
    public string Description { get; }
    public bool IsInteractable { get; }
    public Interaction Interaction { get; }

    public MapLocationData(string name, string description, bool isInteractable, Interaction interaction)
    {
        Name = name;
        Description = description;
        IsInteractable = isInteractable;
        Interaction = interaction;
    }
}

public class Interaction
{
    private readonly string _prompt;
    private readonly Action _action;

    public Interaction(string prompt, Action action)
    {
        _prompt = prompt;
        _action = action;
    }

    public void Choose()
    {
        Console.WriteLine($"\n{_prompt}");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        _action.Invoke();
    }
}