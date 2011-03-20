using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GeocodeService;
using BingGeocode;
using System.Security.Cryptography;

/// <summary>
using System.Web.Services;
/// Summary description for WebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class WebService : System.Web.Services.WebService
{

    public WebService()
    {

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
            return retVal.Y + " , " + retVal.X;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    [WebMethod]
    public string GetAddressFromCoord(string locationString)
    {
        try
        {
            string results = "";
            string key = "AjdBmWL-51nlsoocyGsHAnE3eNOADU1CPWmJ-9kmPL1s7X13jurTlrRx1A49WzwD";
            GeocodeService.ReverseGeocodeRequest reverseGeocodeRequest = new GeocodeService.ReverseGeocodeRequest();

            // Set the credentials using a valid Bing Maps key
            reverseGeocodeRequest.Credentials = new GeocodeService.Credentials();
            reverseGeocodeRequest.Credentials.ApplicationId = key;

            // Set the point to use to find a matching address
            GeocodeService.Location point = new GeocodeService.Location();
            string[] digits = locationString.Split(',');

            point.Latitude = double.Parse(digits[0].Trim());
            point.Longitude = double.Parse(digits[1].Trim());

            reverseGeocodeRequest.Location = point;

            // Make the reverse geocode request
            GeocodeService.GeocodeServiceClient geocodeService = new GeocodeService.GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
            GeocodeService.GeocodeResponse geocodeResponse = geocodeService.ReverseGeocode(reverseGeocodeRequest);

            if (geocodeResponse.Results.Length > 0)
                results = geocodeResponse.Results[0].DisplayName;
            else
                results = "No Results found";

            return results;
        }
        catch (Exception ex)
        {
            return "Address not found";
        }

     }

    [WebMethod]
    public string EncryptString(string message, string passphrase)
    {
        try
        {
            byte[] results;
            System.Text.UTF8Encoding utf8 = new System.Text.UTF8Encoding();
            MD5CryptoServiceProvider hashprovider = new MD5CryptoServiceProvider();
            byte[] tdeskey = hashprovider.ComputeHash(utf8.GetBytes(passphrase));
            TripleDESCryptoServiceProvider tdesalgorithm = new TripleDESCryptoServiceProvider();
            tdesalgorithm.Key = tdeskey;
            tdesalgorithm.Mode = CipherMode.ECB;
            tdesalgorithm.Padding = PaddingMode.PKCS7;
            byte[] datatoencrypt = utf8.GetBytes(message);
            try
            {
                ICryptoTransform encryptor = tdesalgorithm.CreateEncryptor();
                results = encryptor.TransformFinalBlock(datatoencrypt, 0, datatoencrypt.Length);
            }
            finally
            {
                tdesalgorithm.Clear();
                hashprovider.Clear();
            }
            return Convert.ToBase64String(results);
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