# Reservaciones Hotel

## Resumen
El proyecto es un CRUD de Reservaciones de Hotel con sus Habitaciones y Usuarios, se uso EntityFrameworkc como ORM, MySQL como gestor de base de datos y ASP.NET.
Tambien se utilizo JWT para la gestion de Tokens/Autenticacióna ademas de xUnit y Moq para los Tests.
Se utilizo Serilog para los logs.

## ¿Como usar la aplicacion?
- [Instalación] Corre el comando dotnet restore para instalar los paquetes.
- [Uso] Dotnet run para iniciar la aplicación


  ## Mejoras
  Creo que se pudo haber mejorado la validacion de las entidades pero por el tiempo solo hice la validacion al registrarse. Tambien las exceptions, se pudieron haber creado personalizadas
  para cada tipo de error pero por lo mismo del tiempo solo utilize System.Exception.
