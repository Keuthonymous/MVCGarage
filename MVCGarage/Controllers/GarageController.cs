using MVCGarage.Models;
using MVCGarage.Repositories;
using MVCGarage.ViewModels.Garage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVCGarage.Controllers
{
    public class GarageController : Controller
    {
        private VehicleRepository vehicles = new VehicleRepository();
        private ParkingSpotsRepository parkingSpots = new ParkingSpotsRepository();

        private IEnumerable<Vehicle> Sort(IEnumerable<Vehicle> list, string sortOrder)
        {
            ViewBag.VehicleIDSortParam = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.VehicleTypeSortParam = sortOrder == "vehicletype_asc" ? "vehicletype_desc" : "vehicletype_asc";
            ViewBag.OwnerSortParam = sortOrder == "owner_asc" ? "owner_desc" : "owner_asc";
            ViewBag.RegistrationPlateSortParam = sortOrder == "regnum_asc" ? "regnum_desc" : "regnum_asc";
            ViewBag.CheckInTimeSortParam = sortOrder == "checkin_asc" ? "checkin_desc" : "checkin_asc";
            ViewBag.ParkingSpotSortParam = sortOrder == "spot_asc" ? "spot_desc" : "spot_asc";
            ViewBag.FeeSortParam = sortOrder == "fee_asc" ? "fee_desc" : "fee_asc";

            switch (sortOrder)
            {
                case "id_desc":
                    list = list.OrderByDescending(v => v.ID);
                    break;
                case "vehicletype_asc":
                    list = list.OrderBy(v => EnumHelper.GetDescriptionAttr(v.VehicleType));
                    break;
                case "vehicletype_desc":
                    list = list.OrderByDescending(v => EnumHelper.GetDescriptionAttr(v.VehicleType));
                    break;
                case "owner_asc":
                    list = list.OrderBy(v => v.Owner);
                    break;
                case "owner_desc":
                    list = list.OrderByDescending(v => v.Owner);
                    break;
                case "regnum_asc":
                    list = list.OrderBy(v => v.RegistrationPlate);
                    break;
                case "regnum_desc":
                    list = list.OrderByDescending(v => v.RegistrationPlate);
                    break;
                case "checkin_asc":
                    list = list.OrderBy(v => DateTime.Equals(v.CheckInTime, null)).ThenBy(v => v.CheckInTime);
                    break;
                case "checkin_desc":
                    list = list.OrderBy(v => DateTime.Equals(v.CheckInTime, null)).ThenByDescending(v => v.CheckInTime);
                    break;
                case "spot_asc":
                    list = InnerJoin(list)
                           .OrderBy(v_p => v_p.ParkingSpot == null)
                           .ThenBy(v_p => GetLabel(v_p.ParkingSpot))
                           .Select(v_p => v_p.Vehicle);
                    break;
                case "spot_desc":
                    list = InnerJoin(list)
                           .OrderBy(v_p => v_p.ParkingSpot == null)
                           .ThenByDescending(v_p => GetLabel(v_p.ParkingSpot))
                           .Select(v_p => v_p.Vehicle);
                    break;
                case "fee_asc":
                    list = InnerJoin(list)
                           .OrderBy(v_p => v_p.ParkingSpot == null)
                           .ThenBy(v_p => GetFee(v_p))
                           .Select(v_p => v_p.Vehicle);
                    break;
                case "fee_desc":
                    list = InnerJoin(list)
                           .OrderBy(v_p => v_p.ParkingSpot == null)
                           .ThenByDescending(v_p => GetFee(v_p))
                           .Select(v_p => v_p.Vehicle);
                    break;
                default:
                    list = list.OrderBy(v => v.ID);
                    break;
            }

            return list;
        }

        private IEnumerable<InnerJoinResult> InnerJoin(IEnumerable<Vehicle> vehicles)
        {
            return vehicles.Select(v => new InnerJoinResult
            {
                Vehicle = v,
                ParkingSpot = parkingSpots.ParkingSpots()
                                          .FirstOrDefault(p => p.VehicleID == v.ID)
            });
        }

        private string GetLabel(ParkingSpot spot)
        {
            if (spot == null)
                return string.Empty;
            else
                return spot.Label;
        }

        private double GetFee(InnerJoinResult innerJoin)
        {
            if (innerJoin.ParkingSpot == null)
                return 0;
            else if (innerJoin.Vehicle.ParkingSpotID == null)
                return innerJoin.ParkingSpot.MonthlyFee();
            else
                return innerJoin.ParkingSpot.Fee;
        }

        public ActionResult DisplayAllVehicles(string sortOrder)
        {
            Dictionary<int, ParkingSpot> dicParkingSpotsVehicles = new Dictionary<int, ParkingSpot>();
            Dictionary<int, ParkingSpot> dicBookedParkingSpots = new Dictionary<int, ParkingSpot>();

            foreach (Vehicle vehicle in vehicles.Vehicles())
            {
                if (vehicle.ParkingSpotID == null)
                {
                    dicParkingSpotsVehicles.Add(vehicle.ID, null);
                    dicBookedParkingSpots.Add(vehicle.ID, parkingSpots.BookedParkingSpot(vehicle.ID));
                }
                else
                {
                    dicParkingSpotsVehicles.Add(vehicle.ID, parkingSpots.ParkingSpot(vehicle.ParkingSpotID));
                    dicBookedParkingSpots.Add(vehicle.ID, null);
                }
            }

            return View(new DisplayVehiclesVM
            {
                ViewName = "DisplayAllVehicles",
                Vehicles = Sort(vehicles.Vehicles(), sortOrder).ToList(),
                ParkingSpotsVehicles = dicParkingSpotsVehicles,
                BookedParkingSpots = dicBookedParkingSpots
            });
        }

        public ActionResult DisplayParkedVehicles(string sortOrder)
        {
            Dictionary<int, ParkingSpot> dicParkingSpots = new Dictionary<int, ParkingSpot>();
            Dictionary<int, ParkingSpot> dicBookedParkingSpots = new Dictionary<int, ParkingSpot>();

            foreach (Vehicle vehicle in vehicles.ParkedVehicles())
            {
                dicParkingSpots.Add(vehicle.ID, parkingSpots.ParkingSpot(vehicle.ParkingSpotID));
                dicBookedParkingSpots.Add(vehicle.ID, null);
            }

            return View(new DisplayVehiclesVM
            {
                ViewName = "DisplayParkedVehicles",
                Vehicles = Sort(vehicles.ParkedVehicles(), sortOrder).ToList(),
                ParkingSpotsVehicles = dicParkingSpots,
                BookedParkingSpots = dicBookedParkingSpots
            });
        }

        // GET: Garage/BookAParkingSpot
        [HttpGet]
        public ActionResult BookAParkingSpot()
        {
            // Allows the user to select a vehicle in the list of already exiting vehicles
            // or to create a new one
            return View(new SelectAVehicleVM { Vehicles = vehicles.UnparkedVehicles() });
        }

        [HttpPost]
        public ActionResult BookAParkingSpot(int? vehicleId)
        {
            Vehicle vehicle = vehicles.Vehicle(vehicleId);

            if (vehicle == null)
                return RedirectToAction("Index", "Home");

            return RedirectToAction("SelectAParkingSpot",
                new SelectAParkingSpotVM
                {
                    VehicleID = vehicle.ID,
                    SelectedVehicle = vehicles.Vehicle(vehicleId),
                    CheckIn = false,
                    ParkingSpots = parkingSpots.AvailableParkingSpots(vehicle.VehicleType),
                    FollowingActionName = "ParkingSpotBooked",
                    FollowingControllerName = "Garage"
                });
        }

        [HttpGet]
        public ActionResult SelectAParkingSpot(SelectAParkingSpotVM viewModel)
        {
            Vehicle vehicle = vehicles.Vehicle(viewModel.VehicleID);

            // Happends if the user clicks on "Submit" without having selected a vehicle
            // or if the URL is manually entered
            if (vehicle == null)
                return RedirectToAction("Index", "Home");

            // Allows the user to select an available parking spot (if any), depending on the type of vehicle
            return View(new SelectAParkingSpotVM
            {
                VehicleID = viewModel.VehicleID,
                SelectedVehicle = vehicle,
                CheckIn = viewModel.CheckIn,
                ParkingSpots = parkingSpots.AvailableParkingSpots(vehicle.VehicleType),
                FollowingActionName = viewModel.FollowingActionName,
                FollowingControllerName = viewModel.FollowingControllerName
            });
        }

        [HttpPost]
        public ActionResult ParkingSpotBooked(SelectAParkingSpotVM viewModel)
        {
            // Check in the vehicle ID to the parking spot
            if (parkingSpots.CheckIn(viewModel.ParkingSpotID, viewModel.VehicleID))
                // Displays the chosen parking spot
                return View(new BookedParkingSpotVM
                {
                    Vehicle = vehicles.Vehicle(viewModel.VehicleID),
                    ParkingSpot = parkingSpots.ParkingSpot(viewModel.ParkingSpotID)
                });
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult CheckInVehicle(SelectAVehicleVM viewModel)
        {
            // Allows the user to select a vehicle in the list of already exiting vehicles
            // or to create a new one
            return View(new SelectAVehicleVM
            {
                Vehicles = vehicles.UnparkedVehicles(),
                VehicleID = viewModel.VehicleID,
            });
        }

        [HttpPost]
        public ActionResult CheckInVehicle(int vehicleId)
        {
            return RedirectToAction("CheckInAVehicle", "Garage", new { vehicleId = vehicleId });
        }

        public ActionResult CheckInAVehicle(int? vehicleId)
        {
            Vehicle vehicle = vehicles.Vehicle(vehicleId);

            if (vehicle == null)
                return RedirectToAction("Index", "Home");

            return RedirectToAction("SelectAParkingSpot",
                new SelectAParkingSpotVM
                {
                    VehicleID = vehicle.ID,
                    SelectedVehicle = vehicles.Vehicle(vehicleId),
                    CheckIn = true,
                    ParkingSpots = parkingSpots.AvailableParkingSpots(vehicle.VehicleType),
                    FollowingActionName = "VehicleCheckedIn",
                    FollowingControllerName = "Garage"
                });
        }

        [HttpPost]
        public ActionResult VehicleCheckedIn(SelectAParkingSpotVM viewModel)
        {
            // Check in the vehicle ID to the parking spot
            parkingSpots.CheckIn(viewModel.ParkingSpotID, viewModel.VehicleID);
            vehicles.CheckIn(viewModel.VehicleID, viewModel.ParkingSpotID);

            // Displays the chosen parking spot
            return View(new VehicleCheckedInVM
            {
                Vehicle = vehicles.Vehicle(viewModel.VehicleID),
                ParkingSpot = parkingSpots.ParkingSpot(viewModel.ParkingSpotID)
            });
        }

        [HttpGet]
        public ActionResult CheckOutVehicle()
        {
            // Allows the user to select a vehicle in the list of already exiting vehicles
            // or to create a new one
            return View(new SelectAVehicleVM { Vehicles = vehicles.ParkedVehicles() });
        }

        [HttpPost]
        public ActionResult CheckOutVehicle(int? vehicleId)
        {
            return RedirectToAction("CheckOutAVehicle", new { vehicleId = vehicleId });
        }

        public ActionResult CheckOutAVehicle(int? vehicleId)
        {
            if (vehicleId == null)
                return RedirectToAction("Index");

            return RedirectToAction("VehicleCheckedOut", new { vehicleId = vehicleId });
        }

        [HttpGet]
        public ActionResult VehicleCheckedOut(int? vehicleId)
        {
            if (vehicleId == null)
                return RedirectToAction("Index", "Home");

            Vehicle vehicle = vehicles.Vehicle(vehicleId);

            if (vehicle == null)
                return RedirectToAction("Index", "Home");

            ParkingSpot parkingSpot = parkingSpots.ParkingSpot(vehicle.ParkingSpotID);

            if (parkingSpot == null)
                return RedirectToAction("Index", "Home");

            // Check out the vehicle ID to the parking spot
            DateTime now = DateTime.Now;
            DateTime checkinTime = (DateTime)vehicle.CheckInTime;
            int nbMinutes = (int)Math.Round((now - (DateTime)vehicle.CheckInTime).TotalMinutes,
                                            0,
                                            MidpointRounding.AwayFromZero);
            double totalAmount = nbMinutes * parkingSpot.Fee;

            parkingSpots.CheckOut(vehicle.ParkingSpotID);
            vehicles.CheckOut(vehicleId);

            // Displays the bill
            return View(new VehicleCheckedOutVM
            {
                Vehicle = vehicle,
                ParkingSpot = parkingSpot,
                CheckInTime = checkinTime,
                NbMinutes = nbMinutes,
                CheckOutTime = now,
                TotalAmount = totalAmount
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                vehicles.Dispose();
                parkingSpots.Dispose();
            }
            base.Dispose(disposing);
        }

        private class InnerJoinResult
        {
            internal Vehicle Vehicle { get; set; }
            internal ParkingSpot ParkingSpot { get; set; }
        }
    }
}
