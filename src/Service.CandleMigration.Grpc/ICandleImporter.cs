using System.ServiceModel;
using System.Threading.Tasks;
using Service.CandleMigration.Grpc.Models;

namespace Service.CandleMigration.Grpc
{
    
    
    [ServiceContract]
    public interface ICandleImporter
    {
        [OperationContract]
        Task<StartImportResponse> StartImportAsync(StartImportRequest request);
        
        [OperationContract]
        Task<GetReportResponse> GetReportAsync();
    }
}