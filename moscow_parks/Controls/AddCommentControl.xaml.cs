using Microsoft.WindowsAzure.MobileServices;
using moscow_parks.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

// Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236

namespace moscow_parks.Controls
{
    public sealed partial class AddCommentControl : UserControl
    {
        public AddCommentControl()
        {
            this.InitializeComponent();
        }

        private MobileServiceCollection<CommentItem, CommentItem> commentItems;
        private IMobileServiceTable<CommentItem> CommentTable = App.MobileService.GetTable<CommentItem>();

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CommentItem item = new CommentItem();
                item.Username = this.Username.Text;
                item.Comment = this.Comment.Text;
                item.CreatedAt = DateTime.Now;
                item.ParkId = ViewModelLocator.MainStatic.CurrentItem.Id.ToString();
                ViewModelLocator.MainStatic.AddBox.IsOpen = false;
                if ((item.Username != "") && (item.Username != ""))
                {
                    await CommentTable.InsertAsync(item);                    
                    MessageDialog result = new MessageDialog("Комментарий успешно добавлен.");
                    result.ShowAsync();
                }
                else
                {
                    MessageDialog result = new MessageDialog("Укажите ваше имя и текст комментария.");
                    result.ShowAsync();
                };
            }
            catch { };
        }
    }
}
