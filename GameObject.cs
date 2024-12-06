using SplashKitSDK;

namespace FlappyBird
{
    public abstract class GameObject
    {
        public abstract Bitmap _ObjectBitmap { get; }
        public double X { get; private set; }
        public virtual double Y { get; protected set; }
        public const int SPEED = 2;

        public GameObject(double x)
        {
            X = x;
        }

        public virtual void Draw()
        {
            X -= SPEED; // Constantly reduce the x-axis value so the object translates from right to left
        }

        public bool IsOffScreen()
        {
            return X + _ObjectBitmap.Width < 0; // Return if the object is out of view
        }
    }
}
