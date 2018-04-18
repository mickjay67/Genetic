using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Xml;

namespace Genetic
{
    public partial class Form1 : Form
    {
        string cityFile = "files\\citiesB.txt";
        string cityFile1 = "files\\citiesB1.txt";
        string cityFile2 = "files\\citiesB2.txt";
        int ct = 0;
        //string xmlOut = "files\\citiesBResp.xml";
        public Form1()
        {
            InitializeComponent();
        }
        // Google Requirements
        private String gmaps_API = "AIzaSyDDzQ01uJT2-ml_FlaE1z79_8VxR4gLvpY";   // This is a private key that each person must acquire from the Google Developers website.
        private String Distance_Matrix_URL = "https://maps.googleapis.com/maps/api/distancematrix/xml?";    // This is the URL that we will make requests to.

        // Step One
        // Capture Input, we need starting locations, as well as potential destinations
        public List<String> myCities = new List<String>();
        public List<String> myCities1 = new List<String>();
        public List<String> myCities2 = new List<String>();

        private void Add_Cities()
        {
            string[] cities = File.ReadAllLines(cityFile);
            string[] cities1 = File.ReadAllLines(cityFile1);
            string[] cities2 = File.ReadAllLines(cityFile2);
            foreach (string city in cities) {
                myCities.Add(city);                
            }
            foreach (string city in cities1)
            {
                myCities1.Add(city);
            }
            foreach (string city in cities2)
            {
                myCities2.Add(city);
            }
            foreach (string a in myCities) { Console.WriteLine(a); }
            foreach (string a in myCities1) { Console.WriteLine(a); }
            foreach (string a in myCities2) { Console.WriteLine(a); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Add_Cities();
            InvokeGoogleAPI();
            MessageBox.Show("Done");
        }







        //***All below here is Distance Matrix API stuff***

        // This is the function that will create the URL, and send for the request to the Google API
        private void InvokeGoogleAPI()
        {
            String request_URL = Distance_Matrix_URL;

            String origins = "origins=";
            String destinations = "destinations=";
            for (int i = 0; i < 6; i++)
            {
                origins = origins + myCities[i].Replace(" ", "+") + "|";  // make origins, replace spaces with + sign
                destinations = destinations + myCities[i].Replace(" ", "+") + "|";  // make destinations, replace spaces with + sign
            }
            request_URL = request_URL + origins + "&" + destinations + "&units=metric&key=" + gmaps_API;
            Send_Request(request_URL);
            origins = "origins=";
            destinations = "destinations=";
            request_URL = Distance_Matrix_URL;

            for (int i = 6; i < myCities.Count; i++)
            {
                origins = origins + myCities[i].Replace(" ", "+") + "|";  // make origins, replace spaces with + sign
                destinations = destinations + myCities[i].Replace(" ", "+") + "|";  // make destinations, replace spaces with + sign
            }
            request_URL = request_URL + origins + "&" + destinations + "&units=metric&key=" + gmaps_API;
            Send_Request(request_URL);
            origins = "origins=";
            destinations = "destinations=";
            request_URL = Distance_Matrix_URL;

            for (int i = 0; i < myCities1.Count; i++)
            {
                origins = origins + myCities1[i].Replace(" ", "+") + "|";  // make origins, replace spaces with + sign
                destinations = destinations + myCities1[i].Replace(" ", "+") + "|";  // make destinations, replace spaces with + sign
            }
            request_URL = request_URL + origins + "&" + destinations + "&units=metric&key=" + gmaps_API;
            Send_Request(request_URL);
            origins = "origins=";
            destinations = "destinations=";
            request_URL = Distance_Matrix_URL;

            for (int i = 0; i < myCities2.Count; i++)
            {
                origins = origins + myCities2[i].Replace(" ", "+") + "|";  // make origins, replace spaces with + sign
                destinations = destinations + myCities2[i].Replace(" ", "+") + "|";  // make destinations, replace spaces with + sign
            }
            request_URL = request_URL + origins + "&" + destinations + "&units=metric&key=" + gmaps_API;
            Send_Request(request_URL);
            origins = "origins=";
            destinations = "destinations=";
            request_URL = Distance_Matrix_URL;

            for (int i = 0; i < myCities.Count; i++)
            {
                origins = origins + myCities[i].Replace(" ", "+") + "|";  // make origins, replace spaces with + sign
                for (int j = 0; j < myCities1.Count; j++) {
                    destinations = destinations + myCities1[j].Replace(" ", "+") + "|";  // make destinations, replace spaces with + sign
                }
                request_URL = request_URL + origins + "&" + destinations + "&units=metric&key=" + gmaps_API;
                Send_Request(request_URL);
                origins = "origins=";
                destinations = "destinations=";
                request_URL = Distance_Matrix_URL;

            }
            for (int i = 0; i < myCities.Count; i++)
            {
                origins = origins + myCities[i].Replace(" ", "+") + "|";  // make origins, replace spaces with + sign
                for (int j = 0; j < myCities2.Count; j++)
                {
                    destinations = destinations + myCities2[j].Replace(" ", "+") + "|";  // make destinations, replace spaces with + sign
                }
                request_URL = request_URL + origins + "&" + destinations + "&units=metric&key=" + gmaps_API;
                Send_Request(request_URL);
                origins = "origins=";
                destinations = "destinations=";
                request_URL = Distance_Matrix_URL;

            }
            for (int i = 0; i < myCities1.Count; i++)
            {
                origins = origins + myCities1[i].Replace(" ", "+") + "|";  // make origins, replace spaces with + sign
                for (int j = 0; j < myCities2.Count; j++)
                {
                    destinations = destinations + myCities2[j].Replace(" ", "+") + "|";  // make destinations, replace spaces with + sign
                }
                request_URL = request_URL + origins + "&" + destinations + "&units=metric&key=" + gmaps_API;
                Send_Request(request_URL);
                origins = "origins=";
                destinations = "destinations=";
                request_URL = Distance_Matrix_URL;

            }

            // string request_URL = Distance_Matrix_URL + origins + "&" + destinations + "&units=metric&key=" + gmaps_API;

            //Send_Request(request_URL);
        }

        // Send Request!
        private void Send_Request(String url)
        {            
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(url);
                var res = (HttpWebResponse)req.GetResponse();
                


                GetResponse(res);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Read in text!
        private void GetResponse(HttpWebResponse response)
        {
            string xmlOut = "files\\citiesBResp"+ct+".xml";
            String responeText = "";
            XmlDocument xdoc = new XmlDocument();
            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                responeText = reader.ReadToEnd();
            }
            xdoc.LoadXml(responeText);
            xdoc.Save(xmlOut);
            ct++;
            //MessageBox.Show(responeText);

            //ParseResponse(responeText);
        }

        
    }
}
