namespace Mashawer.Data.Dtos
{
    public class RepresentativeDTO
    {
        public string Id { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public double? RepresentativeLatitude { get; set; }
        public double? RepresentativeLongitude { get; set; }

    }
}
