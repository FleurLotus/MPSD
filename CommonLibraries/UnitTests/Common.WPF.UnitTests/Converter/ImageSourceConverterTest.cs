namespace Common.WPF.UnitTests
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using NUnit.Framework;

    using ImageSourceConverter = ImageSourceConverter;

    [TestFixture]
    public class ImageSourceConverterTest
    {
        [Test]
        public void TestConvertNoImageSource()
        {
            ImageSourceConverter converter = new ImageSourceConverter();
            object ret = converter.Convert("aaaa", null, null, null);
            Assert.That(ret, Is.EqualTo("aaaa"));
        }
        [Test]
        public void TestConvertStringToImageSource()
        {
            ImageSourceConverter converter = new ImageSourceConverter();
            object ret = converter.Convert("ImageSample.bmp", typeof(ImageSource), null, null);
            Assert.That(ret, Is.Not.Null);
            Assert.That(ret, Is.InstanceOf<BitmapImage>());
        }
        [Test]
        public void TestConvertUriToImageSource()
        {
            ImageSourceConverter converter = new ImageSourceConverter();
            object ret = converter.Convert(new Uri($"file://{(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))}/ImageSample.bmp"), typeof(ImageSource), null, null);
            Assert.That(ret, Is.Not.Null);
            Assert.That(ret, Is.InstanceOf<BitmapImage>());
        }
        [Test]
        public void TestConvertObjectToImageSource()
        {
            ImageSourceConverter converter = new ImageSourceConverter();
            object ret = converter.Convert(new object(), typeof(ImageSource), null, null);
            Assert.That(ret, Is.Not.Null);
            Assert.That(ret, Is.Not.InstanceOf<BitmapImage>());
        }
        [Test]
        public void TestNoConvertBack()
        {
            ImageSourceConverter converter = new ImageSourceConverter();
            Assert.Throws<NotImplementedException>(() => converter.ConvertBack(null, null, null, null));
        }
        [Test]
        public void TestMarkup()
        {
            ImageSourceConverter converter = new ImageSourceConverter();
            Assert.That(converter.ProvideValue(null), Is.EqualTo(converter));
        }
    }
}