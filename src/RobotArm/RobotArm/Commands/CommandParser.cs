using System;
using System.Collections.Generic;
using System.Linq;

namespace RobotArm.Commands
{

    public enum Command
    {
        Wrist = 4,
        Elbow = 16,
        Shoulder = 64,
        Grip = 1,
        Base = 5
    }

    public class CommandParser
    {
        public IEnumerable<Tuple<Command, int>> Parse(string commands)
        {
            return commands.Split(',')
                           .Select(y =>
                           {
                               var parts = y.Split('|');

                               var command = (Command)Enum.Parse(typeof(Command), parts[0]);
                               var arg = int.Parse(parts[1]);

                               return new Tuple<Command, int>(command, arg);

                           });
        }

        public int GetCombinedCodeForCommands(Tuple<Command, int> c)
        {
            return c.Item2 * ((int)c.Item1);
        }
    }

}
