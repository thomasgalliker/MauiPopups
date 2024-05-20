using Mopups.Pages;

namespace MauiSampleApp.Views
{
    public partial class ContextMenuPopupPage : PopupPage
    {
        public ContextMenuPopupPage()
        {
            this.InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}