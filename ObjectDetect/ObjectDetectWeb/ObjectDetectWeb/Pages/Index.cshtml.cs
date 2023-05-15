using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ObjectDetectWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment hostinEnvironment;
        private readonly ILogger<IndexModel> _logger;//Изменить путь на свое локальное хранилище изображений
        private readonly string _storagePath = "D:\\Projects\\DigitalDepartment\\ObjectDetect\\storage";

        public IndexModel(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostinEnvironment, ILogger<IndexModel> logger)
        {
            this.hostinEnvironment = hostinEnvironment;
            _logger = logger;
        }

        public void OnGet()
        {

        }

        [BindProperty]
        public IFormFile Upload { get; set; }
        public async Task<IActionResult> OnPostAsync()
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
            var imageFolder = hostinEnvironment.WebRootPath + "\\Images";
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }
            using FileStream fileResult = new FileStream(hostinEnvironment.WebRootPath + "\\Images\\1.png", FileMode.Create);
            await Upload.CopyToAsync(fileResult);

            return Redirect($"Analyze/{Upload.FileName}");
        }
    }
}