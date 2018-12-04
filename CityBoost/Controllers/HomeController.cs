using CityBoost.Data;
using CityBoost.ModelBinder;
using CityBoost.ViewModels;
using DataTables.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CityBoost.Controllers
{
    public class HomeController : Controller
    {
        private APIIntegration api;

        public HomeController()
        {
            api = APIIntegration.GetAPIInstance();
        }

        public ActionResult Index()
        {
            SearchVM vm = new SearchVM
            {
                Continents = api.GetContinents()
            };

            return View(vm);
        }

        public JsonResult GetContinentCountries(string continentId)
        {
            List<SelectListItem> countries = api.GetCountries(continentId);

            return Json(countries, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchCities([ModelBinder(typeof(CitiesTableBinder))] SearchVM model)
        {
            List<CityVM> cities = api.GetCountryCities(model.CountryId);

            // ------------ search ------------
            if (!string.IsNullOrEmpty(model.Search))
            {
                cities = cities.Where(x => x.Name.ToLower().Contains(model.Search.ToLower())
                                        || x.Population.ToString().Contains(model.Search.ToLower())
                                        || x.Latitude.ToLower().Contains(model.Search.ToLower())
                                        || x.Longitude.ToLower().Contains(model.Search.ToLower())).ToList();
            }

            // ------------ filter & pagination ------------
            int filtered = cities.Count;
            cities = cities.Skip(model.Start).Take(model.Length).ToList();

            // ------------ sort ------------  
            if (!string.IsNullOrEmpty(model.Column) && !string.IsNullOrEmpty(model.SortDir))
            {
                var column = typeof(CityVM).GetProperty(model.Column);
                cities = model.SortDir.ToLower() == "asc" ?
                         cities.OrderBy(x => column.GetValue(x, null)).ToList() :
                         cities.OrderByDescending(x => column.GetValue(x, null)).ToList();
            }

            return Json(new
            {
                draw = model.Draw,
                data = (IEnumerable<CityVM>)cities,
                recordsTotal = filtered,
                recordsFiltered = filtered
            }, JsonRequestBehavior.AllowGet);
        }
    }
}