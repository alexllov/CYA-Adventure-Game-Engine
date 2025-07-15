using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.Modules
{
    internal class Inventory
    {
        private List<string> Items;

        public Inventory()
        {
            Items = new List<string>();
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", Items)}]";
        }

        private void Add(string[] body)
        // Add item to inventory.
        {
            foreach (string item in body)
            {
                Items.Add(item);
            }
        }

        private void Remove(string[] body)
        // Remove item from inventory
        {
            foreach (string item in body)
            {
                Items.Remove(item);
            }
        }

        private bool Check(string[] body)
        {
            foreach (string item in body)
            {
                if (!(Items.Contains(item))) { return false; };
            }
            return true;
        }
    }
}
