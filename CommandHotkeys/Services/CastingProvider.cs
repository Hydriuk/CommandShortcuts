using CommandHotkeys.API;
using CommandHotkeys.Models;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Action = System.Action;

namespace CommandHotkeys.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class CastingProvider : ICastingProvider
    {
        private readonly Dictionary<Player, Dictionary<string, CancellationTokenSource>> _casts = new Dictionary<Player, Dictionary<string, CancellationTokenSource>>();
        private readonly List<Timer> _timers = new List<Timer>();

        public event Action<Player, CommandCandidate>? Casted;

        public CastingProvider() 
        {
            Provider.onEnemyConnected += OnPlayerConnected;
            Provider.onEnemyDisconnected += OnPlayerDisconnected;

            foreach (SteamPlayer sPlayer in Provider.clients)
            {
                OnPlayerConnected(sPlayer);
            }
        }

        public void Dispose()
        {
            Provider.onEnemyConnected -= OnPlayerConnected;
            Provider.onEnemyDisconnected -= OnPlayerDisconnected;

            foreach (var timer in _timers)
            {
                timer.Dispose();
            }
        }

        private void OnPlayerConnected(SteamPlayer sPlayer)
        {
            _casts.Add(sPlayer.player, new Dictionary<string, CancellationTokenSource>());
        }

        private void OnPlayerDisconnected(SteamPlayer sPlayer)
        {
            _casts.Remove(sPlayer.player);
        }

        public bool TryStartCast(Player player, CommandCandidate commandCandidate)
        {
            if (commandCandidate.Command.Casts.Count <= commandCandidate.ValidatingIndex)
                return false;

            HotkeyedCommand currentCommand = commandCandidate.Command;
            double castingTime = currentCommand.Casts[commandCandidate.ValidatingIndex];
            if (castingTime == 0)
                return false;

            // Validation timer
            Timer timer = new Timer(state =>
            {
                Casted?.Invoke(player, commandCandidate);
            }, null, (int)(castingTime * 1000), -1);
            _timers.Add(timer);

            // Cancel method
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(castingTime));
            cancellationTokenSource.Token.Register(() =>
            {
                timer.Dispose();
                _timers.Remove(timer);
                _casts[player].Remove(currentCommand.Name);
            });

            _casts[player].Add(currentCommand.Name, cancellationTokenSource);

            return true;
        }

        public bool IsCasting(Player player, string commandName)
        {
            if (!_casts.TryGetValue(player, out Dictionary<string, CancellationTokenSource> casts))
                return false;

            return casts.ContainsKey(commandName);
        }

        public void AbortCast(Player player, string commandName) 
        {
            if (!_casts.TryGetValue(player, out Dictionary<string, CancellationTokenSource> casts))
                return;

            if (!casts.TryGetValue(commandName, out CancellationTokenSource castCancelToken))
                return;

            castCancelToken.Cancel();
        }
    }
}
