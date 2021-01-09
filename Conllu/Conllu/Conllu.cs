using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Conllu
{
    public class Conllu
    {
        public static IEnumerable<Sentence> ParseFile(string filePath)
            => Parse(File.ReadLines(filePath));
        
        public static IEnumerable<Sentence> ParseText(string text)
            => Parse(text.Split(
                new[] {"\r\n", "\r", "\n"},
                StringSplitOptions.None
            ));

        public static IEnumerable<Sentence> Parse(IEnumerable<string> lines)
        {
            var i = 0;
            var sentence = new Sentence();
            foreach (var line in lines)
            {
                // Finished reading sentence
                if (line == "")
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
                
                var t = Token.FromLine(line);
                if (t != null)
                    sentence.Tokens.Add(t);

                i += 1;
            }

            if (!sentence.IsEmpty())
                yield return sentence;
        }
    }
}