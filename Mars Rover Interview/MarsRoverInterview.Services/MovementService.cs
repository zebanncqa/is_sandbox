using MarsRoverInterview.Common.DTOs;
using MarsRoverInterview.Common.Enums;
using MarsRoverInterview.Common.Models;
using MarsRoverInterview.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MarsRoverInterview.Services
{
    public class MovementService : IMovementService
    {
        private Grid grid;
        public async Task<Rover> MoveRover(Rover rover)
        {
            if (await VerifyCommandFormat(rover.CurrentCommand.ToUpper()))
                rover = await CalculateCoordinate(rover, rover.CurrentCommand.ToCharArray());
            else
                rover.MovementStatus = RoverMovementStatus.IMPROPER_COMMAND_FORMAT;

            return rover;
        }

        /// <summary>
        /// Sets the x and y coordinates of the NorthEast corner of the grid
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>True indicating the status of setting the grid properties</returns>
        public async Task<bool> SetGrid(int x, int y)
        {
            //This method only returns true all of the time because there is no db.
            //This would actually be in different service that manages information about the grid
            //but we don't have that functionality in the app due to my assumptions

            grid = new Grid()
            {
                XCoordinateMax = x,
                YCoordinateMax = y,
                MaximumAmountOfRovers = (x * y) - 1
            };

            return true;
        }

        public async Task<Grid> GetGrid()
        {
            return grid;
        }

        public async Task<bool> ValidateHeadingInput(string desiredHeading)
        {
            var regex = new Regex(@"[^NSEW]");
            var illegalChars = regex.Matches(desiredHeading);
            return !illegalChars.Any();
        }

        public async Task<bool> VerifyCommandFormat(string moveCommand)
        {
            var regex = new Regex(@"[^LRM]");
            var illegalChars = regex.Matches(moveCommand);                
            return !illegalChars.Any();
        }

        #region Private Methods
        /// <summary>
        /// Calculates the coordinates or sets an if movement isn't possible
        /// </summary>
        /// <param name="RoverToCommand"></param>
        /// <param name="currentCoords"></param>
        /// <returns>Rover with updated properties</returns>
        private async Task<Rover> CalculateCoordinate(Rover RoverToCommand, char[] currentCoords)
        {
            var currentHeading = RoverToCommand.Heading[0];
            var currentX = RoverToCommand.XCoordinate.ToString();
            var currentY = RoverToCommand.YCoordinate.ToString();
            RoverToCommand.LastMovementAttempt = DateTime.Now;

            foreach (var command in currentCoords)
            {
                if (command == 'M')
                {
                    switch (currentHeading)
                    {
                        case 'N':
                            currentY = (int.Parse(currentY) + 1).ToString();
                            break;
                        case 'S':
                            currentY = (int.Parse(currentY) - 1).ToString();
                            break;
                        case 'E':
                            currentX = (int.Parse(currentX) + 1).ToString();
                            break;
                        case 'W':
                            currentX = (int.Parse(currentX) - 1).ToString();
                            break;
                    }
                }
                else
                {
                    switch (currentHeading)
                    {
                        case 'N':
                            currentHeading = (command == 'L' ? 'W' : 'E');
                            break;
                        case 'S':
                            currentHeading = (command == 'L' ? 'E' : 'W');
                            break;
                        case 'E':
                            currentHeading = (command == 'L' ? 'N' : 'S');
                            break;
                        case 'W':
                            currentHeading = (command == 'L' ? 'S' : 'N');
                            break;
                    }
                }

                //We won't move the rover because it would be outside of range
                if (int.Parse(currentX) > grid.XCoordinateMax || int.Parse(currentY) > grid.YCoordinateMax)
                {
                    RoverToCommand.MovementStatus = RoverMovementStatus.ON_EDGE_OF_AREA;
                    break;
                }
            }

            if (RoverToCommand.MovementStatus == RoverMovementStatus.SUCCESFUL)
            {
                RoverToCommand.XCoordinate = int.Parse(currentX);
                RoverToCommand.YCoordinate = int.Parse(currentY);
                RoverToCommand.LastMovedTime = DateTime.Now;
                RoverToCommand.PreviousCommands.Add(string.Join("", currentCoords));
                RoverToCommand.CurrentCommand = string.Empty;
            }

            return RoverToCommand;
        }
        #endregion
    }
}
