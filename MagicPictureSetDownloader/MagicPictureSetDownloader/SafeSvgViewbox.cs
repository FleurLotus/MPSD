namespace MagicPictureSetDownloader
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    using SharpVectors.Converters;

    public sealed class SafeSvgViewbox : Viewbox
    {
        private readonly SvgViewbox _inner = new SvgViewbox();
        private object _pendingSource;
        private bool _applyScheduled;
        private const int MaxRetries = 2;
        private int _retryCount;

        public SafeSvgViewbox()
        {
            Child = _inner;
            _inner.Stretch = System.Windows.Media.Stretch.Uniform;
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                nameof(Source),
                typeof(object),
                typeof(SafeSvgViewbox),
                new PropertyMetadata(null, OnSourceChanged));

        public object Source
        {
            get
            {
                return GetValue(SourceProperty);
            }

            set
            {
                SetValue(SourceProperty, value);
            }
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SafeSvgViewbox ctrl = (SafeSvgViewbox) d;
            ctrl._pendingSource = e.NewValue;
            ctrl._retryCount = 0;
            ctrl.ScheduleApplySource();
        }

        private void ScheduleApplySource()
        {
            if (_applyScheduled)
            {
                return;
            }

            _applyScheduled = true;

            // Defer to Background so DataGrid row/container generation can finish.
            Dispatcher.BeginInvoke(() =>
            {
                _applyScheduled = false;
                TryApplySource();
            }, DispatcherPriority.Background);
        }

        private void TryApplySource()
        {
            object src = _pendingSource;
            if (src == null)
            {
                TryClearInnerSource();
                return;
            }

            Uri uri = null;
            if (src is Uri u)
            {
                uri = u;
            }
            else if (src is string s && !string.IsNullOrWhiteSpace(s))
            {
                // create absolute file Uri for rooted paths, otherwise let parser try relative/absolute
                if (System.IO.Path.IsPathRooted(s))
                {
                    Uri.TryCreate(s, UriKind.Absolute, out uri);
                }
                else
                {
                    Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out uri);
                }
            }

            if (uri == null)
            {
                TryClearInnerSource();
                return;
            }

            try
            {
                _inner.Source = uri;
            }
            catch
            {
                _retryCount++;
                if (_retryCount <= MaxRetries)
                {
                    // schedule one more deferred retry to give the visual tree time to stabilize
                    ScheduleApplySource();
                }
                else
                {
                    TryClearInnerSource();
                }
            }
        }

        private void TryClearInnerSource()
        {
            try
            {
                _inner.Source = null;
            }
            catch
            {
                // swallow
            }
        }
    }
}