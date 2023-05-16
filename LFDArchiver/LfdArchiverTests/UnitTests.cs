using NUnit.Framework;

namespace LfdArchiverTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            File.Copy(Path.Combine(Path.GetDirectoryName(typeof(Tests).Assembly.Location) ?? "", "MISSIONS.LFD")
                        ,Path.Combine(Path.GetDirectoryName(typeof(Tests).Assembly.Location) ?? "", "MISSIONS2.LFD")
                        ,false);
        }

        [Test]
        public void TestLoad()
        {
            var archive = new ResourceArchive();
            var path = Path.Combine(Path.GetDirectoryName(typeof(Tests).Assembly.Location) ?? "", "MISSIONS2.LFD");

            archive.Load(path);
            Assert.That(archive.Entries.Any());
        }

        [Test]
        public void TestDeleteEntry()
        {
            var archive = new ResourceArchive();
            var path = Path.Combine(Path.GetDirectoryName(typeof(Tests).Assembly.Location) ?? "", "MISSIONS2.LFD");

            archive.Load(path);
            archive.DeleteEntry(archive.Entries[0]);
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(Path.Combine(Path.GetDirectoryName(typeof(Tests).Assembly.Location) ?? "", "MISSIONS2.LFD"));
        }
    }
}