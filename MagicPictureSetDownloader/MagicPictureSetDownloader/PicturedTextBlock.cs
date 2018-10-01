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
            PicturedTextBlock textBlock = sender as PicturedTextBlock;

            if (textBlock != null)
            {
                textBlock.Inlines.Clear();

                List<Inline> inlines = args.NewValue as List<Inline>;

                if (inlines != null)
                {
                    textBlock.Inlines.AddRange(inlines);
                }
            }
        }
    }
}