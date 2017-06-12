using MVCGarage.DataAccess;
using MVCGarage.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MVCGarage.Repositories
{
    public class VehiclesRepository : IDisposable
    {
        private GarageContext db = new GarageContext();

        public IEnumerable<Vehicle> Vehicles()
        {
            return db.Vehicles;
        }

        public Vehicle Vehicle(int? id)
        {
            return db.Vehicles.Find(id);
        }

        public Vehicle VehicleByRegistrationPlate(string registrationPlate)
        {
            return Vehicles().SingleOrDefault(v => string.Compare(v.RegistrationPlate, registrationPlate, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        public IEnumerable<Vehicle> VehiclesByRegistrationPlate(string registrationPlate)
        {
            return Vehicles().Where(v => v.RegistrationPlate.ToUpper().Contains(registrationPlate.ToUpper()));
        }

        public void Add(Vehicle vehicle)
        {
            db.Vehicles.Add(vehicle);
            SaveChanges();
        }

        public void Edit(Vehicle vehicle)
        {
            db.Entry(vehicle).State = EntityState.Modified;
            SaveChanges();
        }

        public void Delete(int vehicleId)
        {
            db.Vehicles.Remove(Vehicle(vehicleId));
            SaveChanges();
        }

        private void SaveChanges()
        {
            db.SaveChanges();
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        // This code added to correctly implement the disposable pattern.
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