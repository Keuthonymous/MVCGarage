using MVCGarage.Models;
using System.Collections.Generic;

namespace MVCGarage.ViewModels.Vehicles
{
    public class SelectAVehicleVM
    {
        public IEnumerable<Vehicle> Vehicles { get; set; }
        public int VehicleID { get; set; }

        public string ErrorMessage { get; set; }

        public string FollowingActionName { get; set; }
        public string FollowingControllerName { get; set; }
    }
}