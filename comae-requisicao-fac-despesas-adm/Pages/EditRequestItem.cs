using comae_requisicao_fac_despesas_adm.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace comae_requisicao_fac_despesas_adm.Pages
{
    internal static class EditRequestItem
    {
        private static string errorMessage = "";
        internal static void Print()
        {
            Console.Clear();
            Console.WriteLine(Index.title);
            Console.WriteLine("- Edição dos Itens da Requisição:\n");

            if (Request.requestItems.Count == 0)
            {
                Console.WriteLine("Nenhum item disponível.");
                Console.WriteLine("\nPressione qualquer tecla para voltar.");
                Console.ReadKey();
                EditRequest.Print();
                return;
            }

            Request.PrintRequestItemsTable();

            Console.WriteLine("\n0. Retornar à Edição da Solicitação");

            Input(); // Handle item selection
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

                if (choice >= 1 && choice <= Request.requestItems.Count)
                {
                    EditItem(choice - 1); // -1 to adjust for 0-based index
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

                if (int.TryParse(input, out int selectedItem) && selectedItem >= 1 && selectedItem <= Request.requestItems.Count)
                {
                    EditItem(selectedItem - 1); // -1 to adjust for 0-based index
                }
                else
                {
                    Print(); // Invalid choice, prompt again
                }
            }
        }

        internal static void EditItem(int itemIndex)
        {
            RequestItem requestItem = Request.requestItems[itemIndex];
            Console.Clear();
            Console.WriteLine(Index.title);
            Console.WriteLine($"- Editando Item {itemIndex + 1}:\n");

            int index = 1;
            foreach (var entry in requestItem.Data)
            {
                Console.WriteLine($"{index}. {entry.Key}: {entry.Value.Value}");
                index++;
            }

            Console.WriteLine("\n0. Retornar à lista de itens");

            FieldSelection(itemIndex); // Handle field selection
        }

        internal static void FieldSelection(int itemIndex)
        {
            RequestItem requestItem = Request.requestItems[itemIndex];

            if (Requester.Data.Count < 10)
            {
                Console.Write("\nEscolha um número para editar ou 0 para retornar: ");
                char input = Console.ReadKey(true).KeyChar; // Read a single character without requiring Enter

                if (input == '0')
                {
                    Print(); // Return to the list of items
                    return;
                }

                int choice = (int)char.GetNumericValue(input); // Convert char to numeric value

                if (choice >= 1 && choice <= requestItem.Data.Count)
                {
                    EditField(itemIndex, choice - 1); // -1 to adjust for 0-based index
                }
                else
                {
                    FieldSelection(itemIndex); // Invalid choice, prompt again
                    return;
                }
            }
            else
            {
                // If 10 or more options, use full-line input
                Console.Write("\nDigite o número para editar ou 0 para retornar: ");
                string input = Console.ReadLine();

                if (input == "0")
                {
                    Print(); // Return to the list of items
                    return;
                }

                if (int.TryParse(input, out int selectedField) && selectedField >= 1 && selectedField <= requestItem.Data.Count)
                {
                    EditField(itemIndex, selectedField - 1); // -1 to adjust for 0-based index
                }
                else
                {
                    FieldSelection(itemIndex); // Invalid choice, prompt again
                    return;
                }
            }
        }

        internal static void EditField(int itemIndex, int fieldIndex)
        {
            RequestItem requestItem = Request.requestItems[itemIndex];
            Console.Clear();
            Console.WriteLine(Index.title);
            Console.WriteLine($"- Editando Item {itemIndex + 1}:\n");

            if (errorMessage.Length > 0)
            {
                Console.WriteLine(errorMessage + "\n");
                errorMessage = "";
            }

            string key = new List<string>(requestItem.Data.Keys)[fieldIndex];
            var field = requestItem.Data[key];

            // FreeText input, including handling empty strings
            if (field.Type == FieldType.FreeText)
            {
                Console.WriteLine($"Editando '{key}':");
                Console.WriteLine($"Valor atual: {field.Value}");
                Console.Write("Novo valor (pressione Enter para deixar vazio): ");
                string newValue = Console.ReadLine(); // Allow empty string
                requestItem.Data[key] = (newValue ?? "", FieldType.FreeText); // Update the value
            }
            else if (field.Type == FieldType.RequiredText)
            {
                Console.WriteLine($"Editando '{key}':");
                Console.WriteLine($"Valor atual: {field.Value}");
                Console.Write("Novo valor (campo obrigatório): ");
                string newValue = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(newValue))
                {
                    requestItem.Data[key] = (newValue, FieldType.RequiredText);
                }
                else
                {
                    EditField(itemIndex, fieldIndex); // Retry on invalid input
                    return;
                }
            }
            else if (field.Type == FieldType.Selectable)
            {
                Console.WriteLine($"Editando '{key}':");
                var options = RequestItem.SelectableOptions[key];

                if (options.Count < 10)
                {
                    // Single char input for less than 10 options
                    for (int i = 0; i < options.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {options[i]}");
                    }

                    Console.Write("\nEscolha uma opção: ");
                    char inputChar = Console.ReadKey(true).KeyChar;

                    if (int.TryParse(inputChar.ToString(), out int selectedOption) && selectedOption >= 1 && selectedOption <= options.Count)
                    {
                        requestItem.Data[key] = (options[selectedOption - 1], FieldType.Selectable);
                    }
                    else
                    {
                        EditField(itemIndex, fieldIndex); // Retry on invalid input
                        return;
                    }
                }
                else
                {
                    // Full-line input for 10 or more options
                    for (int i = 0; i < options.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {options[i]}");
                    }

                    Console.Write("\nEscolha uma opção (1-{0}): ", options.Count);
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out int selectedOption) && selectedOption >= 1 && selectedOption <= options.Count)
                    {
                        requestItem.Data[key] = (options[selectedOption - 1], FieldType.Selectable);
                    }
                    else
                    {
                        errorMessage = ("Opção inválida, tente novamente.");
                        EditField(itemIndex, fieldIndex); // Retry on invalid input
                        return;
                    }
                }
            }
            else if (field.Type == FieldType.Currency)
            {
                Console.WriteLine($"Editando '{key}':");
                Console.WriteLine($"Valor atual: {field.Value}");
                Console.Write($"{key} (use ',' como separador decimal): ");
                string input = Console.ReadLine();

                if (Regex.IsMatch(input, @"^\d+,\d{2}$"))
                {
                    requestItem.Data[key] = (input, FieldType.Currency);
                }
                else
                {
                    errorMessage = ("Formato inválido. Por favor, insira um valor com vírgula como separador decimal (ex: 1234,56).");
                    EditField(itemIndex, fieldIndex); // Retry on invalid input
                    return;
                }
            }
            else if (field.Type == FieldType.Phone)
            {
                Console.WriteLine($"Editando '{key}':");
                Console.WriteLine($"Valor atual: {field.Value}");
                Console.Write($"{key} (formato: (00) 00000-0000 ou (00) 0000-0000): ");
                string input = Console.ReadLine();

                // Remove non-digit characters
                string digitsOnly = Regex.Replace(input, @"\D", "");

                if (digitsOnly.Length == 10) // Format as (00) 0000-0000
                {
                    string formattedPhone = $"({digitsOnly.Substring(0, 2)}) {digitsOnly.Substring(2, 4)}-{digitsOnly.Substring(6, 4)}";
                    requestItem.Data[key] = (formattedPhone, FieldType.Phone);
                }
                else if (digitsOnly.Length == 11) // Format as (00) 00000-0000
                {
                    string formattedPhone = $"({digitsOnly.Substring(0, 2)}) {digitsOnly.Substring(2, 5)}-{digitsOnly.Substring(7, 4)}";
                    requestItem.Data[key] = (formattedPhone, FieldType.Phone);
                }
                else
                {
                    errorMessage = ("Número de telefone inválido. Insira um número com 8 ou 9 dígitos.");
                    EditField(itemIndex, fieldIndex); // Retry on invalid input
                    return;
                }
            }

            Request.modified = true;
            EditItem(itemIndex); // After editing, return to the list of fields for the selected item
        }
    }
}
