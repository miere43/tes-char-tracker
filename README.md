# TES Character Tracker
Draws your Skyrim/Fallout 4 character movement map using save files. Works only for default worldspace (eg. no Solstheim map), saves inside interiors are ignored.

## Sample output
* [Fallout 4 - 3072x3072px](http://i.imgur.com/3ls2HeQ.jpg). This map is weird, but still a map.
* [Skyrim - 3000x2000px](http://i.imgur.com/3d3I3S8.jpg).

## Troubleshooting
* If you have errors with reading saves, you should run application from administrator or copy saves to folder, where no admin rights required and change ```Fallout4SaveDir``` or ```SkyrimSaveDir``` in ```settings.json``` to this folder.

## Settings
There is no GUI settings, so you have to edit ```settings.json``` in app. directory. TES Character Tracker uses [JSON](https://en.wikipedia.org/wiki/JSON) to handle it's settings.

```json
"Game": "Fallout 4"
```
Game to use, `Fallout 4` or `Skyrim`.

```json
"IgnoreCharacters": [ "My dumb char", "Another one" ]
```
Do not draw characters with these names (string array).

```json
"FirstDrawCircleRadius": 8.0
```
Circle radius of first drawing of character (float)

```json
"DrawCircleRadius": 4.0
```
Default circle radius (float)

```json
"LineSize": 2.0
```
Line size to draw lines between points.

```json
"SkyrimSaveDir": null
```
Skyrim saves directory. Use `null` to use default saves location.

```json
"SkyrimMapFilePath": "Resources/skyrim-map.jpg"
```
Skyrim map to render things at.

```json
"Fallout4SaveDir": null
```
Fallout 4 saves directory. Use `null` to use default saves location.

```json
"Fallout4MapFilePath": "Resources/fallout4-8-map.jpg"
```
Fallout 4 map to render things at. There are several maps in `Resources` folder, you can use them too.

```json
"LegendX": 1,
"LegendY": 1,
"LegendFontSize": 16,
"LegendFontName": "Segoe UI",
"LegendFontStyle": 0
```
Legend settings. `LegendX` and `LegendY` specifies where it will be drawn on map. `LegendFontStyle` is font drawing style, valid values: `0` - regular, `1` - bold, `2` - italic, `4` - underline, `8` - strikeout. Sum values to combine styles.

```json
"DrawColorsStrings": [
  "LightBlue",
  "Red"
]
```
Which colors to use to draw things. Each character has unique color, if these is not enought colors to cover all characters, then gray will be used instead. You can specify values in HEX, as well as using human-readable names (not all will work).

## Thanks
* Skyrim map is taken from [SK-TamrielWordspaceModdersMaps by Worm](http://www.nexusmods.com/skyrim/mods/2251/?). Thanks! Fallout 4 map is extracted by myself.
