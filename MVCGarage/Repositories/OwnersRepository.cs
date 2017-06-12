using MVCGarage.DataAccess;
using MVCGarage.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVCGarage.Repositories
{
    public class OwnersRepository
    {
        private GarageContext db = new GarageContext();

        public IEnumerable<Owner> GetAllOwners ()
        {
            return db.Owners.ToList();
        }

        public void AddNewOwner(Owner owner)
        {
            db.Owners.Add(owner);
            db.SaveChanges();
        }

        public void RemoveOwner(Owner owner)
        {
            db.Owners.Remove(owner);
            db.SaveChanges();
        }

        public void EditOwner(Owner owner)
        {
            db.Entry(owner).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}