using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Conllu.Enums;
using Conllu.Extensions;

namespace Conllu
{
    public class Token
    {
        /// <summary>
        /// The ID of the token
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The end of the ID range in case of a multiword token
        /// </summary>
        public int? RangeId { get; set; }
        
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
        public Dictionary<int, string> Deps { get; set; } = new Dictionary<int, string>();
        
        /// <summary>
        /// Any other annotation
        /// </summary>
        public string Misc { get; set; }
        
        /// <summary>
        /// The raw text line of the token
        /// </summary>
        public string RawLine { get; private set; }
        
        /// <summary>
        /// Whether the token spans multiple words
        /// </summary>
        public bool IsMultiwordToken => RangeId != null;

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
                return null;

            // Create new token
            var t = new Token(line);
            
            var index = comps[0].Split("-");
            t.Id = int.Parse(index[0]);
            if (index.Length == 2)
                t.RangeId = int.Parse(index[1]);

            t.Form = comps[1].ValueOrNull();
            t.Lemma = comps[2].ValueOrNull();
            t.Upos = comps[3].ValueOrNull();
            t.Xpos = comps[4].ValueOrNull();
            t.Feats = ParseMultiValueField(comps[5], "=", k => k, v => v);

            if (int.TryParse(comps[6], out var head))
                t.Head = head;

            t.DepRel = comps[7].ValueOrNull();
            t.Deps = ParseMultiValueField(comps[8], ":", int.Parse, v => v);
            t.Misc = comps[9].ValueOrNull();
            
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