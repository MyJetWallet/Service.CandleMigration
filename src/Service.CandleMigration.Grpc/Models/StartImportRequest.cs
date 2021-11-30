using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.CandleMigration.Grpc.Models
{
    [DataContract]
    public class StartImportRequest
    {
        [DataMember(Order = 1)] public List<string> InstrumentSymbols { get; set; }
        [DataMember(Order = 2)] public int CountCandles { get; set; }
    }
}