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
using System.Xml.Linq;

namespace Genetic
{
    public partial class Form1 : Form
    {

        string cityFile = "files\\citiesB.txt";
        string cityFile1 = "files\\citiesB1.txt";
        string cityFile2 = "files\\citiesB2.txt";
        string cityFile3 = "files\\citiesB3.txt";
        string list = "files\\citiesA.txt";
        string outputFile = "files\\output.csv";
        int ct = 0;
        int mxCt = 63;
        char fileLet = 'B';
        string xmlFile = "files\\citiesAResp.xml";
        int iter = 1000;
        int pop = 100;
        

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
        public List<String> myCities3 = new List<String>();

        private void button1_Click(object sender, EventArgs e)
        {
            //Genetic algo protion
            //GACall();
            
            //XML merge function calls
            MergeFiles();
            
            //Google distance Matrix api functions
            //Add_Cities();
            //InvokeGoogleAPI();
            //MessageBox.Show("Done");
            Application.Exit();
        }

        // *** GA Stuff ***
        private void GACall() {
            
            GARoute bestRoute = GA();
            string output = bestRoute.totMiles + ",";
            for (int i = 0; i < bestRoute.cityList.Length; i++)
            {
                output += bestRoute.cityList[i] + ",";
            }
            output += Environment.NewLine;
            File.AppendAllText(outputFile, output);
            //Console.WriteLine(bestRoute.totMiles);
            for (int i = 0; i < iter; i++)
            {

                GARoute temp = GA();
                if (temp.totMiles < bestRoute.totMiles)
                {
                    bestRoute = temp;
                }
            }
            output = bestRoute.totMiles + ",";
            for (int i = 0; i < bestRoute.cityList.Length; i++)
            {
                output += bestRoute.cityList[i] + ",";
            }
            output += Environment.NewLine;
            File.AppendAllText(outputFile, output);
        }

        private GARoute GA() {
            string[] cities = File.ReadAllLines(list);
            GARoute bestRt = new GARoute();
            string[] best = new string[cities.Length];
            int bestDist = 999999999;
            XDocument matrix = XDocument.Load(xmlFile);
            var origins = matrix.Root.Elements("origin_address"); ;
            var rows = matrix.Root.Elements("row");
            //Console.WriteLine();
            Random rnd = new Random();
            List<GARoute> allRoutes = new List<GARoute>();
            for (int i = 0; i < pop; i++) {
                GARoute temp = new GARoute();
                temp.name = i;
                temp.cityList = cities.OrderBy(x => rnd.Next()).ToArray();
                temp.totMiles = getTotDist(temp.cityList);
                temp = mutate(temp);
                allRoutes.Add(temp);
                //Console.WriteLine(i);
            }
                               
                bestRt.name = 101;
                bestRt.cityList = cities.OrderBy(x => rnd.Next()).ToArray();
                bestRt.totMiles = getTotDist(bestRt.cityList);
                allRoutes.Add(bestRt);
                
            

            List<GARoute> sorted = allRoutes.OrderBy(si => si.totMiles).ToList();
            if (sorted[0].totMiles < bestDist)
            {
                best = sorted[0].cityList;
                bestRt.cityList = sorted[0].cityList;
                bestDist = sorted[0].totMiles;
                bestRt.totMiles = sorted[0].totMiles;
            }
            return bestRt;
        }

        private int getTotDist(string[] cityList) {
            int totDist = 0;
            string[] cities = File.ReadAllLines(list);
            List<int> indList = new List<int>();
            for (int i =0; i< cityList.Length; i++) {
                indList.Add(SearchXML(cityList[i], cities));
            }
            for (int j = 0; j < indList.Count-1; j++)
            {
                
                totDist += getDist(indList[j], indList[j+1]);
                
            }

            return totDist;
        }
            
        private int getDist(int ind1, int ind2) {
            int dist = 0;
            dist = searchDist(ind1,ind2);
            return dist;
        }

        private int searchDist(int first, int second) {
            XDocument matrix = XDocument.Load(xmlFile);
            var origins = matrix.Root.Elements("origin_address"); ;
            var rows = matrix.Root.Elements("row");
            int distances = (int)(matrix.Root.Elements("row").ElementAt(first).Elements("element").Elements("distance").Elements("value").ElementAt(second));
           
            return distances;
        }

        private int SearchXML(string searchSt, string[] sCities) {
            //string[] cities = File.ReadAllLines(list);
            XDocument matrix = XDocument.Load(xmlFile);
            var origins = matrix.Root.Elements("origin_address"); ;
            var rows = matrix.Root.Elements("row");
            int xCt = 0;
            bool nFound = true;
            while (xCt < sCities.Length && nFound) {
                if (sCities[xCt] == searchSt)
                {
                    nFound = false;
                }
                else {
                    xCt++;
                }
            }
            return xCt;

        }

        private GARoute mutate(GARoute rt)
        {
            Random rand = new Random();
            Random rnd = new Random();
            Random ind1R = new Random();
            Random ind2R = new Random();
            int att = rand.Next(1, 101);
            if (att % 10 == 0)
            {
                int ind1 = ind1R.Next(0, 10);
                int ind2 = ind2R.Next(ind1, 10);
                string temp = rt.cityList[ind1];
                rt.cityList[ind2] = rt.cityList[ind1];
                rt.cityList[ind1] = rt.cityList[ind2];
                return rt;
            }
            else if (att % 13 == 0 && att % 2 == 0)
            {
                rt.cityList = rt.cityList.OrderBy(x => rnd.Next()).ToArray();
                return rt;
            }
            else
            {
                return rt;
            }
        }

        class GARoute
        {
            public int name;
            public string[] cityList;
            public int totMiles = 0;

        }

        // *** XML parse and merge stuff ***
        private bool FindCityXML(string city, string file)
        {
            //Overload to take care of the merging
            ct = 0;            
            XDocument matrix = XDocument.Load(file);
            var origins = matrix.Root.Elements("origin_address"); ;
            var rows = matrix.Root.Elements("row");
            int xCt = 0;
            bool nFound = true;
            while (xCt < origins.Count() && nFound)
            {
                if (origins.ElementAt(xCt).ToString() == city)
                {
                    nFound = false;
                }
                else
                {
                    xCt++;
                }
            }
            return !nFound;

        }

        private bool FindDestCityXML(string city, string file)
        {
            //Overload to take care of the merging
            ct = 0;
            XDocument matrix = XDocument.Load(file);
            var origins = matrix.Root.Elements("destination_address"); ;
            var rows = matrix.Root.Elements("row");
            int xCt = 0;
            bool nFound = true;
            while (xCt < origins.Count() && nFound)
            {
                if (origins.ElementAt(xCt).ToString() == city)
                {
                    nFound = false;
                }
                else
                {
                    xCt++;
                }
            }
            return !nFound;

        }

        private int GetXMLInd(string searchSt, string file)
        {
            //string[] cities = File.ReadAllLines(list);
            XDocument matrix = XDocument.Load(file);
            var origins = matrix.Root.Elements("origin_address"); ;
            var rows = matrix.Root.Elements("row");
            int xCt = 0;
            bool nFound = true;
            while (xCt < origins.Count() && nFound)
            {
                if (origins.ElementAt(xCt).ToString() == searchSt)
                {
                    nFound = false;
                }
                else
                {
                    xCt++;
                }
            }
            return xCt;

        }

        private int GetDestXMLInd(string searchSt, string file)
        {
            //string[] cities = File.ReadAllLines(list);
            XDocument matrix = XDocument.Load(file);
            var origins = matrix.Root.Elements("destination_address"); ;
            var rows = matrix.Root.Elements("row");
            int xCt = 0;
            bool nFound = true;
            while (xCt < origins.Count() && nFound)
            {
                if (origins.ElementAt(xCt).ToString() == searchSt)
                {
                    nFound = false;
                }
                else
                {
                    xCt++;
                }
            }
            return xCt;

        }

        private void MergeFiles() {

            string[] cities = File.ReadAllLines(list);
                
            string file1 = "files\\cities" + fileLet + "resp0.xml";
            XDocument f1 = XDocument.Load(file1);
            var o1 = f1.Root.Elements("origin_address");
            var r1 = f1.Root.Elements("row");
            var d1 = f1.Root.Elements("destination_address");

            while (ct < mxCt) {
                
                int ct1 = ct + 1;
                
                string file2 = "files\\cities" + fileLet + "resp" +ct1 + ".xml";
                
                XDocument f2 = XDocument.Load(file2);
                
                var o2 = f2.Root.Elements("origin_address"); 
                var r2 = f2.Root.Elements("row");
                var d2 = f2.Root.Elements("destination_address");
                for (int i = 0; i < o2.Count(); i++) {
                    if (FindCityXML(o2.ElementAt(i).ToString(), file1))
                    {
                        int ind = GetXMLInd(o2.ElementAt(i).ToString(), file1);
                        //create elements to add to Row at ind    
                        //add elements to r1                        
                        foreach (var dist in r2.ElementAt(i).Elements("duration")) {
                            r1.ElementAt(ind).Add(new XElement("element", new XAttribute("distance", new XAttribute(new XAttribute("value", dist.Element("vlaue"))))));
                        }

                    }
                    else {
                        //add o2[i] to o1
                        f1.Elements("DistanceMatrixResponse").ElementAt(0).Add(new XElement("origin_address", o2.ElementAt(i)));
                        
                        //create new elements to add to r1
                        f1.Elements("DistanceMatrixResponse").ElementAt(0).Add(new XElement("row", r2.ElementAt(i)));
                        //add all new elements to r1
                    }

                    

                }
                for (int j = 0; j < d2.Count(); j++) {
                    if (FindDestCityXML(d2.ElementAt(j).ToString(), file1))
                    { }
                    else
                    {
                        f1.Elements("DistanceMatrixResponse").ElementAt(0).Add(new XElement("destination_address", d2.ElementAt(j)));
                    }
                }
                ct++;
                
            }
            //Write new XML file with all cities in it
            o1 = f1.Root.Elements("origin_address");
            r1 = f1.Root.Elements("row");
            d1 = f1.Root.Elements("destination_address");
            XDocument newFile = new XDocument(new XElement("DistanceMatrixResponse"), o1, d1, r1);
            newFile.Save("files\\CitiesBRespMerged.xml");
        }

        //***All below here is Distance Matrix API stuff ***
        private void Add_Cities()
        {
            string[] cities = File.ReadAllLines(cityFile);
            string[] cities1 = File.ReadAllLines(cityFile1);
            string[] cities2 = File.ReadAllLines(cityFile2);
            string[] cities3 = File.ReadAllLines(cityFile3);
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
            foreach (string city in cities3)
            {
                myCities3.Add(city);
            }
            foreach (string a in myCities) { Console.WriteLine(a); }
            foreach (string a in myCities1) { Console.WriteLine(a); }
            foreach (string a in myCities2) { Console.WriteLine(a); }
        }

               

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
            for (int i = 0; i < myCities.Count; i++)
            {
                origins = origins + myCities[i].Replace(" ", "+") + "|";  // make origins, replace spaces with + sign
                for (int j = 0; j < myCities3.Count; j++)
                {
                    destinations = destinations + myCities3[j].Replace(" ", "+") + "|";  // make destinations, replace spaces with + sign
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
            for (int i = 0; i < myCities1.Count; i++)
            {
                origins = origins + myCities1[i].Replace(" ", "+") + "|";  // make origins, replace spaces with + sign
                for (int j = 0; j < myCities3.Count; j++)
                {
                    destinations = destinations + myCities3[j].Replace(" ", "+") + "|";  // make destinations, replace spaces with + sign
                }
                request_URL = request_URL + origins + "&" + destinations + "&units=metric&key=" + gmaps_API;
                Send_Request(request_URL);
                origins = "origins=";
                destinations = "destinations=";
                request_URL = Distance_Matrix_URL;

            }
            for (int i = 0; i < myCities2.Count; i++)
            {
                origins = origins + myCities2[i].Replace(" ", "+") + "|";  // make origins, replace spaces with + sign
                for (int j = 0; j < myCities3.Count; j++)
                {
                    destinations = destinations + myCities3[j].Replace(" ", "+") + "|";  // make destinations, replace spaces with + sign
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
            string xmlOut = "files\\cities"+fileLet+"resp"+ct+".xml";
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
