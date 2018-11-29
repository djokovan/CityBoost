using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CityBoost.ViewModels
{
    public class SearchVM
    {
        public string ContinentId { get; set; }

        public List<SelectListItem> Continents { get; set; }

        public string CountryId { get; set; }

        public List<SelectListItem> Countires { get; set; }

        // DataTablesResponse params
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public string Column { get; set; }
        public string SortDir { get; set; }
        public string Search { get; set; }
    }
}