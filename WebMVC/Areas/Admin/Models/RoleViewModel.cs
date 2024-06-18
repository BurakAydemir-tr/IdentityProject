namespace WebMVC.Areas.Admin.Models
{
    public class RoleViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; //null olmayacağını belirtiyoruz.
    }
}
