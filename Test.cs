using ControllerExChanges.Entity;
using ControllerExChanges.Enums;
using ControllerExChanges.Interfaces;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControllerExChanges.Controller;
using Microsoft.Extensions.DependencyInjection;
using ControllerExChanges.Connectors.RandomData;

namespace ConsoleTest
{
    internal class Test
    {
        internal Test()
        {
            IServiceCollection services = new ServiceCollection()
                                            .AddSingleton<ILogger, Serilog.Core.Logger>();

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            ILogger log = BuildLogger();

            Controller controller = Controller.GetController(log);

            _connector = controller.CreateConnector(ExchangeType.RandomConnector);

            if (_connector == null) return;

            //ConnectParametrs parametrs = _connector.ConnectParametrs;

            //parametrs.ExchangeType = ExchangeType.RandomConnector;
            //parametrs.Login = "serg-225@mail.ru";
            //parametrs.Password = "=Ss*#]Vt";
            //parametrs.Path = "http://193.161.214.142:8081";

            ((RandomConnector)_connector).SetParams(70000, step: 1);

            _connector.ConnectStatusChangeEvent += _connector_ConnectStatusChangeEvent1;

            _connector.NewTradeEvent += _connector_NewTradeEvent;
            _connector.NewMarketDepthEvent += _connector_NewMarketDepthEvent;
            _connector.SecuritiesChangeEvent += Connector_SecuritiesChangeEvent;
            ConnectStatus state = _connector.Connect().Result;

            Console.WriteLine("Start ConnectStatus = " + state);
        }

        private void _connector_NewMarketDepthEvent(MarketDepth marketDepth)
        {
            if (marketDepth.Bids.Count > 0)
                Console.WriteLine("Bid {0} Ask {1} ", marketDepth.Bids[0].Volume, marketDepth.Asks[0].Volume);
        }

        private void _connector_ConnectStatusChangeEvent1(ConnectStatus status)
        {
            Console.WriteLine("ConnectStatus = " + status);
        }

        IConnector? _connector;



        private async void Connector_SecuritiesChangeEvent(System.Collections.Concurrent.ConcurrentDictionary<string, Security> securities)
        {
            if (securities == null
                || securities.Count == 0) return;

            Console.WriteLine("Securities = " + securities.Count);

            Security? security;

            if (securities.TryGetValue("2678455", out security))
            {
                bool res = await _connector.SubscribeToSecurity(security);

                Console.WriteLine("SubscribeToSecurity " + security.Name + " res = " + res);
            }
        }

        private void _connector_NewTradeEvent(Trade trade)
        {
            Console.WriteLine("Price {0} Volume {1} Operation {2}", trade.Price, trade.Volume, trade.Operation);
        }

        private ILogger BuildLogger()
        {
            if (!Directory.Exists(@"Log"))
            {
                Directory.CreateDirectory(@"Log");
            }

            ILogger log = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.File(new CompactJsonFormatter(),
                                        @"Log\" + DateTime.Now.ToShortDateString() + "_file.log", LevelAlias.Minimum, 104857600L)
                        .CreateLogger();

            return log;
        }
    }




}
