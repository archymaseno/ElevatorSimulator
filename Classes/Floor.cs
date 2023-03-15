using ElevatorSimulator.Enum;
using ElevatorSimulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.Classes
{
    public class Floor : IFloor
    {
        IPassenger _IPassenger;
        public int currfloor;
        public List<Passenger> Passengers;

        public Floor(int floor)
        {
            currfloor = floor;
            Passengers = new List<Passenger>();
        }
        public void AddPassenger(Passenger passenger)
        {
            lock (Passengers)
            {
                Passengers.Add(passenger);
            }
        }
        public void ElevatorHasArrived(Elevator elev, Directions elevDir)
        {
            lock (Passengers)
            {


                // unload Paasengers destined to this floor
                List<Passenger> unloadedPassengers = elev.UnloadPassengers();

                // onboard passengers
                int LoadCount = 0;
                for (int i = Passengers.Count - 1; i >= 0; i--)
                {
                    Passenger passenger = Passengers[i];
                    Directions passengerDir = passenger.DestinationFloor > currfloor ? Directions.Up : Directions.Down;

                    if (passengerDir == elevDir && passenger.DestinationFloor != currfloor)
                    {

                        if (LoadCount + 1 > (int)LoadStates.Full)
                        {
                            Console.WriteLine($"Elevator: {elev.ElevatorId} is Full! Can only carry max of {(int)LoadStates.Full} persons");
                        }
                        else
                        {
                            elev.LoadPassengers(passenger);
                            LoadCount++;
                            Passengers.RemoveAt(i);
                            Console.WriteLine($"Elevator: {elev.ElevatorId} at floor: {currfloor} Loading {1} Passenger  Going to:{passenger.DestinationFloor}");
                        }
                    }
                }

                // Add unloaded passenggers back to floor.
                for (int i = 0; i < unloadedPassengers.Count; i++)
                    AddPassenger(unloadedPassengers[i]);
            }
        }
    }
}
