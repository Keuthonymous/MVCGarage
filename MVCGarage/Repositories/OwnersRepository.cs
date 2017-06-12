using MVCGarage.DataAccess;
using MVCGarage.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVCGarage.Repositories
{
    public class OwnersRepository : IDisposable
    {
        private GarageContext db = new GarageContext();

        public IEnumerable<Owner> GetAllOwners ()
        {
            return db.Owners;
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

        public Owner Find(int? id)
        { 
            var query = (from o in db.Owners
                        where o.ID == id
                        select o).FirstOrDefault();

            return query;
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    db.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}