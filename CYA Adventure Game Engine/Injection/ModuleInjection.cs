using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.Injection
{
    public static class ModuleInjection
    {
        public static IServiceCollection AddBaseModules(this IServiceCollection services)
        {
            services.AddSingleton<IModule, Dice>();
            services.AddSingleton<IModule, Inventory>();
            services.AddSingleton<IModule, Sound>();
            return services;
        }

        public static IServiceCollection AddExternalModules(this IServiceCollection services)
        {
            // Gets location that the exe is currently within.
            var location = AppDomain.CurrentDomain.BaseDirectory;
            // "**/modules/**/*.dll"
            /*
             * ** = go any # dirs deep
             * modules = match on modules folder
             * .dll = matches on the dll files.
             */
            var moduleFiles = Directory.GetFiles(location, "**/modules/**/*.dll");
            foreach (var file in moduleFiles)
            {
                // Load the assembly to get access to the types.
                var ass = Assembly.LoadFrom(file);
                if (ass is not null) 
                { 
                    // Filter ass to leave just the IModules.
                    var types = ass.ExportedTypes.Where(a => a.GetInterface(nameof(IModule)) is not null);
                    foreach (var type in types) 
                    { 
                        // Generic adder as unknown details prior to runtime.
                        services.AddSingleton(typeof(IModule), type);
                    }
                }
                else
                {
                    // put warning about unfound ass here
                }
            }

            return services;
        }
    }
}
