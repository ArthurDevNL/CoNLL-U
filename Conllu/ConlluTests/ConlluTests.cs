using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Conllu;
using Conllu.Enums;
using Conllu.Extensions;
using NUnit.Framework;

namespace ConlluTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestParse()
        {
            var assembly = typeof(Tests).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("ConlluTests.Resources.TestSentence.conllu");
            // ReSharper disable once AssignNullToNotNullAttribute
            using var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();
            
            var result = ConlluParser.ParseText(text).ToList();
            Assert.That(result.Count, Is.EqualTo(1));

            var sentence = result.First();
            Assert.That(sentence.Tokens.Count, Is.EqualTo(10));
            Assert.That(sentence.IsEmpty(), Is.False);

            var token = sentence.Tokens.First();
            Assert.That(token.Form, Is.EqualTo("The"));
            Assert.That(token.Lemma, Is.EqualTo("the"));
            Assert.That(token.Upos, Is.EqualTo("DET"));
            Assert.That(token.UposEnum, Is.EqualTo(PosTag.Det));
            Assert.That(token.Xpos, Is.EqualTo("DT"));
            Assert.That(token.Feats.Count, Is.EqualTo(2));
            Assert.That(token.Feats.ContainsKey("Definite"), Is.True);
            Assert.That(token.Feats["Definite"], Is.EqualTo("Def"));
            Assert.That(token.Feats.ContainsKey("PronType"), Is.True);
            Assert.That(token.Feats["PronType"], Is.EqualTo("Art"));
            Assert.That(token.Head, Is.EqualTo(4));
            Assert.That(token.DepRel, Is.EqualTo("det"));
            Assert.That(token.DepRelEnum, Is.EqualTo(DependencyRelation.Det));
            Assert.That(token.Deps, Is.Not.Null);
            Assert.That(token.Deps.Any(), Is.False);
            Assert.That(token.Misc, Is.Null);
            Assert.That(token.RawLine, Is.EqualTo("1	The	the	DET	DT	Definite=Def|PronType=Art	4	det	_	_"));
        }
        
        [Test]
        public void TestSentenceMetadata()
        {
            var assembly = typeof(Tests).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("ConlluTests.Resources.TestMetadata.conllu");
            // ReSharper disable once AssignNullToNotNullAttribute
            using var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();
            var result = ConlluParser.ParseText(text).ToList();
            Assert.That(result.Count, Is.EqualTo(1));

            var s = result.First();
            Assert.That(s.Metadata.ContainsKey("text"), Is.True);
            Assert.That(s.Metadata["text"], Is.EqualTo("The quick brown fox jumps over the lazy dog."));
        }

        [Test]
        public void TestParseLargeFile()
        {
            var assembly = typeof(Tests).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("ConlluTests.Resources.en_ewt-ud-dev.conllu");
            // ReSharper disable once AssignNullToNotNullAttribute
            using var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();
            var result = ConlluParser.ParseText(text).ToList();
            Assert.That(result.Count, Is.EqualTo(2002));
            Assert.That(result.All(x => !x.IsEmpty()), Is.True);
            Assert.That(result.All(s => s.AsDependencyTree() != null), Is.True);
        }

        [Test]
        public void TestSerializeParse()
        {
            var sentence = new Sentence(new List<Token>
            {
                Token.FromLine("1	The	the	DET	DT	Definite=Def|PronType=Art	4	det	_	_"),
                Token.FromLine("2	quick	quick	ADJ	JJ	Degree=Pos	4	amod	_	_"),
                Token.FromLine("3	brown	brown	ADJ	JJ	Degree=Pos	4	amod	_	_"),
                Token.FromLine("4	fox	fox	NOUN	NN	Number=Sing	5	nsubj	_	_"),
                Token.FromLine("5	jumps	jump	VERB	VBZ	Mood=Ind|Number=Sing|Person=3|Tense=Pres|VerbForm=Fin	0	root	_	_"),
                Token.FromLine("6	over	over	ADP	IN	_	9	case	_	_"),
                Token.FromLine("7	the	the	DET	DT	Definite=Def|PronType=Art	9	det	_	_"),
                Token.FromLine("8	lazy	lazy	ADJ	JJ	Degree=Pos	9	amod	_	_"),
                Token.FromLine("9	dog	dog	NOUN	NN	Number=Sing	5	nmod	_	SpaceAfter=No"),
                Token.FromLine("10	.	.	PUNCT	.	_	5	punct	_	_")
            }, new Dictionary<string, string>
            {
                { "text", "The quick brown fox jumps over the lazy dog."}
            });
            var serialized = ConlluParser.Serialize(new List<Sentence> {sentence});
            var serializedLines = serialized.SplitLines();
            
            // Compare to the file
            var assembly = typeof(Tests).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("ConlluTests.Resources.TestSentence.conllu");
            // ReSharper disable once AssignNullToNotNullAttribute
            using var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();
            var textLines = text.SplitLines();
            Assert.That(textLines, Is.EqualTo(serializedLines));
            
            // Re-parse
            var parsed = ConlluParser.ParseText(text).ToList();
            Assert.That(parsed.Count, Is.EqualTo(1));

            var s = parsed.First();
            Assert.That(s.Tokens.Count, Is.EqualTo(10));
            Assert.That(s.Metadata.Count, Is.EqualTo(1));
            Assert.That(s.Metadata.ContainsKey("text"), Is.True);
            Assert.That(s.Metadata["text"], Is.EqualTo("The quick brown fox jumps over the lazy dog."));
        }

        [Test]
        public void TestCreateParseTree()
        {
            var assembly = typeof(Tests).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("ConlluTests.Resources.TestSentence.conllu");
            // ReSharper disable once AssignNullToNotNullAttribute
            using var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();
            
            var result = ConlluParser.ParseText(text).FirstOrDefault();
            Assert.That(result, Is.Not.Null);
            var tree = result.AsDependencyTree();
            Assert.That(tree.Value.Id, Is.EqualTo(5));
            Assert.That(tree.Value.Form, Is.EqualTo("jumps"));
            Assert.That(tree.Children.Count(), Is.EqualTo(3));
            Assert.That(tree.Children.ToList()[0].Children.Count(), Is.EqualTo(3));
            Assert.That(tree.Children.ToList()[1].Children.Count(), Is.EqualTo(3));
            Assert.That(tree.Children.ToList()[2].Children.Count(), Is.EqualTo(0));
        }

        [Test]
        public void TestRawTokenSequence()
        {
            var sentence = new Sentence(new List<Token>
            {
                Token.FromLine("1	The	the	DET	DT	Definite=Def|PronType=Art	4	det	_	_"),
                Token.FromLine("2	quick	quick	ADJ	JJ	Degree=Pos	4	amod	_	_"),
                Token.FromLine("3	brown	brown	ADJ	JJ	Degree=Pos	4	amod	_	_"),
                Token.FromLine("4	fox	fox	NOUN	NN	Number=Sing	5	nsubj	_	_"),
                Token.FromLine("5	jumps	jump	VERB	VBZ	Mood=Ind|Number=Sing|Person=3|Tense=Pres|VerbForm=Fin	0	root	_	_"),
                Token.FromLine("6	over	over	ADP	IN	_	9	case	_	_"),
                Token.FromLine("7	the	the	DET	DT	Definite=Def|PronType=Art	9	det	_	_"),
                Token.FromLine("8	lazy	lazy	ADJ	JJ	Degree=Pos	9	amod	_	_"),
                Token.FromLine("9	dog	dog	NOUN	NN	Number=Sing	5	nmod	_	SpaceAfter=No"),
                Token.FromLine("10	.	.	PUNCT	.	_	5	punct	_	_")
            });

            Assert.That(sentence.RawTokenSequence(), Is.EqualTo("The quick brown fox jumps over the lazy dog."));
        }

        [Test]
        public void TestDependencyTreeSubTypes()
        {
            var assembly = typeof(Tests).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("ConlluTests.Resources.en_ewt-ud-dev.conllu");
            // ReSharper disable once AssignNullToNotNullAttribute
            using var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();
            var result = ConlluParser.ParseText(text).ToList();

            var s = result[150];
            Assert.That(s.AsDependencyTree().Children.Count, Is.EqualTo(4));
        }

        [Test]
        public void TestCompareTokenIdentifiers()
        {
            var ti1 = new TokenIdentifier("1");
            var ti2 = new TokenIdentifier("2");
            Assert.That(ti1 < ti2, Is.True);
            Assert.That(ti2 > ti1, Is.True);
            Assert.That(ti1 <= ti2, Is.True);
            Assert.That(ti2 >= ti1, Is.True);
            Assert.That(ti1 == ti2, Is.False);

            var ti13 = new TokenIdentifier("1-3");
            var ti4 = new TokenIdentifier("4.1");
            Assert.That(ti13.IsInRange(ti2.Id), Is.True);
            Assert.That(ti13.IsInRange(ti4.Id), Is.False);
            Assert.That(ti13.IsMultiwordIndex, Is.True);

            var ti14 = new TokenIdentifier("1-4");
            Assert.That(ti14 != ti13, Is.True);

            var ti42 = new TokenIdentifier("4.2");
            Assert.That(ti4 != ti42, Is.True);
            Assert.That(ti4 != ti1, Is.True);
            
            Assert.That(ti1 != ti2, Is.True);
            Assert.That(ti1.Equals(ti2), Is.False);
            
            var ti1Again = new TokenIdentifier("1");
            Assert.That(ti1 == ti1Again, Is.True);
            Assert.That(ti1.Equals(ti1Again), Is.True);
        }

        [Test]
        public void TestIdentifierSerialize()
        {
            var ti1 = new TokenIdentifier("1");
            Assert.That(ti1.Serialize(), Is.EqualTo("1"));
            
            var ti13 = new TokenIdentifier("1-3");
            Assert.That(ti13.Serialize(), Is.EqualTo("1-3"));
            
            var ti4 = new TokenIdentifier("4.1");
            Assert.That(ti4.Serialize(), Is.EqualTo("4.1"));
        }

        [Test]
        public void TestTreeEquality()
        {
            var s1 = new Sentence(new List<Token>
            {
                Token.FromLine("1	The	the	DET	DT	Definite=Def|PronType=Art	4	det	_	_"),
                Token.FromLine("2	quick	quick	ADJ	JJ	Degree=Pos	4	amod	_	_"),
                Token.FromLine("3	brown	brown	ADJ	JJ	Degree=Pos	4	amod	_	_"),
                Token.FromLine("4	fox	fox	NOUN	NN	Number=Sing	5	nsubj	_	_"),
                Token.FromLine("5	jumps	jump	VERB	VBZ	Mood=Ind|Number=Sing|Person=3|Tense=Pres|VerbForm=Fin	0	root	_	_"),
                Token.FromLine("6	over	over	ADP	IN	_	9	case	_	_"),
                Token.FromLine("7	the	the	DET	DT	Definite=Def|PronType=Art	9	det	_	_"),
                Token.FromLine("8	lazy	lazy	ADJ	JJ	Degree=Pos	9	amod	_	_"),
                Token.FromLine("9	dog	dog	NOUN	NN	Number=Sing	5	nmod	_	SpaceAfter=No"),
                Token.FromLine("10	.	.	PUNCT	.	_	5	punct	_	_")
            });
            
            var s2 = new Sentence(new List<Token>
            {
                Token.FromLine("1	The	the	DET	DT	Definite=Def|PronType=Art	4	det	_	_"),
                Token.FromLine("2	quick	quick	ADJ	JJ	Degree=Pos	4	amod	_	_"),
                Token.FromLine("3	brown	brown	ADJ	JJ	Degree=Pos	4	amod	_	_"),
                Token.FromLine("4	fox	fox	NOUN	NN	Number=Sing	5	nsubj	_	_"),
                Token.FromLine("5	jumps	jump	VERB	VBZ	Mood=Ind|Number=Sing|Person=3|Tense=Pres|VerbForm=Fin	0	root	_	_"),
                Token.FromLine("6	over	over	ADP	IN	_	9	case	_	_"),
                Token.FromLine("7	the	the	DET	DT	Definite=Def|PronType=Art	9	det	_	_"),
                Token.FromLine("8	lazy	lazy	ADJ	JJ	Degree=Pos	9	amod	_	_"),
                Token.FromLine("9	dog	dog	NOUN	NN	Number=Sing	5	nmod	_	SpaceAfter=No"),
                Token.FromLine("10	.	.	PUNCT	.	_	5	punct	_	_")
            });
            
            var s1Tree = s1.AsDependencyTree();
            var s2Tree = s2.AsDependencyTree();
            Assert.That(s1Tree.Equals(s2Tree), Is.True);

            var t = Token.FromLine("11	test	test	ADJ	JJ	Degree=Pos	9	amod	_	_");
            s1Tree.AddChild(new Tree<Token, DependencyRelation>(t, DependencyRelation.Cc));
            Assert.That(s1Tree.Equals(s2Tree), Is.False);
        }

        [Test]
        public void TestTreeWhere()
        {
            var s1 = new Sentence(new List<Token>
            {
                Token.FromLine("1	The	the	DET	DT	Definite=Def|PronType=Art	4	det	_	_"),
                Token.FromLine("2	quick	quick	ADJ	JJ	Degree=Pos	4	amod	_	_"),
                Token.FromLine("3	brown	brown	ADJ	JJ	Degree=Pos	4	amod	_	_"),
                Token.FromLine("4	fox	fox	NOUN	NN	Number=Sing	5	nsubj	_	_"),
                Token.FromLine("5	jumps	jump	VERB	VBZ	Mood=Ind|Number=Sing|Person=3|Tense=Pres|VerbForm=Fin	0	root	_	_"),
                Token.FromLine("6	over	over	ADP	IN	_	9	case	_	_"),
                Token.FromLine("7	the	the	DET	DT	Definite=Def|PronType=Art	9	det	_	_"),
                Token.FromLine("8	lazy	lazy	ADJ	JJ	Degree=Pos	9	amod	_	_"),
                Token.FromLine("9	dog	dog	NOUN	NN	Number=Sing	5	nmod	_	SpaceAfter=No"),
                Token.FromLine("10	.	.	PUNCT	.	_	5	punct	_	_")
            });
            var s1Tree = s1.AsDependencyTree();

            var over = s1Tree.WhereBfs(x => x.Form == "over").FirstOrDefault();
            Assert.That(over, Is.Not.Null);
            Assert.That(over.Value.Id, Is.EqualTo(6));

            var lazy = s1Tree.WhereDfs(x => x.DepRelEnum == DependencyRelation.Amod).FirstOrDefault();
            Assert.That(lazy, Is.Not.Null);
            Assert.That(lazy.Value.Id, Is.EqualTo(8));
        }
    }
}