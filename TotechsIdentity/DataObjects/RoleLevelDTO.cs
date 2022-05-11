namespace TotechsIdentity.DataObjects
{
    public class RoleLevelDTO
    {
        public string RoleName { get; set; }
        public string Description { get; set; }
        public string LevelColor { get; set; }

        public string? RoleLevelIcon { get; set; }
        public int Level { get; set; }
    }
}
