﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GeocodeService;
using BingGeocode;
using System.Security.Cryptography;
using ImageryService;
using System.Text;
using System.Net.Mail;
using System.Text.RegularExpressions;
using RoutingService;

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
            return "Address could not be resolved";
        }
    }

    [WebMethod]
    public string GetAddressFromCoord(string commaseparatedcoords)
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
            string[] digits = commaseparatedcoords.Split(',');

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
        catch (Exception)
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
            return "Error in encrypting string";
        }
    }

    [WebMethod]
    public string DecryptString(string encryptedmessage, string passphrase)
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
            byte[] datatodecrypt = Convert.FromBase64String(encryptedmessage);
            try
            {
                ICryptoTransform decryptor = tdesalgorithm.CreateDecryptor();
                results = decryptor.TransformFinalBlock(datatodecrypt, 0, datatodecrypt.Length);
            }
            finally
            {
                tdesalgorithm.Clear();
                hashprovider.Clear();
            }
            return utf8.GetString(results);
        }
        catch (Exception)
        {
            return "Error in decrypting string";
        }
        
    }

    [WebMethod]
    public String AsciiConversion(String stringtoconvert)
    {
        try
        {
            string c = string.Empty;
            string a;
            byte[] ASCIIValues = Encoding.ASCII.GetBytes(stringtoconvert);
            foreach (byte b in ASCIIValues)
            {
                a = Convert.ToString(b);
                c = String.Concat(c, " ", a);
            }
            return "The ascii value of string is " + c;
        }
        catch (Exception)
        {
            return "Operation Failed";
        }
    }

    [WebMethod]
    public string GetImageFromAddress(string address)
    {
        try
        {
            string locationString = GetCoordFromAddress(address);
            string key = "AjdBmWL-51nlsoocyGsHAnE3eNOADU1CPWmJ-9kmPL1s7X13jurTlrRx1A49WzwD";
            MapUriRequest mapUriRequest = new MapUriRequest();

            // Set credentials using a valid Bing Maps key
            mapUriRequest.Credentials = new ImageryService.Credentials();
            mapUriRequest.Credentials.ApplicationId = key;

            // Set the location of the requested image
            mapUriRequest.Center = new ImageryService.Location();
            string[] digits = locationString.Split(',');
            mapUriRequest.Center.Latitude = double.Parse(digits[0].Trim());
            mapUriRequest.Center.Longitude = double.Parse(digits[1].Trim());

            // Set the map style and zoom level
            MapUriOptions mapUriOptions = new MapUriOptions();
            mapUriOptions.Style = MapStyle.AerialWithLabels;
            mapUriOptions.ZoomLevel = 17;

            // Set the size of the requested image in pixels
            mapUriOptions.ImageSize = new ImageryService.SizeOfint();
            mapUriOptions.ImageSize.Height = 480;
            mapUriOptions.ImageSize.Width = 640;

            mapUriRequest.Options = mapUriOptions;

            //Make the request and return the URI
            ImageryServiceClient imageryService = new ImageryServiceClient("BasicHttpBinding_IImageryService");
            MapUriResponse mapUriResponse = imageryService.GetMapUri(mapUriRequest);
            return mapUriResponse.Uri;
        }
        catch (Exception)
        {
            return "Error in processing address";
        }
    }

    [WebMethod]
    public int WordCount(string sentence)
    {
        try
        {
            bool stripTags = false;
            int countedWords = 0;
            if (stripTags == false)
            {
                countedWords = sentence.Split(' ').Length;
            }
            else
            {
                Regex tagMatch = new Regex("<[^>]+>");
                sentence = tagMatch.Replace(sentence, "");
                countedWords = sentence.Split(' ').Length;
            }
            return countedWords;
        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    [WebMethod]
    public string SendEmail(string email)
    {
        try
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("replytoarjunmukherji@gmail.com");
            mail.To.Add(email);
            mail.Subject = "Test Mail";
            mail.Body = "This is a test mail.";
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("replytoarjunmukherji@gmail.com", "anwashadhar");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
            return "Success";
        }
        catch (Exception)
        {
            return "Failed";
        }
    }

    [WebMethod]
    public string GenerateMajorRoutes(string address)
    {
        try
        {
            string locationString = GetCoordFromAddress(address);
            string results = "";
            string key = "AjdBmWL-51nlsoocyGsHAnE3eNOADU1CPWmJ-9kmPL1s7X13jurTlrRx1A49WzwD";
            MajorRoutesRequest majorRoutesRequest = new MajorRoutesRequest();

            // Set the credentials using a valid Bing Maps key
            majorRoutesRequest.Credentials = new RoutingService.Credentials();
            majorRoutesRequest.Credentials.ApplicationId = key;

            // Set the destination of the routes from major roads
            Waypoint endPoint = new Waypoint();
            endPoint.Location = new RoutingService.Location();
            string[] digits = locationString.Split(',');
            endPoint.Location.Latitude = double.Parse(digits[0].Trim());
            endPoint.Location.Longitude = double.Parse(digits[1].Trim());
            endPoint.Description = "Location";

            // Set the option to return full routes with directions
            MajorRoutesOptions options = new MajorRoutesOptions();
            options.ReturnRoutes = true;

            majorRoutesRequest.Destination = endPoint;
            majorRoutesRequest.Options = options;

            // Make the route-from-major-roads request
            RouteServiceClient routeService = new RouteServiceClient("BasicHttpBinding_IRouteService");

            // The result is an MajorRoutesResponse Object
            MajorRoutesResponse majorRoutesResponse = routeService.CalculateRoutesFromMajorRoads(majorRoutesRequest);

            Regex regex = new Regex("<[/a-zA-Z:]*>",
              RegexOptions.IgnoreCase | RegexOptions.Multiline);

            if (majorRoutesResponse.StartingPoints.Length > 0)
            {
                StringBuilder directions = new StringBuilder();

                for (int i = 0; i < majorRoutesResponse.StartingPoints.Length; i++)
                {
                    directions.Append(String.Format("Coming from {1}\n", i + 1,
                        majorRoutesResponse.StartingPoints[i].Description));

                    for (int j = 0;
                      j < majorRoutesResponse.Routes[i].Legs[0].Itinerary.Length; j++)
                    {
                        //Strip tags
                        string step = regex.Replace(
                          majorRoutesResponse.Routes[i].Legs[0].Itinerary[j].Text, string.Empty);
                        directions.Append(String.Format("     {0}. {1}\n", j + 1, step));
                    }
                }

                results = directions.ToString();
            }
            else
                results = "No Routes found";

            return results;
        }
        catch (Exception)
        {
            return "No Routes Found";
        }
    }

    [WebMethod]
    public string GetCurrentTime()
    {
        return String.Format("The current time is {0}.", DateTime.Now.ToString());

    }
}


public class CoordinateInfo
{
    public double X { get; set; }
    public double Y { get; set; }
}