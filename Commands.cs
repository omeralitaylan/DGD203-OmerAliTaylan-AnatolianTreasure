/*
 I asked you a question about the use of AI in the classroom. Before asking that question, I had made another much more detailed game with AI and another code structure, but after I asked you the question, I changed my mind and decided to cancel this game and create something with your structure.

I built the project on the code structure you gave. After some features I added to the game, I encountered errors and edited the project with Claude AI to fix these errors.
 */

namespace IslandAdventure;
using IslandAdventure.Structs;

public class Commands
{
    #region REFERENCES
    private Player _player;
    private Game _game;
    private GameMap _map;
    private SaveManager _saveManager;
    private Inventory _inventory;
    #endregion

    #region INITIALIZATION
    public Commands(Game game, GameMap map, SaveManager saveManager, Player player, Inventory inventory)
    {
        _game = game;
        _map = map ;
        _saveManager = saveManager;
        _player = player;
        _inventory = inventory;
    }
    #endregion

    #region METHODS
    public void ExecuteCommand(string command)
    {
        switch (command?.ToLower())
        {
            case "help":
                ShowHelp();
                break;
            case "who":
                PlayerWho();
                break;
            case "look":
                LookCommand();
                break;
            case "talk":
                TalkCommand();
                break;
            case "interact" or "int":
                InteractCommand();
                break;
            case "inventory" or "i":
                ShowInventory();
                break;
            case "east" or "e":
                MoveCommand(Direction.East);
                break;
            case "north" or "n":
                MoveCommand(Direction.North);
                break;
            case "west" or "w":
                MoveCommand(Direction.West);
                break;
            case "south" or "s":
                MoveCommand(Direction.South);
                break;
            case "exit":
                _game.GiveExitCommand();
                break;
            case "save":
                Console.WriteLine("Saving...");
                SaveGame();
                break;
            case "load":
                Console.WriteLine("Loading...");
                LoadGame();
                break;
            case "clear":
                Console.Clear();
                break;
            default:
                if (command?.StartsWith("choose ") == true)
                {
                    if (int.TryParse(command.Substring(7), out int choice))
                    {
                        HandleDialogueChoice(choice);
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice number.");
                    }
                }
                else
                {
                    Console.WriteLine("Unknown command. Type 'help' for available commands.");
                }
                break;
        }
    }

    public void ShowHelp()
    {
        Console.WriteLine("\nAvailable Commands:");
        Console.WriteLine("- look: Examine your surroundings");
        Console.WriteLine("- interact/int: Interact with the current location");
        Console.WriteLine("- north/n, south/s, east/e, west/w: Move in that direction");
        Console.WriteLine("- inventory/i: Check your inventory");
        Console.WriteLine("- who: Display your name");
        Console.WriteLine("- save: Save your progress");
        Console.WriteLine("- load: Load a saved game");
        Console.WriteLine("- exit: Return to main menu");
        Console.WriteLine("- clear: Clear the console screen");
        Console.WriteLine("- talk: Talk to someone in the current location");
    }

    private void PlayerWho()
    {
        Console.WriteLine($"You are {_player.Name}, an explorer seeking the secrets of the Mysterious Island!");
    }

    private void LookCommand()
    {
        var currentLocation = _map.GetCurrentLocation();
        if (currentLocation != null)
        {
            Console.WriteLine($"\n{currentLocation.Name}");
            Console.WriteLine(currentLocation.Description);
            
            if (currentLocation.HasNPC)
            {
                Console.WriteLine("\nThere is someone here you can talk to. Use 'talk' command to start a conversation.");
            }
        }
        else
        {
            Console.WriteLine("You are in an unknown location.");
        }
    }

    private void InteractCommand()
    {
        _map.InteractWithCurrentLocation();
    }

    private void ShowInventory()
    {
        Console.WriteLine("\n=== Your Inventory ===");
        _inventory.DisplayInventory();
    }

    public void MoveCommand(Direction direction)
    {
        var currentPosition = _map.GetPlayerPosition();
        var newPosition = direction switch
        {
            Direction.North => currentPosition with { Y = currentPosition.Y - 1 }, // Reversed Y coordinate
            Direction.South => currentPosition with { Y = currentPosition.Y + 1 }, // Reversed Y coordinate
            Direction.East => currentPosition with { X = currentPosition.X + 1 },
            Direction.West => currentPosition with { X = currentPosition.X - 1 },
            _ => currentPosition
        };

        if (!_map.IsValidPosition(newPosition))
        {
            var message = direction switch
            {
                Direction.North => "The thick jungle growth blocks your path northward, its vines and branches forming an impenetrable wall.",
                Direction.South => "Steep cliffs drop away to the churning sea below. You cannot proceed south.",
                Direction.East => "A wall of mist rises before you, so dense you can't see your hand in front of your face. Best not venture east.",
                Direction.West => "The shoreline curves sharply here, with treacherous rocks making westward travel impossible.",
                _ => "You cannot go that way."
            };
            Console.WriteLine($"\n{message}");
            return;
        }

        var directionText = direction switch
        {
            Direction.North => "northward, climbing higher into the island's mysteries",
            Direction.South => "southward, descending toward the whispering waves",
            Direction.East => "eastward, where the morning mist parts before you",
            Direction.West => "westward, following the ancient shoreline",
            _ => direction.ToString().ToLower()
        };

        Console.WriteLine($"\nYou journey {directionText}...");
        Thread.Sleep(1000);  // Brief pause for dramatic effect
        
        _map.MovePlayer(direction);
        
        // Show the new location using LookCommand
        LookCommand();
    }

    private void SaveGame()
    {
        Console.Clear();
        Console.WriteLine("\n=== Save Game ===");
        Console.WriteLine("Enter a name for your save file (or press Enter to cancel):");
        
        string saveName = Console.ReadLine();
        if (!string.IsNullOrEmpty(saveName))
        {
            Console.WriteLine("\nSaving your progress...");
            _saveManager.SaveGame(saveName);
            Console.WriteLine("Your progress has been saved. The island will remember your journey.\n");
        }
    }

    private void LoadGame()
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
        Console.WriteLine("\nEnter the number of the save to load (or press Enter to cancel):");
        
        string input = Console.ReadLine();
        if (string.IsNullOrEmpty(input)) return;

        if (int.TryParse(input, out int choice) && choice > 0 && choice <= saves.Length)
        {
            Console.WriteLine("\nLoading your previous expedition...");
            if (_saveManager.LoadGame(saves[choice - 1]))
            {
                Console.WriteLine($"\nWelcome back to {_map.GetCurrentLocation().Name}");
                Console.WriteLine(_map.GetCurrentLocation().Description);
            }
        }
        else
        {
            Console.WriteLine("Invalid choice.");
            Thread.Sleep(1000);
        }
    }

    private void TalkCommand()
    {
        var currentLocation = _map.GetCurrentLocation();
        if (currentLocation == null || !currentLocation.HasNPC)
        {
            Console.WriteLine("There is no one here to talk to.");
            return;
        }

        Console.WriteLine("\n" + currentLocation.InteractionText);
        
        if (currentLocation.NPCDialogueOptions.Count > 0)
        {
            Console.WriteLine("\nHow would you like to respond?");
            var options = currentLocation.NPCDialogueOptions.Keys.ToArray();
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i]}");
            }
            Console.WriteLine("\nUse 'choose <number>' to select your response.");
        }
        else
        {
            // Fallback to basic NPC dialogue if no options are available
            var dialogue = _map.GetNextNPCDialogue();
            if (!string.IsNullOrEmpty(dialogue))
            {
                Console.WriteLine("\n" + dialogue);
            }
        }
    }

    private void HandleDialogueChoice(int choice)
    {
        var currentLocation = _map.GetCurrentLocation();
        if (currentLocation == null || !currentLocation.HasNPC)
        {
            Console.WriteLine("There is no active conversation.");
            return;
        }

        var options = currentLocation.NPCDialogueOptions.Keys.ToArray();
        if (choice < 1 || choice > options.Length)
        {
            Console.WriteLine("Invalid choice number.");
            return;
        }

        string selectedOption = options[choice - 1];
        if (currentLocation.NPCDialogueOptions.TryGetValue(selectedOption, out string[] responses))
        {
            if (responses.Length > 0)
            {
                // Randomly select a response for variety
                var response = responses[new Random().Next(responses.Length)];
                Console.WriteLine($"\nYou: {selectedOption}");
                Console.WriteLine($"NPC: {response}");
            }
        }
    }
    #endregion
}
