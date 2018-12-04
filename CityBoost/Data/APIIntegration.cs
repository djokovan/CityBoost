using CityBoost.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CityBoost.Data
{
    public class APIIntegration
    {
        readonly string teleportAPI = System.Configuration.ConfigurationManager.AppSettings["API"];
        private static APIIntegration _apiInstance;

        private APIIntegration()
        {

        }

        public static APIIntegration GetAPIInstance()
        {
            if (_apiInstance == null)
            {
                _apiInstance = new APIIntegration();
            }

            return _apiInstance;
        }

        public List<SelectListItem> GetContinents()
        {
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;

                string continentsURL = teleportAPI + "continents";
                string continentsResponse = client.DownloadString(continentsURL);
                var continentsListJSON = JsonConvert.DeserializeObject<dynamic>(continentsResponse)["_links"]["continent:items"];

                List<SelectListItem> continents = new List<SelectListItem>();
                foreach (var c in continentsListJSON)
                {
                    string val = c["href"].ToString();
                    var name = c["name"].ToString();

                    string key = "geonames:";

                    continents.Add(new SelectListItem { Value = val.Substring(val.IndexOf(key), key.Length + 2), Text = name });
                }

                return continents;
            }
        }

        public List<SelectListItem> GetCountries(string continentId)
        {
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;

                string countriesURL = teleportAPI + "continents/" + continentId + "/countries";
                string countriesResponse = client.DownloadString(countriesURL);
                var countriesListJSON = JsonConvert.DeserializeObject<dynamic>(countriesResponse)["_links"]["country:items"];

                List<SelectListItem> countries = new List<SelectListItem>();
                foreach (var c in countriesListJSON)
                {
                    string val = c["href"].ToString();
                    var name = c["name"].ToString();

                    string key = "iso_alpha2:";

                    countries.Add(new SelectListItem { Value = val.Substring(val.IndexOf(key), key.Length + 2), Text = name });
                }

                return countries;
            }
        }

        public List<CityVM> GetCountryCities(string country)
        {
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;

                string citiesURL = teleportAPI + "cities";
                citiesURL = citiesURL + "?search=" + country;
                string citiesResponse = client.DownloadString(citiesURL);
                var citiesListJSON = JsonConvert.DeserializeObject<dynamic>(citiesResponse)["_embedded"]["city:search-results"];

                List<CityVM> citiesList = new List<CityVM>();
                foreach (var city in citiesListJSON)
                {
                    string fullName = city["matching_full_name"].ToString();
                    if (!fullName.Contains(country))
                    {
                        continue;
                    }

                    string cityUrl = city["_links"]["city:item"]["href"].ToString();
                    string cityResponse = client.DownloadString(cityUrl);
                    var cityDetailsJSON = JsonConvert.DeserializeObject<dynamic>(cityResponse);

                    List<UrbanAreaDetails> details = new List<UrbanAreaDetails>();
                    if (cityDetailsJSON["_links"].ContainsKey("city:urban_area"))
                    {
                        string cityUrbanArea = cityDetailsJSON["_links"]["city:urban_area"]["href"].ToString() + "details";
                        string cityUrbanAreaResponse = client.DownloadString(cityUrbanArea);
                        var categories = JsonConvert.DeserializeObject<dynamic>(cityUrbanAreaResponse)["categories"];

                        foreach (var category in categories)
                        {
                            var categoryData = category["data"];

                            List<ViewModels.Data> UADataList = new List<ViewModels.Data>();
                            foreach (var data in categoryData)
                            {
                                var dataDict = new RouteValueDictionary(data);
                                string val = dataDict.FirstOrDefault(x => x.Key.Contains("_value")).Value.ToString();

                                UADataList.Add(new ViewModels.Data
                                {
                                    DataId = dataDict["id"].ToString(),
                                    DataLabel = dataDict["label"].ToString(),
                                    DataValue = val
                                });
                            }

                            details.Add(new UrbanAreaDetails
                            {
                                UAId = category["id"].ToString(),
                                UALabel = category["label"].ToString(),
                                UAData = UADataList
                            });
                        }
                    }

                    string population = cityDetailsJSON["population"].ToString();
                    string latitude = cityDetailsJSON["location"]["latlon"]["latitude"].ToString();
                    string longitude = cityDetailsJSON["location"]["latlon"]["longitude"].ToString();

                    citiesList.Add(new CityVM
                    {
                        CityURL = cityUrl,
                        Name = fullName,
                        Population = int.Parse(population),
                        Latitude = latitude,
                        Longitude = longitude,
                        Details = details.Any() ? details : null
                    });
                }

                return citiesList;
            }
        }
    }
}