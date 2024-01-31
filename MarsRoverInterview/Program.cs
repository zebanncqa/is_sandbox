using MarsRoverInterview.Common.DTOs;
using MarsRoverInterview.Common.Enums;
using MarsRoverInterview.Common.Models;
using MarsRoverInterview.Services;
using MarsRoverInterview.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Net.NetworkInformation;

#region DI and Startup
var services = new ServiceCollection();
ConfigureServices(services);
services.AddSingleton<ConsoleHarness, ConsoleHarness>()
        .BuildServiceProvider()
        .GetService<ConsoleHarness>()
        .Start();

static void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<IMovementService, MovementService>();
}
#endregion 

public class ConsoleHarness
{
    private readonly IMovementService _movementService;
    private Rover rover;

    public ConsoleHarness(IMovementService movementService)
    {
        rover = new();
        _movementService = movementService;
    }

    public async Task Start()
    {
        Console.WriteLine("Welcome to the mars rover app.");

        bool moveRover = true;
        await GatherGridInformation();

        do
        {
            if (await _movementService.GetGrid() is null)
                await GetRoverCoords();

            if (!rover.PreviousCommands.Any())
                await GetRoverCoords();

            await GetRoverCommand();
            await AttemptToMoveRover();

            var goodResponse = false;

            while (!goodResponse)
            {
                Console.WriteLine("Would you like to move the rover again (Y/N)");
                var continueAnswer = Console.ReadLine();

                if (continueAnswer.Equals("n", StringComparison.OrdinalIgnoreCase))
                {
                    goodResponse = true;
                    moveRover = false;
                }
                else if (continueAnswer.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    moveRover = true;
                    goodResponse = true;
                }
                else
                {
                    Console.WriteLine("You've entered something other than y or n");
                    goodResponse = false;
                }
            }            
        }
        while (moveRover);
    }

    private async Task GatherGridInformation()
    {
        string[] gridCoords;
        int xCoord = -1, yCoord = -1;
        do
        {
            Console.WriteLine("Where is the North East corner of the rover area?  (Expecting two space delimited integers)");
            gridCoords = Console.ReadLine().Trim().Split(' ');

            if (gridCoords.Length != 2)
                Console.WriteLine("You have not entered two values delimited by a space");
            else if (!int.TryParse(gridCoords[0], out xCoord))
                Console.WriteLine("The first value you've entered is not a number");
            else if (!int.TryParse(gridCoords[1], out yCoord))
                Console.WriteLine("The second value you've entered is not a number");
            else
            {
                if (!await _movementService.SetGrid(xCoord, yCoord))
                {
                    Console.WriteLine("You have entered invalid coordinates");
                    xCoord = -1;
                }
            }


        }
        while (xCoord == -1 || yCoord == -1);
    }

    private async Task GetRoverCoords()
    {
        string heading = string.Empty;
        string[] roverCoords;
        int roverXCoord = -1, roverYCoord = -1;
        do
        {
            Console.WriteLine("Please confirm this rovers coordinates and heading.  (Expecting a space delimited set of two integers and a alpha character)");
            roverCoords = Console.ReadLine().Split(' ');
            
            if (roverCoords.Length != 3)
                Console.WriteLine("You have not entered three values delimited by a space");
            else if (!int.TryParse(roverCoords[0], out roverXCoord))
                Console.WriteLine("The first value you've entered is not a number");
            else if (!int.TryParse(roverCoords[1], out roverYCoord))
                Console.WriteLine("The second value you've entered is not a number");
            else if (int.TryParse(roverCoords[2], out _))
                Console.WriteLine("The third value you've entered is not a string");
            else
            {
                heading = roverCoords[2].Trim().ToUpper();
                if (!await _movementService.ValidateHeadingInput(heading))
                {
                    Console.WriteLine("You have entered a value that is not a correct heading.  Please only enter N, S, E, W");
                    heading = string.Empty;
                }
                    
            }
        }
        while (roverXCoord == -1 || roverYCoord == -1 || string.IsNullOrEmpty(heading));
        rover = new Rover(roverXCoord, roverYCoord, heading);
    }

    private async Task GetRoverCommand()
    {
        string desiredCommand;
        do
        {
            Console.WriteLine("Please enter the movement command you would like to send to the rover.");
            desiredCommand = Console.ReadLine();
            desiredCommand = desiredCommand.Trim().ToUpper();

            if (desiredCommand.Split(' ').Length > 1)
            {
                Console.WriteLine("The command you entered is in an improper format.  Please review the spec and try again.");
                desiredCommand = string.Empty;
            }
            else
            {
                if (!await _movementService.VerifyCommandFormat(desiredCommand))
                {
                    Console.WriteLine("The command you entered contains values that are not accepted.  It must contain L, M, or R");
                    desiredCommand = string.Empty;
                }
            }

        }
        while (string.IsNullOrEmpty(desiredCommand));

        rover.CurrentCommand = desiredCommand;
    }

    private async Task AttemptToMoveRover()
    {
        Console.WriteLine("The rover is going to attempt to move now.");
        var updatedRover = await _movementService.MoveRover(rover);
        switch (updatedRover.MovementStatus)
        {
            case RoverMovementStatus.SUCCESFUL:
                Console.WriteLine($"The rover has moved succesfully and it's new coordinates are {updatedRover.ReportCoordinates()}");
                break;
            case RoverMovementStatus.COLLISION_POSSIBLE:
                Console.WriteLine("The rover has not moved because it could crash into anothe rover at the desired end destination.");
                break;
            case RoverMovementStatus.ON_EDGE_OF_AREA:
                Console.WriteLine("The rover could not move because it is on the edge of the grid and has stopped to avoid being out of the area.");
                break;
        }
    }
}