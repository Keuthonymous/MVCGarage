using MVCGarage.DataAccess;
using MVCGarage.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVCGarage.Repositories
{
    public class CheckInsRepository : IDisposable
    {
        private GarageContext db = new GarageContext();

        public IEnumerable<CheckIn> CheckIns()
        {
            return db.CheckIns;
        }

        public CheckIn CheckIn(int? id)
        {
            return db.CheckIns.Find(id);
        }

        /// <summary>
        /// Indicates if the parking spot is currently taken or not (booked or parked on)
        /// </summary>
        /// <param name="parkingSpotId">ID of the parking spot</param>
        /// <returns></returns>
        public bool IsAvailable(int parkingSpotId)
        {
            return CheckInByParkingSpot(parkingSpotId) == null;
        }

        /// <summary>
        /// Inidicates if the vehicle currently uses a parking spot (booked or parked on)
        /// </summary>
        /// <param name="vehicleId">ID of the vehicle</param>
        /// <returns></returns>
        public bool IsParked(int vehicleId)
        {
            return CheckInByVehicle(vehicleId) == null;
        }

        /// <summary>
        /// Returns informations on the currently parked/booked parking spot
        /// </summary>
        /// <param name="parkingSpotId">Parking spot currently occupied by a vehicle</param>
        /// <returns></returns>
        public CheckIn CheckInByParkingSpot(int parkingSpotId)
        {
            return CheckIns().SingleOrDefault(ch => ch.ParkingSpotID == parkingSpotId && !ch.Free);
        }

        /// <summary>
        /// Returns informations on the currently parked/booked parking spot
        /// </summary>
        /// <param name="vehicleId">ID of the vehicle currently parked on the parking spot</param>
        /// <returns></returns>
        public CheckIn CheckInByVehicle(int vehicleId)
        {
            return CheckIns().SingleOrDefault(ch => ch.VehicleID == vehicleId && !ch.Free);
        }

        /// <summary>
        /// Returns historical informations about all the times the parking spot has been occupied
        /// </summary>
        /// <param name="parkingSpotId">ID of the booked parking spot</param>
        /// <returns></returns>
        public IEnumerable<CheckIn> CheckInsByParkingSpot(int parkingSpotId)
        {
            return CheckIns().Where(ch => ch.Free && ch.ParkingSpotID == parkingSpotId);
        }

        /// <summary>
        /// Returns historical informations about all the times a parking spot has been occupied by that vehicle
        /// </summary>
        /// <param name="vehicleId">ID of the vehicle which the parking spots have been booked for</param>
        /// <returns></returns>
        public IEnumerable<CheckIn> CheckInsByVehicle(int vehicleId)
        {
            return CheckIns().Where(ch => ch.Free && ch.VehicleID == vehicleId);
        }

        public void Add(CheckIn checkIn)
        {
            db.CheckIns.Add(checkIn);
            SaveChanges();
        }

        public void Edit(CheckIn checkIn)
        {
            db.Entry(checkIn).State = EntityState.Modified;
            SaveChanges();
        }

        public void CheckIn(int vehicleId, int parkingSpotId)
        {
            Add(new CheckIn
            {
                Free = false,
                Booked = false,
                VehicleID = vehicleId,
                ParkingSpotID = parkingSpotId,
                CheckInTime = DateTime.Now
            });
        }

        public void CheckOut(int id)
        {
            CheckIn checkIn = CheckIn(id);
            if (checkIn != null)
            {
                checkIn.Free = true;
                Edit(checkIn);
            }
        }

        public void Book(int vehicleId, int parkingSpotId)
        {
            Add(new CheckIn
            {
                Free = false,
                Booked = true,
                VehicleID = vehicleId,
                ParkingSpotID = parkingSpotId,
                CheckInTime = DateTime.Now
            });
        }

        public void Unbook(int id)
        {
            CheckIn checkIn = CheckIn(id);
            if (checkIn != null)
            {
                checkIn.Free = true;
                Edit(checkIn);
            }
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