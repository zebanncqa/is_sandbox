using MarsRoverInterview.Common.DTOs;
using MarsRoverInterview.Common.Models;

namespace MarsRoverInterview.Services.Interfaces
{
    public interface IMovementService
    {
        Task<Rover> MoveRover(Rover rover);
        Task<bool> SetGrid(int x, int y);
        Task<Grid> GetGrid();
        Task<bool> ValidateHeadingInput(string desiredHeading);
        Task<bool> VerifyCommandFormat(string moveCommand);
    }
}
