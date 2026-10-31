# NihongoHub

> ⚠️ **Notice: This project is not yet complete!**
>
> This project is still in its early stages of development. There is no release ready for general use yet. Features, APIs, data models, and configuration formats may change in ways that are not compatible with previous versions. Do not use the current code in a production environment.

[简体中文](docs/README.zh-CN.md) | [日本語](docs/README.ja.md)

NihongoHub is a Japanese learning platform for managing and practicing Japanese vocabulary.

This project aims to help Japanese learners remember Japanese words more effectively. When practicing vocabulary on devices such as smartphones, learners may face several difficulties. For example, the system's Japanese input method may suggest words or predict what the user wants to type. Some applications also cannot provide practice based on the actual words that users have learned.

NihongoHub aims to solve these problems and provide a Japanese learning system that users can manage themselves. Users can freely add words to their vocabulary notebooks and organize them into different groups. They can also practice and test their knowledge using selected notebooks.

The frontend will provide a custom Japanese input feature. Users will be able to enter kanji and kana without relying on their system's Japanese input method. This helps prevent input suggestions and predictions from affecting the user's answers.

During vocabulary tests, users must actually type the correct kanji or kana as their answers instead of choosing from multiple options.

We currently plan to provide the features described above. More features may be added in the future.

The project is currently focused on browser-based use. We also plan to develop mobile applications in the future.

## Core Features

- **Custom Japanese input:** Enter kanji and kana without relying on the system's Japanese input method.
- **Vocabulary practice and tests:** Test your knowledge by typing answers. (Score recording is not yet implemented.)
- **Vocabulary management:** Manage Japanese words, including kanji, kana, pitch accent, meanings, and notes.
- **Vocabulary notebooks:** Create notebooks and add custom tags.
- **User management.**

The features listed above describe the project's plans. Not all of them have been implemented yet.

## Technology Stack

- **Backend:** C# / ASP.NET Core Minimal API (.NET 10)
- **Database:** LiteDB
- **Authentication:** JWT
- **Logging:** Serilog
- **Frontend (planned):** HTML, CSS, and JavaScript

## Repository Structure

- `src/` — Backend source code
- `web/` — Frontend source code (planned; this directory does not exist in the repository yet)

## Development

Requires the .NET 10 SDK.

To build the project:

```bash
dotnet build src
```

The project is still under active development and is not yet ready for production use.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.