using MVCGarage.Models;
using MVCGarage.Repositories;
using MVCGarage.ViewModels.ParkingSpots;
using MVCGarage.ViewModels.Vehicles;
using System.Net;
using System.Web.Mvc;

namespace MVCGarage.Controllers
{
    public class VehiclesController : Controller
    {
        private VehicleRepository db = new VehicleRepository();

        // GET: Vehicles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle vehicle = db.Vehicle(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            return View(vehicle);
        }

        // GET: Vehicles/Create
        public ActionResult Create(CreateVehicleVM viewModel)
        {
            ViewBag.SelectVehicleTypes = EnumHelper.PopulateDropList();

            if (viewModel.OriginActionName == null)
                viewModel.OriginActionName = "DisplayAllVehicles";

            if (viewModel.OriginControllerName == null)
                viewModel.OriginControllerName = "Garage";

            return View(viewModel);
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,VehicleType,Owner,RegistrationPlate,CheckInTime,ParkingSpot")] Vehicle vehicle,
                                   string originActionName,
                                   string originControllerName)
        {
            if (ModelState.IsValid)
            {
                // Check that the registration plate is still unique
                if (db.VehicleByRegistrationPlate(vehicle.RegistrationPlate) != null)
                {
                    ViewBag.SelectVehicleTypes = EnumHelper.PopulateDropList();

                    return View(new CreateVehicleVM
                    {
                        Vehicle = vehicle,
                        OriginControllerName = originControllerName,
                        OriginActionName = originActionName,
                        ErrorMessage = "A vehicle with the same registration plate already exists!"
                    });
                }

                vehicle.RegistrationPlate = vehicle.RegistrationPlate.ToUpper();

                db.Add(vehicle);
                return RedirectToAction(originActionName, originControllerName, new SelectAVehicleVM
                {
                    VehicleID = vehicle.ID
                });
            }

            ViewBag.SelectVehicleTypes = EnumHelper.PopulateDropList();

            return View(new CreateVehicleVM
            {
                Vehicle = vehicle,
                OriginControllerName = originControllerName,
                OriginActionName = originActionName
            });
        }

        // GET: Vehicles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle vehicle = db.Vehicle(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }

            ViewBag.SelectVehicleTypes = EnumHelper.PopulateDropList();

            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,VehicleType,Owner,RegistrationPlate,CheckInTime,ParkingSpot")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                db.Edit(vehicle);
                return RedirectToAction("DisplayAllVehicles", "Garage");
            }
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle vehicle = db.Vehicle(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            db.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult CheckInVehicle(SelectAVehicleVM viewModel, string errorMessage)
        {
            // Allows the user to select a vehicle in the list of already exiting vehicles
            // or to create a new one
            return View(new SelectAVehicleVM
            {
                ErrorMessage = errorMessage,
                Vehicles = db.UnparkedVehicles(),
                VehicleID = viewModel.VehicleID,
            });
        }

        [HttpPost]
        public ActionResult CheckInVehicle(int? vehicleId)
        {
            if (vehicleId == null)
                return RedirectToAction("CheckInVehicle", new { errorMessage = "You must select a vehicle!" });
            else
                return RedirectToAction("CheckInAVehicle", "Garage", new { vehicleId = vehicleId });
        }

        [HttpGet]
        public ActionResult CheckOutVehicle(string errorMessage)
        {
            // Allows the user to select a vehicle in the list of already exiting vehicles
            // or to create a new one
            return View(new SelectAVehicleVM
            {
                Vehicles = db.ParkedVehicles(),
                ErrorMessage = errorMessage
            });
        }

        [HttpPost]
        public ActionResult CheckOutVehicle(int? vehicleId)
        {
            return RedirectToAction("CheckOutAVehicle", "Garage", new { vehicleId = vehicleId });
        }

        // GET: Vehicles/BookAParkingSpot
        [HttpGet]
        public ActionResult BookAParkingSpot(string errorMessage)
        {
            // Allows the user to select a vehicle in the list of already exiting vehicles
            // or to create a new one
            return View(new SelectAVehicleVM
            {
                ErrorMessage = errorMessage,
                Vehicles = db.UnparkedVehicles()
            });
        }

        [HttpPost]
        public ActionResult BookAParkingSpot(int? vehicleId)
        {
            return RedirectToAction("BookAParkingSpotForAVehicle", new { vehicleId = vehicleId });
        }

        public ActionResult BookAParkingSpotForAVehicle(int? vehicleId, string errorMessage)
        {
            Vehicle vehicle = db.Vehicle(vehicleId);

            if (vehicle == null)
                return RedirectToAction("BookAParkingSpot",
                                        new { errorMessage = "You must select a vehicle!" });

            return RedirectToAction("SelectAParkingSpot",
                                    "ParkingSpots", 
                                    new 
                                    {
                                        vehicleID = vehicle.ID,
                                        checkIn = false,
                                        errorMessage = errorMessage,
                                    });
        }

        [HttpGet]
        public ActionResult UnbookParkingSpot(string errorMessage)
        {
            // Allows the user to select a vehicle in the list of already exiting vehicles
            // or to create a new one
            return View(new SelectAVehicleVM
            {
                Vehicles = new GarageController().BookedSpotsVehicles(),
                ErrorMessage = errorMessage
            });
        }

        [HttpPost]
        public ActionResult UnbookParkingSpot(int? vehicleId)
        {
            return RedirectToAction("UnbookAParkingSpot", "Garage", new { vehicleId = vehicleId });
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
