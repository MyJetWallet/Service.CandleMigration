using System;
using System.Linq;
using System.Threading.Tasks;
using MyJetWallet.Sdk.Grpc;
using ProtoBuf.Grpc.Client;
using Service.CandleMigration.Client;
using Service.CandleMigration.Domain;
using Service.CandleMigration.Grpc.Models;
using SimpleTrading.Abstraction.Candles;
using SimpleTrading.CandlesHistory.Grpc;
using SimpleTrading.CandlesHistory.Grpc.Contracts;

namespace TestApp
{
    class Program
    {
        private static string _askConnectionString = "DefaultEndpointsProtocol=https;AccountName=newspotuatcandlesask;AccountKey=SMr3hySc53tKY017FC6g0KBUeQxP+9noJFwfSiVoK1LmRO7fBgxO1vWMTwwhzbj3NInWZSNJTPAIYuuW4sm8xg==;EndpointSuffix=core.windows.net";

        private static string _bidConnectionString = "DefaultEndpointsProtocol=https;AccountName=newspotuatcandlesbid;AccountKey=Lxlqr1carF0MXENjIZynYwWvmr/ohbUmAbv0mouBRbway5qdlH5MaNUF0uaSfI0pAVRvVjXm/fCksm3ITXGg4w==;EndpointSuffix=core.windows.net";
        
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();

            
            var factory = new MyGrpcClientFactory("http://192.168.70.6:5961");

            var api = factory.CreateGrpcService<ISimpleTradingCandlesHistoryGrpc>();

            var service = new CandleImporter(api);

            await service.ImportInstrumentFromBinance("SOLUSD", "SOLBUSD", 2, false, 1000);
            
            
            
            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
