using SplashKitSDK;

namespace FlappyBird
{
    public class BottomPipe : GameObject
    {
        private Bitmap _pipeBitmapDay;
        private Bitmap _pipeBitmapNight;
        private Bitmap _currentPipeBitmap;

        public override Bitmap _ObjectBitmap { get { return _currentPipeBitmap; } }

        public BottomPipe(double x, double y, bool isDayTheme) : base(x)
        {
            _pipeBitmapDay = new Bitmap("BottomPipeDay", "bottompillar_day1.png");
            _pipeBitmapNight = new Bitmap("BottomPipeNight", "bottompillar_night.png");
            _currentPipeBitmap = isDayTheme ? _pipeBitmapDay : _pipeBitmapNight;
            Y = y;
        }

        public void SetTheme(bool isDay)
        {
            _currentPipeBitmap = isDay ? _pipeBitmapDay : _pipeBitmapNight;
        }

        override public void Draw()
        {
            base.Draw();
            _currentPipeBitmap.Draw(X, Y);
        }
    }
}
