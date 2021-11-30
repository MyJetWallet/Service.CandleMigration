using System.Runtime.Serialization;

namespace Service.CandleMigration.Grpc.Models
{
    [DataContract]
    public class GetReportResponse
    {
        [DataMember(Order = 1)] public bool IsActive { get; set; }
        [DataMember(Order = 2)] public string Report { get; set; }
    }
}