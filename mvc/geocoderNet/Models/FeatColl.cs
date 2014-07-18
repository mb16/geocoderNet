
namespace geocoderNet.Models
{
    public class FeatColl
    {

        public string type { get { return "FeatureCollection"; } }
        public string address { get; set; }
        public Feat[] features { get; set; }
        public string error { get; set; }

    }
}