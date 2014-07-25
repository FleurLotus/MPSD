﻿namespace MagicPictureSetDownloader
{
    using MagicPictureSetDownloader.ViewModel;

    /// <summary>
    /// Interaction logic for CredentialInput.xaml
    /// </summary>
    public partial class CredentialInput
    {
        public CredentialInput(CredentialInputViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
