using Mopups.Interfaces;
using Mopups.Pages;
using Popups.Maui.Prism.Extensions;
using Popups.Maui.Prism.Internal;
using Prism.Behaviors;
using Prism.Common;

namespace Popups.Maui.Prism.Behaviors
{
    public class BackgroundPopupDismissalBehavior : BehaviorBase<PopupPage>
    {
        private IPopupNavigation PopupNavigation { get; }

        private IWindowManager WindowManager { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="BackgroundPopupDismissalBehavior"/>
        /// </summary>
        /// <param name="popupNavigation">The <see cref="IPopupNavigation"/> instance.</param>
        /// <param name="windowManager">The <see cref="IWindowManager"/> instance.</param>
        public BackgroundPopupDismissalBehavior(IPopupNavigation popupNavigation, IWindowManager windowManager)
        {
            this.PopupNavigation = popupNavigation;
            this.WindowManager = windowManager;
        }

        /// <inheritdoc />
        protected override void OnAttachedTo(PopupPage bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.BackgroundClicked += this.OnBackgroundClicked;
        }

        /// <inheritdoc />
        protected override void OnDetachingFrom(PopupPage bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.BackgroundClicked -= this.OnBackgroundClicked;
        }

        private void OnBackgroundClicked(object sender, EventArgs e)
        {
            // If the Popup Page is not going to dismiss we don't need to do anything
            if (!this.AssociatedObject.CloseWhenBackgroundIsClicked)
            {
                return;
            }

            var parameters = PopupUtilities.CreateBackNavigationParameters();

            this.InvokePageInterfaces(this.AssociatedObject, parameters, false);
            this.InvokePageInterfaces(this.TopPage(), parameters, true);
        }

        private void InvokePageInterfaces(Page page, INavigationParameters parameters, bool navigatedTo)
        {
            PageUtilities.InvokeViewAndViewModelAction<INavigatedAware>(page, (view) =>
            {
                if (navigatedTo)
                {
                    view.OnNavigatedTo(parameters);
                }
                else
                {
                    view.OnNavigatedFrom(parameters);
                }
            });
            PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(page, (view) => view.IsActive = navigatedTo);

            if (!navigatedTo)
            {
                PageUtilities.InvokeViewAndViewModelAction<IDestructible>(this.AssociatedObject, (view) => view.Destroy());
            }
        }

        private Page TopPage()
        {
            Page page;
            if (this.PopupNavigation.PopupStack.Any(p => p != this.AssociatedObject))
            {
                page = this.PopupNavigation.PopupStack.LastOrDefault(p => p != this.AssociatedObject);
            }
            else if (this.WindowManager.Windows[^1].Page!.Navigation.ModalStack.Count > 0)
            {
                page = this.WindowManager.Windows[^1].Page!.Navigation.ModalStack.LastOrDefault();
            }
            else
            {
                page = this.WindowManager.Windows[^1].Page!.Navigation.NavigationStack.LastOrDefault();
            }

            page ??= this.WindowManager.Windows[^1].Page!;

            return page.GetDisplayedPage();
        }
    }
}