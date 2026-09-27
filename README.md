# FrizzQOL Mob Cap

Limits how many stars a creature can have when its level is set. Creatures already in the world keep the stars they have until their level is set again.

This does not change world level, health, or damage on its own. It only caps the star level used when a creature's level is applied.

## Config

`BepInEx/config/com.frizzqol.mobcap.cfg`

| Setting | Default | What it does |
| --- | --- | --- |
| MaxStars | 2 | Most stars a new creature can have once 2 stars are allowed. 2 is the normal Valheim maximum. |
| TwoStarsAfterDay | 0 | 2-star creatures cannot appear before this day. 0 allows them from day 0. Example: 100 blocks 2 stars on days 0 through 99. |

## Multiplayer

Install this on the dedicated server and on every client. Use the same config on each of them.

## Install

Install with r2modman or the Thunderstore Mod Manager.

To install by hand, copy `FrizzQOL.MobCap.dll` into `BepInEx/plugins`.

## Requirements

- Valheim
- [BepInExPack for Valheim](https://thunderstore.io/c/valheim/p/denikson/BepInExPack_Valheim/)

## License

[MIT](LICENSE). You can use, copy, change, and share this mod. Keep the copyright notice with any copy.

## Building

1. Copy `Environment.props.example` to `Environment.props`.
2. Set your Valheim and BepInEx folders in that file.
3. From this folder, run:

```
dotnet build MobCap.sln -c Release
```

The plugin file is `FrizzQOL.MobCap.dll`, under the project `bin\Release\net48` folder.

`Environment.props` stays on your machine. It is listed in `.gitignore`.

## Publishing

This folder is ready to push as its own public repository. Create an empty GitHub repo named `FrizzQOL.MobCap`. Do not add a README, license, or gitignore on GitHub. Those files are already here. Then run:

```
git remote add origin https://github.com/<you>/FrizzQOL.MobCap.git
git push -u origin main
```

Set `website_url` in `Package/manifest.json` to that repository before the Thunderstore upload.

## Thunderstore package

Zip these files from `Package` together with the Release dll:

- `manifest.json`
- `README.md`
- `CHANGELOG.md`
- `LICENSE`
- `icon.png`
- `FrizzQOL.MobCap.dll`
