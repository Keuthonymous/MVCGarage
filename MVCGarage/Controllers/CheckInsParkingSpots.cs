using MVCGarage.Models;
using MVCGarage.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCGarage.Controllers
{
    public class CheckInsParkingSpots
    {
        ParkingSpotsRepository parkingSpots = new ParkingSpotsRepository();
        CheckInsRepository checkIns = new CheckInsRepository();

        public CheckIn CheckInByParkingSpot(int parkingSpotId)
        {
            return checkIns.CheckInByParkingSpot(parkingSpotId);
        }

        public Vehicle ParkedVehicle(int parkingSpotId)
        {
            CheckIn checkIn = CheckInByParkingSpot(parkingSpotId);
            if (checkIn == null)
                return null;
            else
                return checkIn.Vehicle;
        }

        public string Availability(int parkingSpotId)
        {
            CheckIn checkIn = checkIns.CheckInByParkingSpot(parkingSpotId);

            if (checkIn == null)
                return "Yes";
            else
            {
                if (checkIn.Booked)
                    return "Booked by " + checkIn.Vehicle.RegistrationPlate;
                else
                    return "Taken by " + checkIn.Vehicle.RegistrationPlate;
            }
        }

        public string Availability(CheckIn checkIn)
        {
            if (checkIn == null)
                return "Yes";
            else
            {
                if (checkIn.Booked)
                    return "Booked by " + checkIn.Vehicle.RegistrationPlate;
                else
                    return "Taken by " + checkIn.Vehicle.RegistrationPlate;
            }
        }

        public IEnumerable<ParkingSpot> AvailableParkingSpots(int? vehicleTypeId = null)
        {
            return parkingSpots.ParkingSpots(vehicleTypeId).Select(p => new
            {
                ParkingSpot = p,
                CheckIns = checkIns.CheckIns().Where(ch => !ch.Free && ch.ParkingSpotID == p.ID)
            })
            .Where(chp => chp.CheckIns.Count() == 0)
            .Select(chp => chp.ParkingSpot);
        }
    }
}