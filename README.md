# Arabic Poems

A reader for classical and modern Arabic poetry on Windows 10 and 11: about 42,000 poems by 352 poets, from the pre-Islamic era to the poets of the twentieth century. Written in C# as a Universal Windows (UWP) app, with a set of small WPF tools that turned the poem texts into the data file the app reads.

## Features

- Poets grouped by era, from the pre-Islamic era (العصر الجاهلي) on, and modern poets by region and country.
- A short biography of the poet, when the data file has one.
- Poems grouped by meter (البحر), with the meter shown on each poem.
- Search in poem and poet names from the title bar, and full-text search in the poems with a filter by poet and meter; an option ignores the differences between diacritics and hamza forms.
- Favourites, recently viewed poems, copy and share a poem, and the poem font size in the settings.
- The poems are shown in the Amiri font.

## Versions

| Tag | App | Project | Date |
|---|---|---|---|
| `v1.0.13` | Poetry 1.0.13 | `Poetry.sln` | 2016-01-14 |
| `v1.1.0` | Poetry 2 | `Poetry 2.sln` | 2016-07-11 |
| `v2.0.0` | Arabic Poems, rewritten with a navigation pane and pages | `Arabic_Poems.sln` | 2018-01-08 |
| `v2.1.0` | Arabic Poems 1.0.2 | `Arabic Poems.sln` | 2020-02-14 |

Each tag holds that version's app at the repository root.

## Tools

The `Tools` folder holds the WPF tools (.NET Framework 4.5 and 4.5.2) that prepared the data:

| Tool | What it does |
|---|---|
| `Peoms Serializer` | Reads the poem text files exported from the source database (an `age >> poet >> poem` title line before each poem) and saves the list of poets. |
| `Make poem file` | Builds `Poems.dat`, the "Arabic poems encrypted pack v1.0" that every version of the app opens. Its Poem Maker V2 window reads a pack, lists the lines with over-long words so they can be corrected, and saves the result. |
| `Poem Serializer` | The poet, poem and line classes of a planned new file format; its Read Poems button does nothing yet. |
| `Make Poerty 2 File and Index` | Reads a folder of poem text files into poets, poems and lines, builds an index of the words and shows a summary. Meant for a second file format that was never finished. |

## The poems file

The poems are not in this repository: they are other people's texts. They come from the Access database "دواوين الشعر العربي على مر العصور" (Shamela format), whose texts and poet biographies were collected from the Adab website (موقع أدب). `Poems.dat` comes with the app in the Release download.

The meter of each poem was matched offline against the [Ashaar](https://github.com/ARBML/Ashaar) dataset by ARBML, which is for non-commercial use. Only the meter names are stored in `Poems.dat`; no text from that dataset is included.

## Requirements

Windows 10 version 1709 or later, or Windows 11, x64.

## Install

1. Download the `.7z` file of the latest version from [Releases](https://github.com/Mohammad-Diab/Arabic-Poems/releases) and extract it.
2. Double-click `Install.cmd` in the extracted folder and accept the administrator prompt. The script adds the package's certificate (`Arabic-Poems.cer`) to **Trusted People** on the PC, installs the framework packages from the `Dependencies` folder that are missing (.NET Native 2.2 and VCLibs 14.0, x64), then installs the signed app package. Developer Mode is not needed.
3. Start **Arabic Poems** from the Start menu and pick `Poems.dat` from the extracted folder. The app remembers it; another file can be picked in the settings.

The package is signed with a self-signed certificate, which is why the certificate is added to Trusted People first; only the administrator prompt for that step needs elevation, the app is installed for the current user. To remove the app, uninstall it from the Start menu, or right-click `Uninstall.ps1` and pick **Run with PowerShell**: it also removes the certificate when no other app from the same publisher is installed. The installed app does not need the extracted folder, only `Poems.dat`. The older versions on the Releases page install the same way.

## Building

```
git clone https://github.com/Mohammad-Diab/Arabic-Poems.git
```

### The app

Needs Visual Studio with the **Universal Windows Platform development** workload and the Windows SDK 10.0.22621.0. Open `Arabic Poems.sln` and build, or run:

```
MSBuild "Arabic Poems.sln" /t:Restore /p:Configuration=Release /p:Platform=x64
MSBuild "Arabic Poems\Arabic Poems.csproj" /p:Configuration=Release /p:Platform=x64 /p:AppxBundle=Never /p:UapAppxPackageBuildMode=SideloadOnly /p:GenerateAppxPackageOnBuild=true /p:AppxPackageDir=<output>\
```

The package is written to `<output>`; Release builds compile with .NET Native and take a few minutes.

The older versions build the same way after checking out their tag, with these changes for current tools:

- `v2.0.0`: change the `Microsoft.NETCore.UniversalWindowsPlatform` version in `Arabic Poems\Arabic Poems.csproj` from 6.0.4 to 6.2.8, and add `/p:TargetPlatformVersion=10.0.22621.0 /p:AppxPackageSigningEnabled=false` to the build line.
- `v1.0.13` and `v1.1.0`: these use `project.json`, which current NuGet no longer restores. Move its four packages into the `.csproj` as `PackageReference` items (`Microsoft.NETCore.UniversalWindowsPlatform` 6.2.8 and the three `Microsoft.ApplicationInsights` packages 1.0.0), delete `project.json` and `project.lock.json`, and add `/p:TargetPlatformVersion=10.0.22621.0 /p:TargetPlatformMinVersion=10.0.16299.0 /p:AppxPackageSigningEnabled=false` to the restore and build lines.

### The tools

```
MSBuild "Tools\<tool>\<tool>.sln" /p:Configuration=Release
```

The exe is written to `Tools\<tool>\<tool>\bin\Release`. Recent Visual Studio versions don't ship the .NET Framework 4.5 and 4.5.2 reference assemblies; if the build stops with `MSB3644`, extract the [Microsoft.NETFramework.ReferenceAssemblies.net45](https://www.nuget.org/packages/Microsoft.NETFramework.ReferenceAssemblies.net45) package (for Peoms Serializer) or [Microsoft.NETFramework.ReferenceAssemblies.net452](https://www.nuget.org/packages/Microsoft.NETFramework.ReferenceAssemblies.net452) (for the others) and add `/p:TargetFrameworkRootPath=<package>\build`.

## Author

Mohammad Diab

## License

[Apache 2.0](LICENSE). The Amiri font in `Arabic Poems\Assets\Fonts` is under the [SIL Open Font License](Arabic%20Poems/Assets/Fonts/OFL.txt).
