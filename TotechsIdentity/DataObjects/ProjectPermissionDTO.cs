namespace TotechsIdentity.DataObjects
{
    public class ProjectPermissionDTO : BaseDTO
    {
        public UserDTO RequestUser { get; set; }
        public string  ProjectId { get; set; }
        public bool    IsApproved { get; set; }
    }
}
