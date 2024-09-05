using comae_requisicao_fac_despesas_adm.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace comae_requisicao_fac_despesas_adm.Pages
{
    internal static class NewRequestItem
    {
        // Input method to edit request item data
        internal static void Input()
        {
            RequestItem requestItem = new RequestItem();
            Console.Clear();

            // Iterate over the fields in the request item
            foreach (var key in new List<string>(requestItem.Data.Keys))
            {
                bool validInput = false; // Flag for valid input
                string errorMessage = string.Empty;
                while (!validInput)
                {
                    Console.Clear();
                    Console.WriteLine(Index.title);
                    Console.WriteLine("- Inserir Despesa Administrativa:\n");
                    Console.WriteLine("Pressione ESC para cancelar a operação.\n");

                    var field = requestItem.Data[key];

                    if (RequestItem.DataTemplate[key].Description != "")
                    {
                        Console.WriteLine(RequestItem.DataTemplate[key].Description + "\n");
                    }

                    if (errorMessage.Length > 0)
                    {
                        Console.WriteLine(errorMessage + "\n");
                        errorMessage = "";
                    }

                    // Detect ESC key for cancellation
                    if (Console.KeyAvailable)
                    {
                        var keyPressed = Console.ReadKey(true);

                        if (keyPressed.Key == ConsoleKey.Escape)
                        {
                            Console.WriteLine("Operação cancelada.");
                            EditRequest.Print(); // Return to the EditRequest page
                            return; // Exit method
                        }
                    }

                    if (field.Type == FieldType.FreeText)
                    {
                        // Handle free text input
                        Console.Write($"{key}: ");
                        string newValue = Console.ReadLine();
                        requestItem.Data[key] = (newValue, FieldType.FreeText); // Update the field value
                        validInput = true; // Free text is always valid
                    }
                    else if (field.Type == FieldType.RequiredText)
                    {
                        // Handle required text input
                        Console.Write($"{key} (campo obrigatório): ");
                        string newValue = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(newValue)) // Ensure the input is not empty or whitespace
                        {
                            requestItem.Data[key] = (newValue, FieldType.RequiredText);
                            validInput = true; // Input is valid
                        }
                        else
                        {
                            errorMessage = "Este campo é obrigatório. Por favor, insira um valor.";
                        }
                    }
                    else if (field.Type == FieldType.Selectable)
                    {
                        // Handle selectable options
                        Console.WriteLine($"{key}: ");
                        var options = RequestItem.SelectableOptions[key];

                        // If there are less than 10 options, use single char input
                        if (options.Count < 10)
                        {
                            for (int i = 0; i < options.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {options[i]}");
                            }

                            char input = Console.ReadKey(true).KeyChar;

                            // Detect ESC key during selection
                            if (input == (char)ConsoleKey.Escape)
                            {
                                Console.WriteLine("Operação cancelada.");
                                EditRequest.Print(); // Return to the EditRequest page
                                return; // Exit method
                            }

                            int selectedOption = (int)char.GetNumericValue(input);

                            if (selectedOption >= 1 && selectedOption <= options.Count)
                            {
                                requestItem.Data[key] = (options[selectedOption - 1], FieldType.Selectable);
                                validInput = true; // Set flag to true when valid input is received
                            }
                        }
                        else
                        {
                            // If there are 10 or more options, use full-line input
                            for (int i = 0; i < options.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {options[i]}");
                            }

                            string input = Console.ReadLine();

                            // Detect ESC key during full-line input
                            if (input == ((char)ConsoleKey.Escape).ToString())
                            {
                                Console.WriteLine("Operação cancelada.");
                                EditRequest.Print(); // Return to the EditRequest page
                                return; // Exit method
                            }

                            if (int.TryParse(input, out int selectedOption) && selectedOption >= 1 && selectedOption <= options.Count)
                            {
                                requestItem.Data[key] = (options[selectedOption - 1], FieldType.Selectable);
                                validInput = true; // Set flag to true when valid input is received
                            }
                        }
                    }
                    else if (field.Type == FieldType.Currency)
                    {
                        // Handle currency input with validation
                        Console.Write($"{key} (apenas números e use ',' como separador decimal, com duas casas): ");
                        string input = Console.ReadLine();

                        // Validate currency format (digits with a comma as decimal separator)
                        if (Regex.IsMatch(input, @"^\d+,\d{2}$"))
                        {
                            requestItem.Data[key] = (input, FieldType.Currency);
                            validInput = true; // Input is valid
                        }
                        else
                        {
                            errorMessage = ("Formato inválido. Por favor, insira apenas números com vírgula como separador decimal (ex: 1234,56).");
                        }
                    }
                }
            }

            Request.requestItems.Add(requestItem);
            Request.modified = true;
            EditRequest.Print();
        }
    }
}
