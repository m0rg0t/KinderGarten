using Bing.Maps;
using Callisto.Controls;
using moscow_parks.Controls;
using moscow_parks.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Шаблон элемента страницы сведений о группе задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234229

namespace moscow_parks
{
    /// <summary>
    /// Страница, на которой показываются общие сведения об отдельной группе, включая предварительный просмотр элементов
    /// внутри группы.
    /// </summary>
    public sealed partial class ParksListPage : moscow_parks.Common.LayoutAwarePage
    {
        public ParksListPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Заполняет страницу содержимым, передаваемым в процессе навигации. Также предоставляется любое сохраненное состояние
        /// при повторном создании страницы из предыдущего сеанса.
        /// </summary>
        /// <param name="navigationParameter">Значение параметра, передаваемое
        /// <see cref="Frame.Navigate(Type, Object)"/> при первоначальном запросе этой страницы.
        /// </param>
        /// <param name="pageState">Словарь состояния, сохраненного данной страницей в ходе предыдущего
        /// сеанса. Это значение будет равно NULL при первом посещении страницы.</param>
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Присвоить this.DefaultViewModel["Group"] связываемую группу
            // TODO: Присвоить this.DefaultViewModel["Items"] коллекцию связываемых элементов

            await ViewModelLocator.MainStatic.LoadData();

            foreach (ChildPlaceItem item in ViewModelLocator.MainStatic.Items)
            {
                Pushpin pushpin = new Pushpin();
                MapLayer.SetPosition(pushpin, new Location(item.Lat, item.Lon));
                pushpin.Name = item.Id.ToString();
                pushpin.Tapped += pushpinTapped;
                map.Children.Add(pushpin);
            };         
        }

        Callisto.Controls.Flyout box = new Callisto.Controls.Flyout();

        private async void pushpinTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Pushpin tappedpin = sender as Pushpin;  // gets the pin that was tapped
            if (null == tappedpin) return;  // null check to prevent bad stuff if it wasn't a pin.
            ViewModelLocator.MainStatic.CurrentItem = (ChildPlaceItem)ViewModelLocator.MainStatic.Items.FirstOrDefault(c => c.Id.ToString() == tappedpin.Name.ToString());

            var x = MapLayer.GetPosition(tappedpin);

            box = new Callisto.Controls.Flyout();
            box.Placement = PlacementMode.Top;
            box.Content = new TouristControl();
            box.PlacementTarget = sender as UIElement;
            box.IsOpen = true;
            //MessageDialog dialog = new MessageDialog("You are here " + x.Latitude + " " + x.Longitude);
            //await dialog.ShowAsync();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested -= Settings_CommandsRequested;
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += Settings_CommandsRequested;
            base.OnNavigatedTo(e);
        }

        void Settings_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            try
            {
                var viewAboutPage = new SettingsCommand("", "Об авторе", cmd =>
                {
                    //(Window.Current.Content as Frame).Navigate(typeof(AboutPage));
                    var settingsFlyout = new Callisto.Controls.SettingsFlyout();
                    settingsFlyout.Content = new About();
                    settingsFlyout.HeaderText = "Об авторе";

                    settingsFlyout.IsOpen = true;
                });
                args.Request.ApplicationCommands.Add(viewAboutPage);

                var viewAboutMalukahPage = new SettingsCommand("", "Политика конфиденциальности", cmd =>
                {
                    var settingsFlyout = new Callisto.Controls.SettingsFlyout();
                    settingsFlyout.Content = new Privacy();
                    settingsFlyout.HeaderText = "Политика конфиденциальности";

                    settingsFlyout.IsOpen = true;
                });
                args.Request.ApplicationCommands.Add(viewAboutMalukahPage);
            }
            catch { };
        }

        private void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                box.IsOpen = false;
                var item = ((ChildPlaceItem)e.ClickedItem);
                this.Frame.Navigate(typeof(ParkDetailPage), item.Id.ToString());
            }
            catch { };
        }

        private void MapButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                box.IsOpen = false;
                this.Frame.Navigate(typeof(MapPage));
            }
            catch { };
        }

        private void MapTitle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                box.IsOpen = false;
                this.Frame.Navigate(typeof(MapPage));
            }
            catch { };
        }
    }
}
