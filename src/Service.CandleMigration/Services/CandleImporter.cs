using System.Threading.Tasks;
using Service.CandleMigration.Domain;
using Service.CandleMigration.Grpc;
using Service.CandleMigration.Grpc.Models;

namespace Service.CandleMigration.Services
{
    public class CandleImporter: ICandleImporter
    {
        private readonly ImportProcessor _processor;

        public CandleImporter(ImportProcessor processor)
        {
            _processor = processor;
        }


        public async Task<StartImportResponse> StartImportAsync(StartImportRequest request)
        {
            if (request.CountCandles == 0)
                request.CountCandles = 45000;
            
            var result = _processor.StartImport(request.InstrumentSymbols, request.CountCandles);
            return new StartImportResponse()
            {
                Result = result
            };
        }

        public async Task<GetReportResponse> GetReportAsync()
        {
            var resp = new GetReportResponse()
            {
                IsActive = _processor.IsActive(),
                Report = _processor.Report()
            };

            return resp;
        }
    }
}