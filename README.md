# Sistema de Gestión de Pacientes y Historias Médicas

Este repositorio contiene un programa de Windows Forms desarrollado en C# utilizando .NET 6 y Visual Studio 2022. El programa utiliza EntityFramework Core junto con SQLite para la creación y administración de pacientes y sus historias médicas.

## Características

El sistema de gestión de pacientes y historias médicas cuenta con las siguientes características:

-   Creación y almacenamiento de pacientes con información personal como nombre, fecha de nacimiento, número de teléfono, obra social, etc.
-   Registro y seguimiento de historias médicas de los pacientes, incluyendo síntomas, diagnósticos, tratamientos y medicamentos recetados.
-   Búsqueda y filtrado de pacientes y historias médicas por diferentes criterios como nombre, edad, género, diagnóstico, etc.
-   Edición y actualización de la información de pacientes y sus historias médicas.
-   Eliminación de pacientes y sus historias médicas de forma segura.

## Requisitos

Para ejecutar el programa de Windows Forms y utilizar el sistema de gestión de pacientes y historias médicas, se requiere:

-   Sistema operativo Windows.
-   [.NET 6 Runtime.](https://dotnet.microsoft.com/es-es/download/dotnet/6.0)
-   [SQLite3.](https://www.sqlite.org/index.html)

## Configuración

Siga los siguientes pasos para configurar el proyecto en su entorno de desarrollo:

1.  Clone este repositorio en su máquina local o descárguelo como archivo ZIP y extráigalo.
2.  Abra el proyecto en Visual Studio 2022.
3.  Restaure los paquetes NuGet necesarios (EntityFrameworkCore, EntityFrameworkCore.SQLite y EntityFrameworkCore.Tools).
4.  Abra el archivo `appsettings.json` y configure la cadena de conexión a la base de datos SQLite. Asegúrese de especificar una ruta válida para el archivo de base de datos. (Default: Carpeta de Documentos del sistema)
5.  Compile el proyecto y verifique que no haya errores.
6.  Ejecute el programa y explore las diferentes funcionalidades del sistema de gestión de pacientes y historias médicas.

## Contribución

Si desea contribuir a este proyecto, siga estos pasos:

1.  Forkee este repositorio.
2.  Cree una rama para su nueva funcionalidad (`git checkout -b feature/nueva-funcionalidad`).
3.  Realice los cambios y realice confirmaciones con mensajes descriptivos.
4.  Envíe un pull request a la rama principal del repositorio.
5.  Espere a que se revise y apruebe su pull request.
6.  ¡Gracias por su contribución!

## Licencia

Este proyecto se distribuye bajo la licencia [MIT](/LICENSE).

## Contacto

Si tiene alguna pregunta o sugerencia relacionada con este proyecto, puede ponerse en contacto conmigo a través de mi [Discord](https://discord.com/users/Gaspipi#2014).
