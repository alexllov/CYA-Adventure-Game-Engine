namespace CYA_Adventure_Game_Engine.Modules
{
    public class Inventory : IModule, IInstantiable<Inventory>, IEnumerable<object>
    {
        private List<object> Items;

        public Inventory()
        {
            Items = new List<object>();
        }

        public Inventory Make() { return new Inventory(); }

        public override string ToString()
        {
            return $"[{string.Join(", ", Items)}]";
        }

        public void Add(object body)
        // Add item to inventory.
        {
            Items.Add(body);
            //foreach (string item in body)
            //{
            //    Items.Add(item);
            //}
        }

        public void Remove(object body)
        // Remove item from inventory
        {
            Items.Remove(body);
            //foreach (string item in body)
            //{
            //    Items.Remove(item);
            //}
        }

        public bool Check(object body)
        {
            if (!(Items.Contains(body))) { return false; } else { return true; }
            //foreach (string item in body)
            //{
            //    if (!(Items.Contains(item))) { return false; }
            //}
            //return true;
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
