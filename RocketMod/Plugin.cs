using Rocket.Core.Plugins;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandHotkeys.RocketMod
{
    public class Plugin : RocketPlugin<RocketConfiguration>
    {

        protected override void Load()
        {
            if (Level.isLoaded)
            {
                LateLoad();
            }
            else
            {
                Level.onPostLevelLoaded += OnPostLevelLoaded;
            }
        }

        protected override void Unload()
        {
            Level.onPostLevelLoaded -= OnPostLevelLoaded;
        }

        private void OnPostLevelLoaded(int level) => LateLoad();

        private void LateLoad()
        {
            CSteamID groupId = GroupManager.generateUniqueGroupID();

            GroupManager.addGroup(GroupManager.generateUniqueGroupID(), "Team");
        }
    }
}
