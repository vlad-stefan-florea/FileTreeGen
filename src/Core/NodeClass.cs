namespace Core
{
    public enum NodeType
    {
        Folder,
        File,
    }

    public class Node
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public NodeType Type { get; set; }
        public int Level { get; set; }
        public string Path { get; set; } = string.Empty;
    }
}
