using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using comae_requisicao_fac_despesas_adm.Models;

namespace comae_requisicao_fac_despesas_adm.Pages
{
    internal static class LoadRequest
    {
        private const int PageSize = 2; // Number of results per page
        private static int currentPage = 0; // Current page number
        private static string[] requestFiles; // Stores the list of request files

        // Method to display the list of saved requests with pagination
        internal static void Print(bool resetPage = false)
        {
            if (resetPage)
            {
                currentPage = 0;
            }

            Console.Clear();
            Console.WriteLine(Index.title);
            Console.WriteLine("- Carregar Solicitação:\n");

            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "requests");

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("Nenhuma solicitação encontrada.");
                Console.WriteLine("\nPressione qualquer tecla para voltar.");
                Console.ReadKey();
                return;
            }

            // Load all request files
            requestFiles = Directory.GetFiles(directoryPath, "*.csv")
                                    .OrderByDescending(file => File.GetCreationTime(file)) // Sort by creation date descending
                                    .ToArray();

            if (requestFiles.Length == 0)
            {
                Console.WriteLine("Nenhuma solicitação encontrada.");
                Console.WriteLine("\nPressione qualquer tecla para voltar.");
                Console.ReadKey();
                return;
            }

            // Calculate field sizes
            int requesterFieldCount = 3;
            foreach (var item in Requester.Data)
            {
                if (item.Value.Preview)
                {
                    requesterFieldCount++;
                }
            }

            rows = new List<string[]>();
            fieldSizes = new int[requesterFieldCount];

            int start = currentPage * PageSize;
            int end = Math.Min(start + PageSize, requestFiles.Length);

            for (int i = start; i < end; i++)
            {
                string filePath = requestFiles[i];
                string fileName = Path.GetFileNameWithoutExtension(filePath).ToUpper().Substring(0, 8);
                DateTime creationDate = File.GetCreationTime(filePath);
                DateTime lastModified = File.GetLastWriteTime(filePath);

                var firstLine = File.ReadLines(filePath).Skip(1).FirstOrDefault();
                if (firstLine != null)
                {
                    var requesterData = firstLine.Split(';').Take(Requester.Data.Count).ToArray();
                    var previewData = Requester.Data
                        .Where(entry => entry.Value.Preview)
                        .Select((entry, index) => requesterData.Length > index ? requesterData[index] : "")
                        .ToArray();

                    string[] row = new string[requesterFieldCount];
                    row[0] = fileName.PadRight(9);
                    for(int j = 0; j < previewData.Length; j++)
                    {
                        row[j+1] = previewData[j] + " ";
                    }

                    row[row.Length - 2] = $"{creationDate:dd/MM/yyyy HH:mm:ss} ";
                    row[row.Length - 1] = $"{lastModified:dd/MM/yyyy HH:mm:ss} ";

                    for (int j = 0; j < row.Length; j++)
                    {
                        fieldSizes[j] = Math.Max(fieldSizes[j], row[j].Length);
                    }

                    rows.Add(row);
                }
            }

            // Display header
            DisplayHeader();

            // Display requests for the current page
            DisplayPage();

            // Handle input
            Input();
        }

        static int[] fieldSizes;
        static List<string[]> rows;

        // Method to display the table header
        private static void DisplayHeader()
        {
            var previewHeaders = Requester.Data
                .Where(entry => entry.Value.Preview)
                .Select(entry => entry.Key)
                .ToArray();

            Console.Write("   ");
            Console.Write("ID".PadRight(fieldSizes[0]));
            for (int i = 0;  i < previewHeaders.Length; i++)
            {
                fieldSizes[i+1] = Math.Max(fieldSizes[i + 1], previewHeaders[i].Length + 1);
                Console.Write($"{previewHeaders[i]} ".PadRight(fieldSizes[i+1]));
            }
            fieldSizes[fieldSizes.Length - 2] = Math.Max(fieldSizes[fieldSizes.Length - 2], "Data de Criação ".Length);
            fieldSizes[fieldSizes.Length - 1] = Math.Max(fieldSizes[fieldSizes.Length - 1], "Data de Modificação ".Length);
            Console.Write($"Data de Criação ".PadRight(fieldSizes[fieldSizes.Length - 2]));
            Console.Write($"Data de Modificação ".PadRight(fieldSizes[fieldSizes.Length - 1]));
            Console.Write("\n");

            Console.WriteLine("   " + new string('-', fieldSizes.Sum()));
        }

        // Method to display requests for a specific page
        private static void DisplayPage()
        {
            for (int i = 0; i < rows.Count; i++)
            {
                Console.Write($"{(i + 1).ToString()}. ");

                for (int j = 0; j < rows[i].Length; j++)
                {
                    Console.Write($"{rows[i][j].PadRight(fieldSizes[j])}");
                }
                Console.Write("\n");
            }

            Console.WriteLine($"Pagina {currentPage+1}/{Math.Ceiling((double)requestFiles.Length / PageSize)}\n");

            // Display pagination controls
            Console.WriteLine("\nEscolha um número para editar.\nPressione , ou . para navegar entre as páginas.\nPressione 0 para voltar ao menu.");
        }

        // Method to handle user input
        private static void Input()
        {
            var input = Console.ReadKey(true);
            var key = input.Key;

            if (key == ConsoleKey.OemComma && currentPage > 0)
            {
                currentPage--;
                Print();
                return;
            }
            else if (key == ConsoleKey.OemPeriod && (currentPage + 1) * PageSize < requestFiles.Length)
            {
                currentPage++;
                Print();
                return;
            }
            else if (key == ConsoleKey.D0 || key == ConsoleKey.NumPad0)
            {
                Index.Print(); // Return to main menu
                return;
            }
            int choice = (int)char.GetNumericValue(input.KeyChar); // Convert char to numeric value

            int start = currentPage * PageSize;
            int end = Math.Min(start + PageSize, requestFiles.Length);

            if (choice >= 1 && choice <= end - start)
            {
                int fileNumber = start + choice - 1;
                IO.Load(requestFiles[fileNumber]);
                return;
            }
            else
            {
                Print(); // Invalid choice, prompt again
                return;
            }
            
        }
    }
}
