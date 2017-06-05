using MVCGarage.Models;

namespace MVCGarage.ViewModels.Vehicles
{
    public class CreateVehicleVM
    {
        public Vehicle Vehicle { get; set; }
        public string OriginActionName { get; set; }
        public string OriginControllerName { get; set; }
        public string ErrorMessage { get; set; }
    }
}