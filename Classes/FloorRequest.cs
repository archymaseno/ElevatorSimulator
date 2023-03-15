using ElevatorSimulator.Enum;
using ElevatorSimulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.Classes
{
    class FloorRequest:IFloorRequest
    {
        public int elevfloor { get; set; }
        public Directions elevdir { get; set; }
        public FloorRequest(int floor, Directions dir)
        {
            elevfloor = floor; 
            elevdir = dir;
        }

    }
}
