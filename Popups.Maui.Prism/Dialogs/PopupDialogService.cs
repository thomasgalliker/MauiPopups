using Microsoft.Maui.Layouts;
using Mopups.Interfaces;
using Mopups.Pages;
using Popups.Maui.Prism.Behaviors;
using Popups.Maui.Prism.Behaviors.Xaml;
using Prism.Common;
using Prism.Dialogs.Xaml;

namespace Popups.Maui.Prism.Dialogs
{
    /// <summary>
    /// Provides an implementation of the <see cref="IDialogService" /> that uses <see cref="PopupPage"/>
    /// as the background page container.
    /// </summary>
    public class PopupDialogService : IDialogService
    {
        /// <summary>
        /// The Style name expected in the Application <see cref="ResourceDictionary" />
        /// </summary>
        public const string PopupOverlayStyle = "PrismDialogMaskStyle";

        private IPopupNavigation PopupNavigation { get; }

        private IContainerProvider ContainerProvider { get; }

        /// <summary>
        /// Creates a new <see cref="PopupDialogService" />
        /// </summary>
        /// <param name="popupNavigation">The <see cref="IPopupNavigation" /> service to push and pop the <see cref="PopupPage" />.</param>
        /// <param name="containerProvider">The <see cref="IContainerProvider" /> to resolve the Dialog View.</param>
        public PopupDialogService(IPopupNavigation popupNavigation, IContainerProvider containerProvider)
        {
            this.PopupNavigation = popupNavigation;
            this.ContainerProvider = containerProvider;
        }

        /// <inheritdoc />
        public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            try
            {
                parameters = UriParsingHelper.GetSegmentParameters(name, parameters);

                var view = this.CreateViewFor(UriParsingHelper.GetSegmentName(name));
                var popupPage = CreatePopupPageForView(view);

                var dialogAware = this.InitializeDialog(view, parameters);

                if (!parameters.TryGetValue<bool>(KnownDialogParameters.CloseOnBackgroundTapped,
                        out var closeOnBackgroundTapped))
                {
                    var dialogLayoutCloseOnBackgroundTapped = DialogLayout.GetCloseOnBackgroundTapped(view);
                    if (dialogLayoutCloseOnBackgroundTapped.HasValue)
                    {
                        closeOnBackgroundTapped = dialogLayoutCloseOnBackgroundTapped.Value;
                    }
                }

                if (!parameters.TryGetValue<bool>(KnownPopupDialogParameters.Animated, out var animated))
                {
                    animated = true;
                }

                var popupDialogLayoutIsAnimationEnabled = PopupDialogLayout.GetIsAnimationEnabled(view);
                popupPage.IsAnimationEnabled = popupDialogLayoutIsAnimationEnabled ?? true;

                //dialogAware.RequestClose = new(DialogAwareRequestClose);

                void CloseOnBackgroundClicked(object sender, EventArgs args)
                {
                    DialogAwareRequestClose(new DialogParameters());
                }

                void DialogAwareRequestClose(IDialogParameters outParameters)
                {
                    try
                    {
                        var result = this.CloseDialog(outParameters ?? new DialogParameters(), popupPage, view);
                        if (result.Exception is DialogException { Message: DialogException.CanCloseIsFalse })
                        {
                            return;
                        }

                        if (closeOnBackgroundTapped)
                        {
                            popupPage.BackgroundClicked -= CloseOnBackgroundClicked;
                        }

                        callback?.Invoke(result);
                        GC.Collect();
                    }
                    catch (DialogException dex)
                    {
                        if (dex.Message != DialogException.CanCloseIsFalse)
                        {
                            callback?.Invoke(new DialogResult
                            {
                                Exception = dex,
                                Parameters = parameters
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        callback?.Invoke(new DialogResult
                        {
                            Exception = ex,
                            Parameters = parameters
                        });
                    }
                }

                if (closeOnBackgroundTapped)
                {
                    popupPage.BackgroundClicked += CloseOnBackgroundClicked;
                }

                Action<IDialogParameters> closeCallback = closeOnBackgroundTapped ? DialogAwareRequestClose : _ => { };
                this.PushPopupPage(popupPage, view, closeCallback, animated);
            }
            catch (Exception ex)
            {
                callback?.Invoke(new DialogResult { Exception = ex });
            }
        }

        private static PopupPage CreatePopupPageForView(BindableObject view)
        {
            var popupPage = new PopupDialogContainer();

            var hasSystemPadding = view.GetValue(PopupDialogLayout.HasSystemPaddingProperty);
            if (hasSystemPadding != null)
            {
                popupPage.HasSystemPadding = (bool)hasSystemPadding;
            }

            var hasKeyboardOffset = view.GetValue(PopupDialogLayout.HasKeyboardOffsetProperty);
            if (hasKeyboardOffset != null)
            {
                popupPage.HasKeyboardOffset = (bool)hasKeyboardOffset;
            }

            return popupPage;
        }

        private View CreateViewFor(string name)
        {
            var view = (View)this.ContainerProvider.Resolve<object>(name);
            return view;
        }

        private IDialogAware GetDialogController(View view)
        {
            if (view is IDialogAware viewAsDialogAware)
            {
                return viewAsDialogAware;
            }

            return view.BindingContext switch
            {
                null => throw new DialogException(DialogException.NoViewModel),
                IDialogAware dialogAware => dialogAware,
                _ => throw new DialogException(DialogException.ImplementIDialogAware)
            };
        }

        private IDialogAware InitializeDialog(View view, IDialogParameters parameters)
        {
            var dialog = this.GetDialogController(view);

            dialog.OnDialogOpened(parameters);

            return dialog;
        }

        private IDialogResult CloseDialog(IDialogParameters parameters, PopupPage popupPage, View dialogView)
        {
            try
            {
                if (parameters is null)
                {
                    parameters = new DialogParameters();
                }

                if (!parameters.TryGetValue<bool>(KnownPopupDialogParameters.Animated, out var animated))
                {
                    animated = true;
                }

                var dialogAware = this.GetDialogController(dialogView);

                if (!dialogAware.CanCloseDialog())
                {
                    throw new DialogException(DialogException.CanCloseIsFalse);
                }

                dialogAware.OnDialogClosed();
                this.PopupNavigation.RemovePageAsync(popupPage, animated);

                return new DialogResult
                {
                    Parameters = parameters
                };
            }
            catch (DialogException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return new DialogResult
                {
                    Exception = ex,
                    Parameters = parameters
                };
            }
        }

        private async void PushPopupPage(PopupPage popupPage, View dialogView, Action<IDialogParameters> closeCallback,
            bool animated = true)
        {
            var mask = DialogLayout.GetMask(dialogView);
            var gesture = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command<IDialogParameters>(closeCallback),
                CommandParameter = new DialogParameters()
            };

            if (mask is null)
            {
                var overlayStyle = GetOverlayStyle(dialogView);

                mask = new BoxView
                {
                    Style = overlayStyle
                };
            }

            mask.SetBinding(VisualElement.WidthRequestProperty, new Binding { Path = "Width", Source = popupPage });
            mask.SetBinding(VisualElement.HeightRequestProperty, new Binding { Path = "Height", Source = popupPage });
            mask.GestureRecognizers.Add(gesture);

            var overlay = new AbsoluteLayout();
            var relativeWidth = DialogLayout.GetRelativeWidthRequest(dialogView);
            if (relativeWidth != null)
            {
                dialogView.SetBinding(VisualElement.WidthRequestProperty,
                    new Binding("Width",
                        BindingMode.OneWay,
                        new RelativeContentSizeConverter { RelativeSize = relativeWidth.Value },
                        source: popupPage));
            }

            var relativeHeight = DialogLayout.GetRelativeHeightRequest(dialogView);
            if (relativeHeight != null)
            {
                dialogView.SetBinding(VisualElement.HeightRequestProperty,
                    new Binding("Height",
                        BindingMode.OneWay,
                        new RelativeContentSizeConverter() { RelativeSize = relativeHeight.Value },
                        source: popupPage));
            }

            //AbsoluteLayout.SetLayoutFlags(content, AbsoluteLayoutFlags.PositionProportional);
            //AbsoluteLayout.SetLayoutBounds(content, new Rectangle(0f, 0f, popupPage.Width, popupPage.Height));
            AbsoluteLayout.SetLayoutFlags(dialogView, AbsoluteLayoutFlags.PositionProportional);
            var popupBounds = DialogLayout.GetLayoutBounds(dialogView);
            AbsoluteLayout.SetLayoutBounds(dialogView, popupBounds);
            //overlay.Children.Add(content);
            if (DialogLayout.GetUseMask(dialogView) ?? true)
            {
                overlay.Children.Add(mask);
            }
            else
            {
                overlay.GestureRecognizers.Add(gesture);
            }

            overlay.Children.Add(dialogView);
            popupPage.Content = overlay;
            await this.PopupNavigation.PushAsync(popupPage, animated);
        }

        private static Style GetOverlayStyle(View popupView)
        {
            var style = DialogLayout.GetMaskStyle(popupView);
            if (style != null)
            {
                return style;
            }

            if (Application.Current!.Resources.ContainsKey(PopupOverlayStyle))
            {
                style = (Style)Application.Current.Resources[PopupOverlayStyle];
                if (style.TargetType == typeof(BoxView))
                {
                    return style;
                }
            }

            var overlayStyle = new Style(typeof(BoxView));
            overlayStyle.Setters.Add(new Setter { Property = VisualElement.OpacityProperty, Value = 0.75 });
            overlayStyle.Setters.Add(new Setter { Property = VisualElement.BackgroundColorProperty, Value = Colors.Black });

            Application.Current.Resources[PopupOverlayStyle] = overlayStyle;
            return overlayStyle;
        }

        private class DialogResult : IDialogResult
        {
            public Exception Exception { get; set; }
            public IDialogParameters Parameters { get; set; }

            public ButtonResult Result { get; }
        }

        public void ShowDialog(string name, IDialogParameters parameters, DialogCallback callback)
        {
            throw new NotImplementedException();
        }
    }
}