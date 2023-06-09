﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;
using System.Text.Json;

namespace DefectWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment hostinEnvironment;
        private readonly ILogger<IndexModel> _logger;
        //Изменить путь на свое локальное хранилище изображений
        private readonly string _storagePath = "D:\\Projects\\DigitalDepartment\\Defect\\storage";

        public IndexModel(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostinEnvironment, ILogger<IndexModel> logger)
        {
            this.hostinEnvironment = hostinEnvironment;
            _logger = logger;
        }

        public void OnGet()
        {
            var current = Directory.GetCurrentDirectory();
        }

        // Obstruction color 
        private readonly Color obsColor = Color.Red;
        [BindProperty]
        public IFormFile Upload { get; set; }
        public async Task OnPostAsync()
        {
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
            var file = Path.Combine(_storagePath, Upload.FileName);
            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                await Upload.CopyToAsync(fileStream);
            }

            var httpClient = new HttpClient();
            var response = httpClient.GetAsync($"http://localhost:8000/{Upload.FileName}").Result;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var deserializeResult = JsonSerializer.Deserialize<Response>(result);

                var imageBytes = DrawingImage(deserializeResult);

                HttpContext.Response.ContentType = "image/png";
                HttpContext.Response.BodyWriter.WriteAsync(imageBytes);
            }
        }

        private byte[] DrawingImage(Response response)
        {
            using var ms = new MemoryStream();
            // Create the image
            using FileStream fileResult = new FileStream(_storagePath + "\\" + Upload.FileName, FileMode.Open);
            using Bitmap bitmap = new Bitmap(fileResult);
            // Create the graphics 
            using Graphics graphics = Graphics.FromImage(bitmap);


            var points = new List<List<Point>>();
            foreach (var res in response.Result)
            {
                var x1 = Convert.ToInt32(res[0].X);
                var x2 = Convert.ToInt32(res[1].X);
                var y1 = Convert.ToInt32(res[0].Y);
                var y2 = Convert.ToInt32(res[1].Y);
                points.Add(new List<Point> { new Point(x1, y1), new Point(x1, y2) });
                points.Add(new List<Point> { new Point(x1, y2), new Point(x2, y2) });
                points.Add(new List<Point> { new Point(x2, y2), new Point(x2, y1) });
                points.Add(new List<Point> { new Point(x2, y1), new Point(x1, y1) });
            }
            // Add obstructions
            using (Pen pen = new Pen(new SolidBrush(obsColor), 5))
            {
                for (int i = 0; i < points.Count; i++)
                {
                    graphics.DrawLine(pen, points[i][0], points[i][1]);
                }
            }
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

        class Response
        {
            public List<List<Result>> Result { get; set; }
        }

        class Result
        {
            public string X { get; set; }
            public string Y { get; set; }
        }
    }
}