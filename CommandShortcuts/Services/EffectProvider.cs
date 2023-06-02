using CommandShortcuts.API;
using Hydriuk.UnturnedModules.Adapters;
using Hydriuk.UnturnedModules.Extensions;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;

namespace CommandShortcuts.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class EffectProvider : IEffectProvider
    {
        private readonly string _validatedEffectGUID;
        private TriggerEffectParameters _validatedEffect;

        private readonly IThreadAdapter _threadAdapter;

        public EffectProvider(IConfigurationAdapter<Configuration> configuration, IThreadAdapter threadAdapter) 
        {
            _validatedEffectGUID = configuration.Configuration.ValidatedEffectGUID;
            _threadAdapter = threadAdapter;

            if (Level.isLoaded)
                InitEffect();
            else
                Level.onPostLevelLoaded += OnLevelLoaded;
        }

        public void Dispose()
        {
            Level.onPostLevelLoaded -= OnLevelLoaded;
        }

        public void SendValidatedEffect(Player player)
        {
            if (_validatedEffectGUID == string.Empty)
                return;

            var effect = _validatedEffect;
            effect.SetRelevantPlayer(player.GetTransportConnection());
            effect.position = player.transform.position;

            _threadAdapter.RunOnMainThread(() => EffectManager.triggerEffect(effect));
        }

        private void OnLevelLoaded(int level) => InitEffect();
        private void InitEffect()
        {
            if (_validatedEffectGUID == string.Empty)
                return;

            _validatedEffect = new TriggerEffectParameters(new Guid(_validatedEffectGUID));
        }
    }
}
