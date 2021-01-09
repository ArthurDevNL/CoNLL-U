using System.IO;
using System.Linq;
using System.Reflection;
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

            var s = result.First();
            Assert.AreEqual(s.Tokens.Count, 10);
            Assert.False(s.IsEmpty());
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

        // [Test]
        public void TestParseLargeFile()
        {
            var assembly = typeof(Tests).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("ConlluTests.Resources.en_ewt-ud-dev.conllu");
            // ReSharper disable once AssignNullToNotNullAttribute
            using var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();
            var result = Conllu.Conllu.ParseText(text).ToList();
            Assert.AreEqual(1, result.Count);

            var s = result.First();
            Assert.IsTrue(s.Metadata.ContainsKey("text"));
            Assert.AreEqual("The quick brown fox jumps over the lazy dog.", s.Metadata["text"]);
        }
    }
}