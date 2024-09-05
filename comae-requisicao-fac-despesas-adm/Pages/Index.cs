using System;

namespace comae_requisicao_fac_despesas_adm.Pages
{
    internal static class Index
    {
        internal static string title = "";
        internal static string welcomeMessage = "";

        internal static void Print()
        {
            Console.Clear();
            Console.WriteLine(Index.title + "\n");

            // Print the welcome message
            Console.WriteLine(welcomeMessage);
            Console.WriteLine(); // Line break for better readability

            // Display options
            Console.WriteLine("Por favor, escolha uma das opções abaixo:");
            Console.WriteLine("1. Nova solicitação");
            Console.WriteLine("2. Editar solicitação existente");
            Console.WriteLine("3. Sair");

            Input();
        }

        internal static void Input()
        {
            // Capture user input without requiring ENTER
            Console.Write("\nEscolha uma opção: ");
            char input = Console.ReadKey(true).KeyChar; // Read a single key without displaying it

            // Handle the user's selection
            switch (input)
            {
                case '1':
                    NewRequest.Print();
                    // Call the method to handle new requests
                    break;
                case '2':
                    LoadRequest.Print();
                    // Call the method to edit existing requests
                    break;
                case '3':
                    Console.WriteLine("\nSaindo do programa...");
                    Environment.Exit(0);
                    break;
                default:
                    Print(); // Recursively call Print to try again
                    break;
            }
        }
    }
}
