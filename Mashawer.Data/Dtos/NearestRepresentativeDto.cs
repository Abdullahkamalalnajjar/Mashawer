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
        public double? RepresentativeLatitude { get; set; }  // خط العرض
        public double? RepresentativeLongitude { get; set; } // خط الطول
        public double DistanceInKm { get; set; }  // ✅ المسافة بالكيلومتر
        public double DeleveryPrice { get; set; } // سعر التوصيل
    }
}
