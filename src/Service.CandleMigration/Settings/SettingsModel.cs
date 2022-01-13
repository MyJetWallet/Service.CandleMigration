using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.CandleMigration.Settings
{
    public class SettingsModel
    {
        [YamlProperty("CandleMigration.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("CandleMigration.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("CandleMigration.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }
        
        [YamlProperty("CandleMigration.AzureBidStorageConnString")]
        public string AzureBidStorageConnString { get; set; }
        
        [YamlProperty("CandleMigration.AzureAskStorageConnString")]
        public string AzureAskStorageConnString { get; set; }
        
        [YamlProperty("CandleMigration.CandlesHistoryGrpc")]
        public string CandlesHistoryGrpc { get; set; }
        
        [YamlProperty("CandleMigration.MyNoSqlReaderHostPort")]
        public string MyNoSqlReaderHostPort { get; set; }
        
        [YamlProperty("CandleMigration.SpotServiceBusHostPort")]
        public string SpotServiceBusHostPort { get; set; }
    }
}
