using MVCGarage.Models;
using MVCGarage.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCGarage.Controllers
{
    public class CheckInsVehicles
    {
        VehiclesRepository vehicles = new VehiclesRepository();
        CheckInsRepository checkIns = new CheckInsRepository();

        public IEnumerable<Vehicle> ParkedVehicles()
        {
            return vehicles.Vehicles()
                .Select(v => new
                {
                    Vehicle = v,
                    CheckIn = checkIns.CheckIns().Where(ch => !ch.Booked && !ch.Free && ch.VehicleID == v.ID)
                })
                .Select(v_ch => v_ch.Vehicle);
        }

        public IEnumerable<Vehicle> UnparkedVehicles()
        {
            return vehicles.Vehicles()
                .Select(v => new
                {
                    Vehicle = v,
                    CheckIn = checkIns.CheckIns().Where(ch => ch.Free && ch.VehicleID == v.ID)
                })
                .Select(v_ch => v_ch.Vehicle);
        }
    }
}