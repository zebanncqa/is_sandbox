using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsRoverInterview.Common.Enums
{
    public enum RoverMovementStatus
    {
        SUCCESFUL = 0,
        COLLISION_POSSIBLE = 1,
        ON_EDGE_OF_AREA = 2,
        IMPROPER_COMMAND_FORMAT = 3
    }
}
