using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityBoost.ViewModels
{
    public class CityVM
    {
        public string CityURL { get; set; }

        public string Name { get; set; }

        public int Population { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public List<UrbanAreaDetails> Details { get; set; }
    }

    public class UrbanAreaDetails
    {
        public string UAId { get; set; }
        public string UALabel { get; set; }
        public List<Data> UAData { get; set; }
    }

    public class Data
    {
        public string DataId { get; set; }
        public string DataLabel { get; set; }
        public string DataValue { get; set; }
    }
}