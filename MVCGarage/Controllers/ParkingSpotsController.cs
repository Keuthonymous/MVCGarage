using MVCGarage.Models;
using MVCGarage.Repositories;
using MVCGarage.ViewModels.ParkingSpots;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MVCGarage.Controllers
{
    public class ParkingSpotsController : Controller
    {
        private ParkingSpotsRepository db = new ParkingSpotsRepository();


        private IEnumerable<ParkingSpot> Sort(IEnumerable<ParkingSpot> list, string sortOrder)
        {
            ViewBag.LabelSortParam = string.IsNullOrEmpty(sortOrder) ? "label_desc" : "label_asc";
            ViewBag.AvailableSortParam = sortOrder == "available_asc" ? "available_desc" : "available_asc";
            ViewBag.VehicleTypeSortParam = sortOrder == "vehicletype_asc" ? "vehicletype_desc" : "vehicletype_asc";
            ViewBag.FeeSortParam = sortOrder == "fee_asc" ? "fee_desc" : "fee_asc";

            switch (sortOrder)
            {
                case "label_desc":
                    list = list.OrderByDescending(p => p.Label);
                    break;
                case "available_asc":
                    list = list.OrderBy(p => p.Available());
                    break;
                case "available_desc":
                    list = list.OrderByDescending(p => p.Available());
                    break;
                case "vehicletype_asc":
                    list = list.OrderBy(p => EnumHelper.GetDescriptionAttr(p.VehicleType));
                    break;
                case "vehicletype_desc":
                    list = list.OrderByDescending(p => EnumHelper.GetDescriptionAttr(p.VehicleType));
                    break;
                case "fee_asc":
                    list = list.OrderBy(p => p.Fee);
                    break;
                case "fee_desc":
                    list = list.OrderByDescending(p => p.Fee);
                    break;
                default:
                    list = list.OrderBy(p => p.Label);
                    break;
            }

            return list;
        }

        // GET: ParkingSpots
        public ActionResult Index(string sortOrder, bool filterAvailableOnly = false)
        {
            IEnumerable<ParkingSpot> parkingSpots = null;

            if (filterAvailableOnly)
                parkingSpots = db.AvailableParkingSpots();
            else
                parkingSpots = db.ParkingSpots();

            return View(Sort(parkingSpots, sortOrder));
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
            return View(parkingSpot);
        }

        // GET: ParkingSpots/Create
        public ActionResult Create(CreateParkingSpotsVM viewModel)
        {
            if (viewModel.VehicleType == ETypeVehicle.undefined)
                ViewBag.SelectVehicleTypes = EnumHelper.PopulateDropList();

            if (viewModel.OriginActionName == null)
                viewModel.OriginActionName = "Index";

            return View(viewModel);
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
                db.Add(parkingSpot);
                return RedirectToAction("Index");
            }

            ViewBag.SelectVehicleTypes = EnumHelper.PopulateDropList();

            return View(parkingSpot);
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
