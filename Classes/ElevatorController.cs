using ElevatorSimulator.Enum;
using ElevatorSimulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.Classes
{


    public class ElevatorController : IElevatorController
    {
        IFloor iFloor;
        internal Floor[] Floors;
        internal Elevator[] Elevators;
        bool IsOperating;
        Queue<FloorRequest> floorRequests;
       
        public void Start(int numFloors, int numElevators)
        {
            Floors = new Floor[numFloors];
           
            for (int i = 0; i < numFloors; i++)
            {
                Floors[i] = new Floor(i);
            }

            Elevators = new Elevator[numElevators];
            for (int i = 0; i < numElevators; i++)
            {
                Elevators[i] = new Elevator(i, numFloors);
                Elevators[i].OnElevatorEvent += HandleElevatorEvent;
            }

            //Run thread per request 
            floorRequests = new Queue<FloorRequest>();
            Thread thread = new Thread(new ThreadStart(Operate));
            thread.Start();

            //each Elevator must be on its own thread
            for (int i = 0; i < numElevators; i++)
            {
                thread = new Thread(new ThreadStart(Elevators[i].Operate));
                thread.Start();
            }
        }
        public void CallElevator(int floor, Directions dir)
        {
            lock (floorRequests)
            {
                floorRequests.Enqueue(new FloorRequest(floor, dir));
            }
        }
        void Operate()
        {
            IsOperating = true;
            while (IsOperating)
            {
                while (floorRequests.Count > 0)
                {
                    bool IsManaged = false;
                    lock (floorRequests)
                    {
                        FloorRequest rq = floorRequests.Peek();
                        int i = FindBestElevator(rq.elevfloor, rq.elevdir);
                        if (i >= 0)
                        {
                            floorRequests.Dequeue();
                            Elevators[i].RequestFloor(rq.elevfloor, rq.elevdir);
                            IsManaged = true;
                        }
                    }
                    if (!IsManaged)
                    {
                        Thread.Sleep(3000);
                    }
                }
            }
        }
        public void Stop()
        {
            for (int i = 0; i < Elevators.Length; i++)
            {
                Elevators[i].StopOperation();
            }
            IsOperating = false;
        }

        int FindBestElevator(int floor, Directions dir)
        {

            bool goingUp = dir == Directions.Up;

            int closestIdleElev_Id = -1;
            int closestIdleElev_Dist = int.MaxValue;

            int closestMovingElev_Id = -1;
            int closestMovingElev_Dist = int.MaxValue;

            for (int i = 0; i < Elevators.Length; i++)
            {
                Elevators[i].GetState(out MovementStates elevState, out Directions elevDir, out int elevFloor,out LoadStates ElevLoadState);
                int dist = Math.Abs(floor - elevFloor);
                

                if (elevState == MovementStates.Idle && ElevLoadState !=LoadStates.Full)
                {
                    if (dist < closestIdleElev_Dist)
                    {
                        closestIdleElev_Id = i;
                        closestIdleElev_Dist = dist;
                    }
                }
                else if (elevDir == dir && (goingUp ? elevFloor <= floor : elevFloor >= floor) && ElevLoadState != LoadStates.Full)
                {
                    if (dist < closestMovingElev_Dist)
                    {
                        closestMovingElev_Id = i;
                        closestMovingElev_Dist = dist;
                    }
                }
            }

            int bestId;
            if (closestIdleElev_Id == -1 && closestMovingElev_Id == -1)
            {
                bestId = -1;
            }
            else if (closestMovingElev_Id == -1)
            {
                bestId = closestIdleElev_Id;
            }
            else if (closestIdleElev_Id == -1)
            {
                bestId = closestMovingElev_Id;
            }
            else
            {
                bestId = closestMovingElev_Dist < closestIdleElev_Dist ? closestMovingElev_Id : closestIdleElev_Id;
            }

            return bestId;
        }
        void HandleElevatorEvent(Elevator elev, MovementStates state, int floor, Directions dir)
        {
            if (state == MovementStates.VisitingFloor)
            {
                Floors[floor].ElevatorHasArrived(elev, dir);

            }
        }
    }
}
