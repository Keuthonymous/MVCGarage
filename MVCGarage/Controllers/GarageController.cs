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
        private VehiclesRepository vehicles = new VehiclesRepository();
        private ParkingSpotsRepository parkingSpots = new ParkingSpotsRepository();
        private CheckInsRepository checkIns = new CheckInsRepository();

        public Vehicle Vehicle(int? vehicleId)
        {
            return vehicles.Vehicle(vehicleId);
        }

        public List<Vehicle> BookedSpotsVehicles()
        {
            List<Vehicle> bookedSpotsVehicles = new List<Vehicle>();

            foreach (CheckIn checkIn in checkIns.CheckIns())
            {
                if (checkIn.Booked && !checkIn.Free)
                    bookedSpotsVehicles.Add(vehicles.Vehicle(checkIn.VehicleID));
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
                    list = list.OrderBy(v => v.VehicleType.Type);
                    break;
                case "vehicletype_desc":
                    list = list.OrderByDescending(v => v.VehicleType.Type);
                    break;
                case "owner_asc":
                    list = list.OrderBy(v => v.Owner);
                    break;
                case "owner_desc":
                    list = list.OrderByDescending(v => v.Owner);
                    break;
                case "checkin_asc":
                    list = InnerJoin(list)
                           .OrderBy(v_ch_p => v_ch_p.CheckIn == null || DateTime.Equals(v_ch_p.CheckIn.CheckInTime, null))
                           .ThenBy(v_ch_p => GetCheckInTime(v_ch_p.CheckIn))
                           .Select(v_ch_p => v_ch_p.Vehicle);
                    break;
                case "checkin_desc":
                    list = InnerJoin(list)
                           .OrderBy(v_ch_p => v_ch_p.CheckIn == null || DateTime.Equals(v_ch_p.CheckIn.CheckInTime, null))
                           .ThenByDescending(v_ch_p => GetCheckInTime(v_ch_p.CheckIn))
                           .Select(v_ch_p => v_ch_p.Vehicle);
                    break;
                case "spot_asc":
                    list = InnerJoin(list)
                           .OrderBy(v_ch_p => v_ch_p.CheckIn == null)
                           .ThenBy(v_ch_p => GetLabel(v_ch_p.ParkingSpot))
                           .Select(v_ch_p => v_ch_p.Vehicle);
                    break;
                case "spot_desc":
                    list = InnerJoin(list)
                           .OrderBy(v_ch_p => v_ch_p.CheckIn == null)
                           .ThenByDescending(v_ch_p => GetLabel(v_ch_p.ParkingSpot))
                           .Select(v_ch_p => v_ch_p.Vehicle);
                    break;
                case "fee_asc":
                    list = InnerJoin(list)
                           .OrderBy(v_ch_p => v_ch_p.CheckIn == null)
                           .ThenBy(v_ch_p => GetFee(v_ch_p))
                           .Select(v_ch_p => v_ch_p.Vehicle);
                    break;
                case "fee_desc":
                    list = InnerJoin(list)
                           .OrderBy(v_ch_p => v_ch_p.CheckIn == null)
                           .ThenByDescending(v_ch_p => GetFee(v_ch_p))
                           .Select(v_ch_p => v_ch_p.Vehicle);
                    break;
                default:
                    list = list.OrderBy(v => v.RegistrationPlate);
                    break;
            }

            return list;
        }

        private IEnumerable<InnerJoinResult> InnerJoin(IEnumerable<Vehicle> vehicles)
        {
            return from p in parkingSpots.ParkingSpots()
                   join ch in checkIns.CheckIns() on p.ID equals ch.ParkingSpotID
                   join v in vehicles on ch.VehicleID equals v.ID
                   select new InnerJoinResult { Vehicle = v, ParkingSpot = p, CheckIn = ch };
        }

        private DateTime GetCheckInTime(CheckIn checkIn)
        {
            if (checkIn == null)
                return new DateTime();
            else
                return (DateTime)checkIn.CheckInTime;
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
            if (innerJoin.CheckIn == null)
                return 0;
            else if (innerJoin.CheckIn.Booked)
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
            Dictionary<int, CheckIn> dicParkingSpotsVehicles = new Dictionary<int, CheckIn>();

            foreach (Vehicle vehicle in vehicles.Vehicles())
            {
                dicParkingSpotsVehicles.Add(vehicle.ID, checkIns.CheckInByVehicle(vehicle.ID));
            }

            return View(new DisplayVehiclesVM
            {
                ViewName = "DisplayAllVehicles",
                Vehicles = Sort(vehicles.Vehicles(), sortOrder).ToList(),
                ParkingSpotsVehicles = dicParkingSpotsVehicles
            });
        }

        public ActionResult DisplayParkedVehicles(string sortOrder)
        {
            Dictionary<int, CheckIn> dicParkingSpots = new Dictionary<int, CheckIn>();

            List<Vehicle> parkedVehicles = new List<Vehicle>();

            foreach (Vehicle vehicle in vehicles.Vehicles())
            {
                CheckIn checkIn = checkIns.CheckInByVehicle(vehicle.ID);

                if (checkIn != null)
                {
                    parkedVehicles.Add(vehicle);

                    if (checkIn.Booked)
                        dicParkingSpots.Add(vehicle.ID, null);
                    else
                        dicParkingSpots.Add(vehicle.ID, checkIn);
                }
            }

            return View(new DisplayVehiclesVM
            {
                ViewName = "DisplayParkedVehicles",
                Vehicles = Sort(parkedVehicles, sortOrder).ToList(),
                ParkingSpotsVehicles = dicParkingSpots
            });
        }

        public ActionResult UnbookAParkingSpot(int? vehicleId)
        {
            if (vehicleId == null)
                return RedirectToAction("UnbookParkingSpot",
                                        "Vehicles",
                                        new { errorMessage = "You must select a vehicle!" });

            return RedirectToAction("ParkingSpotUnbooked", "CheckIns", new { vehicleId = vehicleId });
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

        public ActionResult CheckOutAVehicle(int? vehicleId)
        {
            if (vehicleId == null)
                return RedirectToAction("CheckOutVehicle",
                                        "Vehicles",
                                        new { errorMessage = "You must select a vehicle!" });

            return RedirectToAction("VehicleCheckedOut", "CheckIns", new { vehicleId = vehicleId });
        }

        public ActionResult Search(string searchedValue, string sortOrder)
        {
            if (string.IsNullOrEmpty(searchedValue))
                return RedirectToAction("Index");

            List<DetailsParkingSpotVM> foundParkingSpots = new List<DetailsParkingSpotVM>();
            ParkingSpotsController parkingSpotsController = new ParkingSpotsController();

            foreach (ParkingSpot foundParkingSpot in parkingSpotsController.Sort(parkingSpots.ParkingSpotsByIdentifiant(searchedValue), sortOrder))
            {
                CheckIn checkIn = checkIns.CheckInByParkingSpot(foundParkingSpot.ID);
                Vehicle parkedVehicle = null;

                if (checkIn != null)
                    parkedVehicle = checkIn.Vehicle;

                foundParkingSpots.Add(new DetailsParkingSpotVM
                {
                    Availability = new CheckInsParkingSpots().Availability(checkIn),
                    ParkingSpot = foundParkingSpot,
                    Vehicle = parkedVehicle
                });
            }

            Dictionary<int, CheckIn> dicParkingSpotsVehicles = new Dictionary<int, CheckIn>();

            IEnumerable<Vehicle> foundVehicles = Sort(vehicles.VehiclesByRegistrationPlate(searchedValue), sortOrder);

            foreach (Vehicle vehicle in foundVehicles)
                dicParkingSpotsVehicles.Add(vehicle.ID, checkIns.CheckInByVehicle(vehicle.ID));

            return View(new SearchResultsVM
            {
                SearchedValue = searchedValue,
                FoundVehicles = new DisplayVehiclesVM
                {
                    ViewName = "Search",
                    Vehicles = foundVehicles,
                    ParkingSpotsVehicles = dicParkingSpotsVehicles
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
            internal CheckIn CheckIn { get; set; }
        }
    }
}
