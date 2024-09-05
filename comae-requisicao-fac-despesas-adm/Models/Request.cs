using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace comae_requisicao_fac_despesas_adm.Models
{
    internal static class Request
    {
        internal static string operation = "";
        internal static string uniqueIdentifier = "";
        internal static bool modified = false;

        internal static List<RequestItem> requestItems = new List<RequestItem>();

        // Method to print the request items in a tabular format
        internal static void PrintRequestItemsTable()
        {
            if (requestItems.Count == 0)
            {
                Console.WriteLine("Nenhum item de requisição disponível.");
                return;
            }

            // Get the headers (field names) from the first RequestItem
            var headers = RequestItem.DataTemplate.Keys.ToList();

            // Calculate the maximum width for each column (header or data)
            var columnWidths = headers.Select(header =>
                Math.Max(header.Length,
                    requestItems.Max(item => item.Data.ContainsKey(header) ? item.Data[header].Value.Length : 0))
            ).ToList();

            int previewHeadersCount = 0;
            int previewHeadersLength = 0;

            // Print table headers with proper alignment
            Console.Write("N   "); // Add the index column header for numerals
            previewHeadersLength += 4; // Add space for the "N" column

            for (int i = 0; i < headers.Count; i++)
            {
                if (!RequestItem.DataTemplate[headers[i]].Preview)
                    continue;

                previewHeadersCount++;
                previewHeadersLength += columnWidths[i] + 2; // Add 2 for padding between columns

                Console.Write(headers[i].PadRight(columnWidths[i] + 2));
            }
            Console.WriteLine();

            // Print a separator line of the correct length
            Console.WriteLine(new string('-', previewHeadersLength));

            // Print each request item as a row in the table with a simple numeral index
            int index = 1;
            foreach (var item in requestItems)
            {
                Console.Write($"{index.ToString()} ".PadRight(4)); // Display index as simple numerals (1, 2, etc.)

                for (int i = 0; i < headers.Count; i++)
                {
                    if (!RequestItem.DataTemplate[headers[i]].Preview)
                        continue;

                    var header = headers[i];
                    var value = item.Data.ContainsKey(header) ? item.Data[header].Value : "N/A";
                    Console.Write(value.PadRight(columnWidths[i] + 2));
                }
                Console.WriteLine(); // Move to the next row
                index++; // Increment the index
            }
        }



        // Method to generate a unique identifier based on the computer's MAC address
        internal static void GenerateUniqueIdentifier()
        {
            // Get the current timestamp in ticks
            long timestamp = DateTime.UtcNow.Ticks;

            // Get the MAC address of the system
            string macAddress = GetMacAddress();

            // Combine the MAC address and the timestamp into a single string
            string combinedString = $"{macAddress}-{timestamp}";

            // Create a hash from the combined string
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedString));

                // Convert the byte array to a hexadecimal string
                StringBuilder hexString = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hexString.Append(b.ToString("x2"));
                }

                uniqueIdentifier = hexString.ToString(); // Return the unique hash
            }
        }

        // Method to get the first active MAC address of the machine
        private static string GetMacAddress()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var ni in networkInterfaces)
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    return string.Join(":", ni.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")));
                }
            }

            return null; // No active MAC address found
        }
    }
}
