using ElevatorSimulator.Classes;
using ElevatorSimulator.Enum;

namespace ElevatorSimulator.Interfaces
{
    public interface IElevator
    {
        event Elevator.ElevatorEventHandler OnElevatorEvent;

        void CloseDoor();
        void GetState(out MovementStates state, out Directions dir, out int elevatorFloor, out LoadStates elevatorLoadState);
        void LoadPassengers(Passenger passenger);
        bool MoveElevator();
        void OpenDoor();
        void Operate();
        void ProcessFloorRequests();
        void RequestFloor(int floor, Directions dir);
        void StopOperation();
        List<Passenger> UnloadPassengers();
    }
}