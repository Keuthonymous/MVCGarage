using MVCGarage.Models;
using MVCGarage.Repositories;
using MVCGarage.ViewModels.ParkingSpots;
using MVCGarage.ViewModels.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MVCGarage.Controllers
{
    public class ParkingSpotsController : Controller
    {
        private ParkingSpotsRepository db = new ParkingSpotsRepository();

        public IEnumerable<ParkingSpot> Sort(IEnumerable<ParkingSpot> list, string sortOrder)
        {
            ViewBag.LabelSortParam = string.IsNullOrEmpty(sortOrder) ? "label_desc" : "label_asc";
            ViewBag.AvailableSortParam = sortOrder == "available_asc" ? "available_desc" : "available_asc";
            ViewBag.VehicleTypeSortParam = sortOrder == "vehicletype_asc" ? "vehicletype_desc" : "vehicletype_asc";
            ViewBag.FeeSortParam = sortOrder == "fee_asc" ? "fee_desc" : "fee_asc";

            ViewBag.RegistrationPlateSortParam = sortOrder == "regnum_asc" ? "regnum_desc" : "regnum_asc";
            ViewBag.OwnerSortParam = sortOrder == "owner_asc" ? "owner_desc" : "owner_asc";
            ViewBag.VehicleVehicleTypeSortParam = sortOrder == "vehicletype_asc" ? "vehicletype_desc" : "vehicletype_asc";
            ViewBag.VehicleCheckInTimeSortParam = sortOrder == "checkin_asc" ? "checkin_desc" : "checkin_asc";
            ViewBag.ParkingSpotSortParam = sortOrder == "spot_asc" ? "spot_desc" : "spot_asc";
            ViewBag.VehicleFeeSortParam = sortOrder == "fee_asc" ? "fee_desc" : "fee_asc";

            switch (sortOrder)
            {
                case "label_desc":
                    list = list.OrderByDescending(p => p.Label);
                    break;
                case "available_asc":
                    list = list.OrderBy(p => Availability(p));
                    break;
                case "available_desc":
                    list = list.OrderByDescending(p => Availability(p));
                    break;
                case "vehicletype_asc":
                    list = list.OrderBy(p => EnumHelper.GetDescriptionAttr(p.VehicleType));
                    break;
                case "vehicletype_desc":
                    list = list.OrderByDescending(p => EnumHelper.GetDescriptionAttr(p.VehicleType));
                    break;
                case "fee_asc":
                    list = list.OrderBy(p => Availability(p).StartsWith("Booked") ? p.MonthlyFee() : p.GetFee());
                    break;
                case "fee_desc":
                    list = list.OrderByDescending(p => Availability(p).StartsWith("Booked") ? p.MonthlyFee() : p.GetFee());
                    break;
                default:
                    list = list.OrderBy(p => p.Label);
                    break;
            }

            return list;
        }

        public string Availability(ParkingSpot parkingSpot)
        {
            if (parkingSpot == null)
                return string.Empty;

            if (parkingSpot.VehicleID == null)
                return "Yes";
            else
            {
                Vehicle vehicle = new GarageController().Vehicle(parkingSpot.VehicleID);

                if (vehicle.ParkingSpotID == parkingSpot.ID)
                    return "Taken by " + vehicle.RegistrationPlate;
                else
                    return "Booked by " + vehicle.RegistrationPlate;
            }
        }

        // GET: ParkingSpots
        public ActionResult Index(string sortOrder, bool filterAvailableOnly = false)
        {
            IEnumerable<ParkingSpot> parkingSpots = null;

            if (filterAvailableOnly)
                parkingSpots = db.AvailableParkingSpots();
            else
                parkingSpots = db.ParkingSpots();

            List<DetailsParkingSpotVM> viewModel = new List<DetailsParkingSpotVM>();

            foreach (ParkingSpot parkingSpot in Sort(parkingSpots, sortOrder).ToList())
                viewModel.Add(new DetailsParkingSpotVM
                {
                    Availability = Availability(parkingSpot),
                    ParkingSpot = parkingSpot,
                    Vehicle = new GarageController().Vehicle(parkingSpot.VehicleID)
                });

            return View(viewModel);
        }

        // GET: ParkingSpots/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ParkingSpot parkingSpot = db.ParkingSpot(id);
            if (parkingSpot == null)
            {
                return HttpNotFound();
            }
            return View(new DetailsParkingSpotVM
            {
                ParkingSpot = parkingSpot,
                Vehicle = new GarageController().Vehicle(parkingSpot.VehicleID)
            });
        }

        // GET: ParkingSpots/Create
        public ActionResult Create(CreateParkingSpotsVM viewModel)
        {
            if (viewModel.VehicleType == ETypeVehicle.undefined)
                ViewBag.SelectVehicleTypes = EnumHelper.PopulateDropList();

            return View(new CreateParkingSpotsVM
            {
                DefaultFees = db.DefaultFees()
            });
        }

        // POST: ParkingSpots/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,VehicleID,Label,Fee,VehicleType")] ParkingSpot parkingSpot)
        {
            if (ModelState.IsValid)
            {
                // Check that the registration plate is still unique
                if (db.ParkingSpotByIdentifiant(parkingSpot.Label) != null)
                {
                    ViewBag.SelectVehicleTypes = EnumHelper.PopulateDropList();

                    return View(new CreateParkingSpotsVM
                    {
                        ParkingSpot = parkingSpot,
                        DefaultFees = db.DefaultFees(),
                        ErrorMessage = "A parking spot with the same identifiant already exists!"
                    });
                }

                if (parkingSpot.Fee == null)
                    parkingSpot.Fee = db.DefaultFee(parkingSpot.VehicleType);

                db.Add(parkingSpot);
                return RedirectToAction("Index");
            }

            ViewBag.SelectVehicleTypes = EnumHelper.PopulateDropList();

            return View(new CreateParkingSpotsVM { ParkingSpot = parkingSpot, DefaultFees = db.DefaultFees() });
        }

        // GET: ParkingSpots/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ParkingSpot parkingSpot = db.ParkingSpot(id);
            if (parkingSpot == null)
            {
                return HttpNotFound();
            }

            ViewBag.SelectVehicleTypes = EnumHelper.PopulateDropList();
            return View(parkingSpot);
        }

        // POST: ParkingSpots/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,VehicleID,Label,Fee,VehicleType")] ParkingSpot parkingSpot)
        {
            if (ModelState.IsValid)
            {
                db.Edit(parkingSpot);
                return RedirectToAction("Index");
            }
            return View(parkingSpot);
        }

        // GET: ParkingSpots/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ParkingSpot parkingSpot = db.ParkingSpot(id);
            if (parkingSpot == null)
            {
                return HttpNotFound();
            }
            return View(parkingSpot);
        }

        // POST: ParkingSpots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            db.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult SelectAParkingSpot(int? vehicleId, bool checkIn, string errorMessage)
        {
            if (vehicleId == null)
                return RedirectToAction("BookAParkingSpot", "Vehicles", new { errorMessage = "You must select a vehicle!" });

            Vehicle vehicle = new GarageController().Vehicle(vehicleId);

            if (vehicle == null)
                return RedirectToAction("BookAParkingSpot", "Vehicles", new { errorMessage = "You must select a vehicle!" });

            // Allows the user to select an available parking spot (if any), depending on the type of vehicle
            return View(new SelectAParkingSpotVM
            {
                VehicleID = (int)vehicleId,
                SelectedVehicle = vehicle,
                CheckIn = checkIn,
                ErrorMessage = errorMessage,
                ParkingSpots = db.AvailableParkingSpots(vehicle.VehicleType)
            });
        }

        [HttpPost]
        public ActionResult SelectAParkingSpot(SelectAParkingSpotVM viewModel)
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

            ParkingSpot parkingSpot = db.ParkingSpot(viewModel.ParkingSpotID);

            if (parkingSpot == null)
                return RedirectToAction("SelectAParkingSpot",
                                        new
                                        {
                                            vehicleId = viewModel.VehicleID,
                                            checkIn = viewModel.CheckIn,
                                            errorMessage = "You must select a parking spot!"
                                        });

            if (viewModel.CheckIn)
                return RedirectToAction("VehicleCheckedIn",
                                        "Garage",
                                        new SelectAParkingSpotVM
                                        {
                                            ParkingSpotID = viewModel.ParkingSpotID,
                                            VehicleID = viewModel.VehicleID
                                        });
            else
                return RedirectToAction("ParkingSpotBooked",
                                        "Garage",
                                        new SelectAParkingSpotVM
                                        {
                                            ParkingSpotID = viewModel.ParkingSpotID,
                                            VehicleID = viewModel.VehicleID
                                        });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
