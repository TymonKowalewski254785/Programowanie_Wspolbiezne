//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data.Test
{
  [TestClass]
  public class BallUnitTest
  {
        [TestMethod]
        public void MoveTestMethod()
        {
            Vector initialPosition = new(10.0, 10.0);
            Ball newInstance = new(initialPosition, new Vector(1.0, 0.0));

            IVector currentPosition = new Vector(0.0, 0.0);
            int numberOfCallBackCalled = 0;

            newInstance.NewPositionNotification += (sender, position) =>
            {
                Assert.IsNotNull(sender);
                currentPosition = position;
                numberOfCallBackCalled++;
            };

            newInstance.Move(new Vector(1.0, 0.0));

            Assert.AreEqual(1, numberOfCallBackCalled);

            Assert.AreEqual(
                new Vector(11.0, 10.0),
                currentPosition);
        }

        [TestMethod]
        public void PhysicsMove_ShouldRespectVelocity()
        {
            Ball b = new(new Vector(0, 0), new Vector(2, 3));

            IVector pos = null;

            ManualResetEvent ev = new(false);

            b.NewPositionNotification += (s, p) =>
            {
                pos = p;
                ev.Set();
            };

            b.Move(b.Velocity);

            bool signaled = ev.WaitOne(500);

            Assert.IsTrue(signaled, "Event was not raised");

            Assert.IsNotNull(pos);

            Assert.AreEqual(2.0, pos.x, 0.0001);
            Assert.AreEqual(3.0, pos.y, 0.0001);
        }
    }
}