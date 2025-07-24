using CYA_Adventure_Game_Engine;
namespace External_Modules.Battle_Library
{
    public class Battle : IModule
    {
        public Battle() { }

        /// <summary>
        /// Runs the core battle loop. Will return 1 of 3 strings:
        /// "ran" = player successfully ran away.
        /// "won" = player won the battle.
        /// "lost" = player lost the battle.
        /// 
        /// </summary>
        /// <param name="player">CombatPlayer object</param>
        /// <param name="enemy">Enemy object</param>
        /// <returns></returns>
        public string Trigger(CombatPlayer player, Enemy enemy)
        {
            Console.WriteLine($"A wild {enemy.Name} appears! ({enemy.Stat}: {enemy.StatValue})");
            Console.WriteLine("Options:\n1. Fight!\n2. Run!!!");
            bool wonBattleFlag = false;
            while (true)
            {
                Console.Write("Enter choice: ");
                string? choice = Console.ReadLine();
                if (choice is not null
                    && int.TryParse(choice, out int i)
                    && (i == 1 || i == 2))
                {
                    if (i == 1)
                    {
                        wonBattleFlag = HandleFight(player, enemy);
                        if (wonBattleFlag)
                        {
                            HandleVictory(player, enemy);
                            return "won";
                        }
                        else
                        {
                            HandleDefeat(player, enemy);
                            return "lost";
                        }
                    }
                    // Attempt to run away.
                    else if (i == 2)
                    {
                        bool run = AttemptRunAway(player, enemy);
                        if (!run)
                        {
                            Console.WriteLine($"The {enemy.Name} is too strong, you can't run.");
                        }
                        // Successfully ran away.
                        else
                        {
                            Console.WriteLine($"You successfully ran away from the {enemy.Name}!");
                            return "ran";
                        }
                    }
                }
                else { Console.WriteLine("Invalid choice."); }
            }
        }

        private bool HandleFight(CombatPlayer player, Enemy enemy)
        {
            Console.WriteLine("You chose to fight!");
            string battleType = enemy.Stat;
            // Present the user's weapons for them to choose how they want to attack.
            List<Weapon> allPlayerWeapons = [.. player.Inventory.OfType<Weapon>()];
            List<Weapon> battleAppropriate = [new Weapon("fists", "1d4", 0, "strength")];
            battleAppropriate = [.. battleAppropriate.Concat(allPlayerWeapons.Where(w => w.Stat == battleType))];
            string text = "";
            for (int i = 0; i < battleAppropriate.Count; i++)
            {
                text += $"{i + 1}. {battleAppropriate[i]}\n";
            }
            Console.WriteLine(text);
            Weapon? playerWeapon;
            while (true)
            {
                Console.Write("Choose one of your weapons: ");
                string? choice = Console.ReadLine();
                // Check: made a choice, is number, & within range.
                if (choice is not null
                    && int.TryParse(choice, out int i)
                    && i > 0
                    && i <= battleAppropriate.Count)
                {
                    playerWeapon = battleAppropriate[i - 1];
                    break;
                }
                else { Console.WriteLine("Invalid choice."); }

            }

            // Run player's attack.
            int playerDamage;
            if (battleType == "strength")
            {
                playerDamage = playerWeapon.Run() + player.Strength;
            }
            else
            {
                playerDamage = playerWeapon.Run() + player.Arcane;
            }
            Console.WriteLine($"You attack the {enemy.Name} for a total of {playerDamage} points.");


            // Run the enemy's attack.
            (string enemyAttackText, int enemyDamage) = enemy.Attack.Run();
            Console.WriteLine($"The {enemy.Name} {enemyAttackText} It does {enemyDamage} damage.");

            return playerDamage > enemyDamage;
        }


        private void HandleDefeat(CombatPlayer player, Enemy enemy)
        {
            Console.WriteLine($"You were defeated by the {enemy.Name}. You lose 1 life. The {enemy.Name} loses interest and walks off.");
            player.Health -= 1;
        }

        /// <summary>
        /// On victory, the player can gain experience OR take the enemy's weapon - if its an obtainable weapon.
        /// </summary>
        private void HandleVictory(CombatPlayer player, Enemy enemy)
        {
            Console.WriteLine($"You defeated the {enemy.Name}!");
            List<string> choices = [$"1. Gain {enemy.StatValue} experience."];
            if (Weapons.WeaponList.TryGetValue(enemy.Attack.Weapon.Name, out Weapon? enemyWeapon))
            {
                choices.Add($"2. Take the enemy's weapon: {enemyWeapon}.");
            }
            if (choices.Count == 1)
            {
                player.Experience += enemy.StatValue;
                Console.WriteLine($"You gain {enemy.StatValue} experience points.");
                return;
            }
            else
            {
                Console.WriteLine(string.Join("\n", choices));
                while (true)
                {
                    Console.Write("Choose an option: ");
                    string? choice = Console.ReadLine();
                    if (choice is not null
                        && int.TryParse(choice, out int i)
                        && (i == 1 || i == 2))
                    {
                        if (i == 1)
                        {
                            player.Experience += enemy.StatValue;
                            Console.WriteLine($"You gain {enemy.StatValue} experience points.");
                            return;
                        }
                        else if (i == 2)
                        {
                            player.Inventory.Add(enemyWeapon!);
                            Console.WriteLine($"You take the {enemyWeapon!.Name}.");
                            return;
                        }
                    }
                    else { Console.WriteLine("Invalid choice."); }
                }
            }
        }

        private bool AttemptRunAway(CombatPlayer player, Enemy enemy)
        {
            int enemyStatValue = enemy.StatValue;
            string enemyStat = enemy.Stat;
            int playerStatValue;
            if (enemyStat == "strength") { playerStatValue = player.Strength; }
            else if (enemyStat == "arcane") { playerStatValue = player.Arcane; }
            else { throw new Exception("Unknown enemy stat type."); }

            return playerStatValue > enemyStatValue;
        }
    }
}
