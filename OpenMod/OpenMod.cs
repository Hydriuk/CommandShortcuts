using CommandHotkeys.API;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;


[assembly: PluginMetadata("CommandHotkeys", DisplayName = "CommandHotkeys", Author = "Hydriuk")]

namespace CommandHotkeys.OpenMod
{
    public class Plugin : OpenModUnturnedPlugin
    {
        private readonly IServiceProvider _serviceProvider;

        public Plugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async UniTask OnLoadAsync()
        {
            _serviceProvider.GetRequiredService<IHotkeyManager>();
        }

        protected override async UniTask OnUnloadAsync()
        {
        }
    }
}
