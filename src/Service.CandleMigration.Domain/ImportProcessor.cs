using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.AssetsDictionary.Client;

namespace Service.CandleMigration.Domain
{
    public class ImportProcessor
    {
        private readonly CandleImporter _importer;
        private readonly ISpotInstrumentDictionaryClient _dictionary;
        private StringBuilder _report = new StringBuilder();
        private object _gate = new object();
        private bool _process = false;
        private Task _task;

        public ImportProcessor(CandleImporter importer, ISpotInstrumentDictionaryClient dictionary)
        {
            _importer = importer;
            _dictionary = dictionary;
        }

        public string StartImport(List<string> instruments = null, int deph = 45000)
        {
            if (instruments == null)
                instruments = _dictionary.GetAllSpotInstruments().Select(e => e.Symbol).ToList();
            
            lock (_gate)
            {
                if (_process)
                    return "Cannot start process. Exist another active process";

                _process = true;

                _report = new StringBuilder();

                _report.AppendLine("Start new import process.");
                _report.AppendLine($"Instruments: {instruments.Aggregate((s, s1) => s + ";" + s1)}");
                _report.AppendLine($"deph: {deph}");
                _report.AppendLine();
            }

            _task = ExecuteImport(instruments, deph);

            return "Import is started";
        }

        private async Task ExecuteImport(List<string> instruments, int deph)
        {
            foreach (var symbol in instruments)
            {
                var instrument = _dictionary.GetAllSpotInstruments().FirstOrDefault(e => e.Symbol == symbol);
                if (instrument == null)
                {
                    lock (_gate) _report.AppendLine($"Cannot find instrument {symbol}");
                    continue;
                }

                if (instrument.ConvertSourceExchange != "Binance")
                {
                    lock (_gate)
                        _report.AppendLine(
                            $"Cannot execute import from {instrument.ConvertSourceExchange}, instrument {symbol}");
                    
                    Console.WriteLine($"Cannot execute import from {instrument.ConvertSourceExchange}, instrument {symbol}");
                    continue;
                }

                if (string.IsNullOrEmpty(instrument.ConvertSourceMarket))
                {
                    lock (_gate) _report.AppendLine($"Cannot execute import instrument {symbol}. ExternalMarket is empty");
                    Console.WriteLine($"Cannot execute import instrument {symbol}. ExternalMarket is empty");
                    continue;
                }

                var market = instrument.ConvertSourceMarket;
                var accuracy = instrument.Accuracy;

                Console.WriteLine($"Start import {symbol} from {market} [acc: {accuracy}; dep: {deph}] ...");
                lock (_gate) _report.AppendLine($"Start import {symbol} from {market} [acc: {accuracy}; dep: {deph}] ...");
                await _importer.ImportInstrumentFromBinance(symbol, market, accuracy, false, deph);
                lock (_gate) _report.AppendLine($"Finish import {symbol} from {market}.");
                Console.WriteLine($"Finish import {symbol} from {market}.");
                lock (_gate) _report.AppendLine();
            }

            lock (_gate)
            {
                _report.AppendLine($"Import is finished");
                _process = false;
            }
        }

        public string Report()
        {
            lock (_gate) return _report.ToString();
        }

        public bool IsActive()
        {
            lock (_gate) return _process;
        }
    }
}