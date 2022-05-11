namespace Entities
{
    public class RoleLevel : BaseEntity
    {
        public string  LevelName         { get; set; }
        public string  Description       { get; set; }
        public string  LevelColor        { get; set; }

        public string? RoleLevelIcon     { get; set; }
        public int     Level             { get; set; }
    }
}
