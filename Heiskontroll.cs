using System;
using System.Threading;

namespace QUESTION3
{
	public class Program 
	{
		private const string QUIT = "q";
		public static void Main (string[] args)
		{
			Start:
				int desFloor;
                string floorInput;
                string input = string.Empty;
                Heis heis;
                Console.WriteLine ("Enter total floors");
				floorInput = Console.ReadLine();
				if (Int32.TryParse (floorInput, out desFloor))
                    if (desFloor < 2) {
                       Console.WriteLine ("That' doesn't make sense...");
                       goto Start; 
                    }
                    else {
                        heis = new Heis (desFloor);
                    }	
				else {
					Console.WriteLine ("That' doesn't make sense...");
					goto Start;
				}
                Console.WriteLine ("User is inside lift at first floor, press lift buttons (separate floors with comma)");
				while (input != QUIT) 
				{
					input = Console.ReadLine ();
                    string[] floors = input.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    int lastFloor=0;
                    foreach (string floor in floors)
                    {
                        if (Int32.TryParse (floor, out desFloor)){
                            heis.FloorPressed (desFloor);
                            heis.isFinalStop = false;
                        }
                        else if (input == QUIT)
						    System.Environment.Exit(1);
					    else
						    Console.WriteLine ("You have pressed an incorrect floor, Please try again");
                        lastFloor = desFloor;
                    }
                    heis.isFinalStop = true;
                    heis.getNewInput(lastFloor);
				}
		}
	}

	public class Heis
	{
		public int currentFloor = 1;
		public HeisStatus Status = HeisStatus.STOPPED;
        public bool isFinalStop = false;
        private int topFloor;
        private bool[] floorReady;

		public Heis (int numberOfFloors)
		{
			floorReady = new bool[numberOfFloors + 1];
			topFloor = numberOfFloors;
		}

		private void Stop(int floor) 
		{
			Status = HeisStatus.STOPPED;
			currentFloor = floor;
			floorReady[floor] = false;
            Console.WriteLine ("Stopped at floor {0}", floor);         
            if (isFinalStop) {
                getNewInput(floor);
            }
            else {
                Status = HeisStatus.UP;
            }
		}
        public void getNewInput(int floor) {
            	const string QUIT = "q";
                string buttonType = string.Empty;
                while (buttonType != QUIT)
				{			
                    Console.WriteLine ("Choose button type, press f for floor button. e for lift button");
                    buttonType = Console.ReadLine();
                    if (buttonType == "f") {
                        Console.WriteLine ("Which floor button user has pressed?");
                        getInput(floor);
                    }
                    else if (buttonType == "e") {
                        Console.WriteLine ("User is inside lift, press lift button");
                        getInput(floor);
                    }
                     else if (buttonType == "q") {
                        System.Environment.Exit(1);
                     }
                    else {
                        Console.WriteLine ("Wrong button type");
                        Status = HeisStatus.STOPPED;
                    }
                }
        }
        
		private void getInput(int floor) 
		{
            const string QUIT = "q";
            string input = string.Empty;
            while (input != QUIT) 
            {
                input = Console.ReadLine ();
                if (Int32.TryParse (input, out floor))
                    FloorPressed (floor);
                else if (input == QUIT)
                    System.Environment.Exit(1);
                else
                    Console.WriteLine ("You have pressed an incorrect floor, Please try again");
            }
        }

		private void Descend(int floor) 
		{
            int timeToDest = estimateTime(currentFloor,floor);
            Console.WriteLine("Moving DOWN, time to destination {0} seconds", timeToDest);
            Thread.Sleep(timeToDest);
			for (int i = currentFloor; i >= 1; i--) 
			{
				if (floorReady[i])
					Stop(floor);
				else
					continue;
			}
			Status = HeisStatus.STOPPED;
		}

		private void Ascend(int floor) 
		{
            int timeToDest = estimateTime(currentFloor,floor);
            Console.WriteLine("Moving UP, time to destination {0} seconds", timeToDest);
            Thread.Sleep(timeToDest * 500);
			for (int i = currentFloor; i <= topFloor; i++) 
			{
					if (floorReady[i])
						Stop(floor);
					else 
						continue;
			}
			Status = HeisStatus.STOPPED;
		}

		void StayPut () 
		{ 
			Console.WriteLine ("That's our current floor"); 
		}

        private int estimateTime(int currentFloor, int destinationFloor)
        {
            int timeForMovingOneFloor = 1; //Unit is second
            int floorDiff = Math.Abs(destinationFloor - currentFloor);
            int estimateMove = floorDiff * timeForMovingOneFloor;
            return estimateMove;
        }
		public void FloorPressed (int floor)
		{
			if (floor > topFloor) {
				Console.WriteLine ("Lift only has {0} floors", topFloor);
				return;
			}
			floorReady[floor] = true;
			switch (Status) {
				case HeisStatus.DOWN :
					Descend(floor);
					break;
				case HeisStatus.STOPPED:
					if (currentFloor < floor)
						Ascend(floor);
					else if (currentFloor == floor)
						StayPut();
					else
						Descend(floor);
					break;
				case HeisStatus.UP :
						Ascend(floor);
						break;
				default:
                    Console.WriteLine ("Wrong status");
					break;
			}
		}

		public enum HeisStatus
		{
			UP,
			STOPPED,
			DOWN
		}
       
	}
}