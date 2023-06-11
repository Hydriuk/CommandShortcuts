using CommandShortcuts.API;
using CommandShortcuts.Services;
using Hydriuk.RocketModModules.Adapters;
using Hydriuk.UnturnedModules.Adapters;
using Hydriuk.UnturnedModules.PlayerKeys;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandShortcuts.RocketMod
{
    public class Plugin : RocketPlugin<RocketConfiguration>
    {
        public static Plugin Instance { get; private set; }

        private IThreadAdapter _threadAdapter;
        private IPermissionAdapter _permissinoAdapter;
        private IPlayerKeysController _playerKeysController;
        private ICommandAdapter _commandAdapter;
        private ITranslationAdapter _translationAdapter;
        private IEnvironmentAdapter _environmentAdapter;

        private ICastingProvider _castingProvider;
        private ICooldownProvider _cooldownProvider;
        private IEffectProvider _effectProvider;
        private IHotkeyValidator _hotkeyValidator;
        private ICommandController _commandController;
        private IHotkeyController _hotkeyController;

        protected override void Load()
        {
            Instance = this;

            // Parse configuration hotkeys
            foreach (var shortcut in Configuration.Instance.Shortcuts)
                shortcut.Validate();

            _threadAdapter = new ThreadAdapter();
            _permissinoAdapter = new PermissionAdapter();
            _playerKeysController = new PlayerKeysController();
            _commandAdapter = new CommandAdapter();
            _translationAdapter = new TranslationsAdapter(Translations.Instance);
            _environmentAdapter = new EnvironmentAdapter(this);

            _castingProvider = new CastingProvider();
            _cooldownProvider = new CooldownProvider(_environmentAdapter);
            _effectProvider = new EffectProvider(Configuration.Instance, _threadAdapter);
            _hotkeyValidator = new HotkeyValidator(_castingProvider, _effectProvider);
            _commandController = new CommandController(Configuration.Instance, _commandAdapter, _threadAdapter, _cooldownProvider, _translationAdapter);
            _hotkeyController = new HotkeyController(Configuration.Instance, _commandController, _permissinoAdapter, _playerKeysController, _hotkeyValidator);
        }

        protected override void Unload()
        {
            _playerKeysController?.Dispose();
            _castingProvider?.Dispose();
            _cooldownProvider?.Dispose();
            _effectProvider?.Dispose();
            _hotkeyValidator?.Dispose();
            _hotkeyController?.Dispose();
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "CoolingDown", "This hotkey is cooling down : {Seconds} seconds remaining" }
        };
    }
}
