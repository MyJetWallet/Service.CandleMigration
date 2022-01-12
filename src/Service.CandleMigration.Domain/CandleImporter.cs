using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Newtonsoft.Json;
using SimpleTrading.Abstraction.Candles;
using SimpleTrading.CandlesHistory.AzureStorage;
using SimpleTrading.CandlesHistory.Grpc;
using SimpleTrading.CandlesHistory.Grpc.Contracts;

namespace Service.CandleMigration.Domain
{
    public class CandleImporter
    {
        public static string AzureBidStorageConnString { get; set; }
        public static string AzureAskStorageConnString { get; set; }

        private readonly string _azureBidStorageConnString;
        private readonly string _azureAskStorageConnString;
        private readonly ISimpleTradingCandlesHistoryGrpc _candleGrpc;
        private static HttpClient _http = new();

        public CandleImporter(ISimpleTradingCandlesHistoryGrpc candleGrpc)
        {
            _azureBidStorageConnString = AzureBidStorageConnString;
            _azureAskStorageConnString = AzureAskStorageConnString;
            _candleGrpc = candleGrpc;
        }

        public async Task ImportInstrumentFromBinance(string symbol, string source, int digits, bool isRevert = false,
            int deph = 45000)
        {
            var storage = new CandlesPersistentAzureStorage(
                () => _azureBidStorageConnString,
                () => _azureAskStorageConnString);

            Console.WriteLine();
            Console.WriteLine($"========= Load {symbol} from {source} =======");

            var candle = CandleType.Minute;
            await LoadInterval(candle, source, storage, symbol, digits, isRevert, deph);

            candle = CandleType.Hour;
            await LoadInterval(candle, source, storage, symbol, digits, isRevert, deph);

            candle = CandleType.Day;
            await LoadInterval(candle, source, storage, symbol, digits, isRevert, deph);

            candle = CandleType.Month;
            await LoadInterval(candle, source, storage, symbol, digits, isRevert, deph);

//            var reloadResult = await _candleGrpc.ReloadInstrumentAsync(new ReloadInstrumentContract()
//            {
//                InstrumentId = symbol
//            });
            
            Console.WriteLine($"Instrument {symbol} is imported.");
        }
        
        private static async Task LoadInterval(CandleType candle, string source, CandlesPersistentAzureStorage storage,
            string symbol, int digits, bool isRevert, int deph)
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
            while (data.Any() && count < deph)
            {
                Console.Write($"Read {data.Count} items from Binance ... ");
                
                await storage.BulkSave(symbol, true, digits, candle, data);
                Console.WriteLine($"Save {data.Count} items to bids");
                
                await storage.BulkSave(symbol, false, digits, candle, data);
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