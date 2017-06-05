using MVCGarage.Models;
using MVCGarage.Repositories;
using MVCGarage.ViewModels.Garage;
using MVCGarage.ViewModels.ParkingSpots;
using MVCGarage.ViewModels.Shared;
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

        public Vehicle Vehicle(int? vehicleId)
        {
            return vehicles.Vehicle(vehicleId);
        }

        public List<Vehicle> BookedSpotsVehicles()
        {
            List<Vehicle> bookedSpotsVehicles = new List<Vehicle>();

            foreach (ParkingSpot parkingSpot in parkingSpots.ParkingSpots())
            {
                if (parkingSpot.VehicleID != null)
                {
                    Vehicle vehicle = vehicles.Vehicle(parkingSpot.VehicleID);
                    if (vehicle.ParkingSpotID == null)
                    {
                        bookedSpotsVehicles.Add(vehicle);
                    }
                }
            }

            return bookedSpotsVehicles;
        }

        private IEnumerable<Vehicle> Sort(IEnumerable<Vehicle> list, string sortOrder)
        {
            ViewBag.RegistrationPlateSortParam = string.IsNullOrEmpty(sortOrder) ? "regnum_desc" : "regnum_asc";
            ViewBag.OwnerSortParam = sortOrder == "owner_asc" ? "owner_desc" : "owner_asc";
            ViewBag.VehicleVehicleTypeSortParam = sortOrder == "vehicletype_asc" ? "vehicletype_desc" : "vehicletype_asc";
            ViewBag.VehicleCheckInTimeSortParam = sortOrder == "checkin_asc" ? "checkin_desc" : "checkin_asc";
            ViewBag.ParkingSpotSortParam = sortOrder == "spot_asc" ? "spot_desc" : "spot_asc";
            ViewBag.VehicleFeeSortParam = sortOrder == "fee_asc" ? "fee_desc" : "fee_asc";

            ViewBag.LabelSortParam = sortOrder == "label_asc" ? "label_desc" : "label_asc";
            ViewBag.AvailableSortParam = sortOrder == "available_asc" ? "available_desc" : "available_asc";
            ViewBag.VehicleTypeSortParam = sortOrder == "vehicletype_asc" ? "vehicletype_desc" : "vehicletype_asc";
            ViewBag.FeeSortParam = sortOrder == "fee_asc" ? "fee_desc" : "fee_asc";

            switch (sortOrder)
            {
                case "regnum_desc":
                    list = list.OrderByDescending(v => v.RegistrationPlate);
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
                case "checkin_asc":
                    list = InnerJoin(list)
                           .OrderBy(v_p => v_p.ParkingSpot == null || DateTime.Equals(v_p.ParkingSpot.CheckInTime, null))
                           .ThenBy(v_p => GetCheckInTime(v_p.ParkingSpot))
                           .Select(v_p => v_p.Vehicle);
                    break;
                case "checkin_desc":
                    list = InnerJoin(list)
                           .OrderBy(v_p => v_p.ParkingSpot == null || DateTime.Equals(v_p.ParkingSpot.CheckInTime, null))
                           .ThenByDescending(v_p => GetCheckInTime(v_p.ParkingSpot))
                           .Select(v_p => v_p.Vehicle);
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
                    list = list.OrderBy(v => v.RegistrationPlate);
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

        private DateTime GetCheckInTime(ParkingSpot spot)
        {
            if (spot == null)
                return new DateTime();
            else
                return (DateTime)spot.CheckInTime;
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
                return innerJoin.ParkingSpot.GetFee();
        }

        public ActionResult Index()
        {
            return View();
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

            List<Vehicle> parkedVehicles = new List<Vehicle>();

            foreach (ParkingSpot parkingSpot in parkingSpots.ParkingSpots())
            {
                if (parkingSpot.VehicleID != null)
                {
                    Vehicle vehicle = vehicles.Vehicle(parkingSpot.VehicleID);
                    parkedVehicles.Add(vehicle);

                    if (vehicle.ParkingSpotID == parkingSpot.ID)
                    {
                        dicParkingSpots.Add((int)parkingSpot.VehicleID, parkingSpot);
                        dicBookedParkingSpots.Add((int)parkingSpot.VehicleID, null);
                    }
                    else
                    {
                        dicParkingSpots.Add((int)parkingSpot.VehicleID, null);
                        dicBookedParkingSpots.Add((int)parkingSpot.VehicleID, parkingSpot);
                    }
                }
            }

            return View(new DisplayVehiclesVM
            {
                ViewName = "DisplayParkedVehicles",
                Vehicles = Sort(parkedVehicles, sortOrder).ToList(),
                ParkingSpotsVehicles = dicParkingSpots,
                BookedParkingSpots = dicBookedParkingSpots
            });
        }

        [HttpGet]
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
                return RedirectToAction("BookAParkingSpotForAVehicle",
                                        "Vehicles",
                                        new
                                        {
                                            vehicleId = viewModel.VehicleID,
                                            errorMessage = "You must select a parking spot!"
                                        });
        }

        public ActionResult UnbookAParkingSpot(int? vehicleId)
        {
            if (vehicleId == null)
                return RedirectToAction("UnbookParkingSpot",
                                        "Vehicles",
                                        new { errorMessage = "You must select a vehicle!" });

            return RedirectToAction("ParkingSpotUnbooked", new { vehicleId = vehicleId });
        }

        [HttpGet]
        public ActionResult ParkingSpotUnbooked(int? vehicleId)
        {
            if (vehicleId == null)
                return RedirectToAction("Index");

            Vehicle vehicle = vehicles.Vehicle(vehicleId);

            if (vehicle == null)
                return RedirectToAction("Index");

            ParkingSpot parkingSpot = parkingSpots.BookedParkingSpot(vehicle.ID);

            if (parkingSpot == null)
                return RedirectToAction("Index");

            // Check out the vehicle ID to the parking spot
            DateTime now = DateTime.Now;
            DateTime checkinTime = (DateTime)parkingSpot.CheckInTime;
            int nbMonths = (int)Math.Truncate((now - (DateTime)parkingSpot.CheckInTime).TotalDays / 30) + 1;
            double totalAmount = nbMonths * parkingSpot.MonthlyFee();

            parkingSpots.CheckOut(parkingSpot.ID);

            // Displays the bill
            return View(new ParkingSpotUnbooked
            {
                Vehicle = vehicle,
                ParkingSpot = parkingSpot,
                CheckInTime = checkinTime,
                NbMonths = nbMonths,
                CheckOutTime = now,
                TotalAmount = totalAmount
            });
        }

        public ActionResult CheckInAVehicle(int? vehicleId, string errorMessage)
        {
            Vehicle vehicle = vehicles.Vehicle(vehicleId);

            if (vehicle == null)
                return RedirectToAction("Index");

            return RedirectToAction("SelectAParkingSpot",
                                    "ParkingSpots",
                                    new
                                    {
                                        vehicleID = vehicle.ID,
                                        checkIn = true,
                                        errorMessage = errorMessage,
                                    });
        }

        [HttpGet]
        public ActionResult VehicleCheckedIn(SelectAParkingSpotVM viewModel)
        {
            Vehicle vehicle = new GarageController().Vehicle(viewModel.VehicleID);

            if (vehicle == null)
                return RedirectToAction("BookAParkingSpot",
                                        "Vehicles",
                                        new
                                        {
                                            checkIn = viewModel.CheckIn,
                                            errorMessage = "You must select a vehicle!"
                                        });

            // Check in the vehicle ID to the parking spot
            ParkingSpot parkingSpot = parkingSpots.ParkingSpot(viewModel.ParkingSpotID);

            if (parkingSpot == null)
                return RedirectToAction("CheckInAVehicle", new
                {
                    vehicleId = viewModel.VehicleID,
                    errorMessage = "You must select a parking spot!"
                });

            parkingSpots.CheckIn(viewModel.ParkingSpotID, viewModel.VehicleID);
            vehicles.CheckIn(viewModel.VehicleID, viewModel.ParkingSpotID);

            // Displays the chosen parking spot
            return View(new VehicleCheckedInVM
            {
                Vehicle = vehicles.Vehicle(viewModel.VehicleID),
                ParkingSpot = parkingSpots.ParkingSpot(viewModel.ParkingSpotID)
            });
        }

        public ActionResult CheckOutAVehicle(int? vehicleId)
        {
            if (vehicleId == null)
                return RedirectToAction("CheckOutVehicle",
                                        "Vehicles",
                                        new { errorMessage = "You must select a vehicle!" });

            return RedirectToAction("VehicleCheckedOut", new { vehicleId = vehicleId });
        }

        [HttpGet]
        public ActionResult VehicleCheckedOut(int? vehicleId)
        {
            if (vehicleId == null)
                return RedirectToAction("Index");

            Vehicle vehicle = vehicles.Vehicle(vehicleId);

            if (vehicle == null)
                return RedirectToAction("Index");

            ParkingSpot parkingSpot = parkingSpots.ParkingSpot(vehicle.ParkingSpotID);

            if (parkingSpot == null)
                return RedirectToAction("Index");

            // Check out the vehicle ID to the parking spot
            DateTime now = DateTime.Now;
            DateTime checkinTime = (DateTime)parkingSpot.CheckInTime;
            int nbMinutes = (int)Math.Truncate((now - (DateTime)parkingSpot.CheckInTime).TotalMinutes) + 1;
            double totalAmount = nbMinutes * parkingSpot.GetFee();

            parkingSpots.CheckOut(parkingSpot.ID);
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

        public ActionResult Search(string searchedValue, string sortOrder)
        {
            if (string.IsNullOrEmpty(searchedValue))
                return RedirectToAction("Index");

            List<DetailsParkingSpotVM> foundParkingSpots = new List<DetailsParkingSpotVM>();
            ParkingSpotsController parkingSpotsController = new ParkingSpotsController();

            foreach (ParkingSpot foundParkingSpot in parkingSpotsController.Sort(parkingSpots.ParkingSpotsByIdentifiant(searchedValue), sortOrder))
            {
                Vehicle parkedVehicle = null;

                if (foundParkingSpot != null)
                    parkedVehicle = vehicles.Vehicle(foundParkingSpot.VehicleID);

                foundParkingSpots.Add(new DetailsParkingSpotVM
                {
                    Availability = parkingSpotsController.Availability(foundParkingSpot),
                    ParkingSpot = foundParkingSpot,
                    Vehicle = parkedVehicle
                });
            }

            Dictionary<int, ParkingSpot> dicParkingSpotsVehicles = new Dictionary<int, ParkingSpot>();
            Dictionary<int, ParkingSpot> dicBookedParkingSpots = new Dictionary<int, ParkingSpot>();

            IEnumerable<Vehicle> foundVehicles = Sort(vehicles.VehiclesByRegistrationPlate(searchedValue), sortOrder);

            foreach (Vehicle vehicle in foundVehicles)
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

            return View(new SearchResultsVM
            {
                SearchedValue = searchedValue,
                FoundVehicles = new DisplayVehiclesVM
                {
                    ViewName = "Search",
                    Vehicles = foundVehicles,
                    ParkingSpotsVehicles = dicParkingSpotsVehicles,
                    BookedParkingSpots = dicBookedParkingSpots
                },
                FoundParkingSpots = foundParkingSpots
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
