using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BingGeocode;

public static class GeocodeUtil
{
    public static CoordinateInfo GeocodeAddress(string address, string Key)
    {
        CoordinateInfo retVal = null;
        string key = Key;//System.Configuration.ConfigurationManager.AppSettings["BingToken"];
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
                retVal = new CoordinateInfo();//(IPoint)TRSConn.serverContext.CreateObject("esriGeometry.Point");

                retVal.X = geocodeResponse.Results[0].Locations[0].Longitude;
                retVal.Y = geocodeResponse.Results[0].Locations[0].Latitude;

            }
        }

        return retVal;
    }

    public static string ReverseGeocodeAddress(double X, double Y, string Key)
    {
        string Results = "";
        try
        {
            // Set a Bing Maps key before making a request
            string key = Key;

            ReverseGeocodeRequest reverseGeocodeRequest = new ReverseGeocodeRequest();

            // Set the credentials using a valid Bing Maps key
            reverseGeocodeRequest.Credentials = new Credentials();
            reverseGeocodeRequest.Credentials.ApplicationId = key;

            // Set the point to use to find a matching address
            Location point = new Location();
            point.Longitude = X;
            point.Latitude = Y;
            reverseGeocodeRequest.Location = point;
            // Make the reverse geocode request
            BingGeocode.GeocodeService geocodeService = new BingGeocode.GeocodeService();
            GeocodeResponse geocodeResponse = geocodeService.ReverseGeocode(reverseGeocodeRequest);
            Results = geocodeResponse.Results[0].DisplayName;
        }
        catch (Exception ex)
        {
            Results = "An exception occurred: " + ex.Message;
        }
        return Results;
    }

    public class CoordinateInfo
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}
