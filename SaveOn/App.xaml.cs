using SaveOn.Models;
using SaveOn.XAML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SaveOn
{

    public partial class App : Application
    {
        static string _Token;
        static NavigationPage _navPage;
        private CouponList _couponList;
        public static int ScreenWidth;
        public static int ScreenHeight;
        //private FacebookConnect _fbConnect = new FacebookConnect();
        private User _user = new User();

        public App(Account _account)
        {
            InitializeComponent();
            string aToken = "";
            //Debug.WriteLine("account ->" + _account.Properties["access_token"]);
            if (_account != null)
                aToken = _account.Properties["access_token"];

            MainPage = new StartPage();
        }
        public static bool IsLoggedIn
        {
            get { return !string.IsNullOrWhiteSpace(_Token); }
        }

        public static string Token
        {
            get { return _Token; }
        }

        public static void SaveToken(string token)
        {
            Debug.WriteLine("setting " + token);
            _Token = token;
        }

        public static Action SuccessfulLoginAction
        {
            get
            {
                return new Action(() => {
                    Debug.WriteLine("logged in");
                    _navPage.Navigation.PopModalAsync();


                });
            }
        }
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
