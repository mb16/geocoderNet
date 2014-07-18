

namespace geocoderNet.Models
{
    public class Feat
    {

        public string type { get { return "Feature"; } }
        public Item properties { get; set; }
        public Geometry geometry { get; set; }
        
    }
}