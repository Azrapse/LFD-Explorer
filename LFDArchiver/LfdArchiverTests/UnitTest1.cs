namespace LfdArchiverTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestLoad()
        {
            var archive = new ResourceArchive();
            archive.Load("MISSIONS.LFD");
            Assert.That(archive.Entries.Any());
        }

        [Test]
        public void Test2()
        {
            Assert.Pass();
        }
    }
}