using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsRoverInterview.Common.DTOs
{
    public class MovementRequestDTO
    {
        public string RoverID { get; set; }
        public string CurrentCoordinates { get; set; }
        public string MovementCommand { get; set; }
        public string Heading { get; set; }
    }
}
