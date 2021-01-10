using System.Collections.Generic;
using System.Linq;
using Conllu.Enums;

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

        public Sentence()
        {
            
        }

        public Sentence(List<Token> tokens, Dictionary<string, string> metadata = null)
        {
            Tokens = tokens;
            Metadata = metadata ?? new Dictionary<string, string>();
        }

        public Tree<Token, DependencyRelation> AsDependencyTree()
        {
            // TODO
            return null;
        }

        public string Serialize()
        {
            var result = "";
            result += string.Join('\n', Metadata.Select(kvp => $"# {kvp.Key} = {kvp.Value}"));
            result += "\n";
            result += string.Join('\n', Tokens.Select(t => t.Serialize()));
            result += "\n\n";
            return result;
        }
    }
}