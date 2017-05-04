using PropertyChanged;
using SaveOn.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaveOn.Models
{
    [ImplementPropertyChanged]
    public class CouponList
    {
        private List<Coupon> coupons = new List<Coupon>();
        public String backpackType { get; set; }
        public User currentUser { get; set; }
        public List<Coupon> couponList
        {
            get { return coupons; }
            set { coupons = value; }
        }

    }
}
