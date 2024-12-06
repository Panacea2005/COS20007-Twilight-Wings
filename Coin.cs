using SplashKitSDK;

namespace FlappyBird
{
    public class Coin : GameObject
    {
        private Bitmap _coinBitmap;
        public override Bitmap _ObjectBitmap { get { return _coinBitmap; } }
        private float _rotationAngle = 0.0f;

        public Coin(double x, double y) : base(x)
        {
            _coinBitmap = new Bitmap("Coin", "moonstone.png");
            Y = y;
        }

        override public void Draw()
        {
            base.Draw();
            // Update the rotation angle
            _rotationAngle += 2.0f;

            // Limit the rotation angle to 360 degrees to prevent overflow
            if (_rotationAngle >= 360.0f)
            {
                _rotationAngle = 0.0f;
            }
            _coinBitmap.Draw(X, Y, SplashKit.OptionRotateBmp(_rotationAngle));
        }
    }
}
