using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace comae_requisicao_fac_despesas_adm
{
    public class KeyValue
    {
        private static readonly string[] DefaultCredits = new string[]
        {
        };

        private Dictionary<string, List<KeyValue>> childrenIndex = null;

        private string comment = string.Empty;

        private KeyValue parent = null;

        public static bool KeyCasing { get; set; } = false;


        /// <summary>
        /// The key component of the keyvalue.
        /// </summary>
        public string Key
        {
            get
            {
                return key;
            }
            set
            {
                // Check if there are any changes.
                if (this.key == value)
                    return;

                // Replace the key in the keyvalue.
                string oldKey = this.key;
                this.key = value;

                // If the parent is not null, we need to update the childrenIndex of the parent.
                if (parent != null)
                {
                    // If the parent does not have the new key, we need to add it.
                    if (!parent.childrenIndex.ContainsKey(value))
                        parent.childrenIndex[value] = new List<KeyValue>();

                    // Replace the old key with the new key in the childrenIndex.
                    parent.childrenIndex[oldKey].Remove(this);
                    parent.childrenIndex[value].Add(this);

                    // If the old key has no children, we need to remove it.
                    if (parent.childrenIndex[oldKey].Count == 0)
                        parent.childrenIndex.Remove(oldKey);
                }
            }
        }
        private string key = string.Empty;

        public string OSData { get; set; } = null;

        /// <summary>
        /// The value component of the keyvalue.
        /// </summary>
        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                // Check if there are any changes.
                if (this.value == value)
                    return;

                if (children != null && children.Count > 0)
                {
                    throw new Exception("A KeyValue with children cannot have a value.");
                }

                this.value = value;
            }
        }
        private string value = null;

        public T GetValue<T>(T defautValue = default(T))
        {
            var valueStr = value;

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter.IsValid(valueStr))
            {
                T result = (T)Convert.ChangeType(valueStr, typeof(T));

                if (defautValue.GetType() == typeof(string) && defautValue != null && valueStr == string.Empty)
                {
                    return defautValue;
                }
                return result;
            }
            else
            {
                return defautValue;
            }
        }

        /// <summary>
        /// The list of KeyValues that are children of this KeyValue.
        /// </summary>
        public List<KeyValue> Children
        {
            get
            {
                return children;
            }
        }
        private List<KeyValue> children = null;

        public bool HasChildren
        {
            get
            {
                if (children == null)
                    return false;

                return children.Count > 0;
            }
        }

        public KeyValue(string key)
        {
            this.key = key;
            childrenIndex = new Dictionary<string, List<KeyValue>>();
            children = new List<KeyValue>();
        }

        public KeyValue(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Adds an existing KeyValue as a child of this KeyValue.
        /// </summary>
        /// <param name="value"> The KeyValue to add as a child. </param>
        public void AddChild(KeyValue value)
        {
            if (this.value != null)
            {
                throw new Exception("A KeyValue with a value cannot have children.");
            }

            if (children == null)
            {
                children = new List<KeyValue>();
            }
            if (childrenIndex == null)
            {
                childrenIndex = new Dictionary<string, List<KeyValue>>();
            }

            string lowerKey = value.Key.ToLowerInvariant();

            if (!childrenIndex.ContainsKey(lowerKey))
                childrenIndex.Add(lowerKey, new List<KeyValue>());

            childrenIndex[lowerKey].Add(value);
            children.Add(value);
            value.parent = this;
        }

        /// <summary>
        /// Adds an existing KeyValue as a child of this KeyValue, replacing its key.
        /// </summary>
        /// <param name="key"> The new key of the KeyValue. </param>
        /// <param name="value"> The KeyValue to add as a child. </param>
        public void AddChild(string key, KeyValue value)
        {
            if (this.value != null)
            {
                throw new Exception("A KeyValue with a value cannot have children.");
            }

            value.key = key;
            AddChild(value);
        }

        /// <summary>
        /// Adds a new KeyValue as a child of this KeyValue.
        /// </summary>
        /// <param name="key"> The key of the new KeyValue. </param>
        /// <param name="value"> The value of the new KeyValue. </param>
        public void AddChild(string key, string value)
        {
            AddChild(new KeyValue(key, value));
        }

        /// <summary>
        /// Removes a child KeyValue from this KeyValue.
        /// </summary>
        /// <param name="value"> The KeyValue to remove. </param>
        public void RemoveChild(KeyValue value)
        {
            if (children.Contains(value))
                children.Remove(value);

            string lowerKey = value.key.ToLowerInvariant();

            if (childrenIndex.ContainsKey(lowerKey) && childrenIndex[lowerKey].Contains(value))
                childrenIndex[lowerKey].Remove(value);

            value.parent = null;
        }

        /// <summary>
        /// Removes a child KeyValue from this KeyValue.
        /// </summary>
        public void ClearChildren()
        {
            childrenIndex = null;
            children = null;
        }

        /// <summary>
        /// Traverse the KeyValue tree and find the first child KeyValue with the specified key.
        /// </summary>
        /// <param name="key"> The key to search for. </param>
        /// <returns> The KeyValue with the specified key, or null </returns>
        public KeyValue FindChildByKey(string key)
        {
            string lowerKey = key.ToLowerInvariant();

            if (childrenIndex != null)
            {
                // First, check if the childrenIndex contains the exact key.
                if (childrenIndex.ContainsKey(lowerKey))
                    return childrenIndex[lowerKey][0];

                // If the childrenIndex does not contain the key, traverse the children and find the first child with the specified key.
                foreach (string k in childrenIndex.Keys)
                {
                    foreach (KeyValue child in childrenIndex[k])
                    {
                        KeyValue result = child.FindChildByKey(key);
                        if (result != null)
                            return result;
                    }
                }
            }

            // If the key was not found, return null.
            return null;
        }

        /// <summary>
        /// Traverse the KeyValue tree and find all child KeyValues with the specified key.
        /// </summary>
        /// <param name="key"> The key to search for. </param>
        /// <returns> A list of KeyValues with the specified key. </returns>
        public List<KeyValue> FindChildrenByKey(string key)
        {
            List<KeyValue> result = new List<KeyValue>();

            string lowerKey = key.ToLowerInvariant();

            if (childrenIndex != null)
            {
                // First, check if the childrenIndex contains the exact key.
                if (childrenIndex.ContainsKey(lowerKey))
                    result.AddRange(childrenIndex[lowerKey]);

                // If the childrenIndex does not contain the key, traverse the children and find all children with the specified key.
                foreach (string k in childrenIndex.Keys)
                {
                    foreach (KeyValue child in childrenIndex[k])
                    {
                        result.AddRange(child.FindChildrenByKey(key));
                    }
                }
            }

            // Return the list of KeyValues with the specified key.
            return result;
        }

        /// <summary>
        /// Get the first child of this KeyValue with the specified key.
        /// </summary>
        /// <param name="key"> The key to search for. </param>
        /// <returns> The first child KeyValue with the specified key, or null </returns>
        public KeyValue GetChildByKey(string key)
        {
            if (childrenIndex != null)
            {
                string lowerKey = key.ToLowerInvariant();

                // First, check if the childrenIndex contains the exact key.
                if (childrenIndex.ContainsKey(lowerKey))
                    return childrenIndex[lowerKey][0];
            }

            // If the key was not found, return null.
            return null;
        }

        /// <summary>
        /// Get all children of this KeyValue with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<KeyValue> GetChildrenByKey(string key)
        {
            if (childrenIndex != null)
            {
                string lowerKey = key.ToLowerInvariant();

                // First, check if the childrenIndex contains the exact key.
                if (childrenIndex.ContainsKey(lowerKey))
                    return childrenIndex[lowerKey];
            }

            // If the key was not found, return an empty list.
            return new List<KeyValue>();
        }

        /// <summary>
        /// Get the value of the first child of this KeyValue with the specified key.
        /// </summary>
        /// <param name="key"> The key to search for. </param>
        /// <returns> The value of the first child with the specified key, or an empty string </returns>
        public string GetChildValue(string key)
        {
            // Get the first child with the specified key.
            KeyValue child = GetChildByKey(key);

            // If the child is not null, return its value. Otherwise, return an empty string.
            return child?.Value ?? "";
        }

        /// <summary>
        /// Get the value of the first child of this KeyValue with the specified key, and convert it to the specified type.
        /// </summary>
        /// <typeparam name="T"> The type to convert the value to. </typeparam>
        /// <param name="key"> The key to search for. </param>
        /// <param name="defautValue"> The default value to return if the value could not be converted. </param>
        /// <returns> The value of the first child with the specified key, converted to the specified type, or the default value </returns>
        public T GetChildValue<T>(string key, T defautValue = default(T))
        {
            // Get the value of the first child with the specified key.
            string valueStr = GetChildValue(key);

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter.IsValid(valueStr))
            {
                // Convert the value to the specified type.
                T result = (T)Convert.ChangeType(valueStr, typeof(T));

                // If the default value is a string and is not null, and the value is an empty string, return the default value.
                if (defautValue.GetType() == typeof(string) && defautValue != null && valueStr == string.Empty)
                    return defautValue;

                // Return the converted value.
                return result;
            }

            // If the value could not be converted, return the default value.
            return defautValue;
        }

        /// <summary>
        /// Get the first child of this KeyValue with the specified key, and set its value. If the child does not exist, create it.
        /// </summary>
        /// <param name="key"> The key to search for. </param>
        /// <param name="value"> The value to set. </param>
        public void SetChildValue(string key, string value)
        {
            // A keyvalue shouldn't have a value and children.
            if (this.value != null)
            {
                throw new Exception("A KeyValue with a value cannot have children.");
            }

            KeyValue child = GetChildByKey(key);
            if (child != null)
            {
                child.Value = value;
                return;
            }

            AddChild(key, new KeyValue(key, value));
        }

        /// <summary>
        /// Splits a string into an array of substrings, handling quoted and unquoted segments. 
        /// <para>Examples:</para>
        /// <para>"key value" becomes ["key", "value"]</para>
        /// <para>"\"key\" \"value\" [something]" becomes ["key", "value", "[something]"]</para>
        /// <para>"key \"a long value\"" becomes ["key", "a long value"]</para>
        /// <para>"key a long value" becomes ["key", "a", "long", "value"]</para>
        /// </summary>
        /// <param name="fullString">The string to split into chunks.</param>
        /// <returns>An array of substrings.</returns>
        public static string[] GetKeyvalueChunks(string fullString)
        {
            List<string> words = new List<string>();

            // Split the string into parts using quotes as delimiters.
            string[] parts = fullString.Split('\"');

            // Trim the full string.
            fullString = fullString.Trim();

            // Loop through the parts
            for (int i = 0; i < parts.Length; i++)
            {
                if (i % 2 == 1)
                {
                    // between quotes
                    string subpart = parts[i].Replace("\"", string.Empty);
                    words.Add(subpart);
                }
                else
                {
                    string[] subparts = parts[i].Split(null);
                    // outside quotes
                    foreach (string subpart in subparts)
                    {
                        if (subpart != string.Empty && subpart != " ")
                            words.Add(subpart);
                    }
                }
            }
            return words.ToArray();
        }

        /// <summary>
        /// Writes the KeyValue tree to a file in Valve KeyValue file format.
        /// </summary>
        /// <param name="path"> The path to the file to write. </param>
        /// <param name="quotes"> Whether to use quotes around keys and values. </param>
        /// <param name="encoding"> The encoding to use when writing the file. If null, default to UTF-8.</param>
        /// <param name="credits"> Whether to add credits to the file. </param>
        /// <param name="spacesOnly">Steam will only properly read a liblist.gam if you use spaces instead of tabs.</param>
        public void Save(string path, bool quotes = true, Encoding encoding = null, bool credits = true, bool spacesOnly = false)
        {
            // Get the byte array of the KeyValue tree in Valve KeyValue file format.
            byte[] bytes = ToByteArray(quotes, encoding, credits, spacesOnly);

            // If the target directory does not exist, create it.
            string dir = Path.GetDirectoryName(path);
            if (dir != "")
            {
                Directory.CreateDirectory(dir);
            }

            // Make sure the file isn't readonly.
            FileInfo myFile = new FileInfo(path);
            if (myFile != null && File.Exists(path))
                myFile.IsReadOnly = false;

            // Write the byte array to the file.
            File.WriteAllBytes(path, bytes);
        }

        /// <summary>
        /// Writes the KeyValue tree to a byte array in Valve KeyValue file format.
        /// </summary>
        /// <param name="quotes"> Whether to use quotes around keys and values. </param>
        /// <param name="encoding"> The encoding to use when writing the file. If null, default to UTF-8.</param>
        /// <param name="credits"> Whether to add credits to the file. </param>
        /// <param name="spacesOnly"> Steam will only properly read a liblist.gam if you use spaces instead of tabs.</param>
        /// <returns> The KeyValue tree as a byte array in Valve KeyValue file format. </returns>
        public byte[] ToByteArray(bool quotes = true, Encoding encoding = null, bool credits = true, bool spacesOnly = false)
        {
            // If the encoding is null, default to UTF-8.
            if (encoding == null)
                encoding = new UTF8Encoding(false);

            // Convert the KeyValue tree to a string chunk.
            string chunk = ToString(quotes, credits, spacesOnly);

            // Convert the chunk to a byte array using the specified encoding.
            var bytes = encoding.GetBytes(chunk);
            return bytes;
        }

        /// <summary>
        /// Writes the KeyValue tree to a string in Valve KeyValue file format.
        /// </summary>
        /// <param name="quotes"> Whether to use quotes around keys and values. </param>
        /// <param name="credits"> Whether to add credits to the file. </param>
        /// <param name="spacesOnly"> Steam will only properly read a liblist.gam if you use spaces instead of tabs.</param>
        /// <returns> The KeyValue tree as a string in Valve KeyValue file format. </returns>
        public string ToString(bool quotes = true, bool credits = true, bool spacesOnly = false)
        {
            List<string> lines = ToStringTraverse(this, 0, quotes, spacesOnly);

            // Add credits to the file.
            if (credits)
            {
                for (int i = 0; i < DefaultCredits.Length; i++)
                {
                    lines.Insert(i, DefaultCredits[i]);
                }
            }
            return string.Join("\r\n", lines);
        }

        /// <summary>
        /// Traverse through the KeyValue node and its children, writing them to a list of strings in Valve KeyValue file format.
        /// </summary>
        /// <param name="node"> The KeyValue node to write. </param>
        /// <param name="level"> The level of the node in the tree. </param>
        /// <param name="quotes"> Whether to use quotes around keys and values. </param>
        /// <param name="spacesOnly"> Steam will only properly read a liblist.gam if you use spaces instead of tabs.</param>
        /// <returns> A list of strings in Valve KeyValue file format. </returns>
        private static List<string> ToStringTraverse(KeyValue node, int level, bool quotes, bool spacesOnly)
        {
            List<string> lines = new List<string>();

            string tabs = string.Empty;
            if (!spacesOnly)
            {
                for (int i = 0; i < level; i++)
                {
                    tabs = tabs + "\t";
                }
            }

            if (node.HasChildren) // It's a parent key
            {
                if (node.key != string.Empty)
                {
                    lines.Add(tabs + (quotes ? "\"" : string.Empty) + node.key + (quotes ? "\"" : string.Empty));
                    lines.Add(tabs + "{");
                }

                foreach (KeyValue entry in node.Children)
                    lines.AddRange(ToStringTraverse(entry, node.key != string.Empty ? level + 1 : level, quotes, spacesOnly));

                if (node.key != string.Empty)
                    lines.Add(tabs + "}");
            }
            else if (node.Key != string.Empty && node.value != null)   // It's a value key
            {
                string line = tabs;
                if (node.key != string.Empty)
                {
                    var value = node.value;
                    while (value.Contains("\""))
                    {
                        value = value.Replace("\"", "$$quotes$$");
                    }
                    while (value.Contains("$$quotes$$"))
                    {
                        value = value.Replace("$$quotes$$", "\\\"");
                    }

                    line = line +
                        (quotes ? "\"" : string.Empty) +
                        node.key +
                        (quotes ? "\"" : string.Empty) +
                        (spacesOnly ? " \"" : "\t\"") +
                        value +
                        "\"";
                }
                if (node.comment != string.Empty)
                {
                    if (node.key != string.Empty)
                        line = line + (spacesOnly ? " " : "\t");

                    line = line + "//" + node.comment;
                }

                lines.Add(line);
            }
            else if (node.comment != string.Empty)   // Comment line
            {
                string line = tabs + "//" + node.comment;
                lines.Add(line);
            }
            else if (node.key == string.Empty)     // Blank line
            {
                lines.Add("");
            }

            return lines;
        }

        /// <summary>
        /// Get the encoding of a file. If the file does not exist, return the default encoding.
        /// </summary>
        /// <param name="path"> The path to the file. </param>
        /// <returns> The encoding of the file. </returns>
        private static Encoding GetFileEncoding(string path)
        {
            if (File.Exists(path))
            {
                Encoding encoding = null;

                // Open a file stream and get the encoding of the file.
                using (StreamReader r = new StreamReader(path))
                {
                    encoding = r.CurrentEncoding;
                }

                return encoding;
            }

            // If the file does not exist, return the default encoding.
            return new UTF8Encoding(false);
        }

        /// <summary>
        /// Get a KeyValue tree from a file in Valve KeyValue file format.
        /// </summary>
        /// <param name="path"> The path to the file to read. </param>
        /// <param name="hasOSInfo"> Whether the file has OS info. </param>
        /// <returns> The KeyValue tree from the file. </returns>
        public static KeyValue FromFile(string path, bool hasOSInfo = false)
        {
            if (File.Exists(path))
            {
                // Get the encoding of the file
                Encoding encoding = GetFileEncoding(path);
                byte[] byteArray = File.ReadAllBytes(path);
                try
                {
                    return KeyValue.FromByteArray(byteArray, encoding, hasOSInfo);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not read file \"" + path + "\". " + e.Message, "Error reading KeyValue file");
                }

                return null;
            }
            else
            {
                Console.WriteLine("Could not find file \"" + path + "\" to read.", "Error reading KeyValue file");
                return null;
            }
        }

        public static KeyValue FromByteArray(byte[] data, Encoding encoding = null, bool hasOSInfo = false)
        {
            // TODO guess the encoding of the byte data.
            if (encoding == null)
                encoding = Encoding.UTF8;

            // Convert the byte array to a string.
            string chunk = encoding.GetString(data);

            // Parse the string as a KeyValue tree.
            return FromString(encoding.GetString(data), hasOSInfo);
        }

        public static KeyValue FromString(string data, bool hasOSInfo = false)
        {
            // Parse Valve chunkfile format
            List<KeyValue> list = new List<KeyValue>();

            // Remove block comments from data
            data = Regex.Replace(data, "/\\*.*?\\*/", string.Empty, RegexOptions.Singleline);

            list = new List<KeyValue>();
            Stack<KeyValuePair<string, KeyValue>> stack = new Stack<KeyValuePair<string, KeyValue>>();

            // Count the number of { and the number of }. If the number is different, throw an exception.
            int openBrackets = data.Count(t => t == '{');
            int closeBrackets = data.Count(t => t == '}');

            if (openBrackets != closeBrackets)
            {
                throw new FormatException("The number of opening and closing brackets does not match.");
            }

            string[] array = Regex.Split(data, "\r\n|\r|\n");
            for (int i = 0; i < array.Length; i++)
            {
                string line = array[i];

                /* Repeat */
                line = ReplaceLineQuotes(line);
                int lastQuotesCount = -1;
                int quotesCount = line.Count(t => t == '"');
                while (quotesCount % 2 != 0 && quotesCount < 20 && i < array.Length - 1)
                {
                    if (lastQuotesCount == quotesCount)
                    {
                        break;
                    }
                    i++;
                    line = line + "\r\n" + array[i];
                    line = ReplaceLineQuotes(line);
                    quotesCount = line.Count(t => t == '"');
                }

                if (quotesCount % 2 != 0)
                {
                    throw new FormatException("The number of quotes in the line is not even.");
                }

                ProcessChunkLine(line, hasOSInfo, stack, list);
            }

            return RefactorRoot(list);
        }

        private static void ProcessChunkLine(string line, bool hasOSInfo, Stack<KeyValuePair<string, KeyValue>> stack, List<KeyValue> list)
        {
            hasOSInfo = true; // Is there a problem with this?

            line = line.Trim();
            line = Regex.Replace(line, @"\s+", " ");

            // Remove comments of line
            if (line.StartsWith("//") || line.StartsWith("!//"))    // TFC has a liblist.gam with a bad line like this. Just preventing a crash.
                return;

            if (line.Contains("//"))
                line = line.Substring(0, line.IndexOf("//"));

            if (line.Length == 0 || line.Equals(""))
                return;

            string[] words = GetKeyvalueChunks(line);

            // This specific case happened on localization files.
            if (words.Length == 2 && (words[0].Length == 0 || words[0][0] == '﻿'))
            {
                words = new string[] { words[1] };
            }

            // If a line has a curly bracket inline (ex: gameinfo { ), let's split the lines, re-read them and return.
            if (!(line.Contains("\"{") || line.Contains("}\"")) && line.Split('{').Length > 1 && !words[0].Contains("{"))
            {
                try
                {
                    // Found a first case
                    var lineSplit = line.Split('{');
                    for (int i = 0; i < lineSplit.Length; i++)
                    {
                        string substring = lineSplit[i];
                        if (i > 0)
                            substring = "{" + substring;

                        ProcessChunkLine(substring, hasOSInfo, stack, list);
                    }
                }
                catch (Exception e)
                {

                }
            }
            else if (!(line.Contains("\"{") || line.Contains("}\"")) && line.Split('}').Length > 1 && !words[0].Contains("}"))
            {
                try
                {
                    var lineSplit = line.Split('}');
                    for (int i = 0; i < lineSplit.Length; i++)
                    {
                        string substring = lineSplit[i];
                        if (i > 0)
                            substring = "}" + substring;

                        ProcessChunkLine(substring, hasOSInfo, stack, list);
                    }
                }
                catch (Exception e)
                {

                }
            }

            else if (line.StartsWith("//"))  // It's a standalone comment line
            {
                try
                {
                    KeyValue comment = new KeyValue("");
                    comment.comment = line.Substring(2);
                    if (stack.Count > 0)
                        stack.Peek().Value.AddChild(comment);
                    else
                        list.Add(comment);
                }
                catch (Exception e)
                {

                }
            }
            else if (words.Length == 0)    // It's a blank line
            {
                try
                {
                    KeyValue blank = new KeyValue("");
                    if (stack.Count > 0)
                        stack.Peek().Value.AddChild(blank);
                    else
                        list.Add(blank);
                }
                catch (Exception e)
                {

                }
            }
            else if (!(line.Contains("\"{") || line.Contains("}\"")) && words.Length > 0 && words[0].Contains("{")) // It opens a group
            {
                // We actually don't need to do anything.
            }
            else if (!(line.Contains("\"{") || line.Contains("}\"")) && words.Length > 0 && words[0].Contains("}"))    // It closes a group
            {
                if (stack.Count > 0)
                {
                    KeyValuePair<string, KeyValue> child = stack.Pop();

                    // Ignore anything that isn't WIN32, so only accept: [$WIN32], [!$OSX], [!$X360] [$WINDOWS] [!$LINUX] [!$POSIX]
                    if (child.Value.OSData != null && child.Value.OSData != "[$WIN32]" && child.Value.OSData != "[!$OSX]" && child.Value.OSData != "[!$X360]" &&
                        child.Value.OSData != "[$WINDOWS]" && child.Value.OSData != "[!$LINUX]" && child.Value.OSData != "[!$POSIX]")
                    {
                        return;
                    }

                    if (stack.Count > 0)
                        stack.Peek().Value.AddChild(child.Key, child.Value);
                    else
                        list.Add(child.Value);
                }
                else
                {
                    throw new FormatException("Trying to close a group with a '}' when there is no group to close.");
                }
            }
            else if (words.Length == 1 || words.Length > 1 && words[1].StartsWith("//"))    // It's a parent key
            {
                try
                {
                    line = line.Replace("\"", string.Empty);
                    if (!KeyCasing)
                        line = line.ToLower();
                    KeyValue parent = new KeyValue(line);
                    stack.Push(new KeyValuePair<string, KeyValue>(line, new KeyValue(line)));
                }
                catch (Exception e)
                {

                }
            }
            else if (words.Length == 2 && hasOSInfo && words[1].StartsWith("[") && words[1].EndsWith("]")) // It's a parent key with OS data
            {
                // It's opening a group with OS info
                // "1" [$WIN32]
                try
                {
                    line = line.Replace("\"", string.Empty);
                    KeyValue parent = new KeyValue(line);
                    parent.OSData = words[1];
                    stack.Push(new KeyValuePair<string, KeyValue>(line, parent));
                }
                catch (Exception e)
                {

                }
            }
            else if (words.Length >= 2) // It's a value key
            {
                try
                {
                    string key = words[0].Replace("\"", string.Empty);
                    if (!KeyCasing)
                        key = key.ToLower();

                    words[1] = words[1].Replace("$$quotes$$", "\"");
                    KeyValue value = new KeyValue(key, words[1]);

                    // HudHealth [$WIN32]
                    if (words.Length > 2 && hasOSInfo && words[2].StartsWith("[") && words[2].EndsWith("]"))
                    {
                        value.OSData = words[2];

                        // Ignore anything that isn't WIN32, so only accept: [$WIN32], [!$OSX], [!$X360]
                        if (value.OSData != "[$WIN32]" && value.OSData != "[$WINDOWS]" && value.OSData != "[!$OSX]" && value.OSData != "[!$X360]" &&
                            value.OSData != "[!$LINUX]" && value.OSData != "[!$POSIX]")
                        {
                            return;
                        }
                    }


                    if (stack.Count > 0)
                        stack.Peek().Value.AddChild(key, value);
                    else
                        list.Add(value);
                }
                catch (Exception e)
                {

                }

            }
        }

        private static string ReplaceLineQuotes(string line)
        {
            while (line.Contains("\\\""))
            {
                line = line.Replace("\\\"", "$$quotes$$");
            }
            return line;
        }

        /// <summary>
        /// Refactors a list of KeyValues into a single root KeyValue. If there is only one KeyValue in the list, it is returned as is.
        /// </summary>
        /// <param name="list"> The list of KeyValues to refactor. </param>
        /// <returns> The root KeyValue. </returns>
        private static KeyValue RefactorRoot(List<KeyValue> list)
        {
            if (list.Count > 1)
            {
                // Create a root KeyValue and add all the KeyValues in the list as children.
                KeyValue root = new KeyValue(string.Empty);
                foreach (KeyValue keyValue in list)
                    root.AddChild(keyValue);

                return root;
            }
            else if (list.Count == 1)
            {
                // If there is only one KeyValue in the list, return it.
                return list[0];
            }
            else
            {
                // If the list is empty, return null.
                return null;
            }
        }
    }
}
