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

        public IEnumerable<Owner> GetOwnersByFirstName(string name)
        {
            var query = from o in db.Owners
                        where o.Fname == name
                        select o;

            return query;
        }

        public IEnumerable<Owner> GetOwnersByLastName(string name)
        {
            var query = from o in db.Owners
                        where o.Lname == name
                        select o;

            return query;
        }

        public IEnumerable<Owner> GetOwnersByGender(string gen)
        {
            var query = from o in db.Owners
                        where o.Gender == gen
                        select o;

            return query;
        }

        public Owner GetOwnerByLiNum(string LiNum)
        {
            var query = (from o in db.Owners
                         where o.LicenseNumber.Contains(LiNum)
                         select o).FirstOrDefault();

            return query;
        }
    }
}