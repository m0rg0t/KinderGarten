using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace CSVParserWinRT.ParserEngines
{
    /// <summary>Parses escaped fields within a single CSV record.</summary>
    public sealed class BetterParserEngine : IParserEngine
    {
        public IAsyncOperation<IList<string>> ExtractFields(char delimiter, char quote, string csvLine)
        {
            // Keep options local: callers can use this engine concurrently.
            return Task.Run<IList<string>>(() => ParseFields(delimiter, quote, csvLine)).AsAsyncOperation();
        }

        public IAsyncOperation<IList<string>> ExtractRecords(char lineDelimiter, string csvText)
        {
            // Multiline quoted records remain outside this engine's contract.
            return Task.Run<IList<string>>(() => new List<string>(csvText.Split(lineDelimiter))).AsAsyncOperation();
        }

        private static IList<string> ParseFields(char delimiter, char quote, string csvLine)
        {
            if (csvLine == null) throw new ArgumentNullException("csvLine");
            var fields = new List<string>();
            string line = csvLine.TrimEnd('\r', '\n');
            // Ignore blank records, including the one after a terminal newline.
            if (line.Length == 0) return fields;

            var field = new StringBuilder();
            bool inQuotes = false;
            bool closedQuote = false;
            for (int i = 0; i < line.Length; i++)
            {
                char current = line[i];
                if (inQuotes)
                {
                    if (current == quote)
                    {
                        if (i + 1 < line.Length && line[i + 1] == quote)
                        {
                            field.Append(quote);
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                            closedQuote = true;
                        }
                    }
                    else field.Append(current);
                }
                else if (current == delimiter)
                {
                    fields.Add(field.ToString());
                    field.Clear();
                    closedQuote = false;
                }
                else if (closedQuote)
                {
                    throw new FormatException("Unexpected text after a quoted CSV field.");
                }
                else if (current == quote && field.Length == 0)
                {
                    inQuotes = true;
                }
                else field.Append(current);
            }
            if (inQuotes) throw new FormatException("Unterminated quoted CSV field.");
            // Empty fields are significant, including the field after a final delimiter.
            fields.Add(field.ToString());
            return fields;
        }
    }
}
