using System.Collections.Generic;
using System.Linq;

namespace Conllu
{
    public class Sentence
    {
        /// <summary>
        /// The list of tokens in the sentence
        /// </summary>
        public List<Token> Tokens { get; } = new List<Token>();

        /// <summary>
        /// The metadata found in the comments before the sentence
        /// </summary>
        public Dictionary<string, string> Metadata { get; } = new Dictionary<string, string>();
        
        public bool IsEmpty() => !Tokens.Any();
    }
}