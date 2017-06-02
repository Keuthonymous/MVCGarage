using MVCGarage.Models;
using MVCGarage.ViewModels.Shared;
using System.Collections.Generic;

namespace MVCGarage.ViewModels.Garage
{
    public class SelectAVehicleVM
    {
        public IEnumerable<Vehicle> Vehicles { get; set; }
        public int VehicleID { get; set; }

        public string FollowingActionName { get; set; }
        public string FollowingControllerName { get; set; }

        public EActionType ActionType { get; set; }
    }
}