using System;

namespace Conllu
{
    public class TokenIdentifier: IComparable<TokenIdentifier>
    {
        /// <summary>
        /// The ID of the token
        /// </summary>
        public int Id { get; init; }
        
        /// <summary>
        /// In the case of a span ID (e.g. 1-2 or 3-5), the span ID is the second number of the ID
        /// </summary>
        public int? SpanId { get; init; }
        
        /// <summary>
        /// In the case of an empty node ID (e.g. 1.2 or 3.5), the sub ID is the second number in the ID  
        /// </summary>
        public int? SubId { get; init; }
        
        public bool IsMultiwordIndex => SpanId != null;

        public bool IsEmptyNode => SubId != null;

        /// <summary>
        /// Parses a new token identifier
        /// </summary>
        /// <param name="index">Should either be a single integer, or two integers separated by a dash or a dot (1, 1-2, 1.2)</param>
        public TokenIdentifier(string index)
        {
            if (index.Contains("."))
            {
                var comps = index.Split(".");
                Id = int.Parse(comps[0]);
                SubId = int.Parse(comps[1]);
            } 
            else if (index.Contains("-"))
            {
                var comps = index.Split("-");
                Id = int.Parse(comps[0]);
                SpanId = int.Parse(comps[1]);
            }
            else
            {
                Id = int.Parse(index);
            }
        }

        public string Serialize()
        {
            if (IsMultiwordIndex)
                return $"{Id}-{SpanId}";
            if (IsEmptyNode)
                return $"{Id}.{SubId}";

            return $"{Id}";
        }
        
        /// <summary>
        /// Tokens are sorted by their ID. Between tokens with the same ID, the order is: multi word (by span id), ID and them empty nodes bu sub ID.
        /// </summary>
        public int CompareTo(TokenIdentifier other)
        {
            if (Id == other.Id)
            {
                if (IsMultiwordIndex)
                {
                    if (other.IsMultiwordIndex && SpanId.HasValue && other.SpanId.HasValue)
                        return SpanId.Value.CompareTo(other.SpanId.Value);
                    return -1; // The other is not a multi word
                }

                if (IsEmptyNode)
                {
                    if (other.IsEmptyNode && SubId.HasValue && other.SubId.HasValue)
                        return SubId.Value.CompareTo(other.SubId.Value);
                    return 1; // The other is not an empty node
                }

                return 0;
            }

            return Id.CompareTo(other.Id);
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is TokenIdentifier ti)
                return CompareTo(ti) == 0;
            
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, SpanId, SubId);
        }

        public static bool operator >(TokenIdentifier li, TokenIdentifier ri)
            => li?.CompareTo(ri) > 0;

        public static bool operator <(TokenIdentifier li, TokenIdentifier ri)
            => li?.CompareTo(ri) < 0;

        public static bool operator >=(TokenIdentifier li, TokenIdentifier ri)
            => li?.CompareTo(ri) >= 0;

        public static bool operator <=(TokenIdentifier li, TokenIdentifier ri)
            => li?.CompareTo(ri) <= 0;

        public static bool operator ==(TokenIdentifier li, TokenIdentifier ri)
            => li?.CompareTo(ri) == 0;

        public static bool operator !=(TokenIdentifier li, TokenIdentifier ri)
            => li?.CompareTo(ri) != 0;

        /// <summary>
        /// Returns whether the id is the same or in the range of the span in case of a multi word index
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsInRange(int id)
        {
            if (!IsMultiwordIndex)
                return id == Id;

            // ReSharper disable once PossibleInvalidOperationException
            return id >= Id && id <= SpanId.Value;
        }
    }
}