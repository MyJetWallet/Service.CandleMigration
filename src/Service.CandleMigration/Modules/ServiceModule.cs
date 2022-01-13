﻿using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Sdk.Grpc;
using MyJetWallet.Sdk.NoSql;
using Service.AssetsDictionary.Client;
using Service.CandleMigration.Domain;
using SimpleTrading.CandlesHistory.Grpc;

namespace Service.CandleMigration.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var factory = new MyGrpcClientFactory("http://192.168.70.6:5961");
            var client = factory.CreateGrpcService<ISimpleTradingCandlesHistoryGrpc>();

            builder.RegisterInstance(client).As<ISimpleTradingCandlesHistoryGrpc>().SingleInstance();


            CandleImporter.AzureAskStorageConnString = Program.Settings.AzureAskStorageConnString;
            CandleImporter.AzureBidStorageConnString = Program.Settings.AzureBidStorageConnString;
            
            builder
                .RegisterType<CandleImporter>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<CandleImporter>().AsSelf().SingleInstance();

            builder.RegisterType<ImportProcessor>().AsSelf().SingleInstance();


            var noSqlClient = builder.CreateNoSqlClient(() => Program.Settings.MyNoSqlReaderHostPort);

            builder.RegisterAssetsDictionaryClients(noSqlClient);
        }
    }
}