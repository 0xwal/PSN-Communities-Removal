namespace PSNLib.Models
{
    public class Community
    {
        public string Name { get; internal set; }
        public string Id { get; internal set; }
        public int MembersCount { get; internal set; }
        public string Role { get; internal set; }
        public bool IsOpen { get; internal set; }
    }
}
