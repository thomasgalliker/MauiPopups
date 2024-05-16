using System.Diagnostics;
using Mopups.Interfaces;
using Mopups.Pages;
using Popups.Maui.Prism.Internal;
using Prism.Common;

namespace Popups.Maui.Prism
{
    /// <summary>
    /// Implements the <see cref="INavigationService" /> for working with <see cref="PopupPage" />
    /// </summary>
    public class PopupPageNavigationService : PageNavigationService
    {
        /// <summary>
        /// Gets the <see cref="IPopupNavigation" /> service.
        /// </summary>
        protected IPopupNavigation PopupNavigation { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="PopupPageNavigationService" />
        /// </summary>
        /// <param name="popupNavigation"></param>
        /// <param name="container"></param>
        /// <param name="windowManager"></param>
        /// <param name="eventAggregator"></param>
        /// <param name="pageAccessor"></param>
        public PopupPageNavigationService(
            IPopupNavigation popupNavigation,
            IContainerProvider container,
            IWindowManager windowManager,
            IEventAggregator eventAggregator,
            IPageAccessor pageAccessor)
            : base(container, windowManager, eventAggregator, pageAccessor)
        {
            this.PopupNavigation = popupNavigation;
            // _page = windowManager.Windows[^1].Page;
        }

        /// <inheritdoc />
        public override async Task<INavigationResult> GoBackAsync(INavigationParameters parameters)
        {
            INavigationResult result;
            parameters.TryGetValue(KnownNavigationParameters.Animated, out bool? animated);
            try
            {
                switch (PopupUtilities.TopPage(this.PopupNavigation, this._windowManager))
                {
                    case PopupPage popupPage:
                        var segmentParameters = UriParsingHelper.GetSegmentParameters(null, parameters);
                        ((INavigationParametersInternal)segmentParameters).Add("__NavigationMode", NavigationMode.Back);
                        var previousPage = PopupUtilities.GetOnNavigatedToTarget(this.PopupNavigation, this._windowManager);

                        await this.DoPop(popupPage.Navigation, false, animated ?? false);

                        PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(popupPage, a => a.IsActive = false);
                        PageUtilities.OnNavigatedFrom(popupPage, segmentParameters);
                        PageUtilities.OnNavigatedTo(previousPage, segmentParameters);
                        PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(previousPage, a => a.IsActive = true);
                        PageUtilities.DestroyPage(popupPage);
                        result = new NavigationResult();
                        break;

                    default:
                        result = await base.GoBackAsync(parameters);
                        break;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Break();
#endif
                result = new NavigationResult(e);
            }

            return result;
        }

        /// <inheritdoc />
        protected override async Task<Page> DoPop(INavigation navigation, bool useModalNavigation, bool animated)
        {
            var page = this._pageAccessor.Page;
            if (this.PopupNavigation.PopupStack.Count > 0 || page is PopupPage)
            {
                await this.PopupNavigation.PopAsync(animated);
                return null;
            }

            return await base.DoPop(navigation, useModalNavigation, animated);
        }

        /// <inheritdoc />
        protected override async Task DoPush(Page currentPage, Page page, bool? useModalNavigation, bool? animated, bool insertBeforeLast = false, int navigationOffset = 0)
        {
            switch (page)
            {
                case PopupPage popup:
                    if (this._windowManager.Windows[^1].Page is null)
                    {
                        throw new NavigationException("Popup Pages cannot be set before the Application.MainPage has been set. You must have a valid NavigationStack prior to navigating.", popup);
                    }

                    await this.PopupNavigation.PushAsync(popup, animated.GetValueOrDefault());
                    break;
                default:
                    if (this.PopupNavigation.PopupStack.Any())
                    {
                        foreach (var pageToPop in this.PopupNavigation.PopupStack)
                        {
                            PageUtilities.DestroyPage(pageToPop);
                        }

                        await this.PopupNavigation.PopAllAsync(animated.GetValueOrDefault());
                    }

                    if (currentPage is PopupPage)
                    {
                        currentPage = PageUtilities.GetCurrentPage(this._windowManager.Windows[^1].Page);
                    }

                    await base.DoPush(currentPage, page, useModalNavigation, animated, insertBeforeLast, navigationOffset);
                    break;
            }
        }

        /// <inheritdoc />
        protected override Page GetCurrentPage()
        {
            if (this.PopupNavigation.PopupStack.Any())
            {
                return this.PopupNavigation.PopupStack.LastOrDefault();
            }
            else
            {
                return base.GetCurrentPage();
            }
        }
    }
}