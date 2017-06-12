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
        private Dictionary<int, double> defaultFees = new Dictionary<int, double> {
        {1, 0.20 },
        {2, 0.50 },
        {3, 0.80 },
        {4, 1.00 }};

        public Dictionary<int, double> DefaultFees()
        {
            return defaultFees;
        }

        public double DefaultFee(int vehicleTypeId)
        {
            if (defaultFees.ContainsKey(vehicleTypeId))
                return defaultFees[vehicleTypeId];
            else
                return 0;
        }

        public IEnumerable<ParkingSpot> ParkingSpots()
        {
            return db.ParkingSpots;
        }

        public IEnumerable<ParkingSpot> ParkingSpots(int? vehicleTypeId)
        {
            return ParkingSpots().Where(p => vehicleTypeId == null || p.VehicleTypeID == vehicleTypeId);
        }

        public ParkingSpot ParkingSpot(int? id)
        {
            return db.ParkingSpots.Find(id);
        }

        public ParkingSpot ParkingSpotByIdentifiant(string label)
        {
            return ParkingSpots().SingleOrDefault(p => string.Compare(p.Label, label, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        public IEnumerable<ParkingSpot> ParkingSpotsByIdentifiant(string label)
        {
            return ParkingSpots().Where(p => p.Label.ToUpper().Contains(label.ToUpper()));
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