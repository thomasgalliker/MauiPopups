using Mopups.Interfaces;
using Popups.Maui.Prism.Extensions;
using Popups.Maui.Prism.Dialogs;

namespace Popups.Maui.Prism.Internal
{
    internal static class PopupUtilities
    {
        public const string NavigationModeKey = "__NavigationMode";

        public static INavigationParameters CreateNewNavigationParameters() =>
            new NavigationParameters().AddNavigationMode(NavigationMode.New);

        public static INavigationParameters CreateBackNavigationParameters() =>
            new NavigationParameters().AddNavigationMode(NavigationMode.Back);

        public static INavigationParameters AddNavigationMode(this INavigationParameters parameters, NavigationMode mode)
        {
            return parameters.AddInternalParameter(NavigationModeKey, mode);
        }

        public static INavigationParameters AddInternalParameter(this INavigationParameters parameters, string key, object value)
        {
            ((INavigationParametersInternal)parameters).Add(key, value);
            return parameters;
        }

        public static Page TopPage(IPopupNavigation popupNavigation, IWindowManager windowManager)
        {
            Page page;
            var popupStack = popupNavigation.PopupStack.Where(x => !(x is PopupDialogContainer));
            var popupPages = popupStack.ToList();
            if (popupPages.Any())
            {
                page = popupPages.LastOrDefault();
            }
            else if (windowManager.Windows.Last().Page!.Navigation.ModalStack.Count > 0)
            {
                page = windowManager.Windows[^1].Page!.Navigation.ModalStack.LastOrDefault();
            }
            else
            {
                page = windowManager.Windows[^1].Page!.Navigation.NavigationStack.LastOrDefault();
            }

            page ??= windowManager.Windows.Last().Page!;

            return page.GetDisplayedPage();
        }

        public static Page GetOnNavigatedToTarget(IPopupNavigation popupNavigation, IWindowManager windowManager)
        {
            Page page;
            if (popupNavigation.PopupStack.Count > 1)
            {
                page = popupNavigation.PopupStack.ElementAt(popupNavigation.PopupStack.Count() - 2);
            }
            else if (windowManager.Windows[^1].Page!.Navigation.ModalStack.Count > 0)
            {
                page = windowManager.Windows[^1].Page!.Navigation.ModalStack.LastOrDefault();
            }
            else
            {
                page = windowManager.Windows[^1].Page!.Navigation.NavigationStack.LastOrDefault();
            }

            page ??= windowManager.Windows[^1].Page!;

            return page.GetDisplayedPage();
        }
    }
}