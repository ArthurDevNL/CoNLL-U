namespace Conllu.Enums
{
    /// <summary>
    /// The list of Universal Dependency Relations. For more information,
    /// see https://universaldependencies.org/u/dep/index.html
    /// </summary>
    public enum DependencyRelation
    {
        Acl, // clausal modifier of noun (adjectival clause)
        Advcl, // adverbial clause modifier
        Advmod, // adverbial modifier
        Amod, // adjectival modifier
        Appos, // appositional modifier
        Aux, // auxiliary
        Case, // case marking
        Cc, // coordinating conjunction
        Ccomp, // clausal complement
        Clf, // classifier
        Compound, // compound
        Conj, // conjunct
        Cop, // copula
        Csubj, // clausal subject
        Dep, // unspecified dependency
        Det, // determiner
        Discourse, // discourse element
        Dislocated, // dislocated elements
        Expl, // expletive
        Fixed, // fixed multiword expression
        Flat, // flat multiword expression
        Goeswith, // goes with
        Iobj, // indirect object
        List, // list
        Mark, // marker
        Nmod, // nominal modifier
        Nsubj, // nominal subject
        Nummod, // numeric modifier
        Obj, // object
        Obl, // oblique nominal
        Orphan, // orphan
        Parataxis, // parataxis
        Punct, // punctuation
        Reparandum, // overridden disfluency
        Root, // root
        Vocative, // vocative
        Xcomp, // open clausal complement
    }
}