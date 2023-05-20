namespace ObjectDetectMobile.Drawables
{
    using Microsoft.Maui.Graphics.Platform;
    using System.Reflection;
    using IImage = Microsoft.Maui.Graphics.IImage;
    public class ClockDrawable : IDrawable
    {
        //Исопльзовалось для тестов. В ПРОЕКТЕ НЕ ВЫЗЫВАЕТСЯ
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            IImage image;
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("ObjectDetectMobile.Resources.Images.people.jpg"))
            {
                // PlatformImage isn't currently supported on Windows.
                image = PlatformImage.FromStream(stream);
            }

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            if (image != null)
            {
                //canvas.DrawImage(image, 10, 10, image.Width, image.Height);
                canvas.StrokeColor = Colors.Red;
                canvas.StrokeSize = 6;
                canvas.DrawRectangle(10, 10, 350, 100);
            }
            /*canvas.StrokeColor = Colors.Red;
            canvas.StrokeSize = 6;
            canvas.DrawLine(10, 10, 90, 200);*/
            /*DateTime curTime = DateTime.Now;
            var clockCenterPoint = new PointF(200, 300);
            var circleRadius = 100;


            canvas.StrokeColor = Colors.Aqua;
            canvas.StrokeSize = 6;

            canvas.DrawCircle(clockCenterPoint, circleRadius);
            canvas.DrawCircle(clockCenterPoint, 5);

            canvas.StrokeSize = 4;
            var hourPoint = GetHourHand(curTime, circleRadius, clockCenterPoint);
            canvas.DrawLine(clockCenterPoint, hourPoint);


            var minutePoint = GetMinuteHand(curTime, circleRadius, clockCenterPoint);
            canvas.DrawLine(clockCenterPoint, minutePoint);

            var secondPoint = GetSecondHand(curTime, circleRadius, clockCenterPoint);
            canvas.DrawLine(clockCenterPoint, secondPoint);*/

        }

        internal static PointF GetHourHand(DateTime curTime, int radius, PointF center)
        {

            int currentHour = curTime.Hour;

            if (currentHour > 12)
                currentHour -= 12;

            var angleDegrees = (currentHour * 360) / 12;
            var angle = (Math.PI / 180.0) * angleDegrees;

            var hourShorter = radius * .8;
            PointF outerPoint = new((float)(hourShorter * Math.Sin(angle)) + center.X, (float)(-hourShorter * Math.Cos(angle)) + center.Y);

            return outerPoint;
        }

        internal static PointF GetMinuteHand(DateTime curTime, int radius, PointF center)
        {

            int currentMin = curTime.Minute;

            var angleDegrees = (currentMin * 360) / 60;
            var angle = (Math.PI / 180.0) * angleDegrees;

            PointF outerPoint = new((float)(radius * Math.Sin(angle)) + center.X, (float)(-radius * Math.Cos(angle)) + center.Y);

            return outerPoint;
        }

        internal static PointF GetSecondHand(DateTime curTime, int radius, PointF center)
        {

            int currentSecond = curTime.Second;

            var angleDegrees = (currentSecond * 360) / 60;
            var angle = (Math.PI / 180.0) * angleDegrees;

            PointF outerPoint = new((float)(radius * Math.Sin(angle) + center.X), (float)(-radius * Math.Cos(angle)) + center.Y);

            return outerPoint;
        }
    }
}
