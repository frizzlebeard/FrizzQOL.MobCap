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

## Source

https://github.com/frizzlebeard/FrizzQOL.MobCap

## Install

Install with r2modman or the Thunderstore Mod Manager.

To install by hand, copy `FrizzQOL.MobCap.dll` into `BepInEx/plugins`.

## Requirements

- Valheim
- [BepInExPack for Valheim](https://thunderstore.io/c/valheim/p/denikson/BepInExPack_Valheim/)

## License

[MIT License](https://opensource.org/licenses/MIT). You can use, copy, change, and share this mod. The LICENSE file shipped with the package has the full text.
