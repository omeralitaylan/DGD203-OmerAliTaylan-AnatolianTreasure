/*
 * I started the project with the code build you provided. After adding some features to the game,
 * I started getting some errors and I used AI to solve them. (Claude Ai)
 */

using IslandAdventure.Structs;

namespace IslandAdventure;

public class MapLocations
{
    private readonly Dictionary<Vector2Int, Location> _locations;
    private readonly GameMap _gameMap;
    private readonly Game _game;
    private int _npcDialogueIndex = 0;

    public IReadOnlyDictionary<Vector2Int, Location> Locations => _locations;

    public MapLocations(GameMap gameMap)
    {
        _gameMap = gameMap;
        _game = gameMap._game;
        _locations = new Dictionary<Vector2Int, Location>
        {
            { new Vector2Int(0, 0), new Location(
                "Lost Cove",
                "The mist clings to the water's surface as waves crash against weathered rocks. A mysterious old sailor sits by a small campfire, his eyes reflecting wisdom of countless voyages.",
                "The old sailor beckons you closer.",
                new[] { new Item { Type = ItemType.Key, Name = "compass", Description = "An ornate brass compass that seems to have a life of its own." } },
                hasNPC: true,
                npcDialogues: new[] {
                    "'Ah, another seeker of mysteries,' he says with a knowing smile.",
                    "'The temple holds great power, but beware its guardians.'",
                    "'Remember, not all treasures are meant to be found.'",
                    "'Use the compass wisely, it points to more than just north.'"
                },
                npcDialogueOptions: new Dictionary<string, string[]>
                {
                    { "Tell me about the temple", new[] {
                        "The temple was built by an ancient civilization. They say it holds a power beyond imagination.",
                        "Many have tried to unlock its secrets, but few have returned to tell the tale.",
                        "The temple's guardians are not of this world. They test both courage and wisdom."
                    }},
                    { "What's so special about this compass?", new[] {
                        "This compass doesn't point north - it points to what your heart truly seeks.",
                        "I've seen it guide many travelers, each to a different destination.",
                        "Some say it was crafted by the same people who built the temple."
                    }},
                    { "Have you explored the island yourself?", new[] {
                        "Aye, I've walked these shores for longer than I care to remember.",
                        "The island... it changes. What you see today might not be there tomorrow.",
                        "I've learned to respect its mysteries, and so should you."
                    }}
                })
            },
            { new Vector2Int(0, 1), new Location(
                "Forgotten Harbor",
                "What once was a bustling fishing village now stands frozen in time. A massive iron door blocks the entrance to what appears to be an ancient storage room.",
                "The iron door is locked tight. Perhaps a key would help?",
                new[] { new Item { Type = ItemType.Currency, Name = "ancient_coin", Description = "A weathered coin bearing unfamiliar markings." } },
                requiresItem: true,
                requiredItemName: "rusty_key")
            },
            { new Vector2Int(1, 0), new Location(
                "Whispering Woods",
                "Gnarled trees with phosphorescent leaves cast an ethereal glow. A hooded figure stands among the trees, next to an old chest.",
                "The hooded figure turns to you.",
                new[] { new Item { Type = ItemType.Key, Name = "rusty_key", Description = "An old key, its surface covered in rust." } },
                hasNPC: true,
                npcDialogues: new[] {
                    "'The key you seek lies within this chest.'",
                    "'But first, a riddle: What has keys but no locks, space but no room, and you can enter but not go in?'",
                    "'Ah, a keyboard! You are wise. Take the key.'"
                })
            },
            { new Vector2Int(1, 1), new Location(
                "Temple Summit",
                "Ancient stone steps lead to a magnificent temple. The temple's entrance is sealed with mysterious symbols.",
                "The temple door has a slot that seems to match your compass perfectly.",
                new[] { new Item { Type = ItemType.Key, Name = "crystal", Description = "A crystal that pulses with mysterious energy." } },
                requiresItem: true,
                requiredItemName: "compass",
                isEndLocation: true)
            }
        };
    }

    public bool HasLocation(Vector2Int position)
    {
        return _locations.ContainsKey(position);
    }

    public string GetLocationName(Vector2Int position)
    {
        return _locations.TryGetValue(position, out var location) 
            ? location.Name 
            : "Unknown Location";
    }

    public string GetLocationDescription(Vector2Int position)
    {
        return _locations.TryGetValue(position, out var location)
            ? location.Description
            : "You shouldn't be here...";
    }

    public string GetNextNPCDialogue(Vector2Int position)
    {
        if (!_locations.TryGetValue(position, out var location) || !location.HasNPC)
            return string.Empty;

        if (_npcDialogueIndex >= location.NPCDialogues.Length)
            _npcDialogueIndex = 0;

        return location.NPCDialogues[_npcDialogueIndex++];
    }

    public bool TryInteract(Vector2Int position, Inventory inventory)
    {
        if (!_locations.TryGetValue(position, out var location))
            return false;

        if (location.RequiresItem)
        {
            if (!inventory.HasItem(location.RequiredItemName))
            {
                Console.WriteLine(location.InteractionText);
                return false;
            }

            if (location.IsEndLocation)
            {
                ShowEndingSequence();
                return true;
            }
        }

        if (location.HasNPC)
        {
            Console.WriteLine(GetNextNPCDialogue(position));
        }
        else
        {
            Console.WriteLine(location.InteractionText);
        }

        return true;
    }

    private void ShowEndingSequence()
    {
        Console.Clear();
        Console.WriteLine("\n=== The Temple's Secret ===\n");
        Console.WriteLine("As you place the compass into the temple door, the ground begins to shake.");
        Thread.Sleep(2000);
        Console.WriteLine("\nThe ancient symbols on the door illuminate one by one, casting an ethereal blue light.");
        Thread.Sleep(2000);
        Console.WriteLine("\nSlowly, the massive door slides open, revealing a chamber filled with advanced technology");
        Console.WriteLine("far beyond anything from 1875...");
        Thread.Sleep(2000);
        Console.WriteLine("\nIn the center stands a holographic projection showing Earth... and countless other worlds.");
        Thread.Sleep(2000);
        Console.WriteLine("\nYou realize the island wasn't just a mystery to be solved...");
        Console.WriteLine("It was a gateway left by an ancient civilization, waiting for humanity to find it.");
        Thread.Sleep(2000);
        Console.WriteLine("\nCongratulations! You've completed The Mysterious Island!");
        Console.WriteLine("\nPress any key to return to the main menu...");
        Console.ReadKey();
        _game.GiveExitCommand();
    }
}

public class Location
{
    public string Name { get; }
    public string Description { get; }
    public string InteractionText { get; }
    public Item[] Items { get; }
    public bool RequiresItem { get; }
    public string RequiredItemName { get; }
    public bool HasNPC { get; }
    public string[] NPCDialogues { get; }
    public Dictionary<string, string[]> NPCDialogueOptions { get; }
    public bool IsEndLocation { get; }

    public Location(string name, string description, string interactionText, Item[] items, 
                   bool requiresItem = false, string requiredItemName = "", 
                   bool hasNPC = false, string[] npcDialogues = null,
                   Dictionary<string, string[]> npcDialogueOptions = null,
                   bool isEndLocation = false)
    {
        Name = name;
        Description = description;
        InteractionText = interactionText;
        Items = items;
        RequiresItem = requiresItem;
        RequiredItemName = requiredItemName;
        HasNPC = hasNPC;
        NPCDialogues = npcDialogues ?? Array.Empty<string>();
        NPCDialogueOptions = npcDialogueOptions ?? new Dictionary<string, string[]>();
        IsEndLocation = isEndLocation;
    }
}
