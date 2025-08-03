using CYA_Adventure_Game_Engine.Modules;
using System.Reflection;

namespace CYA_Adventure_Game_Engine.Injection
{
    public static class ModuleInjection
    {
        private static Dictionary<string, IModule> AddBaseModules(this Dictionary<string, IModule> modules)
        {
            modules["dice"] = new Dice();
            modules["inventory"] = new Inventory();
            modules["sound"] = new Sound();
            modules["baseplayer"] = new BasePlayer();
            return modules;
        }

        private static Dictionary<string, IModule> AddExternalModules(this Dictionary<string, IModule> modules)
        {
            // Gets location that the exe is currently within.
            string location = AppDomain.CurrentDomain.BaseDirectory;
            // "**/modules/**/*.dll"
            /*
             * ** = go any # dirs deep
             * modules = match on modules folder
             * .dll = matches on the dll files.
             */
            string[] moduleFiles = Directory.GetFiles(location, "modules\\*.dll");
            foreach (var file in moduleFiles)
            {
                // Load the assembly to get access to the types.
                Assembly ass = Assembly.LoadFrom(file);
                if (ass is not null)
                {
                    // Filter ass to leave just the IModules.
                    var types = ass.ExportedTypes.Where(a => a.GetInterface(nameof(IModule)) is not null);
                    if (types.Any())
                    {
                        // TODO: Consider changing this s.t. IInstantiables are treated separatly from static.
                        // so to allow things that don't need to be instantiated to avoid it.
                        foreach (var type in types)
                        {
                            if (type is IStatic)
                            {
                                modules[type.Name.ToLower()] = (IModule)type;
                            }
                            else
                            {
                                IModule instanciatedModule = (IModule)Activator.CreateInstance(type);
                                modules[type.Name.ToLower()] = instanciatedModule;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No suitable modules found in {file}.");
                    }
                }
                else
                {
                    Console.WriteLine("Found file in modules folder that could not be loaded.");
                }
            }
            return modules;
        }

        public static Dictionary<string, IModule> LoadModules()
        {
            Dictionary<string, IModule> modules = new();
            modules = modules.AddBaseModules()
                             .AddExternalModules();
            return modules;
        }
    }
}
