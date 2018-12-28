using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TemperatureWebSite.Models
{
    /*
     *
     *  {
            "id": 1,
            "value": 36,
            "unit": 1,
            "time": "2018-12-19T11:35:00"
        }
     * 
     */
    public class TemperatureRecord
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public double Value { get; set; }

        // property names and data types must match
        // for (de-)serialization to work properly
        public int Unit { get; set; }

        public string UnitName => GetUnitName(Unit);

        public static string GetUnitName(int id)
        {
            switch (id)
            {
                case 1:
                    return "Celsius";
                case 2:
                    return "Fahrenheit";
                default:
                    return null;
            }
        }
    }
}
