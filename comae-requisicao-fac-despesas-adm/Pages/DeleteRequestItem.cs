using comae_requisicao_fac_despesas_adm.Models;
using System;

namespace comae_requisicao_fac_despesas_adm.Pages
{
    internal static class DeleteRequestItem
    {
        internal static void Print()
        {
            Console.Clear();
            Console.WriteLine(Index.title);
            Console.WriteLine("- Excluir Itens da Requisição:\n");

            if (Request.requestItems.Count == 0)
            {
                Console.WriteLine("Nenhum item disponível para exclusão.");
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
                Console.Write("\nEscolha um número para excluir ou 0 para retornar: ");
                char input = Console.ReadKey(true).KeyChar; // Read a single character without requiring Enter

                if (input == '0')
                {
                    EditRequest.Print(); // Return to EditRequest page
                    return;
                }

                int choice = (int)char.GetNumericValue(input); // Convert char to numeric value

                if (choice >= 1 && choice <= Request.requestItems.Count)
                {
                    ConfirmDelete(choice - 1); // -1 to adjust for 0-based index
                }
                else
                {
                    Print(); // Invalid choice, prompt again
                }
            }
            else
            {
                // If 10 or more options, use full-line input
                Console.Write("\nDigite o número para excluir ou 0 para retornar: ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int selectedItem) && selectedItem >= 1 && selectedItem <= Request.requestItems.Count)
                {
                    ConfirmDelete(selectedItem - 1); // -1 to adjust for 0-based index
                }
                else
                {
                    Print(); // Invalid choice, prompt again
                }
            }
        }

        internal static void ConfirmDelete(int itemIndex)
        {
            Console.Clear();
            Console.WriteLine(Index.title);
            Console.WriteLine($"- Confirmar Exclusão do Item {itemIndex + 1}:\n");

            RequestItem requestItem = Request.requestItems[itemIndex];

            // Display the details of the item being deleted
            foreach (var entry in requestItem.Data)
            {
                Console.WriteLine($"{entry.Key}: {entry.Value.Value}");
            }

            Console.Write("\nTem certeza que deseja excluir este item? (S/N): ");
            char confirm = Console.ReadKey(true).KeyChar;

            if (confirm == 'S' || confirm == 's')
            {
                // Remove the selected item
                Request.requestItems.RemoveAt(itemIndex);
                EditRequest.message = ("\nItem excluído com sucesso.");
            }
            else
            {
                EditRequest.message = ("\nExclusão cancelada.");
            }

            EditRequest.Print(); // Return to the EditRequest page
        }
    }
}
