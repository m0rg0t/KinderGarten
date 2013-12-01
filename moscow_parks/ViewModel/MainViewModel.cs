using Callisto.Controls;
using GalaSoft.MvvmLight;
using Microsoft.WindowsAzure.MobileServices;
using moscow_parks.Common;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace moscow_parks.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }

        private ObservableCollection<ChildPlaceItem> _items = new ObservableCollection<ChildPlaceItem>();
        /// <summary>
        /// Park items
        /// </summary>
        public ObservableCollection<ChildPlaceItem> Items
        {
            get { return _items; }
            set { 
                _items = value;
                RaisePropertyChanged("Items");
            }
        }

        private bool _loading = false;

        public bool Loading
        {
            get { return _loading; }
            set { 
                _loading = value;
                RaisePropertyChanged("Loading");
            }
        }

        private ChildPlaceItem _currentItem;

        public ChildPlaceItem CurrentItem
        {
            get { return _currentItem; }
            set { 
                _currentItem = value;
                RaisePropertyChanged("CurrentItem");
            }
        }

        public Flyout AddBox = new Flyout();
               

        public async Task<bool> LoadData()
        {
            this.Loading = true;
            //Items = await ParksTable.ToCollectionAsync(100);

            HttpClient client = new HttpClient();
            string earthQuakeData = await client.GetStringAsync("http://m0rg0t.com/childplaces.csv");

            //var file = Windows.Storage.StorageFile.GetFileFromPathAsync("ms-appx:///Data/childplaces.csv");
            //string read = await Windows.Storage.FileIO.ReadTextAsync();

            // Parse the file 
            CsvParser csvParser = new CsvParser();
            csvParser.RawText = earthQuakeData;
            csvParser.HasHeaderRow = true;
            csvParser.Delimiter = ';';

            var results = await csvParser.Parse();

            foreach (var item in results)
            {
                try
                {
                    ChildPlaceItem childPlace = new ChildPlaceItem();
                    childPlace.Id = Int32.Parse(item["global_id"]);
                    childPlace.Name = item["name"];
                    childPlace.Title = item["name"];
                    childPlace.Address = item["adress"];
                    try
                    {
                        double result;
                        if (Double.TryParse(item["Y"].ToString().Replace(",", "."), out result))
                        {
                            childPlace.Lat = Double.Parse(item["Y"].ToString().Replace(",", "."));
                        }
                        else
                        {
                            childPlace.Lat = Double.Parse(item["Y"].ToString());
                        };
                    }
                    catch { };
                    try
                    {
                        //childPlace.Lon = Double.Parse(item["X"].ToString().Replace(",", "."));
                        double result;
                        if (Double.TryParse(item["X"].ToString().Replace(",", "."), out result))
                        {
                            childPlace.Lon = Double.Parse(item["X"].ToString().Replace(",", "."));
                        }
                        else
                        {
                            childPlace.Lon = Double.Parse(item["X"].ToString());
                        };
                    }
                    catch { };
                    childPlace.Ao = item["okrug"];
                    childPlace.District = item["rayon"];
                    childPlace.OfficialAddress = item["adress"];
                    childPlace.Phone = item["telephone"];
                    //childPlace.Fax = item["1_ิเ๊๑"];
                    childPlace.Site = item["web_site"];
                    childPlace.Email = item["e_mail"];
                    //childPlace.Image = "http://dic.academic.ru/pictures/wiki/files/71/Grand_Kremlin_Public_Garden-2.jpg";

                    this.Items.Add(childPlace);
                }
                catch { };
            };

            RaisePropertyChanged("Items");
            this.Loading = false;

            return true;
        }
        
    }
}