# SF2Converter

Pequeña aplicación de consola multiplataforma escrita en C#/.NET para convertir
ficheros `SF2` a `DSPRESET` utilizando un ejecutable externo llamado `SF22DS`.

## Qué hace

- Busca todos los ficheros `.sf2` de una carpeta y todos sus subdirectorios.
- Para cada `archivo.sf2` ejecuta:

```text
SF22DS "archivo.sf2" "archivo.dspreset"
```

- El `.dspreset` se crea en la misma carpeta que el `.sf2`.
- Si el `.dspreset` ya existe, el archivo se omite.
- Muestra el progreso y un resumen al terminar.
- No necesita ninguna biblioteca externa de terceros.

## Requisito importante: SF22DS

Este proyecto es multiplataforma, pero `SF22DS` es un programa externo.
Para convertir realmente los archivos, debe existir una versión de `SF22DS`
compatible con el sistema operativo utilizado.

Por defecto:

- Windows: `SF22DS.exe`
- Linux/macOS: `SF22DS`

Coloca el conversor en la carpeta raíz que vas a procesar.

## Ejemplo

Supongamos esta estructura:

```text
MiBiblioteca/
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

Ejecutando el programa desde `MiBiblioteca` se producirán:

```text
Pianos/Grand/Piano1.dspreset
Pianos/Grand/Piano2.dspreset
Pianos/Electric/EPiano.dspreset
Strings/Violin.dspreset
```

## Uso

### Opción sencilla

Sitúate en la carpeta que quieres procesar y ejecuta:

```bash
SF2Converter
```

El programa utilizará la carpeta actual como raíz.

También puedes indicar explícitamente la carpeta:

```bash
SF2Converter "/ruta/a/MiBiblioteca"
```

### Indicar otra ubicación para SF22DS

El segundo argumento permite indicar la ruta al conversor:

```bash
SF2Converter "/ruta/a/MiBiblioteca" "/ruta/a/SF22DS.exe"
```

## Compilar con Visual Studio

1. Abre `SF2Converter.sln`.
2. Selecciona `Release`.
3. Compila el proyecto.
4. Ejecuta el programa desde la carpeta que contiene la biblioteca SF2.

El proyecto utiliza `.NET 10`.

## Compilar desde la línea de comandos

Necesitas tener instalado el SDK de .NET 10.

```bash
dotnet build -c Release
```

Para ejecutarlo:

```bash
dotnet run -- "/ruta/a/MiBiblioteca"
```

## Publicar como ejecutable

Ejemplo para Windows x64:

```bash
dotnet publish src/SF2Converter/SF2Converter.csproj   -c Release   -r win-x64   --self-contained true   -p:PublishSingleFile=true
```

Ejemplo para Linux x64:

```bash
dotnet publish src/SF2Converter/SF2Converter.csproj   -c Release   -r linux-x64   --self-contained true   -p:PublishSingleFile=true
```

Ejemplo para macOS Apple Silicon:

```bash
dotnet publish src/SF2Converter/SF2Converter.csproj   -c Release   -r osx-arm64   --self-contained true   -p:PublishSingleFile=true
```

La aplicación C# puede publicarse para esos sistemas, pero `SF22DS` debe existir
también para el sistema operativo correspondiente.

## Comportamiento ante archivos existentes

Por seguridad, si ya existe:

```text
Piano1.dspreset
```

el programa no lo sobrescribe y muestra:

```text
OMITIDO: ya existe el .dspreset
```

## Licencia

Este proyecto está publicado bajo la licencia MIT. Consulta `LICENSE`.

## Nota sobre SF22DS

`SF22DS` es un componente externo y no forma parte de este repositorio.
Su licencia y condiciones de distribución dependen de su autor.

Este proyecto únicamente ejecuta dicho programa y le proporciona dos argumentos:
el fichero SF2 de entrada y el fichero DSPRESET de salida.
