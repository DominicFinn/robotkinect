using System.Collections.Generic;
using System.Threading;
using RobotArm.Commands;

namespace RobotArm
{
    public class RobotStateMachine
    {
        private const int Miliseconds = 50;
        private CommandParser CommandParser;

        private IDictionary<Command, Joint> JointCommands = new Dictionary<Command, Joint>()
            {
                { Command.Base, new Joint()},
                { Command.Wrist, new Joint()},
                { Command.Elbow, new Joint()},
                { Command.Shoulder, new Joint()},
                { Command.Grip, new Joint()}
            };

        public RobotStateMachine(CommandParser CommandParser)
        {
            this.CommandParser = CommandParser;
        }

        public void Move(string command)
        {
            var commands = CommandParser.Parse(command);
            var code = 0;

            foreach (var c in commands)
            {

                Joint joint = null;
                this.JointCommands.TryGetValue(c.Item1, out joint);

                if (joint == null)
                    return; //error, the code should return a joint

                if (c.Item2 == 1)
                {
                    if (joint.MoveUp(Miliseconds)) code = code + CommandParser.GetCombinedCodeForCommands(c);
                }
                else if (c.Item2 == 2)
                {
                    if (joint.MoveDown(Miliseconds)) code = code + CommandParser.GetCombinedCodeForCommands(c); ;
                }
                else
                    return; //error, up is 1, down is 2

            }

            var arm = new MaplinRobotArm();
            arm.Connect();
            arm.Update(code);
            Thread.Sleep(Miliseconds);
            arm.ResetArm();
            arm.Update(0);
            arm.Cleanup();
        }
    }
}
