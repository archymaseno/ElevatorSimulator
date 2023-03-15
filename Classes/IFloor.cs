using ElevatorSimulator.Enum;

namespace ElevatorSimulator.Classes
{
    public interface IFloor
    {
        void AddPassenger(Passenger passenger);
        void ElevatorHasArrived(Elevator elev, Directions elevDir);
    }
}