using SplashKitSDK;

namespace FlappyBird
{
    public class TopPipe : GameObject
    {
        private Bitmap _pipeBitmapDay;
        private Bitmap _pipeBitmapNight;
        private Bitmap _currentPipeBitmap;

        public override Bitmap _ObjectBitmap { get { return _currentPipeBitmap; } }

        public TopPipe(double x, double y, bool isDayTheme) : base(x)
        {
            _pipeBitmapDay = new Bitmap("TopPipeDay", "toppillar_day1.png");
            _pipeBitmapNight = new Bitmap("TopPipeNight", "toppillar_night.png");
            _currentPipeBitmap = isDayTheme ? _pipeBitmapDay : _pipeBitmapNight;
            Y = 0 - (_currentPipeBitmap.Height - y);
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
