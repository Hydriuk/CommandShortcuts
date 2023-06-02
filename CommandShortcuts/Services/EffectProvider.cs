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
        private static TriggerEffectParameters _validatedEffect;

        private readonly IThreadAdapter _threadAdapter;

        public EffectProvider(IThreadAdapter threadAdapter) 
        {
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
            var effect = _validatedEffect;
            effect.SetRelevantPlayer(player.GetTransportConnection());
            effect.position = player.transform.position;

            _threadAdapter.RunOnMainThread(() => EffectManager.triggerEffect(effect));
        }

        private void OnLevelLoaded(int level) => InitEffect();
        private void InitEffect()
        {
            _validatedEffect = new TriggerEffectParameters(new Guid("bc41e0feaebe4e788a3612811b8722d3"));
        }
    }
}
