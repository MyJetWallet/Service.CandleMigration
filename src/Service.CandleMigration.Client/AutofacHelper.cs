using Autofac;
using Service.CandleMigration.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.CandleMigration.Client
{
    public static class AutofacHelper
    {
        public static void RegisterCandleMigrationClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new CandleMigrationClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetICandleImporter()).As<ICandleImporter>().SingleInstance();
        }
    }
}
