using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace SaveOn.ViewModels
{
    public class Coupon : TransformViewModel
    {

        //public string[] images = new[] { "Flusi1.jpg", "Flusi2.jpg", "Flusi3.jpg" };
        //protected string[] images = new[] { "Pic1.png", "Pic2.png", "Pic3.png", "Pic4.png" };
        //protected int currentImage = 0;
        public int id { get; set; }
        public string ImageUrl { get; set; }
        public ImageSource ImageStream { get; set; }
        public string StoreId { get; set; }
        public string CouponType { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        protected override void OnSwiped(MR.Gestures.SwipeEventArgs e)
        {
            base.OnSwiped(e);
            //ShoppingList mainInfo = new ShoppingList();
            Debug.WriteLine("direction---" + e.Direction);
            Debug.WriteLine("direction---" + e.Center);
            if (e.Direction == MR.Gestures.Direction.Right)
            {
                // mainInfo.addCoupon(this);          
                Debug.WriteLine("right");
            }
            else if (e.Direction == MR.Gestures.Direction.Left)
            {
                Debug.WriteLine("left");
                //mainInfo.deleteCoupon(this);

            }

        }



        public Coupon()
            : base()
        {
        }
    }
}
