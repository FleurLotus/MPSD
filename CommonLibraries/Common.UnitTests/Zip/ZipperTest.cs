namespace Common.UnitTests.Zip
{
    using System.Collections.Generic;
    using System.IO;
    using Common.Zip;
    using NUnit.Framework;
    
    [TestFixture]
    public class ZipperTest
    {
        private const string ZipFileName = @"ZipTest.zip";

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            var dir = Path.GetDirectoryName(typeof(ZipperTest).Assembly.Location);

            Directory.SetCurrentDirectory(dir);
        }

        [Test]
        public void TestUnzipAll([Values(true, false)] bool fromStream)
        {
            Assert.That(File.Exists(ZipFileName), "Can't find the test file");
            string temporyDirectory = Path.Combine(Path.GetTempPath(), "TestUnzip");
            string[] fileNames = {"File1.txt", "File2.txt"};
            const string subDirName = "Subdir";

            try
            {
                if (Directory.Exists(temporyDirectory))
                {
                    Directory.Delete(temporyDirectory, true);
                }

                if (fromStream)
                {
                    using (Stream fs = new FileStream(ZipFileName, FileMode.Open))
                    {
                        Zipper.UnZipAll(fs, temporyDirectory);
                    }
                }
                else
                {
                    byte[] bytes = File.ReadAllBytes(ZipFileName);
                    Zipper.UnZipAll(bytes, temporyDirectory);
                }

                string[] files = Directory.GetFiles(temporyDirectory);

                Assert.That(files.Length == 2, "The unzipped direcory must have 2 files in the root");

                List<string> expectedfiles = new List<string>(fileNames);

                foreach (string file in files)
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        string fileName = Path.GetFileName(file);
                        Assert.That(expectedfiles.Contains(fileName), "The file name is not in the expected list");
                        expectedfiles.Remove(fileName);

                        string content = sr.ReadToEnd();
                        Assert.That(content == fileName, "The file {0} doesn't content the expected text", file);
                    }
                }

                string[] directories = Directory.GetDirectories(temporyDirectory);
                Assert.That(directories.Length == 1, "The unzipped direcory must have 1 sub directory in the root");

                string subDirectory = directories[0];
                Assert.That(subDirectory == Path.Combine(temporyDirectory, subDirName), "The sub direcory has not the expected name");

                files = Directory.GetFiles(subDirectory);

                Assert.That(files.Length == 2, "The unzipped direcory must have 2 files in the sub directory");

                expectedfiles = new List<string>(fileNames);

                foreach (string file in files)
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        string fileName = Path.GetFileName(file);
                        Assert.That(expectedfiles.Contains(fileName), "The file name is not in the expected list");
                        expectedfiles.Remove(fileName);

                        string content = sr.ReadToEnd();
                        Assert.That(content == string.Format("{0}\\{1}", subDirName, fileName), "The file {0} doesn't content the expected text", file);
                    }
                }

                Assert.That(Directory.GetDirectories(subDirectory).Length == 0, "The unzipped direcory must not have any sub-sub directory in the root");
            }
            finally
            {
                if (Directory.Exists(temporyDirectory))
                {
                    Directory.Delete(temporyDirectory, true);
                }
            }
        }
    }
}
