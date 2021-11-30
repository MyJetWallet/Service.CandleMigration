using System.Runtime.Serialization;

namespace Service.CandleMigration.Grpc.Models
{
    [DataContract]
    public class StartImportResponse
    {
        [DataMember(Order = 1)] public string Result { get; set; } 
    }
}