using AudioWeb.Domain;

namespace AudioWeb.Services.Repository
{
    public interface ITrackRepository
    {
        public List<TrackEntity> GetList();
        public void Add(TrackEntity entity);
    }
}
