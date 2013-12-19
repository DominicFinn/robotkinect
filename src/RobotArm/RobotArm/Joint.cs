namespace RobotArm
{
    public class Joint
    {
        private bool Locked { get; set; }
        private double Position { get; set; }

        private const int THRESHOLD_UP = 500;
        private const int THRESHOLD_DOWN = -500;


        public bool MoveUp(int m)
        {
            
            var isMoved = false;
            if (!Locked)
            {
                Locked = true;

                if (CanMoveUp(m))
                {
                    var movement = Position + (m * 0.85);
                    Position = movement;
                    isMoved = true;
                }

                Locked = false;
            }
            return isMoved;
        }

        private bool CanMoveUp(int m)
        {
            var movement = Position + (m * 0.86);
            return (movement < THRESHOLD_UP && movement > THRESHOLD_DOWN);
        }

        public bool MoveDown(int m)
        {
            var isMoved = false;
            if (!Locked)
            {
                Locked = true;

                if (CanMoveDown(m))
                {
                    var movement = Position - m;
                    Position = movement;
                    isMoved = true;
                }

                Locked = false;
            }
            return isMoved;
        }

        public bool CanMoveDown(int m)
        {
            var movement = Position - m;
            return (movement < THRESHOLD_UP && movement > THRESHOLD_DOWN);
        }
    }
}
