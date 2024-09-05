using comae_requisicao_fac_despesas_adm.Models;
using System;

namespace comae_requisicao_fac_despesas_adm.Pages
{
    internal static class EditRequest
    {
        internal static string message = "";
        internal static void Print()
        {
            Console.Clear();
            Console.WriteLine(Index.title);
            Console.WriteLine("- Edição da solicitação:\n");

            if (!Requester.IsRequesterInfoComplete())
            {
                
                Console.WriteLine("Solicitante não cadastrado.");
                Console.WriteLine();
                Console.WriteLine("1. Inserir dados do solicitante");
                Console.WriteLine("\n0. Retornar ao menu principal");

                char input = Console.ReadKey(true).KeyChar;
                switch (input)
                {
                    case '1':
                        NewRequesterInfo.Input();
                        Print(); // Return to the EditRequest page after updating
                        break;
                    case '0':
                        Index.Print(); // Return to main menu
                        break;
                    default:
                        Console.WriteLine("Opção inválida.");
                        Print();
                        break;
                }
            }
            else
            {
                Console.WriteLine("# Tabela de Despesas Administrativas\n");
                Request.PrintRequestItemsTable();
                Console.WriteLine("\n# Dados do Solicitante\n");
                Requester.PrintRequesterInfo();

                if (!string.IsNullOrEmpty(message))
                {
                    Console.WriteLine($"\n{message}");
                    message = "";
                }
                else
                {
                    Console.WriteLine("\n");
                }

                Console.WriteLine("\n1. Editar dados do solicitante");
                Console.WriteLine("2. Inserir nova despesa administrativa");
                Console.WriteLine("3. Editar despesa administrativa");
                Console.WriteLine("4. Excluir despesa administrativa");
                Console.WriteLine("5. Salvar requisição");
                Console.WriteLine("6. Exportar requisição");
                Console.WriteLine("\n0. Retornar ao menu principal");

                char input = Console.ReadKey(true).KeyChar;
                switch (input)
                {
                    case '1':
                        EditRequesterInfo.Print();
                        break;
                    case '2':
                        // Insert new request logic
                        NewRequestItem.Input();
                        break;
                    case '3':
                        // Edit existing request logic
                        EditRequestItem.Print();
                        break;
                    case '4':
                        // Delete existing request logic
                        DeleteRequestItem.Print();
                        break;
                    case '5':
                        // Save request logic
                        IO.Save();
                        break;
                    case '6':
                        // Export request logic
                        Console.WriteLine("Exportar requisição");
                        break;
                    case '0':
                        Index.Print(); // Return to main menu
                        break;
                    default:
                        Console.WriteLine("Opção inválida.");
                        Print();
                        break;
                }
            }
        }
    }
}
