namespace Calia.Services.AuthAPI.Models.Dto
{
    public class UsersDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Roles { get; set; } // Kullanıcı rolleri
        public bool Lock_Unlock { get; set; }
    }
}
