namespace Entities
{
    public class ProjectPermission : BaseEntity
    {
        public User   RequestUser { get; set; }
        public string ProjectId   { get; set; }
        public bool   IsApproved  { get; set; }
    }
}
