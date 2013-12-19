using System;
using System.Security.Policy;
using Microsoft.Owin.Hosting;
using Owin;
using RobotArm.Commands;

namespace RobotArm
{
    class Program
    {
        static void Main(string[] args)
        {
            string uri = "http://*:8080/";

            using (WebApp.Start<Startup>(uri))
            {               
                Console.WriteLine("Started");
                Console.ReadKey();
                Console.WriteLine("Stopping");
            }
        }

        public class Startup
        {
            private RobotStateMachine robotStateMachine;

            public Startup()
            {
                this.robotStateMachine = new RobotStateMachine(new CommandParser());
            }

            public void Configuration(IAppBuilder app)
            {
                app.Run((context) =>
                    {
                        String queryString = string.Empty;

                        if (context.Request.QueryString.HasValue)
                        {
                            queryString = Uri.UnescapeDataString(context.Request.QueryString.Value);

                            robotStateMachine.Move(queryString);

                            Console.WriteLine(queryString);
                            Console.WriteLine("Moving for command " + queryString);
                        }

                        return context.Response.WriteAsync("Response: " + queryString);
                    });
            }
        }

        public class RobotInterface
        {
            private readonly RobotStateMachine robotStateMachine;

            public RobotInterface(RobotStateMachine robotStateMachine)
            {
                this.robotStateMachine = robotStateMachine;
            }

            public void Do(string command)
            {
                robotStateMachine.Move(command);
            }
        }

    }
}
