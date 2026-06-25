namespace Core
{
    public class Node
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsFile { get; set; }
        public bool IsEmptyDir { get; set; }
        public bool IsSkipped { get; set; }
        public bool IsUnscanned { get; set; }
        public int Level { get; set; }
        public string Path { get; set; } = string.Empty;
    }
}
