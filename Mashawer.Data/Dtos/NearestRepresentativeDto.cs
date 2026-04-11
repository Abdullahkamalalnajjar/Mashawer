namespace Mashawer.Data.Dtos
{
    public class NearestRepresentativeDto
    {
        public string Id { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public double? RepresentativeFormLatitude { get; set; }
        public double? RepresentativeToLatitude { get; set; }

        public double? RepresentativeFromLongitude { get; set; }
        public double? RepresentativeToLongitude { get; set; }

        public double DistanceInKm { get; set; }  // ✅ المسافة بالكيلومتر
        public double DeleveryPrice { get; set; } // سعر التوصيل
    }
}
