namespace Mashawer.Core.Features.Representatives.Queries.Models
{
    public class GetNearestRepresentativeQuery : IRequest<Response<IEnumerable<NearestRepresentativeDto>>>
    {
        public double FromLatitude { get; set; }
        public double FromLongitude { get; set; }
        public double ToLatitude { get; set; }
        public double ToLongitude { get; set; }
        public GetNearestRepresentativeQuery(double fromLatitude, double fromLongitude, double toLat, double toLong)
        {
            FromLatitude = fromLatitude;
            FromLongitude = fromLongitude;
            ToLatitude = toLat;
            ToLongitude = toLong;
        }
    }
}
