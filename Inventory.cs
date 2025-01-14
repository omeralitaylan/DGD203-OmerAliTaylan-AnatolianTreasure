/*
 * I started the project with the code build you provided. After adding some features to the game,
 * I started getting some errors and I used AI to solve them. (Claude Ai)
 */

using IslandAdventure.Structs;

namespace IslandAdventure;

public enum ItemType
{
    Key,
    Currency
}

public class Inventory
{
    private readonly Game _game;
    private readonly List<Item> _items;

    public Inventory(Game game)
    {
        _game = game;
        _items = new List<Item>();
    }

    public Inventory()
    {
        _items = new List<Item>();
    }

    public void AddItem(Item item)
    {
        if (!_items.Any(i => i.Name == item.Name))
        {
            _items.Add(item);
            Console.WriteLine($"Added {item.Name} to your inventory.");
        }
    }

    public bool HasItem(string itemName)
    {
        return _items.Any(i => i.Name == itemName);
    }

    public void RemoveItem(string itemName)
    {
        var item = _items.FirstOrDefault(i => i.Name == itemName);
        if (item.Name != null && _items.Remove(item))
        {
            Console.WriteLine($"Removed {item.Name} from your inventory.");
        }
    }

    public void DisplayInventory()
    {
        if (_items.Count == 0)
        {
            Console.WriteLine("Your inventory is empty.");
            return;
        }

        Console.WriteLine("\nYour inventory contains:");
        foreach (var item in _items)
        {
            Console.WriteLine($"- {item.Name}: {item.Description}");
        }
    }

    public List<Item> GetItems()
    {
        return new List<Item>(_items);
    }

    public void SetItems(List<Item> items)
    {
        _items.Clear();
        _items.AddRange(items);
    }

    public void TakeItem(string itemName)
    {
        var currentLocation = _game.Map.GetCurrentLocation();
        var item = currentLocation.Items.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
        
        if (item.Equals(default(Item)))
        {
            Console.WriteLine($"There is no {itemName} here to take.");
            return;
        }

        if (HasItem(item.Name))
        {
            Console.WriteLine($"You already have the {itemName}.");
            return;
        }

        AddItem(item);
        Console.WriteLine($"You took the {itemName}.");
    }
}
