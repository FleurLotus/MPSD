namespace MagicPictureSetDownloader
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    public class PicturedTextBlock : TextBlock
    {
        public static readonly DependencyProperty InlineCollectionProperty = DependencyProperty.Register("InlineCollection", typeof(List<Inline>),
            typeof(PicturedTextBlock), new UIPropertyMetadata(InlineCollectionCallback));

        public List<Inline> InlineCollection
        {
            get { return (List<Inline>)GetValue(InlineCollectionProperty); }
            set { SetValue(InlineCollectionProperty, value); }
        }

        public static void InlineCollectionCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is PicturedTextBlock textBlock)
            {
                textBlock.Inlines.Clear();


                if (args.NewValue is List<Inline> inlines)
                {
                    textBlock.Inlines.AddRange(inlines);
                }
            }
        }
    }
}