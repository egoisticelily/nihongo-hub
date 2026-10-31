# NihongoHub

[简体中文](docs/README.zh-CN.md) | [日本語](docs/README.ja.md)

NihongoHub is a Japanese-learning platform focused on vocabulary management and practice.

The project aims to provide a flexible vocabulary system in which each user's words are managed as independent records and can be reused across multiple vocabulary notebooks. Notebooks can organize vocabulary for different learning purposes, such as JLPT levels, thematic vocabulary, or custom study materials.

The initial development phase focuses on vocabulary notebooks, word management, and quizzes. The architecture is designed to support additional learning features in the future, with a shared HTTP API for the web client and potential mobile applications.

> **Development status:** Early development. No usable release is currently available. APIs, data models, and configuration formats may change incompatibly.

## Core Features

- Vocabulary management, including kanji, kana, pitch accent, meanings, and notes
- Vocabulary notebooks with custom tags
- Reusable words that can be included in multiple notebooks
- Vocabulary quizzes assembled from selected notebooks
- User authentication and administrative features

These features describe the intended scope of the project; they are not all implemented yet.

## Technology Stack

- **Backend:** C# / ASP.NET Core Minimal API (.NET 10)
- **Database:** LiteDB
- **Authentication:** JWT
- **Logging:** Serilog
- **Frontend:** HTML, CSS, and JavaScript

## Repository Structure

- `src/` — Backend source code
- `web/` — Frontend source code (planned; not yet present in the repository)

## Development

Requires the .NET 10 SDK.

```bash
dotnet build src
```

For local development, run `dotnet run --project src --launch-profile http`. The default endpoint is HTTP port 5000; Kestrel reads the actual host, port, and HTTPS settings from `Config/config.json` beside the built application. Update the launch profile URL if you change that port.

Business API routes are not implemented yet; the server currently configures static file handling only.

The project is under active development and is not intended for production use.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
