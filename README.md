# SF2Converter

> [!WARNING]
> This utility uses [SF22DS](https://github.com/DecentSamples/SF22DS), a converter for SF2 sound fonts into Decent Sampler preset developed by **David Hilowitz**
> `SF22DS` is an external component and is not part of this repository. Its license and distribution terms depend on its author.
> You have to put **SF22DS** in the same directory of **SF2toDSConverter**

## What is `SF2toDSConverter`

`SF2toDSConverter` is a small cross-platform console application written in C#/.NET for converting `SF2` files to `DSPRESET` using an external executable named `SF22DS`.

`SF2toDSConverter` is not a converter really, but also an utility to help the conversion when you have a lot of *SF2 sound fonts* in multiple directories and subdirectories, enabling batch conversion of all discovered files in a single run.

## What does this utility do?

- Searches for all `.sf2` files in a folder and all its subdirectories.
- For each `file.sf2`, executes the standard conversion command:

```text
SF22DS "file.sf2" "file.dspreset"
```

- The `.dspreset` file is created in the same folder as the `.sf2` file.
- If the `.dspreset` file already exists, the file is skipped.
- Displays progress and a summary upon completion.
- Does not require any external third-party libraries.

## Use case

Let us assume this structure:

```text
MyLibrary/
├── SF22DS.exe
├── Pianos/
│   ├── Grand/
│   │   ├── Piano1.sf2
│   │   └── Piano2.sf2
│   └── Electric/
│       └── EPiano.sf2
└── Strings/
    └── Violin.sf2
```

Executing `SF2ToDSConverter` from `MyLibrary` will generate:

```text
Pianos/Grand/Piano1.dspreset
Pianos/Grand/Piano2.dspreset
Pianos/Electric/EPiano.dspreset
Strings/Violin.dspreset
```

## How to use

### Easy and quick option

Navigate to the folder you want to process and run:

```bash
SF2ToDSConverter
```

The program will use the current folder as the root.

You can also explicitly specify the folder:

```bash
SF2ToDSConverter "/folder/other/MyLibrary"
```

### How to specify a different location for SF22DS

The second argument allows you to specify the path to the converter:

```bash
SF2ToDSConverter "/folder/other/MyLibrary" "/Converter/SF22DS.exe"
```

## How to compile the project with Visual Studio

> [!Note]
> Only if you want to build the project at your own.
> (In this repository, You will find binaries ready to use it)

1. Open `SF2ToDSConverter.sln`.
2. Select `Release`.
3. Build the project.
4. Execute the program from the folder that contains the SF2 sound fonts.

The project uses `.NET 10`.

## Compile the project from the command line

You need to have the .NET 10 SDK installed.

```bash
dotnet build -c Release
```

## Publish as executable

Sample for Windows x64:

```bash
dotnet publish src/SF2ToDSConverter/SF2ToDSConverter.csproj   -c Release   -r win-x64   --self-contained true   -p:PublishSingleFile=true
```

Sample for Linux x64:

```bash
dotnet publish src/SF2ToDSConverter/SF2ToDSConverter.csproj   -c Release   -r linux-x64   --self-contained true   -p:PublishSingleFile=true
```

Sample for macOS Apple Silicon:

```bash
dotnet publish src/SF2ToDSConverter/SF2ToDSConverter.csproj   -c Release   -r osx-arm64   --self-contained true   -p:PublishSingleFile=true
```

> [!WARNING]
> This application can be executed for all systems, but `SF22DS` must exist, also for the corresponding operating system.

## Behavior regarding existing files

For safety, if it already exists:

```text
Piano1.dspreset
```

The program does not describe or show it:

```text
SKIPPED: the .dspreset already exists.
```

## License

This project is published under the MIT license. See `LICENSE`.
