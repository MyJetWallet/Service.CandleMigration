using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using MyJetWallet.Sdk.ServiceBus;
using Newtonsoft.Json;
using Service.CandleMigration.Domain.Models;
using SimpleTrading.Abstraction.Candles;
using SimpleTrading.CandlesHistory.Grpc;
using SimpleTrading.ServiceBus.Contracts;
using SimpleTrading.ServiceBus.Models;

namespace Service.CandleMigration.Domain
{
    public class CandleImporter
    {
        private readonly ISimpleTradingCandlesHistoryGrpc _candleGrpc;
        private static HttpClient _http = new();
        private readonly IServiceBusPublisher<CandleMigrationServiceBusContract> _publisher;

        public CandleImporter(ISimpleTradingCandlesHistoryGrpc candleGrpc, IServiceBusPublisher<CandleMigrationServiceBusContract> publisher)
        {
            _candleGrpc = candleGrpc;
            _publisher = publisher;
        }

        public async Task ImportInstrumentFromBinance(string symbol, string source, int digits, bool isRevert = false,
            int deph = 45000)
        {
            Console.WriteLine();
            Console.WriteLine($"========= Load {symbol} from {source} =======");

            var candle = CandleType.Minute;
            await LoadInterval(candle, source, symbol, digits, isRevert, deph);

            candle = CandleType.Hour;
            await LoadInterval(candle, source, symbol, digits, isRevert, deph);

            candle = CandleType.Day;
            await LoadInterval(candle, source, symbol, digits, isRevert, deph);

            candle = CandleType.Month;
            await LoadInterval(candle, source, symbol, digits, isRevert, deph);

//            var reloadResult = await _candleGrpc.ReloadInstrumentAsync(new ReloadInstrumentContract()
//            {
//                InstrumentId = symbol
//            });
            
            Console.WriteLine($"Instrument {symbol} is imported.");
        }
        
        private async Task LoadInterval(CandleType candle, string source, string symbol, int digits, bool isRevert, int depth)
        {
            Console.WriteLine();
            Console.WriteLine($"----- {candle.ToString()} --------");

            var interval = "";
            switch (candle)
            {
                case CandleType.Minute:
                    interval = "1m";
                    break;
                case CandleType.Hour:
                    interval = "1h";
                    break;
                case CandleType.Day:
                    interval = "1d";
                    break;
                case CandleType.Month:
                    interval = "1M";
                    break;
            }

            var data = await GetCandles(source, 1000, interval, 0, isRevert, digits);

            var count = 0;
            while (data.Any() && count < depth)
            {
                Console.Write($"Read {data.Count} items from Binance ... ");

                await _publisher.PublishAsync(data.Select(binanceCandle => new CandleMigrationServiceBusContract()
                {
                    Symbol = symbol,
                    IsBid = true,
                    Digits = digits,
                    Candle = candle,
                    Data = new MigrationCandle
                    {
                        DateTime = binanceCandle.DateTime,
                        Open = binanceCandle.Open,
                        High = binanceCandle.High,
                        Low = binanceCandle.Low,
                        Close = binanceCandle.Close
                    }
                }));
                //await storage.BulkSave(symbol, true, digits, candle, data);
                Console.WriteLine($"Save {data.Count} items to bids");
                
                await _publisher.PublishAsync(data.Select(binanceCandle => new CandleMigrationServiceBusContract
                {
                    Symbol = symbol,
                    IsBid = false,
                    Digits = digits,
                    Candle = candle,
                    Data = new MigrationCandle
                    {
                        DateTime = binanceCandle.DateTime,
                        Open = binanceCandle.Open,
                        High = binanceCandle.High,
                        Low = binanceCandle.Low,
                        Close = binanceCandle.Close
                    }
                }));
                //await storage.BulkSave(symbol, false, digits, candle, data);
                Console.WriteLine($"Save {data.Count} items to asks");

                var lastTime = data.Min(e => e.DateTime).UnixTime();
                count += data.Count;

                data = await GetCandles(source, 1000, interval, lastTime - 1, isRevert, digits);
            }
        }
        
        

        public static async Task<List<BinanceCandle>> GetCandles(string symbol, int limit, string interval,
            long endtime, bool isRevert, int digit)
        {
            var url = "https://api.binance.com/api/v3/klines";

            if (endtime > 0)
                url += $"?symbol={symbol}&limit={limit}&interval={interval}&endTime={endtime}";
            else
                url += $"?symbol={symbol}&limit={limit}&interval={interval}";

            //Console.WriteLine(url);

            var json = await _http.GetStringAsync(url);

            var data = JsonConvert.DeserializeObject<string[][]>(json);

            return data.Select(e => BinanceCandle.Create(e, isRevert, digit)).ToList();



            //https://api.binance.com/api/v3/klines?symbol=BTCUSD&limit=10&interval=1m
            //https://api.binance.com/api/v3/klines?symbol=BTCBUSD&limit=2&interval=1m&endTime=1629381839999

        }
    }
}