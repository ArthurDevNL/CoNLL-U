using System;
using System.Collections.Generic;
using Conllu.Enums;
using Conllu.Extensions;

namespace Conllu
{
    public class Token
    {
        /// <summary>
        /// The main ID of the token. Quick accessor to <see cref="Index"/>
        /// </summary>
        public int Id => Index.Id;
        
        /// <summary>
        /// The complex token index containing the ID, possible span ID or empty node ID
        /// </summary>
        public TokenIndex Index { get; set; }

        /// <summary>
        /// Word form or punctuation symbol
        /// </summary>
        public string Form { get; set; }
        
        /// <summary>
        /// Lemma or stem of word form
        /// </summary>
        public string Lemma { get; set; }
        
        /// <summary>
        /// Universal part-of-speech tag
        /// </summary>
        public string Upos { get; set; }

        /// <summary>
        /// A parsed enum version of the Upos field
        /// </summary>
        public PosTag? UposEnum => Enum.TryParse<PosTag>(Upos, true, out var tag) ? tag : (PosTag?) null;
        
        /// <summary>
        /// Language-specific part-of-speech tag
        /// </summary>
        public string Xpos { get; set; }

        /// <summary>
        /// List of morphological features from the universal feature inventory or from a defined language-specific extension
        /// </summary>
        public Dictionary<string, string> Feats { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// Head of the current word, which is either a value of ID or zero (0)
        /// </summary>
        public int? Head { get; set; }
        
        /// <summary>
        /// Universal dependency relation to the HEAD (root iff HEAD = 0) or a defined language-specific subtype of one
        /// </summary>
        public string DepRel { get; set; }

        /// <summary>
        /// A parsed enum version of DepRel
        /// </summary>
        public DependencyRelation? DepRelEnum => Enum.TryParse<DependencyRelation>(DepRel, true, out var r) ? r : (DependencyRelation?) null;

        /// <summary>
        /// Enhanced dependency graph in the form of a list of head-deprel pairs
        /// </summary>
        public Dictionary<TokenIndex, string> Deps { get; set; } = new Dictionary<TokenIndex, string>();
        
        /// <summary>
        /// Any other annotation
        /// </summary>
        public string Misc { get; set; }
        
        /// <summary>
        /// The raw text line of the token
        /// </summary>
        public string RawLine { get; private set; }

        /// <summary>
        /// Whether the token spans multiple words. Utility method for <see cref="Index"/>
        /// </summary>
        public bool IsMultiwordToken => Index.IsMultiwordIndex;

        /// <summary>
        /// Whether the token is an empty node. Utility method for <see cref="Index"/>
        /// </summary>
        public bool IsEmptyNode => Index.IsEmptyNode;
        
        public override string ToString()
            => Form;

        public Token(string line)
        {
            RawLine = line;
        }
        
        public static Token FromLine(string line)
        {
            // Comment or empty line
            if (line.Trim() == "" || line.StartsWith("#"))
                return null;
            
            var comps = line.Split("\t");
            
            // Invalid number of fields
            if (comps.Length != 10)
                throw new Exception($"Invalid number of fields ({comps.Length}, should be 10) found for token line '{line}'.");

            // Create new token
            var t = new Token(line)
            {
                Index = new TokenIndex(comps[0]),
                Form = comps[1].ValueOrNull(),
                Lemma = comps[2].ValueOrNull(),
                Upos = comps[3].ValueOrNull(),
                Xpos = comps[4].ValueOrNull(),
                Feats = ParseMultiValueField(comps[5], "=", k => k, v => v),
                Head = int.TryParse(comps[6], out var head) ? head : (int?)null,
                DepRel = comps[7].ValueOrNull(),
                Deps = ParseMultiValueField(comps[8], ":", k => new TokenIndex(k), v => v),
                Misc = comps[9].ValueOrNull(),
            };

            return t;
        }

        private static Dictionary<TKey, TValue> ParseMultiValueField<TKey, TValue>(string field, string keyValueSeparator, Func<string, TKey> keyConverter, Func<string, TValue> valueConverter)
        {
            var result = new Dictionary<TKey, TValue>();
            if (field == "_")
                return result;

            var items = field.Split("|");
            foreach (var item in items)
            {
                var comps = item.Split(keyValueSeparator);
                result[keyConverter(comps[0])] = comps.Length > 1 ? valueConverter(comps[1]) : default;
            }

            return result;
        }
    }
}