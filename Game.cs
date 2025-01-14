/*
 * I started the project with the code build you provided. After adding some features to the game,
 * I started getting some errors and I used AI to solve them. (Claude Ai)
 */

using IslandAdventure.Structs;

namespace IslandAdventure;

public class Game
{
    #region REFERENCES
    private Player _player;
    private GameMap _map;
    private Commands _commands;
    private Inventory _inventory;
    private SaveManager _saveManager;
    #endregion

    #region PROPERTIES
    public Player Player => _player;
    public GameMap Map => _map;
    #endregion

    #region VARIABLES
    private const int MapWidth = 2;
    private const int MapHeight = 2;
    private const string NewCommandSeparator = "--------------------------";
    public static readonly Vector2Int DefaultStartingCoordinates = new Vector2Int { X = 0, Y = 0 };
    private bool _isRunning;
    private string _currentCommand;
    #endregion

    #region CONSTRUCTOR
    public Game()
    {
        GenerateStartingInstances();
        CheckForLoadGame();
    }
    #endregion

    #region METHODS
    public void StartGame()
    {
        ShowIntroduction();
        MainMenu();
    }

    private void ShowIntroduction()
    {
        Console.Clear();
        Console.WriteLine("\n=== The Mysterious Island ===\n");
        Console.WriteLine("The year is 1875. You are a seasoned explorer who received a peculiar letter");
        Console.WriteLine("from an anonymous source, speaking of an uncharted island in the South Pacific.");
        Console.WriteLine("The letter contained an old map, coordinates, and a cryptic warning:");
        Console.WriteLine("\n'What lies within the island's heart could change the course of history...'");
        Console.WriteLine("\nAfter months of preparation, you've finally reached the island's shores.");
        Console.WriteLine("The thick mist parts before your small boat as you approach the beach.");
        Console.WriteLine("Your journey into the unknown begins now...");
        Console.WriteLine("\nPress any key to begin your adventure...");
        Console.ReadKey();
    }

    private void MainMenu()
    {
        bool inMenu = true;
        while (inMenu)
        {
            Console.Clear();
            Console.WriteLine("\n=== The Mysterious Island ===");
            Console.WriteLine("The mist swirls around you as you consider your next move...\n");
            if (_saveManager.SaveFileExists())
            {
                Console.WriteLine("1. Continue Your Journey");
                Console.WriteLine("2. Begin New Journey");
                Console.WriteLine("3. Tales of Past Explorers - Credits :D");
                Console.WriteLine("4. Abandon the Expedition");
            }
            else 
            {
                Console.WriteLine("1. Begin Your Journey");
                Console.WriteLine("2. Tales of Past Explorers - Credits :D");
                Console.WriteLine("3. Abandon the Expedition");
            }
            Console.Write("\nWhat will you do? ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    if (_saveManager.SaveFileExists())
                    {
                        LoadGameMenu();
                        StartGameLoop();
                    }
                    else
                    {
                        StartNewGame();
                    }
                    inMenu = false;
                    break;
                case "2":
                    if (_saveManager.SaveFileExists())
                    {
                        StartNewGame();
                        inMenu = false;
                    }
                    else
                    {
                        ShowCredits();
                    }
                    break;
                case "3":
                    if (_saveManager.SaveFileExists())
                    {
                        ShowCredits();
                    }
                    else
                    {
                        Console.WriteLine("\nYou decide to turn back, leaving the island's mysteries unsolved...");
                        Thread.Sleep(2000);
                        Environment.Exit(0);
                    }
                    break;
                case "4":
                    if (_saveManager.SaveFileExists())
                    {
                        ClearGameState(); // Clear state before exiting
                        Console.WriteLine("\nYou decide to turn back, leaving the island's mysteries unsolved...");
                        Thread.Sleep(2000);
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine("\nThe fog clouds your judgment. Try again...");
                    }
                    break;
                default:
                    Console.WriteLine("\nThe fog clouds your judgment. Try again...");
                    break;
            }
        }
    }

    private void LoadGameMenu()
    {
        Console.Clear();
        Console.WriteLine("\n=== Load Game ===");
        var saves = _saveManager.ListSaveFiles();
        
        if (saves.Length == 0)
        {
            Console.WriteLine("No save files found.");
            Thread.Sleep(2000);
            return;
        }

        for (int i = 0; i < saves.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {saves[i]}");
        }
        Console.WriteLine("\nEnter the number of the save to load (or press Enter to go back):");
        
        string input = Console.ReadLine();
        if (string.IsNullOrEmpty(input)) return;

        if (int.TryParse(input, out int choice) && choice > 0 && choice <= saves.Length)
        {
            _saveManager.LoadGame(saves[choice - 1]);
        }
        else
        {
            Console.WriteLine("Invalid choice.");
            Thread.Sleep(1000);
        }
    }

    private void SaveGameMenu()
    {
        Console.Clear();
        Console.WriteLine("\n=== Save Game ===");
        Console.WriteLine("Enter a name for your save file (or press Enter to cancel):");
        
        string saveName = Console.ReadLine();
        if (!string.IsNullOrEmpty(saveName))
        {
            _saveManager.SaveGame(saveName);
            Thread.Sleep(1000);
        }
    }

    private void ShowCredits()
    {
        Console.Clear();
        Console.WriteLine("\n=== Tales of Past Explorers ===");
        Console.WriteLine("Many brave souls have ventured to uncover the island's secrets...\n");
        Console.WriteLine("Chief Explorer: Ömer Ali Taylan");
        Console.WriteLine("Chronicle Keeper: Based on 'In Pursuit of the Mysterious Island'");
        Console.WriteLine("\nTheir stories live on in the island's whispers...");
        Console.WriteLine("\nPress any key to return...");
        Console.ReadKey();
    }

    private void StartNewGame()
    {
        ClearGameState();
        Console.Clear();
        Console.WriteLine("\nWhat is your name, brave explorer? ");
        string playerName = Console.ReadLine();
        _player.SetUp(string.IsNullOrWhiteSpace(playerName) ? "Unknown Explorer" : playerName);
        StartGameLoop();
    }

    private void GenerateStartingInstances()
    {
        _inventory = new Inventory(this);
        _player = new Player(this);
        _player.Inventory = _inventory;  // Connect the inventory to the player
        _saveManager = new SaveManager(this);
        _map = new GameMap(this, MapWidth, MapHeight, DefaultStartingCoordinates);
        _commands = new Commands(this, _map, _saveManager, _player, _inventory);
        _saveManager.Initialize(_player, _map, _inventory);
    }

    private void InitializeStartingInstances()
    {
        // This method is now deprecated and will be removed
        // All initialization is now handled in GenerateStartingInstances
    }

    private void GetPlayerName()
    {
        Console.Clear();
        Console.WriteLine("Before we begin your adventure, tell me your name:");
        string playerName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(playerName))
        {
            playerName = "Mysterious Explorer";
        }

        _player.SetUp(playerName);
        Console.WriteLine($"\nWelcome, {_player.Name}, to the Mysterious Island!");
        Console.WriteLine("\nPress any key to begin your adventure...");
        Console.ReadKey();
    }

    private void CheckForLoadGame()
    {
        if (_saveManager.SaveFileExists())
        {
            Console.WriteLine("\nSave file found. Do you want to load the game? (Y/N)");
            string answer = Console.ReadLine();
            if (answer?.ToLower() == "y")
            {
                LoadGameMenu();
            }
        }
    }

    private void StartGameLoop()
    {
        _isRunning = true;
        Console.Clear();
        GameLoop();
    }

    private void GameLoop()
    {
        while (_isRunning)
        {
            Console.WriteLine(NewCommandSeparator);
            DisplayMapAndLocation();
            Console.Write("\nWhat would you like to do? (type 'help' for commands)\n>> ");
            ReceiveCommand();
            ProcessCommand();
        }
    }

    private void DisplayMapAndLocation()
    {
        Console.WriteLine(@"
+-----------+------------+
| Lost Cove | Whispering |       N
|           | Woods      |       │
+-----------+------------+  W ───┼─── E
| Forgotten | Temple     |       │
| Harbor    | Summit     |       S
+-----------+------------+
");
        Console.WriteLine($"Current Location: {_map.GetCurrentLocationName()}");
    }

    private void ReceiveCommand()
    {
        _currentCommand = Console.ReadLine();
    }

    private void ProcessCommand()
    {
        if (string.IsNullOrWhiteSpace(_currentCommand))
            return;

        _commands.ExecuteCommand(_currentCommand);
    }

    private void UseItem(string itemName)
    {
        if (!_inventory.HasItem(itemName))
        {
            Console.WriteLine($"You don't have {itemName} in your inventory.");
            return;
        }

        var currentLocation = _map.GetCurrentLocation();
        
        // Check for special interactions
        if (currentLocation.Name == "Forgotten Harbor" && itemName == "compass")
        {
            Console.WriteLine("You place the compass in the circular indentation on the temple door.");
            Console.WriteLine("The symbols on the door begin to glow, and you hear ancient mechanisms turning...");
            Console.WriteLine("The massive door slowly swings open, revealing a path to the Temple Summit!");
            _map.UnlockLocation(new Vector2Int(1, 1));
        }
        else if (currentLocation.Name == "Temple Summit" && itemName == "compass" && _inventory.HasItem("crystal"))
        {
            EndGame();
        }
        else
        {
            Console.WriteLine($"Nothing special happens when you use {itemName} here.");
        }
    }

    private void CheckGameEnd()
    {
        var currentLocation = _map.GetCurrentLocation();
        if (currentLocation.Name == "Temple Summit" && _inventory.HasItem("compass") && _inventory.HasItem("crystal"))
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        Console.Clear();
        Console.WriteLine("\n=== The Heart of the Island ===\n");
        Console.WriteLine("As you hold both the compass and the crystal in the temple, they begin to resonate with each other.");
        Console.WriteLine("The temple's symbols burst into brilliant light, and ancient knowledge floods your mind.");
        Console.WriteLine("You've discovered the island's greatest secret - it's not just a place, but a gateway to ancient wisdom.");
        Console.WriteLine("Your journey here has changed you forever, but perhaps that was the true treasure all along.");
        Console.WriteLine("\nCongratulations! You've completed The Mysterious Island adventure!");
        Console.WriteLine("\nPress any key to return to the main menu...");
        Console.ReadKey();
        _isRunning = false;
        StartGame();
    }

    private void ClearGameState()
    {
        // Reset all game instances
        GenerateStartingInstances();
        _currentCommand = string.Empty;
        GC.Collect(); // Force garbage collection to clear memory
    }

    public void GiveExitCommand()
    {
        _isRunning = false;
        MainMenu();
    }
    #endregion
}
