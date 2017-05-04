using Newtonsoft.Json;
using SaveOn.Azure;
using SaveOn.GoogleModels;
using SaveOn.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SaveOn.XAML
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : TabbedPage
	{
        private readonly AzureDataService _azure;
        private GoogleModels.Location vm;
        //WebView webView = new WebView();
        private CouponList couponList = new CouponList();
        //private FacebookUser facebookUser = new FacebookUser();
        private User User = new User();
        //private FacebookUser facebookUser = new FacebookUser();
        private string ClientId = "173756816475288";
        private string token;
        public MainPage(string _aToken)
        {
            token = _aToken;
            //vm = new Location();
            _azure = new AzureDataService();
            
            //var navigationPage = new NavigationPage(new BackpackMain());
            //BindingContext
            Children.Add(new ShoppingList(User));
            Children.Add(new BackpackView(User));



            InitializeComponent();
            //WebViewOnNavigated();
        }

        // public LocationDetailPage(Location location)
        //{

        //}
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            FacebookUser facebookUser = new FacebookUser();
            if (String.IsNullOrEmpty(token))
            {
                Debug.WriteLine(facebookUser);
                await Navigation.PushModalAsync(new LoginPage());
            } else
            {
                facebookUser = await GetFacebookProfileAsync(token);
                Debug.WriteLine("facebookUser"+facebookUser);
                User =_azure.GetUserInfo(facebookUser.Id);
            }
                

        }


        public async Task<FacebookUser> GetFacebookProfileAsync(string accessToken)
        {
            FacebookUser facebookUser = new FacebookUser();
            var requestUrl = "https://graph.facebook.com/v2.8/me/"
                             + "?fields=name,picture,cover,age_range,devices,email,gender,is_verified"
                             + "&access_token=" + accessToken;
            var httpClient = new HttpClient();
            var userJson = await httpClient.GetStringAsync(requestUrl);
            facebookUser = JsonConvert.DeserializeObject<FacebookUser>(userJson);
            return facebookUser;
        }
    }
}
