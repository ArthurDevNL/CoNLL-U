using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Conllu.Extensions;

namespace Conllu
{
    public static class ConlluParser
    {
        /// <summary>
        /// Read and parse a CoNLL-U file
        /// </summary>
        /// <param name="filePath">The path to the file to read</param>
        /// <returns>An enumerable of sentences parsed from the file</returns>
        public static IEnumerable<Sentence> ParseFile(string filePath)
            => Parse(File.ReadLines(filePath));
        
        /// <summary>
        /// Parse the given string
        /// </summary>
        /// <param name="text">The text to parse (should be in CoNLL-U format)</param>
        /// <returns>An enumerable of sentences parsed from the text</returns>
        public static IEnumerable<Sentence> ParseText(string text)
            => Parse(text.SplitLines());

        /// <summary>
        /// Parses an enumerable of lines to an enumerable of sentences
        /// </summary>
        /// <param name="lines">The individual lines of the CoNLL-U data to parse</param>
        /// <returns>An enumerable of sentences parsed from the input</returns>
        /// <exception cref="Exception">When the input data cannot be parsed correctly (invalid format)</exception>
        public static IEnumerable<Sentence> Parse(IEnumerable<string> lines)
        {
            var i = 0;
            var sentence = new Sentence();
            foreach (var line in lines)
            {
                // Finished reading sentence
                if (line == "" && !sentence.IsEmpty())
                {
                    yield return sentence;
                    sentence = new Sentence();
                }

                // Check if the comments contain metadata if we haven't started parsing the sentence yet
                if (line.StartsWith("#") && sentence.IsEmpty())
                {
                    var comps = line.TrimStart('#').Split("=");
                    if (comps.Length == 2)
                        sentence.Metadata[comps[0].Trim()] = comps[1].Trim();
                }

                try
                {
                    var t = Token.FromLine(line);
                    if (t != null)
                        sentence.Tokens.Add(t);
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed parsing line {i}: {e.Message}");
                }

                i += 1;
            }

            if (!sentence.IsEmpty())
                yield return sentence;
        }

        /// <summary>
        /// Serializes an enumerable of sentences into a string that can be written to a CoNLL-U file.
        /// </summary>
        /// <param name="sentences">The sentences to serialize</param>
        /// <returns>The given sentences in a CoNLL-U text format</returns>
        public static string Serialize(IEnumerable<Sentence> sentences)
            => string.Join('\n', sentences.Select(s => s.Serialize()));
    }
}