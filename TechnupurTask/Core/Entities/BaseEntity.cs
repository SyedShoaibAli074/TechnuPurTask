namespace TechnupurTask.Core.Entities
{
    public abstract class BaseEntity
    {
        public long ID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public long CreatedBy { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
