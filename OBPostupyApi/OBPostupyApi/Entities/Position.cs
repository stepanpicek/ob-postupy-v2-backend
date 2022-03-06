namespace OBPostupyApi.Entities
{
    public class Position
    {
        public double lat { get; set; }
        public double lon { get; set; }

        public Position(double lat, double lon)
        {
            this.lat = lat;
            this.lon = lon;
        }
    }
}
