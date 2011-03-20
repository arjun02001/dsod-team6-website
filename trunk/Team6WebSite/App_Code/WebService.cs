using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using GeocodeService;
using BingGeocode;

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
            CoordinateInfo retVal = null;
            string key = "AjdBmWL-51nlsoocyGsHAnE3eNOADU1CPWmJ-9kmPL1s7X13jurTlrRx1A49WzwD";
            GeocodeRequest geocodeRequest = new GeocodeRequest();

            // Set the credentials using a valid Bing Maps key
            geocodeRequest.Credentials = new Credentials();
            geocodeRequest.Credentials.ApplicationId = key;

            // Set the full address query
            geocodeRequest.Query = address;

            // Set the options to only return high confidence results 
            ConfidenceFilter[] filters = new ConfidenceFilter[1];
            filters[0] = new ConfidenceFilter();
            filters[0].MinimumConfidence = Confidence.High;

            // Add the filters to the options
            GeocodeOptions geocodeOptions = new GeocodeOptions();
            geocodeOptions.Filters = filters;
            geocodeRequest.Options = geocodeOptions;

            // Make the geocode request
            // GeocodeServiceClient geocodeService = new GeocodeServiceClient();
            if (geocodeRequest.Query != "")
            {
                BingGeocode.GeocodeService geocodeService = new BingGeocode.GeocodeService();
                GeocodeResponse geocodeResponse = geocodeService.Geocode(geocodeRequest);

                if (geocodeResponse.Results.Length > 0)
                {
                    retVal = new CoordinateInfo();
                    retVal.X = geocodeResponse.Results[0].Locations[0].Longitude;
                    retVal.Y = geocodeResponse.Results[0].Locations[0].Latitude;
                }
            }
            return retVal.X + " , " + retVal.Y ;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}

public class CoordinateInfo
{
    public double X { get; set; }
    public double Y { get; set; }
}