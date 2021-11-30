using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.CandleMigration.Grpc;

namespace Service.CandleMigration.Client
{
    [UsedImplicitly]
    public class CandleMigrationClientFactory: MyGrpcClientFactory
    {
        public CandleMigrationClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public ICandleImporter GetICandleImporter() => CreateGrpcService<ICandleImporter>();
    }
}
