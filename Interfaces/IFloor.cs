using ElevatorSimulator.Classes;
using ElevatorSimulator.Enum;

namespace ElevatorSimulator.Interfaces
{
    public interface IFloor
    {
        void AddPassenger(Passenger passenger);
        void ElevatorHasArrived(Elevator elev, Directions elevDir);
    }
}