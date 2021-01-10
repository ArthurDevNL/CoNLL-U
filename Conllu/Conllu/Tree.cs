using System;
using System.Collections.Generic;
using System.Linq;
using Conllu.Extensions;

namespace Conllu
{
    public class Tree<TVertex, TConnection>
    {
        public SortedDictionary<Tree<TVertex, TConnection>, TConnection> Connections { get; set; }

        public IEnumerable<Tree<TVertex, TConnection>> Children => Connections.Keys;
        
        public TVertex Value { get; set; }

        public bool IsLeaf => Connections.IsNullOrEmpty();

        public Tree(TVertex value, SortedDictionary<Tree<TVertex, TConnection>, TConnection> connections = null)
        {
            Value = value;
            Connections = connections;
        }

        /// <summary>
        /// Traverses the tree depth-first to find elements matching the predicate. 
        /// </summary>
        /// <param name="predicate">The predicate to fullfil</param>
        /// <returns>Any elements that match the predicate in a depth-first search manner</returns>
        public IEnumerable<Tree<TVertex, TConnection>> WhereDfs(Func<TVertex, bool> predicate)
        {
            if (predicate(Value))
                yield return this;

            foreach (var result in Connections.SelectMany(kvp => kvp.Key.WhereDfs(predicate)))
            {
                yield return result;
            }
        }

        public IEnumerable<Tree<TVertex, TConnection>> WhereBfs(Func<TVertex, bool> predicate)
        {
            if (predicate(Value))
                yield return this;
            
            // TODO
        }

        public override string ToString()
            => ToString(0);

        private string ToString(int depth)
        {
            var prefix = new string('\t', depth);
            if (IsLeaf)
                return $"\n{prefix}{Value.ToString()}";

            var result = depth == 0 ? $"{prefix}{Value.ToString()}" : $"\n{prefix}{Value.ToString()}";
            return Children.Aggregate(result, (current, kvp) => current + kvp.ToString(depth + 1));
        }
    }
}