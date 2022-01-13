using System.Runtime.Serialization;
using SimpleTrading.Abstraction.Candles;

namespace Service.CandleMigration.Domain.Models
{
    [DataContract]
    public class CandleMigrationMessage
    {
        public const string TopicName = "jetwallet-candle-migration";
        
        [DataMember(Order = 1)]public string Symbol { get; set; }
        [DataMember(Order = 2)]public bool IsBid { get; set; }
        [DataMember(Order = 3)]public int Digits { get; set; }
        [DataMember(Order = 4)]public CandleType Candle { get; set; }
        [DataMember(Order = 5)]public BinanceCandle Data { get; set; }
    }
}