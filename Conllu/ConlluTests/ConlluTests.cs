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
            Assert.AreEqual(1, result.Count);

            var sentence = result.First();
            Assert.AreEqual(sentence.Tokens.Count, 10);
            Assert.False(sentence.IsEmpty());

            var token = sentence.Tokens.First();
            Assert.AreEqual("The", token.Form);
            Assert.AreEqual("the", token.Lemma);
            Assert.AreEqual("DET", token.Upos);
            Assert.AreEqual(PosTag.Det, token.UposEnum);
            Assert.AreEqual("DT", token.Xpos);
            Assert.AreEqual(2, token.Feats.Count);
            Assert.IsTrue(token.Feats.ContainsKey("Definite"));
            Assert.AreEqual("Def", token.Feats["Definite"]);
            Assert.IsTrue(token.Feats.ContainsKey("PronType"));
            Assert.AreEqual("Art", token.Feats["PronType"]);
            Assert.AreEqual(4, token.Head);
            Assert.AreEqual("det", token.DepRel);
            Assert.AreEqual(DependencyRelation.Det, token.DepRelEnum);
            Assert.IsNotNull(token.Deps);
            Assert.IsTrue(!token.Deps.Any());
            Assert.IsNull(token.Misc);
            Assert.AreEqual("1	The	the	DET	DT	Definite=Def|PronType=Art	4	det	_	_", token.RawLine);
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
            Assert.AreEqual(1, result.Count);

            var s = result.First();
            Assert.IsTrue(s.Metadata.ContainsKey("text"));
            Assert.AreEqual("The quick brown fox jumps over the lazy dog.", s.Metadata["text"]);
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
            Assert.AreEqual(2002, result.Count);
            Assert.IsTrue(result.All(x => !x.IsEmpty()));
            Assert.IsTrue(result.All(s => s.AsDependencyTree() != null));
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
            Assert.AreEqual(textLines, serializedLines);
            
            // Re-parse
            var parsed = ConlluParser.ParseText(text).ToList();
            Assert.AreEqual(1, parsed.Count);

            var s = parsed.First();
            Assert.AreEqual(10, s.Tokens.Count);
            Assert.AreEqual(1, s.Metadata.Count);
            Assert.IsTrue(s.Metadata.ContainsKey("text"));
            Assert.AreEqual("The quick brown fox jumps over the lazy dog.", s.Metadata["text"]);
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
            Assert.NotNull(result);
            var tree = result.AsDependencyTree();
            Assert.AreEqual(5, tree.Value.Id);
            Assert.AreEqual("jumps", tree.Value.Form);
            Assert.AreEqual(3, tree.Children.Count());
            Assert.AreEqual(3, tree.Children.ToList()[0].Children.Count());
            Assert.AreEqual(3, tree.Children.ToList()[1].Children.Count());
            Assert.AreEqual(0, tree.Children.ToList()[2].Children.Count());
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

            Assert.AreEqual("The quick brown fox jumps over the lazy dog.", sentence.RawTokenSequence());
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
            Assert.AreEqual(4, s.AsDependencyTree().Children.Count);
        }

        [Test]
        public void TestCompareTokenIdentifiers()
        {
            var ti1 = new TokenIdentifier("1");
            var ti2 = new TokenIdentifier("2");
            Assert.IsTrue(ti1 < ti2);
            Assert.IsTrue(ti2 > ti1);
            Assert.IsTrue(ti1 <= ti2);
            Assert.IsTrue(ti2 >= ti1);
            Assert.IsFalse(ti1 == ti2);

            var ti13 = new TokenIdentifier("1-3");
            var ti4 = new TokenIdentifier("4.1");
            Assert.IsTrue(ti13.IsInRange(ti2.Id));
            Assert.IsFalse(ti13.IsInRange(ti4.Id));
            Assert.IsTrue(ti13.IsMultiwordIndex);

            var ti14 = new TokenIdentifier("1-4");
            Assert.IsTrue(ti14 != ti13);

            var ti42 = new TokenIdentifier("4.2");
            Assert.IsTrue(ti4 != ti42);
            Assert.IsTrue(ti4 != ti1);
            
            Assert.AreNotEqual(ti1, ti2);
            Assert.IsFalse(ti1.Equals(ti2));
            
            var ti1Again = new TokenIdentifier("1");
            Assert.AreEqual(ti1, ti1Again);
            Assert.IsTrue(ti1.Equals(ti1Again));
        }

        [Test]
        public void TestIdentifierSerialize()
        {
            var ti1 = new TokenIdentifier("1");
            Assert.AreEqual(ti1.Serialize(), "1");
            
            var ti13 = new TokenIdentifier("1-3");
            Assert.AreEqual(ti13.Serialize(), "1-3");
            
            var ti4 = new TokenIdentifier("4.1");
            Assert.AreEqual(ti4.Serialize(), "4.1");
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
            Assert.IsTrue(s1Tree.Equals(s2Tree));

            var t = Token.FromLine("11	test	test	ADJ	JJ	Degree=Pos	9	amod	_	_");
            s1Tree.AddChild(new Tree<Token, DependencyRelation>(t, DependencyRelation.Cc));
            Assert.IsFalse(s1Tree.Equals(s2Tree));
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
            Assert.IsNotNull(over);
            Assert.AreEqual(over.Value.Id, 6);

            var lazy = s1Tree.WhereDfs(x => x.DepRelEnum == DependencyRelation.Amod).FirstOrDefault();
            Assert.IsNotNull(lazy);
            Assert.AreEqual(lazy.Value.Id, 8);
        }
    }
}