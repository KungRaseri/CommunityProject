namespace Data.Models.StreamElements.Booties
{
    public class UserRankResponse : StreamElementsResponse
    {
        public string Channel { get; set; }
        public string Username { get; set; }
        public int Points { get; set; }
        public int PointsAllTime { get; set; }
        public int Rank { get; set; }
    }
}