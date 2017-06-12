using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCGarage.DataAccess;
using MVCGarage.Models;
using MVCGarage.Repositories;

namespace MVCGarage.Controllers
{
    public class OwnersController : Controller
    {
        private OwnersRepository db = new OwnersRepository();

        public IEnumerable<Owner> Sort(IEnumerable<Owner> list, string sortOrder)
        {
            ViewBag.LabelSortParam = string.IsNullOrEmpty(sortOrder) ? "ID_desc" : "ID_asc";
            ViewBag.FNameSortParam = sortOrder == "Fname_asc" ? "Fname_desc" : "Fname_asc";
            ViewBag.LnameSortParam = sortOrder == "Lname_asc" ? "Lname_desc" : "Lname_asc";
            ViewBag.GenderSortParam = sortOrder == "Gender_asc" ? "Gender_desc" : "Gender_asc";
            ViewBag.LiNumSortParam = sortOrder == "LiNum_asc" ? "LiNum_desc" : "LiNum_asc";

            switch (sortOrder)
            { 
                case "ID_desc":
                    list = list.OrderByDescending(o => o.ID);
                    break;

                case "Fname_asc":
                    list = list.OrderBy(o => o.Fname);
                    break;

                case "Fname_desc":
                    list = list.OrderByDescending(o => o.Fname);
                    break;

                case "Lname_asc":
                    list = list.OrderBy(o => o.Lname);
                    break;

                case "Lname_desc":
                    list = list.OrderByDescending(o => o.Lname);
                    break;

                case "Gender_asc":
                    list = list.OrderBy(o => o.Gender);
                    break;

                case "Gender_desc":
                    list = list.OrderByDescending(o => o.Gender);
                    break;

                case "LiNum_asc":
                    list = list.OrderBy(o => o.LicenseNumber);
                    break;

                case "LiNum_desc":
                    list = list.OrderByDescending(o => o.LicenseNumber);
                    break;

                default:
                    list = list.OrderBy(o => o.ID);
                    break;
            }
            return list;
        }

        // GET: Owners
        public ActionResult Index()
        {
            IEnumerable<Owner> Owners = db.GetAllOwners();
            return View(Owners);
        }

        // GET: Owners/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Owner owner = db.Find(id);
            if (owner == null)
            {
                return HttpNotFound();
            }
            return View(owner);
        }

        // GET: Owners/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Owners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Fname,Lname,Gender,LicenseNumber")] Owner owner)
        {
            if (ModelState.IsValid)
            {
                db.AddNewOwner(owner);
                return RedirectToAction("Index");
            }

            return View(owner);
        }

        // GET: Owners/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Owner owner = db.Find(id);
            if (owner == null)
            {
                return HttpNotFound();
            }
            return View(owner);
        }

        // POST: Owners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Fname,Lname,Gender,LicenseNumber")] Owner owner)
        {
            if (ModelState.IsValid)
            {
                db.EditOwner(owner);
                return RedirectToAction("Index");
            }
            return View(owner);
        }

        // GET: Owners/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Owner owner = db.Find(id);
            if (owner == null)
            {
                return HttpNotFound();
            }
            return View(owner);
        }

        // POST: Owners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Owner owner = db.Find(id);
            db.RemoveOwner(owner);
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
