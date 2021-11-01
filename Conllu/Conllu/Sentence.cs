using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conllu.Enums;

namespace Conllu
{
    public class Sentence
    {
        /// <summary>
        /// The list of tokens in the sentence
        /// </summary>
        public List<Token> Tokens { get; } = new();

        /// <summary>
        /// The metadata found in the comments before the sentence
        /// </summary>
        public Dictionary<string, string> Metadata { get; } = new();
        
        public bool IsEmpty() => !Tokens.Any();

        public Sentence()
        {
            
        }

        public Sentence(List<Token> tokens, Dictionary<string, string> metadata = null)
        {
            Tokens = tokens;
            Metadata = metadata ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Constructs a dependency tree from the tokens in the sentence. It will only create the tree from the nodes that have a valid head and dependency relation
        /// </summary>
        /// <returns></returns>
        public Tree<Token, DependencyRelation> AsDependencyTree()
        {
            var map = RawTokens().ToDictionary(t => t.Id, t => t.DepRelEnum.HasValue ? new Tree<Token, DependencyRelation>(t, t.DepRelEnum.Value) : null);
            Tree<Token, DependencyRelation> root = null;
            foreach (var (id, node) in map)
            {
                if (node == null)
                    continue;
                
                if (node.Value.Head == 0 || node.Value.DepRelEnum == DependencyRelation.Root)
                {
                    root = node;
                    continue;
                }

                if (node.Value.Head == null || !map.ContainsKey(node.Value.Head.Value) || node.Value.DepRelEnum == null)
                    continue;

                var parent = map[node.Value.Head.Value];
                parent.AddChild(node);
            }
            
            return root;
        }

        /// <summary>
        /// Returns the list of tokens that make up the raw sentence (ie all non empty nodes and multi word tokens)
        /// </summary>
        public List<Token> RawTokens()
        {
            var ts = new List<Token>();
            var tokenMap = Tokens.GroupBy(t => t.Id).ToDictionary(g => g.Key, g => g.ToList());
            var i = 1;
            while (i <= Tokens.Max(t => t.Id))
            {
                var t = tokenMap[i].First();
                ts.Add(t);
                
                if (t.IsMultiwordToken)
                    i = t.Identifier.SpanId.Value;
                else
                    i += 1;
            }

            return ts;
        }

        /// <summary>
        /// Returns the flat text of the sentence. It either returns the text from the metadata if available or a concatenation of the non empty nodes. 
        /// </summary>
        public string RawTokenSequence()
        {
            if (Metadata.ContainsKey("text"))
                return Metadata["text"];

            var s = new StringBuilder();
            var ts = RawTokens();
            for (var i = 0; i < ts.Count; i++)
            {
                var t = ts[i];
                s.Append(t.Form);

                // Don't append space if MISC contains SpaceAfter=No or if it is the last token
                if (!(t.Misc ?? "").ToLower().Contains("spaceafter=no") && i != ts.Count - 1)
                    s.Append(" ");
            }

            return s.ToString();
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