using AutoMapper;

namespace PermissionServerDemo.Api.Entities.Dtos
{
    [AutoMap(typeof(Aircraft))]
    public class AircraftGetDto
    {
        public string RegNumber { get; private set; }
        public string Model { get; private set; }
        public string ThumbnailUri { get; private set; }
        public bool IsGrounded { get; private set; }
    }
}