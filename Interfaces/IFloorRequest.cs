using ElevatorSimulator.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.Interfaces
{
    public interface IFloorRequest
    {
        public int elevfloor { get; set; }
        public Directions elevdir { get; set; }


    }
}
