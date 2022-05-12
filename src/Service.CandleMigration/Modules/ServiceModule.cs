using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Sdk.Grpc;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using Service.AssetsDictionary.Client;
using Service.CandleMigration.Domain;
using Service.CandleMigration.Domain.Models;
using SimpleTrading.CandlesHistory.Grpc;
using SimpleTrading.ServiceBus.Contracts;

namespace Service.CandleMigration.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var factory = new MyGrpcClientFactory("http://192.168.70.6:5961");
            var client = factory.CreateGrpcService<ISimpleTradingCandlesHistoryGrpc>();

            builder.RegisterInstance(client).As<ISimpleTradingCandlesHistoryGrpc>().SingleInstance();
            
            builder
                .RegisterType<CandleImporter>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<CandleImporter>().AsSelf().SingleInstance();

            builder.RegisterType<ImportProcessor>().AsSelf().SingleInstance();


            var noSqlClient = builder.CreateNoSqlClient(Program.Settings.MyNoSqlReaderHostPort, Program.LogFactory);

            builder.RegisterAssetsDictionaryClients(noSqlClient);

            var serviceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.SpotServiceBusHostPort), Program.LogFactory);

            builder.RegisterMyServiceBusPublisher<CandleMigrationServiceBusContract>(serviceBusClient,
                CandleMigrationServiceBusContract.TopicName, true);
        }
    }
}