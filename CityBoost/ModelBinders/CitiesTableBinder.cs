using CityBoost.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CityBoost.ModelBinder
{
    public class CitiesTableBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var request = controllerContext.RequestContext.HttpContext.Request;

            string country = request.Params.Get("country");
            int draw = int.Parse(request.Params.Get("draw"));
            int start = int.Parse(request.Params.Get("start"));
            int length = int.Parse(request.Params.Get("length"));

            string sortByColumn = request.Params.Get("columns[" + request.Params.Get("order[0][column]") + "][name]");
            string sortColumnDir = request.Params.Get("order[0][dir]");

            string search = request.Params.Get("search[value]");

            return new SearchVM
            {
                CountryId = country,
                Draw = draw,
                Start = start,
                Length = length,
                Search = search,
                Column = sortByColumn,
                SortDir = sortColumnDir
            };
        }
    }
}