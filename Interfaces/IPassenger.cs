using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.Interfaces
{
    public interface IPassenger
    {
        public int PassengerFloor { get; set; }
        public int DestinationFloor { get; set; }   
    }
}
