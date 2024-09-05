using System;
using System.Collections.Generic;

namespace comae_requisicao_fac_despesas_adm.Models
{
    internal class RequestItem
    {
        // Static template to define the structure of the request item data
        internal static Dictionary<string, (FieldType FieldType, bool Preview, string Description)> DataTemplate = new Dictionary<string, (FieldType FieldType, bool Preview, string Description)>
        {

        };

        // Static dictionary for selectable options (can be edited when loading the application)
        internal static Dictionary<string, List<string>> SelectableOptions = new Dictionary<string, List<string>>
        {

        };

        // Instance dictionary to hold the actual data values of this specific request item
        internal Dictionary<string, (string Value, FieldType Type)> Data;

        // Constructor to initialize the RequestItem using the static template
        public RequestItem()
        {
            Data = new Dictionary<string, (string Value, FieldType Type)>();

            // Initialize the data for each field from the static template
            foreach (var field in DataTemplate)
            {
                Data[field.Key] = ("", field.Value.FieldType);
            }
        }

        // Method to check if all fields are filled
        internal bool IsComplete()
        {
            foreach (var field in Data)
            {
                if (string.IsNullOrWhiteSpace(field.Value.Value))
                    return false;
            }
            return true;
        }

        // Method to print the data of the request item
        internal void PrintItemInfo()
        {
            Console.Clear();
            Console.WriteLine("Detalhamento da Requisição:");
            foreach (var entry in Data)
            {
                Console.WriteLine($"{entry.Key}: {entry.Value.Value}");
            }
        }
    }
}
