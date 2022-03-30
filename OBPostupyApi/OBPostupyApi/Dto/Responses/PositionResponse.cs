namespace OBPostupyApi.Dto.Responses
{
    public class PositionResponse
    {
        public PositionResponse(double lat, double lon)
        {
            Lat = lat;
            Lon = lon;
        }

        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
