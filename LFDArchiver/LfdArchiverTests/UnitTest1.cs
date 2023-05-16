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
            var path = Path.Combine(Path.GetDirectoryName(typeof(Tests).Assembly.Location) ?? "", "MISSIONS.LFD");

            archive.Load(path);
            Assert.That(archive.Entries.Any());
        }

        [Test]
        public void Test2()
        {
            Assert.Pass();
        }
    }
}