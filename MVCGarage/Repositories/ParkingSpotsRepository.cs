using MVCGarage.DataAccess;
using MVCGarage.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MVCGarage.Repositories
{
    public class ParkingSpotsRepository : IDisposable
    {
        private GarageContext db = new GarageContext();
        //private Dictionary<ETypeVehicle, double> DefaultFees = new Dictionary<ETypeVehicle, double> { 
        //{ETypeVehicle.car, 0.20 },
        //{ETypeVehicle.car, 0.20 },
        //{ETypeVehicle.car, 0.20 },
        //{ETypeVehicle.car, 0.20 }};


        public IEnumerable<ParkingSpot> ParkingSpots()
        {
            return db.ParkingSpots;
        }

        public ParkingSpot ParkingSpot(int? id)
        {
            return db.ParkingSpots.Find(id);
        }

        public IEnumerable<ParkingSpot> AvailableParkingSpots(ETypeVehicle vehicleType = ETypeVehicle.undefined)
        {
            return ParkingSpots().Where(p => (vehicleType == ETypeVehicle.undefined || p.VehicleType == vehicleType) && p.VehicleID == null);
        }

        public ParkingSpot FirstAvailableParkingSpot(ETypeVehicle vehicleType)
        {
            return AvailableParkingSpots(vehicleType).FirstOrDefault();
        }

        public void Add(ParkingSpot parkingSpot)
        {
            db.ParkingSpots.Add(parkingSpot);
            SaveChanges();
        }

        public void Edit(ParkingSpot parkingSpot)
        {
            db.Entry(parkingSpot).State = EntityState.Modified;
            SaveChanges();
        }

        public bool CheckIn(int parkingSpotId, int vehicleId)
        {
            ParkingSpot parkingSpot = ParkingSpot(parkingSpotId);

            if (parkingSpot == null)
                return false;
            else
            {
                parkingSpot.VehicleID = vehicleId;
                Edit(parkingSpot);
            }

            return true;
        }

        public void CheckOut(int? parkingSpotId)
        {
            if (parkingSpotId != null)
            {
                ParkingSpot parkingSpot = ParkingSpot(parkingSpotId);
                parkingSpot.VehicleID = null;
                Edit(parkingSpot);
            }
        }

        public ParkingSpot BookedParkingSpot(int vehicleId)
        {
            return ParkingSpots().SingleOrDefault(p => p.VehicleID == vehicleId);
        }

        public void Delete(int id)
        {
            db.ParkingSpots.Remove(ParkingSpot(id));
            db.SaveChanges();
        }

        private void SaveChanges()
        {
            db.SaveChanges();
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