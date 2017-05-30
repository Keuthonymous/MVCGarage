﻿using MVCGarage.DataAccess;
using MVCGarage.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVCGarage.Repositories
{
    public class VehicleRepository : IDisposable
    {
        private GarageContext db = new GarageContext();

        public IEnumerable<Vehicle> GetAllVehicles()
        {
            return db.Vehicles;
        }

        public Vehicle Vehicle(int? id)
        {
            return db.Vehicles.Find(id);
        }

        public IEnumerable<Vehicle> ParkedVehicles(ETypeVehicle vehicleType)
        {
            return GetAllVehicles().Where(p => p.VehicleType == vehicleType && p.ParkingSpot == null);
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

        public void CheckIn(int vehicleId, int parkingSpotId)
        {
            Vehicle vehicle = Vehicle(vehicleId);
            vehicle.ParkingSpot = parkingSpotId;
            Edit(vehicle);
        }

        public void CheckOut(int vehicleId)
        {
            Vehicle vehicle = Vehicle(vehicleId);
            vehicle.ParkingSpot = null;
            Edit(vehicle);
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