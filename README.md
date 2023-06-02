<link rel="stylesheet" href="C:\Users\Antonin\Documents\_workspace\Unturned\Projects\vscode-hydriuk.css"></link>
<style>
    @media print {
        @page { margin: 0; size: 30cm 100cm; }
        body { margin: 0.2cm; }
    }
</style>

# CommandShortcuts

This plugin allows players to execute commands using a shortcut instead of writing the command in chat. <sub>*Plugin by [Hydriuk](https://github.com/users/hydriuk)*</sub>

# Table of content

- [**Configuration**](#configuration)
  - [**OpenMod example**](#openmod-example)
  - [**RocketMod example**](#rocketmod-example)
  - [**Options description**](#options-description)
    - [**`ValidatedEffectId`**](#validatedeffectid)
    - [**`ChatIcon`**](#chaticon)
    - [**`Shortcuts`**](#shortcuts)
      - [`Permission`](#permission)
      - [`Command`](#command)
      - [`ExecuteAsConsole`](#executeasconsole)
      - [`Cooldown`](#cooldown)
      - [`Hotkeys`](#hotkeys)
      - [`Casts`](#casts)
- [**Translations**](#translations)
  - [**OpenMod example**](#openmod-example-1)
  - [**RocketMod example**](#rocketmod-example-1)
  - [**Translations description**](#translations-description)
- [**Support**](#support)

# **Configuration**

## **OpenMod example**

```yaml
ValidatedEffectGUID: bc41e0feaebe4e788a3612811b8722d3
ChatIcon: https://i.imgur.com/nD9DLyu.png
Shortcuts:
- Permission: cmdsc.heal.default
Command: "/heal {PlayerID}"
ExecuteAsConsole: true
Cooldown: 3600
Hotkeys:
- Sprint, Plugin1
- None
- Plugin2
Casts:
- 2
- 0
- 0.5
```

## **RocketMod example**

```xml
<?xml version="1.0" encoding="utf-8"?>
<RocketConfiguration xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <ValidatedEffectGUID>bc41e0feaebe4e788a3612811b8722d3</ValidatedEffectGUID>
  <ChatIcon>https://i.imgur.com/nD9DLyu.png</ChatIcon>
  <Shortcuts>
    <Shortcut>
      <Permission>cmdsc.healing.healing</Permission>
      <Command>/heal {PlayerID}</Command>
      <Cooldown>3600</Cooldown>
      <ExecuteAsConsole>true</ExecuteAsConsole>
      <Hotkeys>
        <Hotkey>Sprint, Plugin1</Hotkey>
        <Hotkey>None</Hotkey>
        <Hotkey>Plugin2</Hotkey>
      </Hotkeys>
      <Casts>
        <CastingTime>2</CastingTime>
        <CastingTime>0</CastingTime>
        <CastingTime>0.5</CastingTime>
      </Casts>
    </Shortcut>
  </Shortcuts>
</RocketConfiguration>
```

## **Options description**

### **`ValidatedEffectId`**
- **Value**: Number (from 0 to 65,535)
- **Use**: ID of the effect to be played when a hotkey is validated. To disable, set value to `0`

### **`ChatIcon`**
- **Value**: URL
- **Use**: URL of the icon to be displayed with chat messages sent by the plugin. To disable the icon, leave empty

### **`Shortcuts`**

This section defines all shortcuts players can use.

#### `Permission`
- **Value**: Text
- **Use**: Defines the required permission for the player to use the shortcut

#### `Command`
- **Value**: Text
- **Use**: Defines the command that will be executed when using the shortcut.
- **Parameters**: You can write any of the following terms in your command, they will be replaced with specific values :
  - `{PlayerID}` will be replaced by the **Steam id** of the player using the shortcut
  - `{PlayerName}` will be replaced by the **Steam name** of the player using the shortcut
  - `{PlayerCharName}` will be replaced by the **character name** of the player using the shortcut

#### `ExecuteAsConsole`
- **Value**: `true` or `false`
- **Use**: When true, the command will be executed as if it was executed from the server console. Otherwise as if it was by the player.

#### `Cooldown`
- **Value**: Number, in seconds
- **Use**: How long will the player have to wait before using the shortcut again

#### `Hotkeys`
- **Value**: Possible values :
  - `None`: Release all keys
  - `Jump`: Hold Jump
  - ~~`Primary`~~: Thank you Neslon for beaking it
  - ~~`Secondary`~~: Thank you Neslon for breaking it
  - `Crouch`: Being in the crouch position
  - `Prone`: Being in the prone position
  - `Sprint`: Hold the sprint key
  - `LeanLeft`: Hold the lean left key
  - `LeanRight`: Hold the lean right key
  - `ToggleTactical`: Hold/Press  ?   ?   ?   ?   ?   ?   ?   ? the toggle tactical key while holding a weapon which can use this action
  - `HoldBreath`: Hold the hold breath key
  - `Plugin1`: Hold the plugin hotkey 1
  - `Plugin2`: Hold the plugin hotkey 2
  - `Plugin3`: Hold the plugin hotkey 3
  - `Plugin4`: Hold the plugin hotkey 4
  - `Plugin5`: Hold the plugin hotkey 5

- **Use**: This list defines the hotkeys the player has to press to use the shortcut. The player has to press the hotkeys in the order they appear here. Multiple hotkeys can be put on a single line, separated by "`, `". The player will have to press these keys at the same time.
- **Notes**:
    > As most players have their `Sprint` and `HoldBreath` actions bound to the same key, the plugin automatically pair them when loading the configration so the key can be validated.

    > When making a list of hotkeys for your shortcut, you should always make sure they are not conflicting with other shortcuts. For example, if you have a shortcut which is executed by pressing only the `Plugin1` key, you can't have any other shortcut starting with their first key being `Plugin1`. Players would otherwise never be able to use the shortcut. Permissions and casts can allow you to resolve this kind of conflicts.

#### `Casts`
- **Value**: List of Numbers, in seconds
- **Use**: Defines how long the player has to press the hotkeys for them to be validated. The first number of the list applies to the first hotkey, etc... You can leave it empty if you don't want to use casting time.

# **Translations**

## **OpenMod example**

```yaml
CoolingDown: "This shortcut is cooling down: {Seconds} seconds remaining"
```

## **RocketMod example**

```xml
<?xml version="1.0" encoding="utf-8"?>
<Translations xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <Translation Id="CoolingDown" Value="This hotkey is cooling down : {Seconds} seconds remaining" />
</Translations>
```

## **Translations description**

`CoolingDown`
- **Use**: This text is sent to a player when he tries to use a shortcut that did not cooled down yet
- **Parameters**: The following terms will be replaced :
- `{Seconds}` : will be replaced be the number of seconds remaining for the shortcut to cooldown

# **Support**

If you have any issues, questions or suggestions, please create a post or write a ticket on [this discord](https://discord.gg/HcS7fQ5A4F)