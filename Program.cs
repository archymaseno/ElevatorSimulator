

using ElevatorSimulator.Classes;
using ElevatorSimulator.Enum;

ElevatorController es = new ElevatorController();

Console.WriteLine("Type R to run Elevator Simulator");
bool operate = Console.ReadLine().ToLower() == "r" ? true : false;
Console.WriteLine("The building has 10 floors");
Console.WriteLine("The Building has 3 Elevators with a max capacity of 3 persons");
int numFloors = 10;
int numElevs = 3;
Console.WriteLine();
Console.WriteLine("****** For Interaction and testing we assume that we have 5 Passengers on diiferent floors requesting Elevators********");
Thread.Sleep(3000);
Console.WriteLine();


List<Passenger> Psslist = new List<Passenger>();


for (int i = 0; i < 5; i++)
{
    Console.WriteLine($"Passenger{i}: Which floor are you?");
    int uf = int.Parse(Console.ReadLine());
    if (uf < 0 || uf > 9)
    {
        throw new IndexOutOfRangeException("There are 0 to 9 floors in this buidling");
    }
    int userfloor = uf;


    Console.WriteLine($"Passenger{i}: Which floor are you going to?");
    int df = int.Parse(Console.ReadLine());
    if (df < 0 || df > 9)
    {
        throw new IndexOutOfRangeException("There are 0 to 9 floors in this buidling");
    }
    int destinationfloor = df;

    Passenger pss = new Passenger();
    pss.passengerFloor = userfloor;
    pss.destinationFloor = destinationfloor;
    Psslist.Add(pss);

    Thread.Sleep(2000);
}




while (operate)
{

    es.Start(numFloors, numElevs);

    foreach (var userfl in Psslist)
    {

        Passenger passenger = new Passenger() { destinationFloor = userfl.destinationFloor };
        Directions direction = passenger.destinationFloor > userfl.passengerFloor ? Directions.Up : Directions.Down;

        es.Floors[userfl.passengerFloor].AddPassenger(passenger);

        es.CallElevator(userfl.passengerFloor, direction);
    }



    //Thread thread = new Thread(() => es.CallElevator(startFloor, dir));
    //thread.Start();

    if (Console.ReadKey().Key == ConsoleKey.Escape)
    {
        Console.WriteLine("Do you really want to quit?Y/N");
        if (Console.ReadLine().ToLower() == "y")
        {
            Console.WriteLine("Kindly wait for the Elevator to complete task");
            operate = false;
            es.Stop();
            break;
        }
        else
        {
            break;
            continue;
        }
    }
    if (Console.ReadLine().ToString().ToLower() == "a")
    {
        Console.WriteLine("interrupted by Key a");
    }
}
operate = false;
//es.Stop();