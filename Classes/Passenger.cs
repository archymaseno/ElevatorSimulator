using ElevatorSimulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.Classes
{
    public class Passenger : IPassenger
    {
        public int PassengerFloor { get; set; }
        public int DestinationFloor { get; set; }
    }
}

