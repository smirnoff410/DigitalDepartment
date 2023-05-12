using AudioWeb.Domain;
using AudioWeb.Services.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AudioWeb.Pages
{
    public class UploadFileModel : PageModel
    {
        private readonly ITrackRepository _repository;
        private readonly string _storagePath = "D:\\Projects\\DigitalDepartment\\Audio\\storage";

        public UploadFileModel(ITrackRepository repository)
        {
            _repository = repository;
        }
        public void OnGet()
        {
        }

        [BindProperty]
        public IFormFile Upload { get; set; }
        public async Task OnPostAsync()
        {
            var file = Path.Combine(_storagePath, Upload.FileName);
            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                await Upload.CopyToAsync(fileStream);
            }
            _repository.Add(new TrackEntity
            {
                Name = Upload.FileName,
            });
        }
    }
}
