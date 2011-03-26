using System;
using System.Collections;
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
using ZipService;
using System.Xml;
using IpToCountryService;
using net.webservicex.www;


/// <summary>
using System.Web.Services;
using System.Net;
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
        catch (Exception)
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

    //[WebMethod]
    //public string GenerateMajorRoutes(string address)
    //{
    //    try
    //    {
    //        string locationString = GetCoordFromAddress(address);
    //        string results = "";
    //        string key = "AjdBmWL-51nlsoocyGsHAnE3eNOADU1CPWmJ-9kmPL1s7X13jurTlrRx1A49WzwD";
    //        MajorRoutesRequest majorRoutesRequest = new MajorRoutesRequest();

    //        // Set the credentials using a valid Bing Maps key
    //        majorRoutesRequest.Credentials = new RoutingService.Credentials();
    //        majorRoutesRequest.Credentials.ApplicationId = key;

    //        // Set the destination of the routes from major roads
    //        Waypoint endPoint = new Waypoint();
    //        endPoint.Location = new RoutingService.Location();
    //        string[] digits = locationString.Split(',');
    //        endPoint.Location.Latitude = double.Parse(digits[0].Trim());
    //        endPoint.Location.Longitude = double.Parse(digits[1].Trim());
    //        endPoint.Description = "Location";

    //        // Set the option to return full routes with directions
    //        MajorRoutesOptions options = new MajorRoutesOptions();
    //        options.ReturnRoutes = true;

    //        majorRoutesRequest.Destination = endPoint;
    //        majorRoutesRequest.Options = options;

    //        // Make the route-from-major-roads request
    //        RouteServiceClient routeService = new RouteServiceClient("BasicHttpBinding_IRouteService");

    //        // The result is an MajorRoutesResponse Object
    //        MajorRoutesResponse majorRoutesResponse = routeService.CalculateRoutesFromMajorRoads(majorRoutesRequest);

    //        Regex regex = new Regex("<[/a-zA-Z:]*>",
    //          RegexOptions.IgnoreCase | RegexOptions.Multiline);

    //        if (majorRoutesResponse.StartingPoints.Length > 0)
    //        {
    //            StringBuilder directions = new StringBuilder();

    //            for (int i = 0; i < majorRoutesResponse.StartingPoints.Length; i++)
    //            {
    //                directions.Append(String.Format("Coming from {1}\n", i + 1,
    //                    majorRoutesResponse.StartingPoints[i].Description));

    //                for (int j = 0;
    //                  j < majorRoutesResponse.Routes[i].Legs[0].Itinerary.Length; j++)
    //                {
    //                    //Strip tags
    //                    string step = regex.Replace(
    //                      majorRoutesResponse.Routes[i].Legs[0].Itinerary[j].Text, string.Empty);
    //                    directions.Append(String.Format("     {0}. {1}\n", j + 1, step));
    //                }
    //            }

    //            results = directions.ToString();
    //        }
    //        else
    //            results = "No Routes found";

    //        return results;
    //    }
    //    catch (Exception)
    //    {
    //        return "No Routes Found";
    //    }
    //}


    [WebMethod]
    public string GenerateRandomPassword(int size)
    {
        try
        {

            string[] Characters = new string[62];
            char toInsert = 'a';
            int i=0,j=0,num=0;
            for ( i = 0; i < 26; i++)
            {
                Characters[i] = toInsert.ToString();
                toInsert++;
            }
            toInsert = 'A';
            for (j = i; j < i+26; j++)
            {
                Characters[j] = toInsert.ToString();
                toInsert++;
            }
            i=j;
            for (j = i; j < i + 10; j++)
            {
                Characters[j] = num.ToString();
                num++;
            }
            
            Random RandChar = new Random();
            string Password = "";

            for (int z = 0; z < size; z++)
            {
                Password += Characters[RandChar.Next(0, 62)];
            }

            return Password;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }


    [WebMethod]
    public string GetCurrentTime()
    {
        return String.Format("The current time is {0}.", DateTime.Now.ToString());
    }

    [WebMethod]
    public String FactorialAndPrimeNumber(int number)
    {
        int s, k = 1, m, n = 1;
        string result;
        for (s = 2; s < number - 1; s++)
        {
            if ((number % s) == 0)
            {
                k = 0;
                break;
            }
            else
                k = 1;
        }
        if (k == 0)
            result = "It is not a prime number ";
        else
            result = "It is a prime number";
        for (m = number; m > 0; m--)
            n = m * n;
        return result + " , The facorial of the given number is " + n;
    }

    [WebMethod]
    public string ValidateCreditCardNumber(string cardnumber)
    {
        if (cardnumber.Length < 13)
        {
            return "Invalid Card Number";
        }
        else
        {
            int sum = 0;
            int l = cardnumber.Length;
            int offset = l % 2;
            byte[] digits = new System.Text.ASCIIEncoding().GetBytes(cardnumber);

            for (int i = 0; i < l; i++)
            {
                digits[i] -= 48;
                if (((i + offset) % 2) == 0)
                {
                    digits[i] *= 2;
                }
                sum += (digits[i] > 9) ? digits[i] - 9 : digits[i];
            }
            if (Regex.IsMatch(cardnumber, "(^4[0-9]{12}(?:[0-9]{3})?$)")) 
            { 
                return "Visa Card"; 
            }
            else if (Regex.IsMatch(cardnumber, "(^5[1-5][0-9]{14}$)")) 
            { 
                return "Master card"; 
            }
            else if (Regex.IsMatch(cardnumber, "(^3[47][0-9]{13}$)")) 
            { 
                return "American Express"; 
            }
            else if (Regex.IsMatch(cardnumber, "(^3(?:0[0-5]|[68][0-9])[0-9]{11}$)")) 
            { 
                return "Diners Club"; 
            }
            else 
            { 
                return "Unknown Card Number"; 
            }
        }
    }

    [WebMethod]
    public string GetInfoByZip(string zip)
    {
        try
        {
            USZip uszip = new USZip();
            XmlNode node = uszip.GetInfoByZIP(zip);
            string city = node.SelectSingleNode("//CITY").LastChild.Value;
            string state = node.SelectSingleNode("//STATE").LastChild.Value;
            return city + ", " + state;
        }
        catch (Exception)
        {
            return "Error getting information";
        }
    }

    [WebMethod]
    public string ReverseString(string message)
    {
        try
        {
            char[] array = message.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Validate Email Id with a Regular Expression
    /// </summary>
    /// <param name="emailid"></param>
    /// <returns>True/False</returns>
    [WebMethod]
    public string ValidateEmail(string emailid)
    {
        string email_regex = 
			@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
     + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
     + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
     + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        try
        {
            if (!String.IsNullOrEmpty(emailid))
            {
                if (Regex.IsMatch(emailid, email_regex))
                    return "Valid Email Id";
                else
                    return "Invalid Email Id";
            }
            else
                return "Input Error";
        }
        catch
        {
            return "Input Error";
        }
    }

    /// <summary>
    /// Validate Zip code with Regular Expression
    /// </summary>
    /// <param name="zip"></param>
    /// <returns></returns>
    [WebMethod]
    public string ValidateZip(string zip)
    {
        string zip_regex = "(^[0-9]{5}$)|(^[0-9]{5}-[0-9]{4}$)";
        try
        {
            if (!String.IsNullOrEmpty(zip))
            {
                if (Regex.IsMatch(zip, zip_regex))
                    return "Valid US Zip Code";
                else
                    return "Invalid US Zip Code";
            }
            else
                return "Input Error";
        }
        catch
        {
            return "Input Error";
        }
    }

    /// <summary>
    /// Validate US Phone with Regular Expression
    /// </summary>
    /// <param name="phone"></param>
    /// <returns></returns>
    [WebMethod]
    public string ValidatePhone(string phone)
    {
        string phone_regex = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
        try
        {
            if (!String.IsNullOrEmpty(phone))
            {
                if (Regex.IsMatch(phone, phone_regex))
                    return "Valid US Phone Number";
                else
                    return "Invalid US Phone Number";
            }
            else
                return "Input Error";
        }
        catch
        {
            return "Input Error";
        }
    }

    
    [WebMethod]
    public ArrayList GenerateNPrimes(double n)
    {
        ArrayList primes = new ArrayList();
        try
        {
            int temp = 3;

            if (n > 0) primes.Add(2);

            while (primes.Count < n)
            {
                int sqrtN = Convert.ToInt32(System.Math.Sqrt(n));
                bool isPrime = true;

                for (int i = 0; i < primes.Count && Convert.ToInt32(primes[i]) <= sqrtN; ++i)
                {
                    int x = Convert.ToInt32(primes[i]);
                    if (temp % x == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }

                if (isPrime) primes.Add(temp);
                temp += 2;
            }
            return primes;
        }
        catch (Exception)
        {
            primes.Clear();
            primes.Add("Invalid entry");
            return primes;
        }

    }

    [WebMethod]
    public string BMICalculator(string weightInPounds, string htinFeet, string htinInches)
    {
        try
        {
            double bmi;
            int height;
            double wt = Convert.ToDouble(weightInPounds);
            int htF = Convert.ToInt32(htinFeet);
            int htI = Convert.ToInt32(htinInches);

            height = htF * 12 + htI; // Convert to inches
            //BMI formula - Calculation
            bmi = (wt * 703) / (height * height);

            string wtStat;

            if (bmi >= 30)
                wtStat = "obese";
            else if (bmi <= 29.9 && bmi >= 25)
                wtStat = "overweight";
            else if (bmi <= 24.9 && bmi >= 18.5)
                wtStat = "normal";
            else
                wtStat = "underWeight";

            return "Your Body Mass Index is " + bmi +
                " and the status of your weight is " + wtStat;
        }
        catch (Exception)
        {
            return "Invalid entry";
        }
    }

    [WebMethod]
    public string GetCountryFromIP(string ipaddress)
    {
        try
        {
            try
            {
                IPAddress ipa = IPAddress.Parse(ipaddress);
            }
            catch (Exception)
            {
                return "Invalid IP address";
            }
            WSIP2Country wsip = new WSIP2Country();
            string countrycode = wsip.GetCountryCode(ipaddress);
            if (!string.IsNullOrEmpty(countrycode))
            {
                return countrycode;
            }
            return "An error occured";
        }
        catch (Exception)
        {
            return "An error occured";
        }
    }
    [WebMethod]
    public string GetUDDIRegistryInfo(UDDIRegistry UDDIRegister, string businessName, string businessStartsWith)
    {
        try
        {
            UDDIBusinessFinder uddiBusinessFinder = new UDDIBusinessFinder();
            String businessFinderResult = uddiBusinessFinder.FindBusiness(UDDIRegister, businessName, businessStartsWith);
            return businessFinderResult;
        }
        catch (Exception)
        {
            return "An error Occured";
        }
    }

    public int p;

    // yet to test it
    [WebMethod]
    public int[] QuickSortMethod (int n, string commaSeparatedElements)
    {
        string[] strings = commaSeparatedElements.Split(',');
        int[] elements = new int[commaSeparatedElements.Length];

        for (int i=0; i<commaSeparatedElements.Length; i++)
        {
            elements[i] = int.Parse(strings[i]);
        }

        quicksort(elements, 0, n-1);

        return elements;
    }

    void quicksort(int[] elements, int lb, int ub)
    {
        if(lb>=ub)
            return;
        partition(elements, lb, ub);
        quicksort(elements, lb, p - 1);
        quicksort(elements, p+1, ub);
    }

    void partition(int[] elements, int lb, int ub)
    {
        int a, down, temp, up, i;

        a = elements[lb];
        up = ub;
        down = lb;

        while (down < up)
        {
            while ((elements[down] <= a) && (down < ub))
                down++;
            while (elements[up] > a)
                up--;
            if (down < up)
            {
                temp = elements[down];
                elements[down] = elements[up];
                elements[up] = temp;
            }
        }
        elements[lb] = elements[up];
        elements[up] = a;
        p = up;
    }
}


public class CoordinateInfo
{
    public double X { get; set; }
    public double Y { get; set; }
}