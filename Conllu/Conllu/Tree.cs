using System;
using System.Collections.Generic;
using System.Linq;
using Conllu.Extensions;

namespace Conllu
{
    public class Tree<TVertex, TConnection>: IComparable<Tree<TVertex, TConnection>>
        where TVertex: IComparable<TVertex>
    {
        public List<Tree<TVertex, TConnection>> Children { get; init; }
        
        public TVertex Value { get; init; }
        
        public TConnection Connection { get; init; }

        public bool IsLeaf => Children.IsNullOrEmpty();

        public Tree(TVertex value, TConnection connection, List<Tree<TVertex, TConnection>> connections = null)
        {
            Value = value;
            Connection = connection;
            Children = connections ?? new List<Tree<TVertex, TConnection>>();
        }

        public void AddChild(Tree<TVertex, TConnection> child)
        {
            Children.Add(child);
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

        public override int GetHashCode()
        {
            return HashCode.Combine(Value.GetHashCode(), Connection.GetHashCode(), Children.GetHashCode());
        }

        public override bool Equals(object? obj)
        {
            if (obj is Tree<TVertex, TConnection> t)
                return Value.Equals(t.Value) && Connection.Equals(t.Connection) && Children.SequenceEqual(t.Children);
                
            return false;
        }

        public override string ToString()
        {
            var s = $"{Value.ToString()} ({Connection})";
            return Children.Aggregate(s, (current, kvp) => current + kvp.ToString(1));
        }
        
        private string ToString(int depth)
        {
            var prefix = new string('\t', depth);
            if (IsLeaf)
                return $"\n{prefix}{Value.ToString()} ({Connection})";

            var result = depth == 0 ? $"{prefix}{Value.ToString()} ({Connection})" : $"\n{prefix}{Value.ToString()} ({Connection})";
            return Children.Aggregate(result, (current, kvp) => current + kvp.ToString(depth + 1));
        }
    }
}