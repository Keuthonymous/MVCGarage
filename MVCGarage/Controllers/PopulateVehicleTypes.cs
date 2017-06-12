using MVCGarage.DataAccess;
using MVCGarage.Models;
using System.Collections.Generic;
using System.Web.WebPages.Html;

namespace MVCGarage.Controllers
{
    public class PopulateVehicleTypes
    {
        static GarageContext db = new GarageContext();

        public static List<SelectListItem> PopulateDropList()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (VehicleType vehicleType in db.VehicleTypes)
                items.Add(new SelectListItem
                {
                    Value = vehicleType.ID.ToString(),
                    Text = vehicleType.Type
                });

            return items;
        }
    }
}