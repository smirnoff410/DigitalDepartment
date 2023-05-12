using AudioWeb.Domain;

namespace AudioWeb.Services.Repository
{
    public class TrackRepository : ITrackRepository
    {
        public List<TrackEntity> ExistData = new List<TrackEntity> 
        { 
            new TrackEntity { Id = 1, Name = "Eminem – Mockingbird.mp3" }, 
            new TrackEntity { Id = 2, Name = "miyagi-andy-panda-minor-mp3.mp3" } 
        };

        public void Add(TrackEntity entity)
        {
            entity.Id = ExistData.Last().Id + 1;
            ExistData.Add(entity);
        }

        public List<TrackEntity> GetList()
        {
            return ExistData;
        }
    }
}
