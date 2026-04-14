# StudyIA — API de estudio asistido por IA (ASP.NET Core 8)

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-WebAPI-5C2D91)
![EF Core](https://img.shields.io/badge/EF%20Core-SqlServer-68217A)
![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?logo=swagger&logoColor=black)
![Status](https://img.shields.io/badge/status-en%20desarrollo-blue)

API REST para transformar **archivos PDF** en recursos de estudio: **resúmenes**, **planes de estudio**, **quizzes** y **audio** con TTS. Integra servicios de IA y almacenamiento en Google Drive. Pensada para facilitar el aprendizaje a partir de material teórico.

> **Stack**: ASP.NET Core 8, Entity Framework Core (SQL Server), Swagger/OpenAPI, Integraciones (Gemini/OpenRouter, ElevenLabs TTS, Google Drive).

---

## Tabla de contenidos
- [Características](#características)
- [Arquitectura](#arquitectura)
- [Estructura del proyecto](#estructura-del-proyecto)
- [Requisitos](#requisitos)
- [Configuración](#configuración)
  - [Cadenas de conexión](#cadenas-de-conexión)
  - [Claves y secretos (User Secrets)](#claves-y-secretos-user-secrets)
  - [Certificado de desarrollo HTTPS](#certificado-de-desarrollo-https)
- [Base de datos & Migraciones](#base-de-datos--migraciones)
- [Ejecución](#ejecución)
- [Endpoints principales](#endpoints-principales)
  - [Subida de PDF](#subida-de-pdf)
  - [Generación de resumen](#generación-de-resumen)
  - [Plan de estudio](#plan-de-estudio)
  - [Quiz/preguntas](#quizpreguntas)
  - [Texto a voz](#texto-a-voz)
  - [Otras consultas](#otras-consultas)
- [Flujo recomendado](#flujo-recomendado)
- [Solución de problemas](#solución-de-problemas)
- [Roadmap](#roadmap)
- [Contribuciones](#contribuciones)
- [Licencia](#licencia)

---

## Características
- 📄 **Subida y almacenamiento** de PDFs en **Google Drive**.
- 🔎 **Extracción de texto** del PDF y **persistencia** en SQL Server.
- 🤖 **IA** para:
  - **Resúmenes** de contenido.
  - **Planes de estudio** con fechas, horas por día y nivel.
  - **Quizzes/preguntas** para autoevaluación.
  - **Evaluación de respuestas** (feedback).
- 🔊 **Texto a Voz (TTS)** con ElevenLabs (opcional).
- 🔍 **Swagger/UI** para explorar y probar endpoints rápidamente.
- 🧱 **Clean-ish architecture**: Controllers → Services → Infra/Data (EF Core).

---

## Arquitectura
```
ASP.NET Core Web API
├── Controllers (p.ej., ArchivoController)
├── Services
│   ├── IA (Gemini / OpenRouter)
│   ├── GoogleDrive (upload/gestión de archivos)
│   └── TTS (ElevenLabs)
├── Data (ApplicationDbContext - EF Core)
└── Entities (Archivo, Resumen, Plan, Quiz, etc.)
```

---

## Estructura del proyecto
```
StudyIA/
├─ Controllers/
│  └─ ArchivoController.cs
├─ Data/
│  └─ ApplicationDbContext.cs
├─ Entities/
├─ Migrations/
├─ Services/
│  ├─ IA/
│  ├─ GoogleDrive/
│  └─ TTS/
├─ Properties/
│  └─ launchSettings.json
├─ appsettings.json
├─ appsettings.Development.json
└─ Program.cs
```

---

## Requisitos
- **.NET SDK 8.0+**
- **SQL Server** (Express/LocalDB/Instancia local)
- **Credenciales de Google Drive** (OAuth/Service Account → archivo JSON)
- **Claves de IA** (Gemini u **OpenRouter**, una de las dos)
- (Opcional) **ElevenLabs** para TTS

---

## Configuración

### Cadenas de conexión
En `appsettings.Development.json` agregue la cadena para EF Core:

```json
{
  "ConnectionStrings": {
    "EFCoreDBConnection": "Server=localhost;Database=StudyIADb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```
> Ajuste `Server=` a su instancia (p.ej., `.\SQLEXPRESS` o `(localdb)\MSSQLLocalDB`).

### Claves y secretos (User Secrets)
Use **User Secrets** para mantener credenciales fuera del repo:

```powershell
cd StudyIA
dotnet user-secrets init

dotnet user-secrets set "ConnectionStrings:EFCoreDBConnection" "Server=localhost;Database=StudyIADb;Trusted_Connection=True;TrustServerCertificate=True"

# IA (use al menos una)
dotnet user-secrets set "GeminiIA:ApiKey" "YOUR_GEMINI_KEY"
dotnet user-secrets set "OpenRouter:ApiKey" "YOUR_OPENROUTER_KEY"

# ElevenLabs (opcional - TTS)
dotnet user-secrets set "ElevenLabs:ApiKey" "YOUR_11LABS_KEY"
dotnet user-secrets set "ElevenLabs:VoiceId" "YOUR_VOICE_ID"

# Google Drive
# Convierta su JSON de credenciales a Base64:
# PowerShell:
# [Convert]::ToBase64String([IO.File]::ReadAllBytes("credentials.json"))
# Bash:
# base64 -w0 credentials.json
dotnet user-secrets set "GoogleDrive:CredentialsBase64" "<BASE64_DEL_JSON>"
dotnet user-secrets set "GoogleDrive:FolderId" "YOUR_FOLDER_ID"
```

> **Importante**: sin `CredentialsBase64` y `FolderId` los endpoints que suben/leen de Drive fallarán.

### Certificado de desarrollo HTTPS
```powershell
dotnet dev-certs https --trust
```

---

## Base de datos & Migraciones
Instale la CLI si no la tiene:
```powershell
dotnet tool install -g dotnet-ef
```

Aplique las migraciones incluidas:
```powershell
dotnet restore
dotnet ef database update
```

---

## Ejecución
Con perfiles de `launchSettings.json`:
```powershell
# HTTPS (recomendado en dev)
dotnet run --launch-profile https

# o HTTP
dotnet run --launch-profile http
```

- Swagger quedará disponible en: `https://localhost:7087/swagger` (ajuste al puerto de su perfil).
- Si un puerto está ocupado, edite `Properties/launchSettings.json`:

```json
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "http://localhost:5030",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:7087;http://localhost:5030",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

---

## Endpoints principales

> Ruta base: `/api/Archivo`

### Subida de PDF
**POST** `/api/Archivo/archivo-upload`  
**Content-Type**: `multipart/form-data` (`file` = PDF)

```bash
curl -X POST "https://localhost:7087/api/Archivo/archivo-upload" ^
  -H "accept: application/json" ^
  -H "Content-Type: multipart/form-data" ^
  -F "file=@C:/ruta/tu-archivo.pdf"
```

**Respuesta (ejemplo):**
```json
{
  "fileId": "00000000-0000-0000-0000-000000000000",
  "fileDrive": "1AbCdEfGhIjK",
  "filetexto": "texto extraído del PDF..."
}
```

### Generación de resumen
**POST** `/api/Archivo/generar-resumen/{fileId}`

```bash
curl -X POST "https://localhost:7087/api/Archivo/generar-resumen/00000000-0000-0000-0000-000000000000" ^
  -H "accept: application/json"
```

### Plan de estudio
**POST** `/api/Archivo/generate-plan-studio-ia`

```bash
curl -X POST "https://localhost:7087/api/Archivo/generate-plan-studio-ia" ^
  -H "Content-Type: application/json" ^
  -d "{     "ArchivoId": "00000000-0000-0000-0000-000000000000",     "FechaInicio": "2025-08-12T00:00:00",     "FechaFin": "2025-08-20T00:00:00",     "HorasPorDia": 2,     "Nivel": "medio",     "TiposDeTarea": ["lectura", "resumen"]   }"
```

### Quiz/preguntas
**POST** `/api/Archivo/generate-quiz/{fileId}?cantidad=10`

```bash
curl -X POST "https://localhost:7087/api/Archivo/generate-quiz/00000000-0000-0000-0000-000000000000?cantidad=10" ^
  -H "accept: application/json"
```

### Texto a voz
**POST** `/api/Archivo/pregunta-voz`

```bash
curl -X POST "https://localhost:7087/api/Archivo/pregunta-voz" ^
  -H "Content-Type: application/json" ^
  -d "{ "texto": "Explicá las capas de la arquitectura del proyecto" }" --output respuesta.mp3
```

### Otras consultas
- **GET** `/api/Archivo/listar`
- **GET** `/api/Archivo/get/{fileId}`
- **GET** `/api/Archivo/resumen-list/{fileId}`
- **POST** `/api/Archivo/descripcion/{fileId}`
- **POST** `/api/Archivo/generar-pregunta`
- **POST** `/api/Archivo/evaluar-feedback` (requiere `PreguntaOriginal` y `RespuestaTranscrita` en el body)

---

## Flujo recomendado
1. **Subir PDF** (`/archivo-upload`) → obtiene `fileId` y `fileDrive`.
2. **Generar resumen** y/o **descripción**.
3. **Crear plan de estudio** según fechas/nivel/horas.
4. **Generar quiz** para autoevaluación.
5. (Opcional) **TTS** para escuchar contenidos generados.

---

## Solución de problemas
- **JSON inválido en `launchSettings.json`**: quite comentarios `/* ... */`, verifique llaves y comas.
- **Puerto en uso**: cambie `applicationUrl` o cierre procesos que ocupen el puerto.
- **DB/Migraciones**: revise `ConnectionStrings:EFCoreDBConnection`, ejecute `dotnet ef database update`.
- **Google Drive**: asegure `CredentialsBase64` válido y `FolderId` correcto.
- **IA/TTS**: verifique que las API keys estén en User Secrets.
- **Cert HTTPS**: ejecute `dotnet dev-certs https --trust`.

---

## Roadmap
- ✅ CRUD base de archivos y resultados.
- ✅ Integración IA (resumen, plan, quiz).
- ✅ Integración Google Drive.
- ✅ Swagger/OpenAPI.
- ⏳ Panel web (frontend) para gestionar archivos/planes/quizzes.
- ⏳ Exportaciones (PDF/CSV) de resultados.
- ⏳ Autenticación/JWT y multiusuario.

---

## Contribuciones
Las contribuciones son bienvenidas. Abra un **issue** o envíe un **pull request** con una descripción clara del cambio.

---


