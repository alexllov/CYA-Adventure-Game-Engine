using Newtonsoft.Json;
namespace CYA_Adventure_Game_Engine.Modules
{
    [JsonObject]
    public class Inventory : IModule, IInstantiable<Inventory>, IEnumerable<object>
    {
        public List<object> Items;

        public Inventory()
        {
            Items = [];
        }

        public Inventory Make() { return new Inventory(); }

        public override string ToString()
        {
            return $"[{string.Join(", ", Items)}]";
        }

        public void Add(object item)
        // Add item to inventory.
        {
            Items.Add(item);
        }

        public void Remove(object body)
        // Remove item from inventory
        {
            Items.Remove(body);
        }

        public bool Check(object body)
        {
            if (!(Items.Contains(body))) { return false; } else { return true; }
        }

        public List<object> All()
        {
            return Items;
        }

        public IEnumerator<object> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
