
namespace BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Payment
{
    public class UpdateTicketStatusRequest
    {
        public int Id { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public string Comments { get; set; }
    }
}
