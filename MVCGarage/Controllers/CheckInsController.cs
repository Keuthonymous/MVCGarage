using MVCGarage.Models;
using MVCGarage.Repositories;
using MVCGarage.ViewModels.CheckIns;
using MVCGarage.ViewModels.ParkingSpots;
using System;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;

namespace MVCGarage.Controllers
{
    public class CheckInsController : Controller
    {
        private CheckInsRepository db = new CheckInsRepository();
        private VehiclesRepository vehicles = new VehiclesRepository();
        private ParkingSpotsRepository parkingSpots = new ParkingSpotsRepository();

        //// GET: CheckIns
        //public ActionResult Index()
        //{
        //    var checkIns = db.CheckIns.Include(c => c.ParkingSpot).Include(c => c.Vehicle);
        //    return View(checkIns.ToList());
        //}

        //// GET: CheckIns/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CheckIn checkIn = db.CheckIns.Find(id);
        //    if (checkIn == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(checkIn);
        //}

        //// GET: CheckIns/Create
        //public ActionResult Create()
        //{
        //    ViewBag.ParkingSpotID = new SelectList(db.ParkingSpots, "ID", "Label");
        //    ViewBag.VehicleID = new SelectList(db.Vehicles, "ID", "Owner");
        //    return View();
        //}

        //// POST: CheckIns/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ID,CheckInTime,Booked,Free,ParkingSpotID,VehicleID")] CheckIn checkIn)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.CheckIns.Add(checkIn);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.ParkingSpotID = new SelectList(db.ParkingSpots, "ID", "Label", checkIn.ParkingSpotID);
        //    ViewBag.VehicleID = new SelectList(db.Vehicles, "ID", "Owner", checkIn.VehicleID);
        //    return View(checkIn);
        //}

        //// GET: CheckIns/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CheckIn checkIn = db.CheckIns.Find(id);
        //    if (checkIn == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.ParkingSpotID = new SelectList(db.ParkingSpots, "ID", "Label", checkIn.ParkingSpotID);
        //    ViewBag.VehicleID = new SelectList(db.Vehicles, "ID", "Owner", checkIn.VehicleID);
        //    return View(checkIn);
        //}

        //// POST: CheckIns/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ID,CheckInTime,Booked,Free,ParkingSpotID,VehicleID")] CheckIn checkIn)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(checkIn).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.ParkingSpotID = new SelectList(db.ParkingSpots, "ID", "Label", checkIn.ParkingSpotID);
        //    ViewBag.VehicleID = new SelectList(db.Vehicles, "ID", "Owner", checkIn.VehicleID);
        //    return View(checkIn);
        //}

        //// GET: CheckIns/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CheckIn checkIn = db.CheckIns.Find(id);
        //    if (checkIn == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(checkIn);
        //}

        //// POST: CheckIns/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    CheckIn checkIn = db.CheckIns.Find(id);
        //    db.CheckIns.Remove(checkIn);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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

            db.CheckIn(viewModel.VehicleID, viewModel.ParkingSpotID);

            // Displays the chosen parking spot
            return View(new VehicleCheckedInVM
            {
                Vehicle = vehicles.Vehicle(viewModel.VehicleID),
                ParkingSpot = parkingSpots.ParkingSpot(viewModel.ParkingSpotID)
            });
        }

        [HttpGet]
        public ActionResult VehicleCheckedOut(int? vehicleId)
        {
            if (vehicleId == null)
                return RedirectToAction("Index");

            CheckIn checkIn = new CheckInsVehicles();

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
            db.CheckOut((int)vehicleId);

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
