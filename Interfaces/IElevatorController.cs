using ElevatorSimulator.Classes;
using ElevatorSimulator.Enum;

namespace ElevatorSimulator.Interfaces
{
    public interface IElevatorController
    {
        void CallElevator(int floor, Directions dir);
        void Start(int numFloors, int numElevators);
        void Stop();
    }
}