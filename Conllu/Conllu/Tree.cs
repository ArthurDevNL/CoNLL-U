using System;
using System.Collections.Generic;
using System.Linq;
using Conllu.Extensions;

namespace Conllu
{
    public class Tree<TVertex, TConnection>: IComparable<Tree<TVertex, TConnection>>
        where TVertex: IComparable<TVertex>
    {
        public SortedDictionary<Tree<TVertex, TConnection>, TConnection> Connections { get; }

        public IEnumerable<Tree<TVertex, TConnection>> Children => Connections.Keys;
        
        public TVertex Value { get; set; }

        public bool IsLeaf => Connections.IsNullOrEmpty();

        public Tree(TVertex value, SortedDictionary<Tree<TVertex, TConnection>, TConnection> connections = null)
        {
            Value = value;
            Connections = connections ?? new SortedDictionary<Tree<TVertex, TConnection>, TConnection>();
        }

        public void AddChild(Tree<TVertex, TConnection> child, TConnection connection)
        {
            Connections.Add(child, connection);
        }

        /// <summary>
        /// Traverses the tree depth-first to find elements matching the predicate. 
        /// </summary>
        /// <param name="predicate">The predicate to fulfill</param>
        /// <returns>Any elements that match the predicate in a depth-first search manner</returns>
        public IEnumerable<Tree<TVertex, TConnection>> WhereDfs(Func<TVertex, bool> predicate)
        {
            var s = new Stack<Tree<TVertex, TConnection>>();
            s.Push(this);

            while (s.TryPop(out var node))
            {
                if (predicate(node.Value))
                    yield return node;

                foreach (var child in node.Children)
                    s.Push(child);
            }
        }

        /// <summary>
        /// Traverses the tree breadth-first to find elements matching the predicate. 
        /// </summary>
        /// <param name="predicate">The predicate to fulfill</param>
        /// <returns>Any elements that match the predicate in a breadth-first search manner</returns>
        public IEnumerable<Tree<TVertex, TConnection>> WhereBfs(Func<TVertex, bool> predicate)
        {
            var s = new Queue<Tree<TVertex, TConnection>>();
            s.Enqueue(this);

            while (s.TryDequeue(out var node))
            {
                if (predicate(node.Value))
                    yield return node;

                foreach (var child in node.Children)
                    s.Enqueue(child);
            }
        }

        /// <summary>
        /// Compares the values to one another
        /// </summary>
        public int CompareTo(Tree<TVertex, TConnection> other)
        {
            return Value.CompareTo(other.Value);
        }

        public override string ToString()
        {
            var s = $"{Value.ToString()}";
            return Connections.Aggregate(s, (current, kvp) => current + kvp.Key.ToString(1, kvp.Value));
        }
        
        private string ToString(int depth, TConnection connection)
        {
            var prefix = new string('\t', depth);
            if (IsLeaf)
                return $"\n{prefix}{Value.ToString()} ({connection})";

            var result = depth == 0 ? $"{prefix}{Value.ToString()} ({connection})" : $"\n{prefix}{Value.ToString()} ({connection})";
            return Connections.Aggregate(result, (current, kvp) => current + kvp.Key.ToString(depth + 1, kvp.Value));
        }
    }
}