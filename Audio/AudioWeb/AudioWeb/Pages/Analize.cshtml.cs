using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace AudioWeb.Pages
{
    public class AnalizeModel : PageModel
    {
        class AnalizeResponse
        {
            public string TrackName { get; set; }
            public List<string> Similar { get; set; }
        }

        public string TrackName { get; set; }
        public List<string> SimilarTracks { get; set; }
        public void OnGet(int id)
        {
            var httpClient = new HttpClient();
            var response = httpClient.GetAsync($"http://localhost:8000/{id}").Result;
            if(response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var deserializeResult = JsonSerializer.Deserialize<AnalizeResponse>(result);
                TrackName = deserializeResult.TrackName;
                SimilarTracks = deserializeResult.Similar;
            }
        }
    }
}
