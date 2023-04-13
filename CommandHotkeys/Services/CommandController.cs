using CommandHotkeys.API;
using CommandHotkeys.Models;
using Hydriuk.Unturned.SharedModules.Adapters;
using Hydriuk.Unturned.SharedModules.PlayerKeyModule.Components;
using Hydriuk.Unturned.SharedModules.PlayerKeyModule.Models;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using UnityEngine;

namespace CommandHotkeys.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class CommandController : ICommandController
    {
        private readonly Dictionary<Player, Timer> _pendingCommands = new Dictionary<Player, Timer>();

        // Configuration
        private readonly float _maxHotkeyDelay;

        private readonly ICommandAdapter _commandAdapter;

        public CommandController(IConfigurationAdapter<Configuration> configuration, ICommandAdapter commandAdapter)
        {
            _commandAdapter = commandAdapter;
            _maxHotkeyDelay = 2f;
        }

        public void PrepareCommand(Player player, HotkeyedCommand command)
        {
            if(_pendingCommands.TryGetValue(player, out Timer timer))
            {
                timer.Dispose();
            }

            timer = new Timer(_maxHotkeyDelay*1000);
            timer.AutoReset = false;
            timer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                _commandAdapter.Execute(player, command.Command);
            };

            timer.Start();
        }
    }
}
