# Mars Rover Application

The Mars rover app is a console app that simulates a user who would like to control a rover on a 2D plane. The console will provide the user with a set of prompts that will aid them in controlling the rover.

The user will first be asked for the coordinates Northeast corner of the grid. It should just be two integers separated with a space (ex. 5 5). Next the user will be asked to provide the current coordinates and the heading of the of the rover (5 5 N).

Valid Heading Values:
N = North
E = East
W = West
S = South

The user must finally provide the movement command. The command should leave the rover inside the bounds of the grid. If not, the program will cease movement as to not lose the rover. (ex. LMLMLMRM)

Valid Movement Commands:
L = Left
R = Right
M = Move

