using MarsRoverInterview.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsRoverInterview.Common.Models
{
    public class Rover
    {
        public int ID { get; set; }
        public int XCoordinate { get; set; }
        public int YCoordinate { get; set; }
        public string Heading { get; set; }
        public string CurrentCommand { get; set; }
        public List<string> PreviousCommands { get; set; } = new();
        public DateTime? LastMovedTime { get; set; }
        public DateTime? LastMovementAttempt { get; set; }
        public RoverMovementStatus MovementStatus { get; set; }

        public Rover() { }

        public Rover(int xCoord, int yCoord, string heading)
        {
            XCoordinate = xCoord;
            YCoordinate = yCoord;
            Heading = heading;
        }

        public string ReportCoordinates()
        {
            return string.Join(' ', XCoordinate, YCoordinate, Heading);
        }
    }
}
