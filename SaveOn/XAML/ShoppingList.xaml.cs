using MR.Gestures;
using Newtonsoft.Json;
using Plugin.Geolocator;
using SaveOn.Azure;
using SaveOn.GoogleModels;
using SaveOn.Models;
using SaveOn.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SaveOn.XAML
{
    public partial class ShoppingList
    {
        private string googleKey = "AIzaSyAQbsw46ULkgMsMOUYaDveKGkya0qYOlaU";
        private List<Coupon> masterCoupons = new List<Coupon>();
        public List<Coupon> savedCoupons = new List<Coupon>();
        private FacebookUser facebookUser = new FacebookUser();
        private CouponList couponList = new CouponList();
        private User user = new User();
        private User.FoodPreferences foodPreferences = new User.FoodPreferences();

        List<ImageSource> couponsImages = new List<ImageSource>();
        AzureDataService azure = new AzureDataService();

        private string limeridge = "43.217703,-79.861937";
        private string rad = "2000";
        string nextPage;
        string RestUrl;
        private string name;
        private string nearbySearch = "https://maps.googleapis.com/maps/api/place/radarsearch/json";

        public ShoppingList(User _user)
        {
            BindingContext = couponList;
            user = _user;
            Debug.WriteLine("Shopping List Has ="+user.name);
            //OnDownloadImageButtonClicked();
            InitializeComponent();
        }


        async void OnDownloadImageButtonClicked()
        {


            listView.ItemsSource = null;
            await azure.Initialize();
            user = azure.GetUserInfo(facebookUser.Id);
            foodPreferences = azure.GetUserPreferences(user);

            masterCoupons = await RefreshDataAsync(foodPreferences);
            masterCoupons = filterCoupons(masterCoupons);
            //couponList.createList(masterCoupons);
            couponList.currentUser = user;
            couponList.couponList = masterCoupons;
            // masterCoupons = couponList.couponList;
            //Debug.WriteLine("master coupons at start ---- >>>>"+ masterCoupons.Count);
            //var imageList = await AzureStorage.GetFilesListAsync(ContainerType.Image);
            //foreach (var image in imageList)
            //{
            //    Debug.WriteLine(image);
            //    names.ImageStream = ImageSource.FromUri(new Uri(image));
            //}
            foreach (var names in masterCoupons)
            {
                if (!string.IsNullOrWhiteSpace(names.ImageUrl))
                {
                    var imageData = await AzureStorage.GetFileAsync(ContainerType.Image, names.ImageUrl);
                    names.ImageStream = ImageSource.FromStream(() => new MemoryStream(imageData));
                    couponsImages.Add(ImageSource.FromStream(() => new MemoryStream(imageData)));

                }

            }

            listView.ItemsSource = couponList.couponList;

            // image.SetBinding(Image.SourceProperty, "CouponImage");
            //image.SetBinding(VisualElement.IsVisibleProperty, "CouponImage");
            //BindingContext = couponList.couponList;
            name = azure.FindUserFB(facebookUser.Name, facebookUser.Id);


        }

        private List<Coupon> filterCoupons(List<Coupon> couponList)
        {
            List<string> locationNames = new List<string>();
            List<Coupon> fixedList = new List<Coupon>();
            foreach (var _coupon in couponList)
            {
                if (!locationNames.Contains(_coupon.ImageUrl))
                {
                    locationNames.Add(_coupon.ImageUrl);
                    fixedList.Add(_coupon);
                }
            }
            return fixedList;
        }
        void ListView_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }
            DisplayAlert("Item Selected", e.SelectedItem.ToString(), "Ok");
        }


        private async void GetGPS()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;
            var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

            // fakeLong = 43.217703;
            //fakeLat = -79.861937;
            //Longitude.Text = fakeLong.ToString();         //position.Longitude.ToString();
            // Latitude.Text = fakeLat.ToString();           //position.Latitude.ToString();
        }
        public async Task<List<Coupon>> RefreshDataAsync(User.FoodPreferences userPreferences)
        {
            List<Coupon> couponInfo = new List<Coupon>();
            List<String> couponUrls = new List<String>();


            List<String> top5List = new List<string>();
            String oneRestUrl;
            String allFavFood = "";
            bool first = true;


            top5List = getTop5(userPreferences);
            //adding location
            RestUrl = nearbySearch + "?location=" + limeridge;

            foreach (var favFood in top5List)
            {
                if (first == true)
                {
                    allFavFood = favFood;
                    first = false;
                }
                allFavFood = allFavFood + "&" + favFood;
            }
            oneRestUrl = RestUrl + "&Type=food&keyword=" + allFavFood + "&radius=" + rad + "&key=" + googleKey;
            using (var client = new HttpClient())
            {
                //var watch2 = System.Diagnostics.Stopwatch.StartNew();
                var response = await client.GetStringAsync(string.Format(oneRestUrl));
                var result = JsonConvert.DeserializeObject<GooglePlaces>(response);
                //watch2.Stop();
                //var elapsedMs2 = watch2.ElapsedMilliseconds;
                //Debug.WriteLine( "-------- client response and deserialize ------>" + elapsedMs2);
                foreach (var info in result.results)
                {
                    couponUrls.Add(info.place_id);

                }
                couponInfo = await azure.GetCoupons(couponUrls);
            }
            return couponInfo;
        }

        public List<String> getTop5(User.FoodPreferences foodPreferences)
        {
            String[,] food = new String[18, 2];
            List<String> Top5Food = new List<string>();
            int highestRating;
            int lastRating;
            food[0, 0] = "cafe";
            food[0, 1] = String.Format("{0}", foodPreferences.Cafe);
            food[1, 0] = "canadian";
            food[1, 1] = String.Format("{0}", foodPreferences.Canadian);
            food[2, 0] = "chinese";
            food[2, 1] = String.Format("{0}", foodPreferences.Chinese);
            food[3, 0] = "dessert";
            food[3, 1] = String.Format("{0}", foodPreferences.Dessert);
            food[4, 0] = "fastfood";
            food[4, 1] = String.Format("{0}", foodPreferences.FastFood);
            food[5, 0] = "french";
            food[5, 1] = String.Format("{0}", foodPreferences.French);
            food[6, 0] = "glutenfree";
            food[6, 1] = String.Format("{0}", foodPreferences.GlutenFree);
            food[7, 0] = "greek";
            food[7, 1] = String.Format("{0}", foodPreferences.Greek);
            food[8, 0] = "indian";
            food[8, 1] = String.Format("{0}", foodPreferences.Indian);
            food[9, 0] = "italian";
            food[9, 1] = String.Format("{0}", foodPreferences.Italian);
            food[10, 0] = "japanese";
            food[10, 1] = String.Format("{0}", foodPreferences.Japanese);
            food[11, 0] = "mediterranean";
            food[11, 1] = String.Format("{0}", foodPreferences.Mediterranean);
            food[12, 0] = "mexican";
            food[12, 1] = String.Format("{0}", foodPreferences.Mexican);
            food[13, 0] = "organic";
            food[13, 1] = String.Format("{0}", foodPreferences.Organic);
            food[14, 0] = "thai";
            food[14, 1] = String.Format("{0}", foodPreferences.Thai);
            food[15, 0] = "vegan";
            food[15, 1] = String.Format("{0}", foodPreferences.Vegan);
            food[16, 0] = "vegetarian";
            food[16, 1] = String.Format("{0}", foodPreferences.Vegetarian);
            food[17, 0] = "spanish";
            food[17, 1] = String.Format("{0}", foodPreferences.Spanish);


            //var cafe =;

            return LargestSum(food);
        }
        public List<String> LargestSum(String[,] array)
        {
            int firstLargest = 0, secondLargest = 0, thirdLargest = 0, fourthLargest = 0, fifthLargest = 0;
            List<String> favouriteFood = new List<String>();
            String firstType = "", secondType = "", thirdType = "", fourthType = "", fifthType = "";
            for (int x = 0; x < array.GetLength(0); x++)
            {
                int value = Int32.Parse(String.Format("{0}", array[x, 1]));
                String type = array[x, 0];
                if (value > fifthLargest)
                {
                    if (value > fourthLargest)
                    {
                        if (value > thirdLargest)
                        {
                            if (value > secondLargest)
                            {
                                if (value > firstLargest)
                                {
                                    fifthType = fourthType;
                                    fifthLargest = fourthLargest;
                                    fourthType = thirdType;
                                    fourthLargest = thirdLargest;
                                    thirdType = secondType;
                                    thirdLargest = secondLargest;
                                    secondType = firstType;
                                    secondLargest = firstLargest;
                                    firstType = type;
                                    firstLargest = value;
                                }
                                else
                                {
                                    fifthType = fourthType;
                                    fifthLargest = fourthLargest;
                                    fourthType = thirdType;
                                    fourthLargest = thirdLargest;
                                    thirdType = secondType;
                                    thirdLargest = secondLargest;
                                    secondType = type;
                                    secondLargest = value;
                                }
                            }
                            else
                            {
                                fifthType = fourthType;
                                fifthLargest = fourthLargest;
                                fourthType = thirdType;
                                fourthLargest = thirdLargest;
                                thirdType = type;
                                thirdLargest = value;
                            }

                        }
                        else
                        {
                            fifthType = fourthType;
                            fifthLargest = fourthLargest;
                            fourthType = type;
                            fourthLargest = value;
                        }
                    }
                    else
                    {
                        fifthType = type;
                        fifthLargest = value;
                    }
                }
            }

            favouriteFood.Add(firstType);
            favouriteFood.Add(secondType);
            favouriteFood.Add(thirdType);
            favouriteFood.Add(fourthType);
            favouriteFood.Add(fifthType);
            return favouriteFood;
        }

        private void SetPageContent()
        {
            OnDownloadImageButtonClicked();
            Content = listView;
        }
        private void Image_OnSwiped(object sender, SwipeEventArgs e)
        {
            var imageCoupon = (MR.Gestures.Image)sender;
            var _coupon = couponList.couponList.FirstOrDefault(c => c.ImageStream == imageCoupon.Source);
            if (e.Direction == Direction.Right)
            {
                Debug.WriteLine("ADDING - " + _coupon.ImageUrl);
                //savedCoupons.Add(_coupon);
                azure.AddToBackpack(_coupon, user);

            }
            else if (e.Direction == Direction.Left)
            {
                Debug.WriteLine("DELETING - " + _coupon.ImageUrl);

            }
            couponList.couponList.Remove(_coupon);

            foreach (var c in couponList.couponList)
            {
                Debug.WriteLine("CHECKING - " + c.ImageUrl);
            }
            listView.ItemsSource = null;
            listView.ItemsSource = couponList.couponList;
        }
    }
}
