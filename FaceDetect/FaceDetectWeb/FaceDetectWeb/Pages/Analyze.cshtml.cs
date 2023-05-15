using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace FaceDetectWeb.Pages
{
    public class AnalyzeModel : PageModel
    {
        public string SourceImage { get; set; }
        public List<string> DetectEmotions { get; set; }
        //Изменить путь на свое локальное хранилище изображений
        private readonly string _storagePath = "D:\\Projects\\DigitalDepartment\\FaceDetect\\storage";

        // Obstruction color 
        private readonly Color obsColor = Color.Red;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment hostinEnvironment;

        public AnalyzeModel(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostinEnvironment)
        {
            DetectEmotions = new List<string>();
            this.hostinEnvironment = hostinEnvironment;
        }

        public void OnGet(string fileName)
        {
            SourceImage = "Images/1.png?" + DateTime.UtcNow;

            var httpClient = new HttpClient();
            var response = httpClient.GetAsync($"http://localhost:8000/{fileName}").Result;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var deserializeResult = JsonSerializer.Deserialize<Response>(result);

                //Execute response
                var imageBytes = DrawingImage(fileName, deserializeResult);

                HttpContext.Response.ContentType = "image/png";
                HttpContext.Response.BodyWriter.WriteAsync(imageBytes);
            }
        }

        private byte[] DrawingImage(string fileName, Response response)
        {
            using var ms = new MemoryStream();
            // Create the image
            using System.Drawing.Image fileResult = System.Drawing.Image.FromFile(_storagePath + "\\" + fileName);
            using Bitmap bitmap = new Bitmap(fileResult);
            // Create the graphics 
            using Graphics graphics = Graphics.FromImage(bitmap);

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var points = new List<List<Point>>();
            foreach (var res in response.Result)
            {
                var x1 = res.Coordinate[0].X;
                var x2 = res.Coordinate[1].X;
                var y1 = res.Coordinate[0].Y;
                var y2 = res.Coordinate[1].Y;
                points.Add(new List<Point> { new Point(x1, y1), new Point(x1, y2) });
                points.Add(new List<Point> { new Point(x1, y2), new Point(x2, y2) });
                points.Add(new List<Point> { new Point(x2, y2), new Point(x2, y1) });
                points.Add(new List<Point> { new Point(x2, y1), new Point(x1, y1) });

                RectangleF rectf = new RectangleF(x1, y1, 1000, 50);
                graphics.DrawString(res.Emotion, new Font("Tahoma", 20), Brushes.Black, rectf);
            }
            graphics.DrawString($"Обнаруженных эмоций: {response.Result.Count}", new Font("Tahoma", 20), Brushes.Black, new RectangleF(0, 0, 1000, 50));
            // Add obstructions
            using (Pen pen = new Pen(new SolidBrush(obsColor), 5))
            {
                for (int i = 0; i < points.Count; i++)
                {
                    graphics.DrawLine(pen, points[i][0], points[i][1]);
                }
            }

            graphics.Flush();
            // Save image, image format type is consistent with response content type.
            var imageFolder = hostinEnvironment.WebRootPath + "\\Images";
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }

            bitmap.Save(imageFolder + "\\1.png", System.Drawing.Imaging.ImageFormat.Png);
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            return ms.ToArray();
        }

        public class Coordinate
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public class Result
        {
            public string Emotion { get; set; }
            public List<Coordinate> Coordinate { get; set; }
        }

        public class Response
        {
            public List<Result> Result { get; set; }
        }
    }
}
