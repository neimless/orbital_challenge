using System.Globalization;
using System.IO;
using System.Net;

namespace OrbitalC
{
    public class DataApi
    {
        OrbitalData data;

        public DataApi()
        {
            data = new OrbitalData();
        }

        public OrbitalData GetInitialData()
        {
            var request = WebRequest.Create("https://space-fast-track.herokuapp.com/generate");
            var response = (HttpWebResponse)request.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                string line = "";
                while (line != null)
                {
                    line = sr.ReadLine();
                    if (line != null)
                        this.PopulateSatelliteData(line);
                }
            }

            return this.data;
        }

        private void PopulateSatelliteData(string line)
        {
            if (line.Substring(0, 5) == "#SEED")
            {
                this.data.Seed = line.Substring(7);
            }
            else if (line.Substring(0, 5) == "ROUTE")
            {
                var elements = line.Split(',');
                this.data.StartingPoint = new GroundPoint(double.Parse(elements[1], CultureInfo.InvariantCulture), double.Parse(elements[2], CultureInfo.InvariantCulture));
                this.data.EndingPoint = new GroundPoint(double.Parse(elements[3], CultureInfo.InvariantCulture), double.Parse(elements[4], CultureInfo.InvariantCulture));
            }
            else
            {
                var elements = line.Split(',');
                this.data.AddSatellite(
                    elements[0],
                    double.Parse(elements[1], CultureInfo.InvariantCulture),
                    double.Parse(elements[2], CultureInfo.InvariantCulture),
                    double.Parse(elements[3], CultureInfo.InvariantCulture));
            }
        }
    }
}
