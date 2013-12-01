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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Шаблон элемента страницы сведений об элементе задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234232

namespace moscow_parks
{
    /// <summary>
    /// Страница, на которой отображаются сведения об отдельном элементе внутри группы; при этом можно с помощью жестов
    /// перемещаться между другими элементами из этой группы.
    /// </summary>
    public sealed partial class ParkDetailPage : moscow_parks.Common.LayoutAwarePage
    {
        public ParkDetailPage()
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
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // Разрешение сохраненному состоянию страницы переопределять первоначально отображаемый элемент
            if (pageState != null && pageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = pageState["SelectedItem"];
            }

            // TODO: Присвоить this.DefaultViewModel["Group"] связываемую группу
            // TODO: Присвоить this.DefaultViewModel["Items"] коллекцию связываемых элементов
            // TODO: Назначение выбранного элемента объекту this.flipView.SelectedItem
            this.DefaultViewModel["Items"] = ViewModelLocator.MainStatic.Items;

            var item = ViewModelLocator.MainStatic.Items.FirstOrDefault(c => c.Id.ToString() == (String)navigationParameter);
            this.flipView.SelectedItem = item;
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

        /// <summary>
        /// Сохраняет состояние, связанное с данной страницей, в случае приостановки приложения или
        /// удаления страницы из кэша навигации. Значения должны соответствовать требованиям сериализации
        /// <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">Пустой словарь, заполняемый сериализуемым состоянием.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var selectedItem = this.flipView.SelectedItem;
            // TODO: Создание производного сериализуемого параметра навигации и его назначение объекту pageState["SelectedItem"]
        }

        private void map_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void flipView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = (ChildPlaceItem)this.flipView.SelectedItem;

                Pushpin pushpin = new Pushpin();
                MapLayer.SetPosition(pushpin, new Location(selectedItem.Lat, selectedItem.Lon));
                pushpin.Name = selectedItem.Id.ToString();
                //pushpin.Tapped += pushpinTapped;
                this.map.Children.Clear();
                this.map.Children.Add(pushpin);
                this.map.SetView(new Location(selectedItem.Lat, selectedItem.Lon), 13);
            }
            catch { };
        }

        private void flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModelLocator.MainStatic.CurrentItem = (ChildPlaceItem)this.flipView.SelectedItem;
            /*ViewModelLocator.MainStatic.Stations.CurrentStation = (YAStationItem)this.flipView.SelectedItem;
            DependencyObject deObj = sender as DependencyObject;
            vtree(deObj, 0);*/
            try
            {
                var selectedItem = (ChildPlaceItem)this.flipView.SelectedItem;

                Pushpin pushpin = new Pushpin();
                MapLayer.SetPosition(pushpin, new Location(selectedItem.Lat, selectedItem.Lon));
                pushpin.Name = selectedItem.Id.ToString();
                //pushpin.Tapped += pushpinTapped;
                this.map.Children.Clear();
                this.map.Children.Add(pushpin);
                this.map.SetView(new Location(selectedItem.Lat, selectedItem.Lon), 13);
            }
            catch { };
        }

        private void AddCommentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModelLocator.MainStatic.AddBox = new Callisto.Controls.Flyout();
                ViewModelLocator.MainStatic.AddBox.Placement = PlacementMode.Top;
                ViewModelLocator.MainStatic.AddBox.Content = new AddCommentControl();
                ViewModelLocator.MainStatic.AddBox.PlacementTarget = sender as UIElement;
                ViewModelLocator.MainStatic.AddBox.IsOpen = true;
            }
            catch { };
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    ChildPlaceItem item = (ChildPlaceItem)this.flipView.SelectedItem;
                    ViewModelLocator.MainStatic.CurrentItem = item;

                    //if (item.CommentItems.Count() < 1)
                    //{
                    ViewModelLocator.MainStatic.Loading = true;
                    //await item.LoadComments();
                    ViewModelLocator.MainStatic.Loading = false;

                    /*if (item.CommentItems.Count() < 1)
                    {
                        MessageDialog result = new MessageDialog("К сожалению у района еще не добавлены комментарии о качетсве воды. Вы можете стать первым и добавить свой комментарий.");
                        result.ShowAsync();
                    }
                    else
                    {
                        this.Frame.Navigate(typeof(CommentsSplitPage), item.Id.ToString());
                    };*/
                    //};
                }
                catch { };
            }
            catch { };
        }
    }
}
