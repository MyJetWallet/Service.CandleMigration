using System;
using System.Runtime.Serialization;
using DotNetCoreDecorators;
using Newtonsoft.Json;
using SimpleTrading.Abstraction.Candles;

namespace Service.CandleMigration.Domain.Models
{
    [DataContract]
    public class BinanceCandle: ICandleModel
    {
        [DataMember(Order = 1)]public DateTime DateTime { get; set; }
        [DataMember(Order = 2)]public double Open { get; set; }
        [DataMember(Order = 3)]public double High { get; set; }
        [DataMember(Order = 4)]public double Low { get; set; }
        [DataMember(Order = 5)]public double Close { get; set; }

        public static BinanceCandle Create(string[] data, bool isRevert, int digits)
        {
            if (data.Length < 5)
                throw new Exception($"Cannot parse data:{JsonConvert.SerializeObject(data)} ");

            var ts = long.Parse(data[0]);



            var candle = new BinanceCandle()
            {
                DateTime = ts.UnixTimeToDateTime(),
                Open = double.Parse(data[1]),
                High = double.Parse(data[2]),
                Low = double.Parse(data[3]),
                Close = double.Parse(data[4])
            };

            if (isRevert)
            {
                candle.Open = Math.Round(1 / candle.Open, digits);
                candle.Close = Math.Round(1 / candle.Close, digits);
                var low = Math.Round(1 / candle.High, digits);
                var high = Math.Round(1 / candle.Low, digits);
                candle.High = high;
                candle.Low = low;
            }

            return candle;
        }
    }
}