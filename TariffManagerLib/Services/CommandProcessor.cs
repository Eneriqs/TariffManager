using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TariffManagerLib.Helpers;
using TariffManagerLib.Interfaces;
using TariffManagerLib.Model;
using TariffManagerLib.Parser;

namespace TariffManagerLib
{
    public  class CommandProcessor  : ICommandProcess
    {
        #region Members
        private static CommandProcessor _instance;
        private Dictionary<dynamic, AbstractParserHandler> _registeredCommands;
        private static readonly object _lock = new object();
        private static Serilog.ILogger Log => Serilog.Log.ForContext<CommandProcessor>();
        #endregion
        #region Constructor & Instance

        private CommandProcessor()
        {
            Init();           
        }

        public static CommandProcessor Instance
        {
            get {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CommandProcessor();
                        }
                    }
                }
                return _instance;
            }            
        }
        #endregion
        #region Private Methods
        private void Init() 
        {
            _registeredCommands = new Dictionary<dynamic, AbstractParserHandler>();
            if (Helper.Instance.IsExcelStarted())
            {
                InitCommandFromExel();
            }
            else {
                InitCommandFromURL();
            }
        }
        private void InitCommandFromURL()
        {
            RegisterCommandHandlerServer(CommandName.SeasonCommand, new SeasonHandler());
            RegisterCommandHandlerServer(CommandName.MonthPeriodCommand, new MonthPeriodHandler());
            RegisterCommandHandlerServer(CommandName.TariffLevelComand, new TariffLevelHandler());
            RegisterCommandHandlerServer(CommandName.TimeInfoWorkingDayCommand, new TimeInfoHandler(DayType.WorkingDay));
            RegisterCommandHandlerServer(CommandName.TimeInfoWeekend1Command, new TimeInfoHandler(DayType.Weekend1));
            RegisterCommandHandlerServer(CommandName.TimeInfoWeekend2Command, new TimeInfoHandler(DayType.Weekend2));
            RegisterCommandHandlerServer(CommandName.TariffSmallCommand, new TariffSmallHandler());
            RegisterCommandHandlerServer(CommandName.TariffFullCommand, new TariffFullHandler());
        }

        private void InitCommandFromExel()
        {
            RegisterCommandHandlerServer(Command.Season, new SeasonHandler());
            RegisterCommandHandlerServer(Command.Month, new MonthHandler());
            RegisterCommandHandlerServer(Command.TariffLevel, new TariffLevelHandler());
            RegisterCommandHandlerServer(Command.TimeInfo, new TimeInfoHandler());
            RegisterCommandHandlerServer(Command.TariffSmall, new TariffSmallHandler());
            RegisterCommandHandlerServer(Command.TariffFull, new TariffFullHandler());
            RegisterCommandHandlerServer(Command.StartDate, new StartDateHandler());
        }
        private void RegisterCommandHandlerServer(dynamic commandName, AbstractParserHandler handler)
        {
            _registeredCommands.Add(commandName, handler);
        }
        #endregion
        #region  ICommandProcess
        public void Parse(string commandName, string data, TariffRow tariffInfo)
        {
            var command = Helper.Instance.GetValueFromEnumMember<CommandName>(commandName);
             _registeredCommands.TryGetValue(command, out AbstractParserHandler handler);
            if(handler == null)
            {
                Log.Here().Error($"Command {command} is not exits");
            }
            handler.Parse(data, tariffInfo);
        }

        public dynamic Parse(Command command, string data)
        {
            _registeredCommands.TryGetValue(command, out AbstractParserHandler handler);
            if (handler == null)
            {
                Log.Here().Error($"Command {command} is not exits");
            }
            handler.Parse(data);
            return handler.Result;
        }
        #endregion

    }
}
