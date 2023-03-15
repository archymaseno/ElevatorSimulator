using NUnit;
using NUnit.Compatibility;
using NUnit.Framework;
using ElevatorSimulator.Enum;
using ElevatorSimulator.Classes;
namespace ElevatorSimulator.Tests
{
    public class ElevatorTests
    {
        [Test]
        public void ConstructorTest()
        {
            int elevatorId = 1;
            int numFloors = 10;
            Elevator elevator = new Elevator(elevatorId, numFloors);

            Assert.AreEqual(elevator.ElevatorId, elevatorId);
            Assert.AreEqual(elevator.numOfFloors, numFloors);
            Assert.That(MovementStates.Idle, Is.EqualTo(elevator.elev_state));
            Assert.AreEqual(elevator.elev_direction, Directions.None);
            Assert.AreEqual(elevator.ElevatorFloor, 1);
            Assert.AreEqual(elevator.UpwardFloors.Length, numFloors);
            Assert.AreEqual(elevator.DownwardFloors.Length, numFloors);
            Assert.AreEqual(elevator.Elevatorloadstate, LoadStates.Empty);
            Assert.IsNotNull(elevator.Passengers);
        }

        [Test]
        public void GetStateTest()
        {
            int elevatorId = 1;
            int numFloors = 10;
            Elevator elevator = new Elevator(elevatorId, numFloors);

            MovementStates state;
            Directions dir;
            int elevatorFloor;
            LoadStates elevatorLoadState;

            elevator.GetState(out state, out dir, out elevatorFloor, out elevatorLoadState);

            Assert.AreEqual(state, MovementStates.Idle);
            Assert.AreEqual(dir, Directions.None);
            Assert.AreEqual(elevatorFloor, 1);
            Assert.AreEqual(elevatorLoadState, LoadStates.Empty);
        }
        [Test]
        public void OperateTest()
        {
            int elevatorId = 1;
            int numFloors = 10;
            Elevator elevator = new Elevator(elevatorId, numFloors);

            // Add some passengers
            Passenger pss = new Passenger();
            pss.PassengerFloor = 1;
            pss.DestinationFloor = 5;
            elevator.LoadPassengers(pss);
            pss = new Passenger();
            pss.PassengerFloor = 2;
            pss.DestinationFloor = 5;
            elevator.LoadPassengers(pss);

            // Request some floors
            elevator.RequestFloor(5, Directions.Up);
            elevator.RequestFloor(8, Directions.Down);

            // Start the elevator operation
            Task.Run(() => elevator.Operate());

            // Wait for the elevator to reach the designated floors
            Thread.Sleep(5000);

            // Check the elevator's state after visiting the floors
            MovementStates state;
            Directions dir;
            int elevatorFloor;
            LoadStates elevatorLoadState;

            elevator.GetState(out state, out dir, out elevatorFloor, out elevatorLoadState);

            Assert.AreEqual(state, MovementStates.Moving);
            Assert.AreEqual(dir, Directions.Up);
            Assert.AreEqual(elevatorFloor, 3);
            Assert.AreEqual(elevatorLoadState, LoadStates.Empty);

            // Unload the passengers
            List<Passenger> unloadedPassengers = elevator.UnloadPassengers();
            Assert.AreEqual(unloadedPassengers.Count, 0);

            // Stop the elevator operation
            elevator.StopOperation();
        }
        [Test]
        public void RequestFloorTest()
        {
            Elevator elevator = new Elevator(1, 10);
            elevator.RequestFloor(5, Directions.Up);
            Assert.IsTrue(elevator.UpwardFloors[5]);
        }
        [Test]
        public void TestStopElevator()
        {
            Elevator elevator = new Elevator(1, 10);
            elevator.StopOperation();
            Assert.IsFalse(elevator.IsRunning);
        }
        [Test]
        public void TestLoadPassengers()
        {
            Passenger pss = new Passenger();
            pss.PassengerFloor = 1; pss.DestinationFloor = 9;
            Elevator elevator = new Elevator(1, 10);
            elevator.LoadPassengers(pss);
            Assert.AreEqual(1, elevator.Passengers.Count);
            Assert.AreEqual(1, elevator.Passengers[0].PassengerFloor);
            Assert.AreEqual(9, elevator.Passengers[0].DestinationFloor);
            Assert.IsTrue(elevator.UpwardFloors[9]);
        }


    }
}