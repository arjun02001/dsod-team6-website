using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GeocodeService;
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
           BingGeocode.GeocodeRequest geocodeRequest = new BingGeocode.GeocodeRequest();

            // Set the credentials using a valid Bing Maps key
            geocodeRequest.Credentials = new BingGeocode.Credentials();
            geocodeRequest.Credentials.ApplicationId = key;

            // Set the full address query
            geocodeRequest.Query = address;

            // Set the options to only return high confidence results 
            BingGeocode.ConfidenceFilter[] filters = new BingGeocode.ConfidenceFilter[1];
            filters[0] = new BingGeocode.ConfidenceFilter();
            filters[0].MinimumConfidence = BingGeocode.Confidence.High;

            // Add the filters to the options
            BingGeocode.GeocodeOptions geocodeOptions = new BingGeocode.GeocodeOptions();
            geocodeOptions.Filters = filters;
            geocodeRequest.Options = geocodeOptions;

            // Make the geocode request
            // GeocodeServiceClient geocodeService = new GeocodeServiceClient();
            if (geocodeRequest.Query != "")
            {
                BingGeocode.GeocodeService geocodeService = new BingGeocode.GeocodeService();
                BingGeocode.GeocodeResponse geocodeResponse = geocodeService.Geocode(geocodeRequest);

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

    //[WebMethod]
    //public string GetAddressFromCoord(string address)
    //{
        //string results = "";
        //string key = "AjdBmWL-51nlsoocyGsHAnE3eNOADU1CPWmJ-9kmPL1s7X13jurTlrRx1A49WzwD";
        //ReverseGeocodeRequest reverseGeocodeRequest = new ReverseGeocodeRequest();

        //// Set the credentials using a valid Bing Maps key
        //reverseGeocodeRequest.Credentials = new GeocodeService1.Credentials();
        //reverseGeocodeRequest.Credentials.ApplicationId = key;

        //// Set the point to use to find a matching address
        //GeocodeService1.Location point = new GeocodeService1.Location();
        //string[] digits = locationString.Split(',');

        //point.Latitude = double.Parse(digits[0].Trim());
        //point.Longitude = double.Parse(digits[1].Trim());

        //reverseGeocodeRequest.Location = point;

        //// Make the reverse geocode request
        //GeocodeServiceClient geocodeService = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
        //GeocodeResponse geocodeResponse = geocodeService.ReverseGeocode(reverseGeocodeRequest);

        //if (geocodeResponse.Results.Length > 0)
        //    results = geocodeResponse.Results[0].DisplayName;
        //else
        //    results = "No Results found";

        //return results;




    //}
}

public class CoordinateInfo
{
    public double X { get; set; }
    public double Y { get; set; }
}