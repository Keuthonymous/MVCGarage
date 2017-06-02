using MVCGarage.Models;
using MVCGarage.Repositories;
//using MVCGarage.ViewModels.Garage;
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
                viewModel.OriginActionName = "Index";

            return View(viewModel);
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,VehicleType,Owner,RegistrationPlate,CheckInTime,ParkingSpot")] Vehicle vehicle,
                                   string originActionName,
                                   string originControllerName,
                                   bool displayCheckInTime)
        {
            if (ModelState.IsValid)
            {
                db.Add(vehicle);
                return RedirectToAction(originActionName, originControllerName, new SelectAVehicleVM
                {
                    VehicleID = vehicle.ID,
                    DisplayCheckInTime = displayCheckInTime
                });
            }

            ViewBag.SelectVehicleTypes = EnumHelper.PopulateDropList();

            return View(new CreateVehicleVM
            {
                Vehicle = vehicle,
                OriginControllerName = originControllerName,
                OriginActionName = originActionName,
                DisplayCheckInTime = displayCheckInTime
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
                return RedirectToAction("Index", "Home");
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
        public ActionResult CheckInVehicle(SelectAVehicleVM viewModel)
        {
            // Allows the user to select a vehicle in the list of already exiting vehicles
            // or to create a new one
            return View(new SelectAVehicleVM
            {
                DisplayCheckInTime = false,
                Vehicles = db.UnparkedVehicles(),
                VehicleID = viewModel.VehicleID,
            });
        }

        [HttpPost]
        public ActionResult CheckInVehicle(int vehicleId)
        {
            return RedirectToAction("CheckInAVehicle", "Garage", new { vehicleId = vehicleId });
        }

        [HttpGet]
        public ActionResult CheckOutVehicle()
        {
            // Allows the user to select a vehicle in the list of already exiting vehicles
            // or to create a new one
            return View(new SelectAVehicleVM
            {
                DisplayCheckInTime = true,
                Vehicles = db.ParkedVehicles()
            });
        }

        [HttpPost]
        public ActionResult CheckOutVehicle(int? vehicleId)
        {
            return RedirectToAction("CheckOutAVehicle", "Garage", new { vehicleId = vehicleId });
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
