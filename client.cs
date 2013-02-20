using System;
using System.IO;
using System.Net;
using System.Text;

namespace Examples.System.Net
{
    public class WebRequestPostExample
    {
        public static String getResponse(WebResponse response)
        {
            // Get the stream containing content returned by the server.
            
            Stream dataStream = response.GetResponseStream ();

            String responseFromServer = (new StreamReader (dataStream)).ReadToEnd ();
            dataStream.Close ();
            return responseFromServer;
        }

        public static string get (string _var)
        {
            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create ("http://localhost:6502/" + _var);

            // Set the Method property of the request to POST.
            request.Method = "GET";
            request.ContentType = "text/plain";

            WebResponse response = request.GetResponse ();

            Console.WriteLine (((HttpWebResponse)response).StatusDescription);
            String responseFromServer = getResponse(response);

            // Clean up the streams.
            response.Close ();
            return responseFromServer;
        }

        public static string set (string _var, string _val)
        {
            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create ("http://localhost:6502/"+_var);
            // Set the Method property of the request to POST.
            request.Method = "POST";

            byte[] byteArray = Encoding.UTF8.GetBytes (_val);
            request.ContentType = "text/plain";
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream ();
            dataStream.Write (byteArray, 0, byteArray.Length);
            dataStream.Close ();

            WebResponse response = request.GetResponse ();
            String responseFromServer = getResponse(response);
            response.Close ();

            return responseFromServer;
        }


        public static void Main()
        {
            String responseFromServer;

            // Display the content.
            Console.WriteLine ("Trying to get var foo");
            responseFromServer = get("foo");
            Console.WriteLine (responseFromServer);

            Console.WriteLine ("Trying to set foo = bar ");
            responseFromServer = set("foo", "bar");
            Console.WriteLine (responseFromServer);

            // Display the content.
            Console.WriteLine ("Trying to get var foo");
            responseFromServer = get("foo");
            Console.WriteLine (responseFromServer);
        }

    }
}