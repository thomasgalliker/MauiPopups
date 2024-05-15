using Mopups.Interfaces;
using Mopups.Pages;
using Prism.Behaviors;

namespace Popups.Maui.Prism.Behaviors
{
    /// <summary>
    /// Provides an implementation of the <see cref="IPageBehaviorFactory" /> to add the 
    /// <see cref="BackgroundPopupDismissalBehavior" /> on <see cref="PopupPage" />.
    /// </summary>
    public class PopupPageBehaviorFactory : IPageBehaviorFactory
    {
        IPopupNavigation PopupNavigation { get; }
        IWindowManager ApplicationProvider { get; }

        /// <summary>
        /// Creates a new <see cref="IPageBehaviorFactory" /> for use with <see cref="PopupPage" />.
        /// </summary>
        /// <param name="popupNavigation">The <see cref="IPopupNavigation" /> service.</param>
        /// <param name="applicationProvider">The <see cref="IWindowManager" />.</param>
        public PopupPageBehaviorFactory(IPopupNavigation popupNavigation, IWindowManager applicationProvider)
        {
            this.PopupNavigation = popupNavigation;
            this.ApplicationProvider = applicationProvider;
        }

        /// <inheritdoc />
        public void ApplyPageBehaviors(Page page)
        {
            if (page is PopupPage popupPage)
            {
                popupPage.Behaviors.Add(new BackgroundPopupDismissalBehavior(this.PopupNavigation, this.ApplicationProvider));
            }
        }
    }
}