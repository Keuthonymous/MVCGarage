using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCGarage.Models;
using MVCGarage.DataAccess;

namespace MVCGarage.Repositories
{
    public class VehicleTypeRepository //: IDisposable
    {
        private GarageContext db = new GarageContext();

        public VehicleType VehicleID(int? id)
        {
            return db.VehicleID.Find(id);
        }

        public IEnumerable<VehicleType> VehicleTypes()
        {
            return db.VehicleTypes;
        }

    }
}