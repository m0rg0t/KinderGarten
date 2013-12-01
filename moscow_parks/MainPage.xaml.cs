using Microsoft.WindowsAzure.MobileServices;
using moscow_parks.Common;
using moscow_parks.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace moscow_parks
{
    public class TodoItem
    {
        public int Id { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "complete")]
        public bool Complete { get; set; }
    }


    public sealed partial class MainPage : Page
    {
        private MobileServiceCollection<TodoItem, TodoItem> items;
        private IMobileServiceTable<TodoItem> todoTable = App.MobileService.GetTable<TodoItem>();

        private MobileServiceCollection<ChildPlaceItem, ChildPlaceItem> parkItems;
        private IMobileServiceTable<ChildPlaceItem> ParksTable = App.MobileService.GetTable<ChildPlaceItem>();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void InsertTodoItem(TodoItem todoItem)
        {
            // This code inserts a new TodoItem into the database. When the operation completes
            // and Mobile Services has assigned an Id, the item is added to the CollectionView
            await todoTable.InsertAsync(todoItem);
            items.Add(todoItem);                        
        }

        private async void RefreshTodoItems()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                // This code refreshes the entries in the list view by querying the TodoItems table.
                // The query excludes completed TodoItems
                items = await todoTable
                    .Where(todoItem => todoItem.Complete == false)
                    .ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }
            else
            {
                ListItems.ItemsSource = items;
            }
        }

        private async void UpdateCheckedTodoItem(TodoItem item)
        {
            // This code takes a freshly completed TodoItem and updates the database. When the MobileService 
            // responds, the item is removed from the list 
            await todoTable.UpdateAsync(item);
            items.Remove(item);
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshTodoItems();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var todoItem = new TodoItem { Text = TextInput.Text };
            InsertTodoItem(todoItem);
        }

        private void CheckBoxComplete_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TodoItem item = cb.DataContext as TodoItem;
            UpdateCheckedTodoItem(item);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RefreshTodoItems();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            string earthQuakeData = await client.GetStringAsync("http://m0rg0t.com/parks.csv");

            // Parse the file 
            CsvParser csvParser = new CsvParser();
            csvParser.RawText = earthQuakeData;
            csvParser.HasHeaderRow = true;
            csvParser.Delimiter = ';';

            var results = await csvParser.Parse();

            foreach (var item in results)
            {
                ChildPlaceItem childPlace = new ChildPlaceItem();
                //childPlace.Id = Int32.Parse(item["1_Номер"]);
                childPlace.Name = item["1_Название"];
                childPlace.Title = item["0_label"];
                childPlace.Address = item["1_Адрес"];
                childPlace.Lat = Double.Parse(item["0_y"].ToString().Replace(",","."));
                childPlace.Lon = Double.Parse(item["0_x"].ToString().Replace(",", "."));
                childPlace.Ao = item["1_Административный округ"];
                childPlace.District = item["1_Район"];
                childPlace.OfficialAddress = item["1_Юридический адрес"];
                childPlace.Phone = item["1_Телефон"];
                childPlace.Fax = item["1_Факс"];
                childPlace.Site = item["1_Сайт"];
                childPlace.Email = item["1_Адрес электронной почты"];
                childPlace.Image = "http://dic.academic.ru/pictures/wiki/files/71/Grand_Kremlin_Public_Garden-2.jpg";

                await ParksTable.InsertAsync(childPlace);
            };
           
        }
    }
}
