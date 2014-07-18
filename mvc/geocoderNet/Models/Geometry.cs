

namespace geocoderNet.Models
{
    public class Geometry
    {

        public string type { get { return "Point"; } }
        public double[] coordinates { get; set; }
    
    }
}