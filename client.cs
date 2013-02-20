using System;
using System.IO;
using System.Net;
using System.Text;

namespace Examples.System.Net
{
    public class WebRequestPostExample
    {
        public static string get (string _var)
        {
            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create ("http://localhost:6502/" + _var);
            // Set the Method property of the request to POST.
            request.Method = "GET";
            request.ContentType = "text/plain";
            // Set the ContentLength property of the WebRequest.

            WebResponse response = request.GetResponse ();
            // Display the status.
            Console.WriteLine (((HttpWebResponse)response).StatusDescription);
            
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream ();
            StreamReader reader = new StreamReader (dataStream);
            string responseFromServer = reader.ReadToEnd ();

            
            // Clean up the streams.
            reader.Close ();
            dataStream.Close ();
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
            Console.WriteLine (((HttpWebResponse)response).StatusDescription);
            dataStream = response.GetResponseStream ();
            StreamReader reader = new StreamReader (dataStream);
            string responseFromServer = reader.ReadToEnd ();

            reader.Close ();
            dataStream.Close ();
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