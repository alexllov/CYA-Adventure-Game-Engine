using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.Modules
{
    public class OldInventory : IModule
    // Use to create inventories for players / other entities.
    {
        private List<string> Items;

        public OldInventory()
        {
            Items = new List<string>();
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", Items)}]";
        }

        private void Add(string item)
        // Add item to inventory.
        {
            Items.Add(item);
        }

        private void Remove(string item)
        // Remove item from inventory
        {
            Items.Remove(item);
        }

        // Verify if the requested action can be completed.
        // -> tuple (true/false, "error msg")
        (bool, string) Query(string method, List<string> body)
        {
            switch (method)
            {
                // Pass by default - no conditions.
                case "add":
                    return (true, "");

                // TODO
                // Check items.
                case "remove":
                case "need":
                    List<string> missing = new List<string>();
                    foreach (var item in body)
                    {
                        if (!Items.Contains(item)) {
                            missing.Add(item);
                        }
                    }
                    if (missing.Count is not 0)
                    {
                        return (false, $"You cannot take this action. Inventory missing {string.Join(", ", missing)}.");
                    }
                    else
                    {
                        return (true, "");
                    }

                // Method Error.
                default:
                    return (false, $"Method error. {method} not recognised in Inventory.cs");
            }
        }

        (bool, string) Process(string method, List<string> body)
        {
            //var queryCheck = Query(method, body);
            //if (queryCheck.Item1) {}
                switch (method)
                {
                    case "add":
                        foreach (var item in body)
                        {
                            Add(item);
                        }
                        break;

                    case "remove":
                        foreach (var item in body)
                        {
                            Remove(item);
                        }
                        break;

                    case "need":
                        break;

                    default:
                        break;
                }
            
            return (true, "");
        }
    }
}