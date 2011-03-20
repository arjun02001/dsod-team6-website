using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GeocodeService;

/// <summary>
using System.Web.Services;
/// Summary description for WebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class WebService : System.Web.Services.WebService {

    public WebService () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string GetCoordFromAddress(string address)
    {
        try
        {
            GeocodeUtil.CoordinateInfo Coord = new GeocodeUtil.CoordinateInfo();
            Coord = GeocodeUtil.GeocodeAddress(address, "AjdBmWL-51nlsoocyGsHAnE3eNOADU1CPWmJ-9kmPL1s7X13jurTlrRx1A49WzwD");
            return Coord.Y + " , " + Coord.X;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}
