using System;
using System.Collections.Generic;
using System.Text;

namespace SaveOn.Models
{
    public class Location
    {
        public string Name { get; set; }
        public string Notes { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public string Geolocation { get; set; }
        public DateTime CreatedTimeStamp { get; set; }
    }
}
