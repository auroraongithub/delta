<p align="center">
  <img src="https://raw.githubusercontent.com/ppy/osu/master/assets/lazer.png" width="180" alt="osu! logo" />
</p>

# osu!(v2)

community-driven fork of **osu!lazer** with advanced section and hitobject gimmick systems for mapping and gameplay experiments

## what this fork adds

- section gimmicks and hitobject gimmicks with editor UI
- runtime overlays for status/details/input blocking and custom section effects
- expanded hp/count routing controls and safety handling
- fun-mod forcing and tuning controls
- forced flashlight radius controls with gradual behavior options
- whole-mapset apply fixes and quality-of-life editor improvements
- stronger validation and clamping for dangerous/custom values

## project base

- upstream project: https://github.com/ppy/osu
- this repository tracks and extends upstream osu!lazer behavior

## important server note

- **debug builds** connect to osu!dev server
- **release builds** connect to official osu! server

if you are testing unstable gimmick changes, use debug builds first

## quick build (desktop)

from repository root:

```bash
dotnet build osu.Desktop/osu.Desktop.csproj -c Debug
```

publish debug builds:

```bash
dotnet publish osu.Desktop/osu.Desktop.csproj -c Debug -r win-x64 --self-contained false -o ../builds/windows-debug-v2
dotnet publish osu.Desktop/osu.Desktop.csproj -c Debug -r linux-x64 --self-contained false -o ../builds/linux-debug-v2
```

## reporting bugs

please include:

- map file and exact section/hitobject settings used
- expected vs actual behavior
- crash logs/stack trace if available
- whether you used debug or release build

## contributing

community contributions are welcome

- open issue for bug reports/feature proposals
- open pull request with focused changes
- include tests where possible for regression safety

## license

same license as upstream osu!lazer

see `LICENCE`
