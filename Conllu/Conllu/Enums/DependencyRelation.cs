namespace Conllu.Enums
{
    /// <summary>
    /// The list of Universal Dependency Relations. For more information,
    /// see https://universaldependencies.org/u/dep/index.html
    /// </summary>
    public enum DependencyRelation
    {
        /// <summary>
        /// Clausal modifier of noun (adjectival clause)
        /// </summary>
        Acl,
        /// <summary>
        /// adverbial clause modifier
        /// </summary>
        Advcl, // 
        /// <summary>
        /// Adverbial modifier
        /// </summary>
        Advmod, //
        /// <summary>
        /// Adjectival modifier
        /// </summary>
        Amod, // 
        /// <summary>
        /// Appositional modifier
        /// </summary>
        Appos, //
        /// <summary>
        /// Auxiliary
        /// </summary>
        Aux, // 
        /// <summary>
        /// Case marking
        /// </summary>
        Case, //
        /// <summary>
        /// Coordinating conjunction
        /// </summary>
        Cc, 
        /// <summary>
        /// Clausal complement
        /// </summary>
        Ccomp,
        /// <summary>
        /// Classifier
        /// </summary>
        Clf, 
        /// <summary>
        /// Compound
        /// </summary>
        Compound,
        /// <summary>
        /// Conjunct
        /// </summary>
        Conj,
        /// <summary>
        /// Copula
        /// </summary>
        Cop,
        /// <summary>
        ///  Clausal subject
        /// </summary>
        Csubj,
        /// <summary>
        /// Unspecified dependency
        /// </summary>
        Dep,
        /// <summary>
        /// Determiner
        /// </summary>
        Det,
        /// <summary>
        /// Discourse element
        /// </summary>
        Discourse,
        /// <summary>
        /// Dislocated elements
        /// </summary>
        Dislocated,
        /// <summary>
        /// Expletive
        /// </summary>
        Expl,
        /// <summary>
        /// Fixed multiword expression
        /// </summary>
        Fixed,
        /// <summary>
        /// Flat multiword expression
        /// </summary>
        Flat,
        /// <summary>
        ///  Goes with
        /// </summary>
        Goeswith,
        /// <summary>
        /// Indirect object
        /// </summary>
        Iobj,
        /// <summary>
        /// List
        /// </summary>
        List,
        /// <summary>
        /// Marker
        /// </summary>
        Mark,
        /// <summary>
        /// Nominal modifier
        /// </summary>
        Nmod,
        /// <summary>
        /// Nominal subject
        /// </summary>
        Nsubj,
        /// <summary>
        /// Numeric modifier
        /// </summary>
        Nummod,
        /// <summary>
        /// Object
        /// </summary>
        Obj,
        /// <summary>
        /// Oblique nominal
        /// </summary>
        Obl,
        /// <summary>
        /// Orphan
        /// </summary>
        Orphan,
        /// <summary>
        /// Parataxis
        /// </summary>
        Parataxis,
        // Punctuation
        Punct,
        /// <summary>
        /// Overridden disfluency
        /// </summary>
        Reparandum,
        /// <summary>
        /// Root
        /// </summary>
        Root,
        /// <summary>
        /// Vocative
        /// </summary>
        Vocative,
        /// <summary>
        /// Open clausal complement
        /// </summary>
        Xcomp
    }
}