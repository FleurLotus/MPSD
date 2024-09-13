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
            Assert.That(File.Exists(ZipFileName), Is.True, "Can't find the test file");
            string temporyDirectory = Path.Combine(Path.GetTempPath(), "TestUnzip");
            string[] fileNames = { "File1.txt", "File2.txt" };
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

                Assert.That(files.Length, Is.EqualTo(2), "The unzipped direcory must have 2 files in the root");

                List<string> expectedfiles = new List<string>(fileNames);

                foreach (string file in files)
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        string fileName = Path.GetFileName(file);
                        Assert.That(expectedfiles.Contains(fileName), Is.True, "The file name is not in the expected list");
                        expectedfiles.Remove(fileName);

                        string content = sr.ReadToEnd();
                        Assert.That(content, Is.EqualTo(fileName), $"The file {file} doesn't content the expected text");
                    }
                }

                string[] directories = Directory.GetDirectories(temporyDirectory);
                Assert.That(directories.Length, Is.EqualTo(1), "The unzipped direcory must have 1 sub directory in the root");

                string subDirectory = directories[0];
                Assert.That(subDirectory, Is.EqualTo(Path.Combine(temporyDirectory, subDirName)), "The sub direcory has not the expected name");

                files = Directory.GetFiles(subDirectory);

                Assert.That(files.Length, Is.EqualTo(2), "The unzipped direcory must have 2 files in the sub directory");

                expectedfiles = new List<string>(fileNames);

                foreach (string file in files)
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        string fileName = Path.GetFileName(file);
                        Assert.That(expectedfiles.Contains(fileName), Is.True, "The file name is not in the expected list");
                        expectedfiles.Remove(fileName);

                        string content = sr.ReadToEnd();
                        Assert.That(content, Is.EqualTo($"{subDirName}\\{fileName}"), $"The file {file} doesn't content the expected text");
                    }
                }

                Assert.That(Directory.GetDirectories(subDirectory).Length, Is.EqualTo(0), "The unzipped direcory must not have any sub-sub directory in the root");
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
