using System.Collections.Generic;
using System.Linq;
using Conllu.Extensions;

namespace Conllu
{
    internal readonly struct Node<T>
    {
        public readonly Node<T>[] Children { get; }
        
        public readonly T Value { get; }
        
        public bool IsLeaf => Children.IsNullOrEmpty();

        public Node(T value, IEnumerable<Node<T>> children = null)
        {
            Value = value;
            Children = children.EmptyIfNull().ToArray();
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                return IsLeaf
                    ? Value.GetHashCode()
                    : Children.EmptyIfNull().Aggregate(19, (current, s) => s.GetHashCode() * 31 * current);
            }
        }
        
        public override bool Equals(object obj)
        {
            if (!(obj is Node<T> item))
                return false;
            
            // Check one is a leaf and the other one isn't, return false
            if (IsLeaf && !item.IsLeaf || !IsLeaf && item.IsLeaf)
                return false;

            // If both are leafs, compare their values
            if (IsLeaf && item.IsLeaf)
                return Value.Equals(item.Value);

            return Value.Equals(item.Value) && item.Children.SequenceEqual(Children);
        }
        
        public override string ToString()
            => ToString(0);

        private string ToString(int depth)
        {
            var prefix = new string('\t', depth);
            if (IsLeaf)
                return $"\n{prefix}{Value.ToString()}";

            var result = depth == 0 ? $"{prefix}{Value.ToString()}" : $"\n{prefix}{Value.ToString()}";
            foreach (var child in Children)
                result += child.ToString(depth + 1);
            
            return result;
        }
    }
}