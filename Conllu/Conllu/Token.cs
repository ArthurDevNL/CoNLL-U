using System;
using System.Collections.Generic;
using System.Linq;
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
        public PosTag? Upos { get; set; }
        
        /// <summary>
        /// Language-specific part-of-speech tag
        /// </summary>
        public string Xpos { get; set; }
        
        /// <summary>
        /// List of morphological features from the universal feature inventory or from a defined language-specific extension
        /// </summary>
        public List<string> Feats { get; set; }
        
        /// <summary>
        /// Head of the current word, which is either a value of ID or zero (0)
        /// </summary>
        public int? Head { get; set; }
        
        /// <summary>
        /// Universal dependency relation to the HEAD (root iff HEAD = 0) or a defined language-specific subtype of one
        /// </summary>
        public DependencyRelation? DepRel { get; set; }
        
        /// <summary>
        /// Enhanced dependency graph in the form of a list of head-deprel pairs
        /// </summary>
        public string Deps { get; set; }
        
        /// <summary>
        /// Any other annotation
        /// </summary>
        public string Misc { get; set; }
        
        /// <summary>
        /// The raw text line of the token
        /// </summary>
        public string RawLine { get; set; }
        
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
            t.Upos = Enum.Parse<PosTag>(comps[3], true);
            t.Xpos = comps[4].ValueOrNull();
            t.Feats = comps[5].Split(".").ToList();

            if (int.TryParse(comps[6], out var head))
                t.Head = head;

            t.DepRel = Enum.Parse<DependencyRelation>(comps[7], true);
            t.Deps = comps[8].ValueOrNull();
            t.Misc = comps[9].ValueOrNull();
            
            return t;
        }
    }
}