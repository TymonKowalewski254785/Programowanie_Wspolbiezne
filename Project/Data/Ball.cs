//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data
{
  internal class Ball : IBall
  {
    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity)
    {
      Position = initialPosition;
      Velocity = initialVelocity;
    }

    #endregion ctor

    #region IBall

    public event EventHandler<IVector>? NewPositionNotification;

    public IVector Velocity { get; set; }

    #endregion IBall

    #region private

    private Vector Position;

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }

        internal void Move(Vector delta)
        {
            double diameter = 20;

            double width = 380;
            double height = 400;

            double minX = 0;
            double maxX = width - diameter;

            double minY = 0;
            double maxY = height - diameter;

            double newX = Position.x + delta.x;
            double newY = Position.y + delta.y;
            if (newX < minX) newX = minX;
            else if (newX > maxX) newX = maxX;
            if (newY < minY) newY = minY;
            else if (newY > maxY) newY = maxY;

            Position = new Vector(newX, newY);

            RaiseNewPositionChangeNotification();
        }
        #endregion private
    }
}