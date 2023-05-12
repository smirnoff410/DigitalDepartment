using AudioWeb.Domain;
using AudioWeb.Services.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AudioWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ITrackRepository _repository;
        private readonly ILogger<IndexModel> _logger;
        private readonly string _storagePath = "D:\\Projects\\DigitalDepartment\\Audio\\storage";
        public List<TrackEntity> TrackNames { get; set; }

        public IndexModel(ITrackRepository repository, ILogger<IndexModel> logger)
        {
            _repository = repository;
            _logger = logger;
            TrackNames = new List<TrackEntity>();
        }

        public void OnGet()
        {
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
            var fileNames = Directory.GetFiles(_storagePath).Select(x => x.Split('\\').Last());

            TrackNames.AddRange(_repository.GetList());
        }

        public IActionResult OnPost(int trackId)
        {
            return Redirect($"Analize/{trackId}");
        }
    }
}