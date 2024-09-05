using comae_requisicao_fac_despesas_adm.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace comae_requisicao_fac_despesas_adm.Pages
{
    internal static class NewRequesterInfo
    {
        // Input method to edit requester data
        internal static void Input()
        {
            Console.Clear();

            // Iterate over a copy of the keys to avoid modifying the collection during iteration
            foreach (var key in new List<string>(Requester.Data.Keys))
            {
                bool validInput = false; // Flag for valid input
                string errorMessage = string.Empty; // Store any error messages for display

                while (!validInput)
                {
                    Console.Clear();
                    Console.WriteLine(Index.title);
                    Console.WriteLine("- Edição dos dados do solicitante:\n");
                    var field = Requester.Data[key];

                    if (field.Description != "")
                    {
                        Console.WriteLine(field.Description + "\n");
                    }

                    if (errorMessage.Length > 0)
                    {
                        Console.WriteLine($"{errorMessage}\n");
                        errorMessage = "";
                    }

                    if (field.Type == FieldType.FreeText)
                    {
                        // Handle free text input
                        Console.Write($"{key} (campo facultativo): ");
                        string newValue = Console.ReadLine();
                        Requester.Data[key] = (newValue, FieldType.FreeText, Requester.Data[key].Preview, Requester.Data[key].Description); // Update the field value
                        validInput = true; // Free text is always valid
                    }
                    else if (field.Type == FieldType.RequiredText)
                    {
                        // Handle required text input
                        Console.Write($"{key}: ");
                        string newValue = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(newValue)) // Ensure the input is not empty or whitespace
                        {
                            Requester.Data[key] = (newValue, FieldType.RequiredText, Requester.Data[key].Preview, Requester.Data[key].Description);
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
                        var options = Requester.SelectableOptions[key];

                        // If there are less than 10 options, use single char input
                        if (options.Count < 10)
                        {
                            for (int i = 0; i < options.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {options[i]}");
                            }

                            char input = Console.ReadKey(true).KeyChar;
                            int selectedOption = (int)char.GetNumericValue(input);

                            if (selectedOption >= 1 && selectedOption <= options.Count)
                            {
                                Requester.Data[key] = (options[selectedOption - 1], FieldType.Selectable, Requester.Data[key].Preview, Requester.Data[key].Description);
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

                            int selectedOption = -1;
                            if (int.TryParse(Console.ReadLine(), out selectedOption) && selectedOption >= 1 && selectedOption <= options.Count)
                            {
                                Requester.Data[key] = (options[selectedOption - 1], FieldType.Selectable, Requester.Data[key].Preview, Requester.Data[key].Description);
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
                            Requester.Data[key] = (input, FieldType.Currency, Requester.Data[key].Preview, Requester.Data[key].Description);
                            validInput = true; // Input is valid
                        }
                        else
                        {
                            errorMessage = ("Formato inválido. Por favor, insira apenas números com vírgula como separador decimal (ex: 1234,56).");
                        }
                    }
                    else if (field.Type == FieldType.Phone)
                    {
                        // Handle phone number input with validation and formatting
                        Console.Write($"{key}: ");
                        string input = Console.ReadLine();

                        // Remove all non-digit characters
                        string digitsOnly = Regex.Replace(input, @"\D", "");

                        if (digitsOnly.Length == 10) // Format as (00) 0000-0000
                        {
                            string formattedPhone = $"({digitsOnly.Substring(0, 2)}) {digitsOnly.Substring(2, 4)}-{digitsOnly.Substring(6, 4)}";
                            Requester.Data[key] = (formattedPhone, FieldType.Phone, Requester.Data[key].Preview, Requester.Data[key].Description);
                            validInput = true; // Input is valid
                        }
                        else if (digitsOnly.Length == 11) // Format as (00) 00000-0000
                        {
                            string formattedPhone = $"({digitsOnly.Substring(0, 2)}) {digitsOnly.Substring(2, 5)}-{digitsOnly.Substring(7, 4)}";
                            Requester.Data[key] = (formattedPhone, FieldType.Phone, Requester.Data[key].Preview, Requester.Data[key].Description);
                            validInput = true; // Input is valid
                        }
                        else
                        {
                            errorMessage = "Número de telefone inválido. Insira um número com prefixo e mais 8 ou 9 dígitos.";
                        }
                    }
                }
            }

            Request.modified = true;
        }
    }
}