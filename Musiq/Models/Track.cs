using ManagedBass;

namespace Musiq.Models
{
    public class Track : TagProperties<string>
    {
        public double Score { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
    }
}
