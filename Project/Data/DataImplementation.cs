//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//_____________________________________________________________________________________________________________________________________

using System.Diagnostics;

namespace TP.ConcurrentProgramming.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        #region ctor

        public DataImplementation()
        {
            MoveTimer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16));
        }

        #endregion ctor

        #region DataAbstractAPI

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));

            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            Random random = new Random();

            BallsList.Clear();

            double diameter = 20;

            for (int i = 0; i < numberOfBalls; i++)
            {
                Vector startingPosition = new(
                    random.Next(0, (int)(380 - diameter)),
                    random.Next(0, (int)(400 - diameter))
                );

                Vector velocity = new(
                    (random.NextDouble() - 0.5) * 6,
                    (random.NextDouble() - 0.5) * 6
                );
                double mass = random.Next(1, 10);
                Ball newBall = new(startingPosition, velocity,mass);

                upperLayerHandler(startingPosition, newBall);

                BallsList.Add(newBall);
            }

            MoveTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(16));
        }

        public override void Stop()
        {
            MoveTimer.Change(Timeout.Infinite, Timeout.Infinite);

            BallsList.Clear();
        }

        #endregion DataAbstractAPI

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    MoveTimer.Dispose();

                    BallsList.Clear();
                }

                Disposed = true;
            }
            else
                throw new ObjectDisposedException(nameof(DataImplementation));
        }

        public override void Dispose()
        {
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        private bool Disposed = false;

        private readonly Timer MoveTimer;

        private readonly List<Ball> BallsList = [];

        private readonly object _lock = new();

        private void Move(object? x)
        {
            lock (_lock)
            {
                foreach (Ball ball in BallsList)
                {
                    ball.Move((Vector)ball.Velocity);
                }

                CheckCollisions();
            }
        }

        private void CheckCollisions()
        {
            double diameter = 20;

            for (int i = 0; i < BallsList.Count; i++)
            {
                for (int j = i + 1; j < BallsList.Count; j++)
                {
                    Ball b1 = BallsList[i];
                    Ball b2 = BallsList[j];

                    Vector p1 = (Vector)b1.CurrentPosition;
                    Vector p2 = (Vector)b2.CurrentPosition;

                    double dx = p1.x - p2.x;
                    double dy = p1.y - p2.y;

                    double distSq = dx * dx + dy * dy;

                    if (distSq <= diameter * diameter && distSq > 0.000001)
                    {
                        Vector v1 = (Vector)b1.Velocity;
                        Vector v2 = (Vector)b2.Velocity;

                        double dist = Math.Sqrt(distSq);

                        double nx = dx / dist;
                        double ny = dy / dist;

                        double tx = -ny;
                        double ty = nx;

                        double v1n = v1.x * nx + v1.y * ny;
                        double v1t = v1.x * tx + v1.y * ty;

                        double v2n = v2.x * nx + v2.y * ny;
                        double v2t = v2.x * tx + v2.y * ty;

                        double m1 = b1.Mass;
                        double m2 = b2.Mass;

                        double v1nAfter =
                            (v1n * (m1 - m2) + 2 * m2 * v2n) / (m1 + m2);

                        double v2nAfter =
                            (v2n * (m2 - m1) + 2 * m1 * v1n) / (m1 + m2);

                        Vector newV1 = new(
                            v1nAfter * nx + v1t * tx,
                            v1nAfter * ny + v1t * ty
                        );

                        Vector newV2 = new(
                            v2nAfter * nx + v2t * tx,
                            v2nAfter * ny + v2t * ty
                        );

                        b1.Velocity = newV1;
                        b2.Velocity = newV2;

                        double overlap = diameter - dist;

                        b1.Move(new Vector(nx * overlap / 2, ny * overlap / 2));
                        b2.Move(new Vector(-nx * overlap / 2, -ny * overlap / 2));
                    }
                }
            }
        }

        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
        {
            returnBallsList(BallsList);
        }

        [Conditional("DEBUG")]
        internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
        {
            returnNumberOfBalls(BallsList.Count);
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}