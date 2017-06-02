using MVCGarage.Models;
using MVCGarage.ViewModels.Shared;

namespace MVCGarage.ViewModels.Vehicles
{
    public class CreateVehicleVM
    {
        public Vehicle Vehicle { get; set; }
        public string OriginActionName { get; set; }
        public string OriginControllerName { get; set; }

        public EActionType ActionType { get; set; }
    }
}