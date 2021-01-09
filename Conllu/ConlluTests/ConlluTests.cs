using System.IO;
using System.Linq;
using System.Reflection;
using Conllu.Enums;
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
            
            var result = Conllu.Conllu.ParseText(text).ToList();
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
            var result = Conllu.Conllu.ParseText(text).ToList();
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
            var result = Conllu.Conllu.ParseText(text).ToList();
            Assert.AreEqual(2002, result.Count);
            Assert.IsTrue(result.All(x => !x.IsEmpty()));
        }
    }
}