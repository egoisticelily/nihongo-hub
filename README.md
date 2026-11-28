# NihongoHub Server

> ⚠️ **Demo Stage**
>
> NihongoHub Server is currently in the demo stage. Basic features such as user login, Japanese word management, and word notebook management have been implemented.
>
> The project is still under active development. APIs, data models, and configuration settings may change. This version is mainly for trying out the features and development testing. It is not recommended for production use or for storing important data.

[简体中文](docs/zh-CN/README.zh-CN.md) | [日本語](docs/ja-JP/README.ja.md)

NihongoHub Server is the backend service of the NihongoHub project. It handles user login, data storage, and HTTP APIs for clients.

The project aims to provide a Japanese vocabulary service that users can run and manage by themselves. Users can run their own server and manage Japanese words, word notebooks, and related data through compatible clients.

NihongoHub uses a separate design for the client and server. The server handles data and related tasks, while the client provides the user interface and features. Users can run their own server and connect to it with a client that supports the NihongoHub API.

## Project Components

NihongoHub currently has a server and a separate client:

- **NihongoHub Server:** This repository. It provides the backend service, data storage, user login, and HTTP APIs.
- **NihongoHub App:** An Android / iOS app built with Flutter. It provides mobile features and Japanese vocabulary practice.

Client repository: [egoisticelily/nihongohub-app](https://github.com/egoisticelily/nihongohub-app)

NihongoHub Server does not provide a public hosted service. Users need to run their own server and set the server address and account information in the client.

## Current Features

The following features have been implemented or have basic support in the current demo:

- **User login**
- **User identity checks**
- **Password management**
- **Japanese word management**
- **Word search**
- **Word notebook management**
- **Word and notebook links**
- **Server status checks**

## Features Not Yet Completed

- **Creating accounts for regular users**
- **Admin features for managing other users**
- **Server features for Japanese vocabulary practice**

## Technology Stack

- **Language:** C#
- **Framework:** ASP.NET Core Minimal API (.NET 10)
- **Database:** LiteDB
- **Authentication:** JWT
- **Password hashing:** BCrypt
- **Logging:** Serilog

## Quick Start

### Requirements

- .NET 10 SDK

### Get the Source Code

```bash
git clone https://github.com/egoisticelily/nihongo-hub.git
cd nihongo-hub
```

### Build the Project

Enter the `src/` directory and use the .NET CLI to build or publish the project as usual.

### Start the Server

When using a Release package, you can run the executable file directly.

When building the project yourself, it is recommended to publish it as an executable file before running it.

On the first start, the server creates a `Config` directory in the application's directory. It also creates the default configuration file and database when needed.

If there are no users in the database, the server creates a default admin account. The initial login details are saved in:

```text
Config/initial-admin-credentials.txt
```

Keep these details safe. Change the default password after your first login. Do not share this file or the password with others.

## Server Configuration

On the first start, the server creates the following files and directory:

```text
Config/
├── config.json
├── nihongo.db
└── initial-admin-credentials.txt
```

- `config.json`: Server configuration file.
- `nihongo.db`: Default LiteDB database file.
- `initial-admin-credentials.txt`: Initial login details. This file is only created when the default admin account is first created.

The server address, listening port, HTTPS settings, database file path, and JWT settings are managed through `config.json`.

The default configuration uses HTTP and listens on `0.0.0.0:5000`. It is suitable for early testing on a local or trusted network, but should not be used directly for public access.

For public access or other untrusted networks, configure HTTPS and limit access to the server.

Keep your configuration files safe, especially the JWT key, HTTPS certificate, and certificate password. Do not share your configuration or database files publicly.

## API

The server provides features to clients through HTTP APIs.

The main API groups are:

- Authentication (Auth)
- User-related APIs (User)
- Japanese word management (Words)
- Word notebook management (Vocabulary Notebooks)
- Server status checks (Status)

Detailed API information will be maintained separately.

## Development Status

NihongoHub Server is still in the demo stage.

Basic server setup, database access, user login, and vocabulary management have been implemented. However, some features are not yet complete, and the project has not been fully tested for production use.

APIs, data structures, and configuration settings may change in future updates. It is recommended to back up your database and configuration files before updating.

## Feedback

To report a problem or suggest a feature, please use GitHub Issues:

[GitHub Issues](https://github.com/egoisticelily/nihongo-hub/issues)

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.