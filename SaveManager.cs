/*
 * I started the project with the code build you provided. After adding some features to the game,
 * I started getting some errors and I used AI to solve them. (Claude Ai)
 */

using IslandAdventure.Structs;
using System.IO;
using System.Text.Json;

namespace IslandAdventure;

public class SaveManager
{
    private readonly Game _game;
    private Player _player;
    private GameMap _map;
    private Inventory _inventory;
    private const string SaveFileExtension = ".sgf";
    private const string SaveDirectory = "saves";
    private const string SaveSeparator = "-";

    public SaveManager(Game game)
    {
        _game = game;
        EnsureSaveDirectoryExists();
    }

    private void EnsureSaveDirectoryExists()
    {
        if (!Directory.Exists(SaveDirectory))
        {
            Directory.CreateDirectory(SaveDirectory);
        }
    }

    public void Initialize(Player player, GameMap map, Inventory inventory)
    {
        _player = player;
        _map = map;
        _inventory = inventory;
    }

    public bool SaveFileExists(string saveName = "")
    {
        if (string.IsNullOrEmpty(saveName))
        {
            return Directory.GetFiles(SaveDirectory, $"*{SaveFileExtension}").Length > 0;
        }
        return File.Exists(GetSaveFilePath(saveName));
    }

    private string GetSaveFilePath(string saveName)
    {
        return Path.Combine(SaveDirectory, $"{saveName}{SaveFileExtension}");
    }

    public string[] ListSaveFiles()
    {
        return Directory.GetFiles(SaveDirectory, $"*{SaveFileExtension}")
            .Select(Path.GetFileNameWithoutExtension)
            .ToArray();
    }

    public void SaveGame(string saveName)
    {
        try
        {
            var position = _map.GetPlayerPosition();
            var items = _inventory.GetItems();
            
            // Convert inventory items to a simple string format
            var itemLines = items.Select(item => $"{item.Name}:{item.Description}").ToList();
            
            var saveData = new List<string>
            {
                _player.Name,
                $"{position.X},{position.Y}",
                SaveSeparator
            };
            
            // Add inventory items after the separator
            saveData.AddRange(itemLines);

            File.WriteAllLines(GetSaveFilePath(saveName), saveData);
            Console.WriteLine($"Game saved successfully as '{saveName}'!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save game: {ex.Message}");
        }
    }

    public bool LoadGame(string saveName)
    {
        try
        {
            string saveFilePath = GetSaveFilePath(saveName);
            if (!File.Exists(saveFilePath))
            {
                Console.WriteLine($"Save file '{saveName}' not found.");
                return false;
            }

            var saveData = File.ReadAllLines(saveFilePath);
            if (saveData.Length >= 3) // At least name, position, and separator
            {
                _player.SetUp(saveData[0]);
                var position = ParseToVector2Int(saveData[1]);
                _map.SetPlayerPosition(position);

                // Clear current inventory
                _inventory.SetItems(new List<Item>());

                // Find separator index
                int separatorIndex = Array.IndexOf(saveData, SaveSeparator);
                if (separatorIndex >= 0 && separatorIndex < saveData.Length - 1)
                {
                    // Load inventory items
                    for (int i = separatorIndex + 1; i < saveData.Length; i++)
                    {
                        string itemLine = saveData[i];
                        if (!string.IsNullOrEmpty(itemLine))
                        {
                            var parts = itemLine.Split(':');
                            if (parts.Length == 2)
                            {
                                _inventory.AddItem(new Item { Name = parts[0], Description = parts[1] });
                            }
                        }
                    }
                }

                Console.WriteLine($"Game '{saveName}' loaded successfully!");
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load game: {ex.Message}");
            return false;
        }
    }

    private Vector2Int ParseToVector2Int(string positionString)
    {
        var parts = positionString.Split(',');
        if (parts.Length == 2 && 
            int.TryParse(parts[0], out int x) && 
            int.TryParse(parts[1], out int y))
        {
            return new Vector2Int(x, y);
        }
        return new Vector2Int(0, 0);
    }
}
