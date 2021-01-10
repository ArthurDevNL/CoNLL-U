using System.Collections;

namespace Conllu
{
    public class TokenIdentifier
    {
        /// <summary>
        /// The ID of the token
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// In the case of a span ID (e.g. 1-2 or 3-5), the span ID is the second number of the ID
        /// </summary>
        public int? SpanId { get; set; }
        
        /// <summary>
        /// In the case of an empty node ID (e.g. 1.2 or 3.5), the sub ID is the second number in the ID  
        /// </summary>
        public int? SubId { get; set; }
        
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
    }
}