using ElevatorSimulator.Enum;
using ElevatorSimulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.Classes
{

    public class Elevator : IElevator
    {
        public delegate void ElevatorEventHandler(Elevator elev, MovementStates state, int elevfloor, Directions dir);
        public event ElevatorEventHandler OnElevatorEvent;
        public int ElevatorId;
        public int numOfFloors;
        public MovementStates elev_state;
        public Directions elev_direction;
        public int ElevatorFloor;
        public bool[] UpwardFloors;
        public bool[] DownwardFloors;
        public LoadStates Elevatorloadstate;
        public List<Passenger> Passengers;

        public bool IsRunning;
        public AutoResetEvent threadReleaseFlag;
        public object _lockObj;

        public Elevator(int elevatorId, int numFloors)
        {
            ElevatorId = elevatorId;
            numOfFloors = numFloors;
            elev_state = MovementStates.Idle;
            elev_direction = Directions.None;
            ElevatorFloor = 1;
            UpwardFloors = new bool[numFloors];
            DownwardFloors = new bool[numFloors];
            Elevatorloadstate = LoadStates.Empty;
            Passengers = new List<Passenger>();

            threadReleaseFlag = new AutoResetEvent(false);
            _lockObj = new object();

            for (int i = 0; i < numOfFloors; i++)
            {
                UpwardFloors[i] = false;
                DownwardFloors[i] = false;
            }
        }

        public void GetState(out MovementStates state, out Directions dir, out int elevatorFloor, out LoadStates elevatorLoadState)
        {
            lock (_lockObj)
            {
                state = elev_state;
                dir = elev_direction;
                elevatorFloor = ElevatorFloor;
                elevatorLoadState = Elevatorloadstate;
            }
        }
        public void Operate()
        {
            IsRunning = true;

            while (threadReleaseFlag.WaitOne() && IsRunning)
            {
                lock (_lockObj)
                {
                    elev_state = MovementStates.Moving;
                    elev_direction = Directions.None;
                }

                if (FindClosestMarkedFloor(out int closestMarkedFloor))     //Checking the floors to be visited
                {
                    lock (_lockObj)
                    {
                        if (elev_direction == Directions.None)
                            elev_direction = closestMarkedFloor >= ElevatorFloor ? Directions.Up : Directions.Down;
                    }

                    do
                    {
                        ProcessFloorRequests();
                    }
                    while (MoveElevator());
                }

                lock (_lockObj)
                {
                    elev_state = MovementStates.Idle;
                    elev_direction = Directions.None;
                    Console.WriteLine($"Elevator: {ElevatorId} at floor: {ElevatorFloor} {elev_state} {elev_direction} Passengers: {Passengers.Count}");
                }
            }
        }

        public void StopOperation()
        {
            IsRunning = false;
            threadReleaseFlag.Set();
        }

        public void RequestFloor(int floor, Directions dir)
        {
            lock (_lockObj)
            {
                if (dir == Directions.Up)
                {
                    UpwardFloors[floor] = true;
                }
                else if (dir == Directions.Down)
                {
                    DownwardFloors[floor] = true;
                }

                Console.WriteLine($"Elevator: {ElevatorId} at floor: {ElevatorFloor} Called to floor: {floor}  {dir} Passengers: {Passengers.Count}");
            }
            threadReleaseFlag.Set();
        }

        public void LoadPassengers(Passenger passenger)
        {
            // Mark the floor that the passenger wants.
            lock (_lockObj)
            {
                Passengers.Add(passenger);
                if (passenger.destinationFloor >= ElevatorFloor)
                {
                    UpwardFloors[passenger.destinationFloor] = true;
                }
                else
                {
                    DownwardFloors[passenger.destinationFloor] = true;
                }

            }
            threadReleaseFlag.Set();
        }
        public List<Passenger> UnloadPassengers()
        {
            List<Passenger> unloadedPassengers = new List<Passenger>();

            lock (_lockObj)
            {
                for (int i = Passengers.Count - 1; i >= 0; i--)
                {
                    if (Passengers[i].destinationFloor == ElevatorFloor)
                    {
                        unloadedPassengers.Add(Passengers[i]);
                        Passengers.RemoveAt(i);
                    }
                }
                Console.WriteLine($"Elevator: {ElevatorId} at floor: {ElevatorFloor} Unload {unloadedPassengers.Count} Passenger Passenger: {Passengers.Count}");
            }
            return unloadedPassengers;
        }

        public void ProcessFloorRequests()
        {
            // Console.WriteLine($"Elevator: {ElevatorId} at floor: {ElevatorFloor} Moving: {elev_direction} Passenger: {Passengers.Count}");

            bool floorMarked = false;
            lock (_lockObj)
            {
                floorMarked = elev_direction == Directions.Up ? UpwardFloors[ElevatorFloor] : DownwardFloors[ElevatorFloor];

                if (elev_direction == Directions.Up)
                    UpwardFloors[ElevatorFloor] = false;
                else
                    DownwardFloors[ElevatorFloor] = false;
            }
            if (floorMarked)
            {
                lock (_lockObj)
                    elev_state = MovementStates.VisitingFloor;

                OpenDoor();
                OnElevatorEvent(this, elev_state, ElevatorFloor, elev_direction);
                CloseDoor();

                lock (_lockObj)
                    elev_state = MovementStates.Moving;
            }
        }
        public void OpenDoor()
        {
            Console.WriteLine($"Elevator: {ElevatorId} at floor: {ElevatorFloor} Opening Door Passenger: {Passengers.Count}");
            Thread.Sleep(3000);
        }
        public void CloseDoor()
        {
            Console.WriteLine($"Elevator: {ElevatorId} at Floor: {ElevatorFloor} Closing Door Passengers:{Passengers.Count}");
            Thread.Sleep(2000);
        }
        public bool MoveElevator()
        {
            if (IsAtLastFloor())    //If at Last floor either up or down change the direction
            {
                lock (_lockObj)
                {
                    elev_direction = elev_direction == Directions.Up ? Directions.Down : Directions.Up;
                    Thread.Sleep(1000);
                }
            }
            else
            {
                lock (_lockObj)
                    ElevatorFloor += elev_direction == Directions.Up ? 1 : -1;

                Thread.Sleep(3000);
            }
            Console.WriteLine($"Elevator: {ElevatorId} at floor: {ElevatorFloor} Moving: {elev_direction} Passengers: {Passengers.Count}");
            return FloorsRemaining();  //Keep balance of unprocessed floor on that direction
        }

        bool IsAtLastFloor()
        {
            if (ElevatorFloor == 0 || ElevatorFloor == numOfFloors - 1)
                return true;

            if (IsLastFloorsMarked(out int topMarked, out int bottomMarked))
                return ElevatorFloor >= topMarked && elev_direction == Directions.Up || ElevatorFloor <= bottomMarked && elev_direction == Directions.Down;
            else
                return true;
        }
        bool FindClosestMarkedFloor(out int closestFloor)
        {
            closestFloor = -1;
            int closestFloorDist = int.MaxValue;

            lock (_lockObj)
            {
                for (int i = 0; i < numOfFloors; i++)
                {
                    if ((UpwardFloors[i] || DownwardFloors[i]) && Math.Abs(i - ElevatorFloor) < closestFloorDist)
                    {
                        closestFloor = i;
                        closestFloorDist = Math.Abs(i - ElevatorFloor);
                    }
                }
            }

            return closestFloor != -1;
        }
        bool FloorsRemaining()
        {
            lock (_lockObj)
            {
                int increment = elev_direction == Directions.Up ? 1 : -1;
                for (int i = ElevatorFloor; i >= 0 && i < numOfFloors; i += increment)
                    if (UpwardFloors[i] || DownwardFloors[i])
                    {
                        return true;
                    }
            }
            return false;
        }
        bool IsLastFloorsMarked(out int topMarked, out int bottomMarked)
        {
            bool hasMarked = false;
            topMarked = int.MinValue;
            bottomMarked = int.MaxValue;

            lock (_lockObj)
            {
                for (int i = 0; i < numOfFloors; i++)
                {
                    if (UpwardFloors[i] || DownwardFloors[i])
                    {
                        hasMarked = true;
                        if (i < bottomMarked)
                        {
                            bottomMarked = i;
                        }
                        if (i > topMarked)
                        {
                            topMarked = i;
                        }
                    }
                }
            }

            return hasMarked;
        }
    }
}
