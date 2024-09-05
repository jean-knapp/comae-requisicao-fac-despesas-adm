using comae_requisicao_fac_despesas_adm.Models;
using System.IO;
using System.Text;
using System;
using System.Linq;
using comae_requisicao_fac_despesas_adm.Pages;
using System.Security.Policy;
using System.Collections.Generic;

namespace comae_requisicao_fac_despesas_adm
{
    internal static class IO
    {
        internal static void Save()
        {
            // Define the directory path where the file will be saved
            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "requests");

            // Create the directory if it doesn't exist
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Define the file path using the unique identifier as the file name
            string filePath = Path.Combine(directoryPath, $"{Request.uniqueIdentifier}.csv");

            if (Request.requestItems.Count > 0)
            {
                // Build the CSV content
                StringBuilder csvContent = new StringBuilder();

                // Get requester data
                var requesterData = Requester.Data.Values.Select(v => EscapeCsvField(v.Value)).ToArray();

                // Get the headers (field names) for both requester and request items
                var headers = Requester.Data.Keys.Concat(RequestItem.DataTemplate.Keys).ToList();
                csvContent.AppendLine(string.Join(";", headers)); // Use semicolon as the delimiter

                // Append each request item with the requester data
                foreach (var requestItem in Request.requestItems)
                {
                    var itemData = requestItem.Data.Values.Select(v => EscapeCsvField(v.Value)).ToArray();
                    csvContent.AppendLine(string.Join(";", requesterData.Concat(itemData))); // Use semicolon as the delimiter
                }

                // Write the CSV content to the file
                File.WriteAllText(filePath, csvContent.ToString());

                EditRequest.message = $"Arquivo salvo em: {filePath}";
            } else
            {
                if (File.Exists(filePath))
                {
                    EditRequest.message = $"Arquivo excluído: {filePath}";
                    File.Delete(filePath);
                }
                else
                {
                    EditRequest.message = $"Não há dados para serem salvos.";
                }

                
            }
            EditRequest.Print();
        }

        // Helper method to escape special characters in CSV fields
        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field)) return field;

            // Escape semicolons and quotes by wrapping the field in double quotes
            if (field.Contains(";") || field.Contains("\""))
            {
                field = field.Replace("\"", "\"\""); // Escape double quotes by doubling them
                return $"\"{field}\""; // Wrap the field in double quotes
            }

            return field;
        }

        internal static void Load(string file)
        {
            // Clear the existing requester data and request items
            Requester.Data = Requester.Data.ToDictionary(k => k.Key, k => ("", k.Value.Type, k.Value.Preview, k.Value.Description));
            Request.requestItems.Clear();
            Request.modified = false;
            Request.uniqueIdentifier = Path.GetFileNameWithoutExtension(file);

            // Read the file content
            var lines = File.ReadAllLines(file);

            // Extract headers and data
            var headers = lines[0].Split(';');
            var dataLines = lines.Skip(1);

            // Load the requester data from the first part of the file
            var requesterData = dataLines.First().Split(';').Take(Requester.Data.Count).ToArray();
            int index = 0;
            foreach (var key in Requester.Data.Keys.ToList())
            {
                Requester.Data[key] = (requesterData[index], Requester.Data[key].Type, Requester.Data[key].Preview, Requester.Data[key].Description);
                index++;
            }

            // Load the request items
            foreach (var line in dataLines)
            {
                var itemData = line.Split(';').Skip(Requester.Data.Count).ToArray();
                RequestItem requestItem = new RequestItem();

                index = 0;
                foreach (var key in RequestItem.DataTemplate.Keys)
                {
                    requestItem.Data[key] = (itemData[index], RequestItem.DataTemplate[key].FieldType);
                    index++;
                }

                Request.requestItems.Add(requestItem);
            }

            // Navigate to the EditRequest page
            EditRequest.Print();
        }

        private static FieldType GetFieldType(string str)
        {
            switch(str)
            {
                case "FreeText":
                    return FieldType.FreeText;
                case "Selectable":
                    return FieldType.Selectable;
                case "Currency":
                    return FieldType.Currency;
                case "Phone":
                    return FieldType.Phone;
                case "RequiredText":
                    return FieldType.RequiredText;
                default:
                    return FieldType.FreeText;
            }
        }

        internal static void LoadProgramData()
        {
            KeyValue rootKv = KeyValue.FromFile("config.kv");

            Index.title = rootKv.GetChildValue("Title");
            Index.welcomeMessage  = rootKv.GetChildValue("WelcomeMessage").Replace("\\n", "\n");

            KeyValue operationsKv = rootKv.GetChildByKey("Operations");
            NewRequest.operations.Clear();
            foreach(var operationKv in  operationsKv.Children)
            {
                NewRequest.operations.Add(operationKv.Value);
            }

            KeyValue requesterKv = rootKv.GetChildByKey("Requester");
            Requester.Data.Clear();
            Requester.SelectableOptions.Clear();
            if (requesterKv != null)
            {
                foreach(var optionKv in requesterKv.Children)
                {
                    string name = optionKv.GetChildValue("Name");
                    FieldType fieldType = GetFieldType(optionKv.GetChildValue("FieldType"));
                    bool preview = optionKv.GetChildValue<int>("Preview", 0) == 1;
                    string description = optionKv.GetChildValue("Description").Replace("\\n", "\n");


                    Requester.Data.Add(name, ("", fieldType, preview, description));

                    KeyValue itemsKv = optionKv.GetChildByKey("Items");
                    if (itemsKv != null)
                    {
                        List<string> options = new List<string>();
                        foreach(var itemKv in itemsKv.Children)
                        {
                            options.Add(itemKv.Value);
                        }
                        Requester.SelectableOptions.Add(name, options);
                    }
                }
            }

            // Load RequestItem data
            KeyValue requestItemKv = rootKv.GetChildByKey("RequestItem");
            RequestItem.DataTemplate.Clear();
            RequestItem.SelectableOptions.Clear();
            if (requestItemKv != null)
            {
                foreach (var optionKv in requestItemKv.Children)
                {
                    string name = optionKv.GetChildValue("Name");
                    FieldType fieldType = GetFieldType(optionKv.GetChildValue("FieldType"));
                    bool preview = optionKv.GetChildValue<int>("Preview", 0) == 1;
                    string description = optionKv.GetChildValue("Description").Replace("\\n", "\n");

                    // Add the RequestItem data to the DataTemplate dictionary
                    RequestItem.DataTemplate.Add(name, (fieldType, preview, description));

                    // Handle selectable options if available
                    KeyValue itemsKv = optionKv.GetChildByKey("Items");
                    if (itemsKv != null)
                    {
                        List<string> options = new List<string>();
                        foreach (var itemKv in itemsKv.Children)
                        {
                            options.Add(itemKv.Value);
                        }
                        RequestItem.SelectableOptions.Add(name, options);
                    }
                }
            }
        }

        // Method to load the most recently modified requester data
        internal static void LoadLastRequesterData()
        {
            // Define the directory path where the request files are saved
            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "requests");

            // Check if the directory exists and has any files
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("Nenhum arquivo de solicitação encontrado.");
                return;
            }

            // Get the most recently modified file
            var lastModifiedFile = Directory.GetFiles(directoryPath, "*.csv")
                                            .OrderByDescending(f => File.GetLastWriteTime(f))
                                            .FirstOrDefault();

            if (lastModifiedFile == null)
            {
                Console.WriteLine("Nenhum arquivo de solicitação encontrado.");
                return;
            }

            // Load the file content
            var lines = File.ReadAllLines(lastModifiedFile);

            // Extract requester data from the first row of data
            var requesterData = lines.Skip(1).First().Split(';').Take(Requester.Data.Count).ToArray();
            int index = 0;
            foreach (var key in Requester.Data.Keys.ToList())
            {
                Requester.Data[key] = (requesterData[index], Requester.Data[key].Type, Requester.Data[key].Preview, Requester.Data[key].Description);
                index++;
            }

            Console.WriteLine("Dados do solicitante carregados do arquivo mais recente.");
        }
    }
}
