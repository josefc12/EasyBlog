namespace EasyBlog.Shared.Dtos
{
    public class UserDto 
    {
        public int? Id {get;set;}
        public string? Nickname {get;set;}
        public string? PasswordHash {get;set;}
        public string? Email {get;set;}
        public DateTime? DateCreated {get;set;}
        public DateTime? DateDeleted {get;set;}

    }
}