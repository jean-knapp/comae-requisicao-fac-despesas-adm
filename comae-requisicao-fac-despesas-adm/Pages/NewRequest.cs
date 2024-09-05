using comae_requisicao_fac_despesas_adm.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace comae_requisicao_fac_despesas_adm.Pages
{
    internal static class NewRequest
    {
        // Static internal list of military operations
        internal static List<string> operations = new List<string>
        {

        };

        internal static void Print()
        {
            Console.Clear();
            Console.WriteLine(Index.title);
            Console.WriteLine("- Seleção da operação Militar:\n");

            // Display the operations
            for (int i = 0; i < operations.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {operations[i]}");
            }

            // Provide option to return to the main menu
            Console.WriteLine("\n0. Retornar ao menu principal");

            Input();
        }

        internal static void Input()
        {
            if (operations.Count < 10)
            {
                // If fewer than 10 operations, use character input
                Console.Write("\nEscolha a operação (0 para voltar, 1-{0}): ", operations.Count);
                char input = Console.ReadKey(true).KeyChar;
                int operationIndex = (int)char.GetNumericValue(input) - 1;

                // Handle "0" for returning to the main menu
                if (input == '0')
                {
                    ReturnToMainMenu();
                    return;
                }

                // Validate and confirm the selection
                ValidateSelection(operationIndex);
            }
            else
            {
                // If 10 or more operations, use full-line input
                Console.Write("\nDigite o número da operação e pressione Enter (0 para voltar, 1-{0}): ", operations.Count);
                string input = Console.ReadLine();

                if (input == "0")
                {
                    ReturnToMainMenu();
                    return;
                }

                if (int.TryParse(input, out int operationIndex))
                {
                    operationIndex--; // Adjust for 0-based index
                    ValidateSelection(operationIndex);
                }
                else
                {
                    Console.WriteLine("Entrada inválida. Tente novamente.");
                    Print(); // Restart the selection process
                }
            }
        }

        internal static void ValidateSelection(int operationIndex)
        {
            if (operationIndex >= 0 && operationIndex < operations.Count)
            {
                string selectedOperation = operations[operationIndex];

                // Confirm operation selection
                Console.Clear();
                Console.WriteLine(Index.title);
                Console.WriteLine("- Seleção da operação Militar:\n");

                Console.WriteLine($"Você selecionou a operação: {selectedOperation}");
                Console.Write("Confirma essa operação? (S/N): ");
                char confirm = Console.ReadKey(true).KeyChar;

                if (confirm == 'S' || confirm == 's')
                {
                    CreateNewRequest(selectedOperation);
                }
                else
                {
                    // If user doesn't confirm, go back to choosing an operation, not the main menu
                    Print(); // Restart selection process without returning to the main menu
                }
            }
            else
            {
                Print(); // Restart the selection process
            }
        }

        // Method to return to the main menu
        internal static void ReturnToMainMenu()
        {
            Console.Clear();
            Index.Print(); // Return to the main menu
        }

        internal static void CreateNewRequest(string operation)
        {
            if (Requester.IsRequesterInfoComplete())
            {
                Console.Clear();
                Console.WriteLine(Index.title);
                Console.WriteLine("- Seleção da operação Militar:\n");
                Console.WriteLine("Dados do solicitante");
                EditRequesterInfo.DisplayRequesterInfo();
                Console.WriteLine("\nVocê é o solicitante atual? (S/N): ");
                char confirm = Console.ReadKey(true).KeyChar;

                if (confirm == 'S' || confirm == 's')
                {
                    // Proceed without clearing requester data
                }
                else if (confirm == 'N' || confirm == 'n')
                {
                    // Erase current requester data and continue
                    ClearRequesterData();
                }
                else
                {
                    // Invalid input, requery the user
                    CreateNewRequest(operation);
                    return;
                }
            }

            Request.operation = operation;
            Request.GenerateUniqueIdentifier();
            Request.requestItems.Clear();
            Request.modified = false;
            EditRequest.Print(); // Proceed to EditRequest
        }

        // Method to clear requester data
        internal static void ClearRequesterData()
        {
            // Iterate over Requester data and clear all values
            foreach (var key in Requester.Data.Keys.ToList()) // Create a list to avoid modifying during iteration
            {
                Requester.Data[key] = ("", Requester.Data[key].Type, Requester.Data[key].Preview, Requester.Data[key].Description); // Clear the value, keep the field type
            }
        }
    }
}