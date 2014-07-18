using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elmah;
using geocoderNet.Models;
using System.Text.RegularExpressions;

/* This project is a pretty exact translation of the Geocommons Geocoder project.  If there are bugs,
 * refer to the oroginal project to determine how it should operate.
 * 
 * https://github.com/geocommons/geocoder
 */


namespace geocoderNet.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string q)
        {



            var featColl = new FeatColl();
            try
            {

                if (String.IsNullOrEmpty(q)) throw new Exception();

                var pattern = @"\s+(and|at)\s+";
                var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                q = rgx.Replace(q, " ");

                var items = new Database().geocode(q, false, true);

                featColl.address = q;

                if (items != null)
                {
                    featColl.features = new Feat[items.Count];

                    var index = 0;
                    foreach (var item in items)
                    {
                        var feat = new Feat();
                        feat.properties = item;
                        feat.geometry = new Geometry();
                        feat.geometry.coordinates = new[] {item.lon, item.lat};
                        featColl.features[index] = feat;
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex); //ELMAH Signaling
                featColl.error = "JSON::GeneratorError";
            }

            return View(featColl);
        }

     
    }
}