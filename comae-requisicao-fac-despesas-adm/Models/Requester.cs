using System;
using System.Collections.Generic;

namespace comae_requisicao_fac_despesas_adm.Models
{
    internal static class Requester
    {
        // Dictionary to store requester data fields with FieldType
        internal static Dictionary<string, (string Value, FieldType Type, bool Preview, string Description)> Data = new Dictionary<string, (string Value, FieldType Type, bool Preview, string Description)>
        {

        };

        // Selectable options for fields that require selection (like a dropdown)
        internal static Dictionary<string, List<string>> SelectableOptions = new Dictionary<string, List<string>>
        {

        };

        // Check if all fields are filled
        internal static bool IsRequesterInfoComplete()
        {
            foreach (var field in Data)
            {
                if (string.IsNullOrWhiteSpace(field.Value.Value))
                    return false;
            }
            return true;
        }

        // Print requester data
        internal static void PrintRequesterInfo()
        {
            foreach (var entry in Data)
            {
                Console.WriteLine($"{entry.Key}: {entry.Value.Value}");
            }
        }
    }
}
