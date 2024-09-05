using comae_requisicao_fac_despesas_adm.Models;
using System;
using System.Collections.Generic;

namespace comae_requisicao_fac_despesas_adm.Pages
{
    internal static class EditRequesterInfo
    {
        internal static void DisplayRequesterInfo()
        {
            // Display the current values for each field with line numbers
            int index = 1;
            foreach (var entry in Requester.Data)
            {
                Console.WriteLine($"{index}. {entry.Key}: {entry.Value.Value}");
                index++;
            }
        }

        internal static void Print()
        {
            Console.Clear();
            Console.WriteLine(Index.title);
            Console.WriteLine("- Edição dos dados do solicitante:\n");

            DisplayRequesterInfo();

            Console.WriteLine("\n0. Retornar à Edição da Solicitação");

            Input();
        }

        internal static void Input()
        {
            if (Requester.Data.Count < 10)
            {
                Console.Write("\nEscolha um número para editar ou 0 para retornar: ");
                char input = Console.ReadKey(true).KeyChar; // Read a single character without requiring Enter

                if (input == '0')
                {
                    EditRequest.Print(); // Return to EditRequest page
                    return;
                }

                int choice = (int)char.GetNumericValue(input); // Convert char to numeric value

                if (choice >= 1 && choice <= Requester.Data.Count)
                {
                    // Edit the selected field
                    EditField(choice - 1); // -1 to adjust for 0-based index
                }
                else
                {
                    Print(); // Invalid choice, prompt again
                }
            }
            else
            {
                // If 10 or more options, use full-line input
                Console.Write("\nDigite o número para editar ou 0 para retornar: ");
                string input = Console.ReadLine();

                if (input == "0")
                {
                    EditRequest.Print(); // Return to EditRequest page
                    return;
                }

                if (int.TryParse(input, out int choice) && choice >= 1 && choice <= Requester.Data.Count)
                {
                    // Edit the selected field
                    EditField(choice - 1); // -1 to adjust for 0-based index
                }
                else
                {
                    Print(); // Invalid choice, prompt again
                }
            }
        }

        internal static void EditField(int fieldIndex)
        {
            Console.Clear();
            Console.WriteLine(Index.title);
            Console.WriteLine("- Edição dos dados do solicitante:\n");

            string key = new List<string>(Requester.Data.Keys)[fieldIndex]; // Get the key for the selected field
            var field = Requester.Data[key];

            // Prompt for editing based on field type
            if (field.Type == FieldType.FreeText)
            {
                Console.WriteLine($"Editando '{key}':");
                Console.WriteLine($"Valor atual: {field.Value}");
                Console.Write("Novo valor (pressione Enter para manter o valor atual): ");
                string newValue = Console.ReadLine();

                // If the user presses enter without typing anything, keep the old value
                if (!string.IsNullOrWhiteSpace(newValue))
                {
                    Requester.Data[key] = (newValue, FieldType.FreeText, Requester.Data[key].Preview, Requester.Data[key].Description);
                }
            }
            else if (field.Type == FieldType.Selectable)
            {
                Console.WriteLine($"Editando '{key}':");
                var options = Requester.SelectableOptions[key];

                // Display selectable options
                for (int i = 0; i < options.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {options[i]}");
                }

                // Handle input based on number of options
                if (options.Count < 10)
                {
                    // Read char input for fewer than 10 options
                    Console.Write("\nEscolha uma opção (1-{0}): ", options.Count);
                    char inputChar = Console.ReadKey(true).KeyChar; // Read key without showing it

                    int selectedOption = (int)char.GetNumericValue(inputChar); // Convert char to number
                    if (selectedOption >= 1 && selectedOption <= options.Count)
                    {
                        Requester.Data[key] = (options[selectedOption - 1], FieldType.Selectable, Requester.Data[key].Preview, Requester.Data[key].Description); // Update the value
                    }
                    else
                    {
                        Console.WriteLine("Opção inválida, tente novamente.");
                        EditField(fieldIndex); // Retry on invalid input
                        return;
                    }
                }
                else
                {
                    // Read full-line input for 10 or more options
                    Console.Write("\nEscolha uma opção (1-{0}): ", options.Count);
                    string inputString = Console.ReadLine();

                    if (int.TryParse(inputString, out int selectedOption) && selectedOption >= 1 && selectedOption <= options.Count)
                    {
                        Requester.Data[key] = (options[selectedOption - 1], FieldType.Selectable, Requester.Data[key].Preview, Requester.Data[key].Description); // Update the value
                    }
                    else
                    {
                        Console.WriteLine("Opção inválida, tente novamente.");
                        EditField(fieldIndex); // Retry on invalid input
                        return;
                    }
                }
            }

            // After editing, return to the list of requester data
            Request.modified = true;
            Print();
        }
    }
}
