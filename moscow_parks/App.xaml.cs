using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.MobileServices;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace moscow_parks
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        // This MobileServiceClient has been configured to communicate with your Mobile Service's url
        // and application key. You're all set to start working with your Mobile Service!
        public static MobileServiceClient MobileService = new MobileServiceClient(
            "https://moscow-parks.azure-mobile.net/",
            "FYMJRmyzrmjgNKEfdkUGOHHwbraOqe36"
        );

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Do not repeat app initialization when already running, just ensure that
            // the window is active
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                Window.Current.Activate();
                return;
            }

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
            }

            // Create a Frame to act navigation context and navigate to the first page
            var rootFrame = new Frame();
            if (!rootFrame.Navigate(typeof(ParksListPage)))
            {
                throw new Exception("Failed to create initial page");
            }

            // Place the frame in the current Window and ensure that it is active
            Window.Current.Content = rootFrame;
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        /// <summary>
        /// Вызывается при активации приложения для отображения результатов поиска.
        /// </summary>
        /// <param name="args">Сведения о запросе на активацию.</param>
        protected async override void OnSearchActivated(Windows.ApplicationModel.Activation.SearchActivatedEventArgs args)
        {
            // TODO: Регистрация события Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().QuerySubmitted
            // в OnWindowCreated для ускорения поиска во время выполнения приложения

            // Если в окне еще не используется навигация по фреймам, вставьте собственный фрейм
            var previousContent = Window.Current.Content;
            var frame = previousContent as Frame;

            // Если приложение не содержит фрейм верхнего уровня, то, возможно, это
            // первый запуск приложения. Обычно этот метод и метод OnLaunched 
            // из файла App.xaml.cs могут вызывать общий метод.
            if (frame == null)
            {
                // Создание фрейма, играющего роль контекста навигации, и его связывание с
                // ключом SuspensionManager
                frame = new Frame();
                moscow_parks.Common.SuspensionManager.RegisterFrame(frame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Восстановление сохраненного состояния сеанса только при необходимости
                    try
                    {
                        await moscow_parks.Common.SuspensionManager.RestoreAsync();
                    }
                    catch (moscow_parks.Common.SuspensionManagerException)
                    {
                        //Возникли ошибки при восстановлении состояния.
                        //Предполагаем, что состояние отсутствует, и продолжаем
                    }
                }
            }

            frame.Navigate(typeof(SearchResultsPage), args.QueryText);
            Window.Current.Content = frame;

            // Обеспечение активности текущего окна
            Window.Current.Activate();
        }
    }
}
