# NihongoHub Server

> ⚠️ **Demo 阶段**
>
> NihongoHub Server 目前处于 Demo 开发阶段，已经实现了基础的用户认证、日语词汇管理和单词本管理等功能
>
> 项目仍在积极开发中. API、数据模型和配置格式可能发生变化. 当前版本主要用于功能体验和开发测试，暂不建议部署在生产环境或存储重要数据

[English](../../README.md) | [日本語](../ja-JP/README.ja.md)

NihongoHub Server 是 NihongoHub 项目的后端服务，主要负责用户认证、数据存储以及向客户端提供 HTTP API

项目旨在提供一个由用户自行部署和管理的日语词汇管理服务.用户可以运行自己的服务器，并通过兼容的客户端管理日语词汇、单词本及相关数据

NihongoHub 采用客户端与服务端分离的设计。服务端负责数据和业务处理，客户端负责用户交互与具体功能展示. 用户可以根据自己的需求部署服务端，并使用支持 NihongoHub API 的客户端连接

## 项目组成

NihongoHub 目前由服务端和独立客户端组成：

- **NihongoHub Server：** 本仓库，提供后端服务、数据存储、身份认证及 HTTP API
- **NihongoHub App：** 基于 Flutter 开发的 Android / iOS 客户端，负责移动端交互和日语词汇练习等功能

客户端仓库：[egoisticelily/nihongohub-app](https://github.com/egoisticelily/nihongohub-app)

NihongoHub Server 不提供公共托管服务。用户需要自行部署服务端，并在客户端中配置对应的服务器地址和账号信息

## 当前功能

以下是当前 Demo 阶段已经实现或提供基础支持的服务端功能：

- **用户认证** 
- **用户身份校验** 
- **密码管理** 
- **日语词汇管理**
- **词汇查询** 
- **单词本管理**
- **词汇关联管理**
- **服务状态检测**

## 项目尚未实现/仍有缺陷的部分

- **普通用户账号创建功能尚未完成开发**
- **管理员对其他用户的管理功能尚未完成开发**
- **练习相关的服务端功能尚未完成开发**

## 技术栈

- **开发语言：** C#
- **运行框架：** ASP.NET Core Minimal API（.NET 10）
- **数据库：** LiteDB
- **身份认证：** JWT
- **密码处理：** BCrypt
- **日志：** Serilog

## 快速开始

### 环境要求

- .NET 10 SDK

### 获取源代码

```bash
git clone https://github.com/egoisticelily/nihongo-hub.git
```

### 编译项目

进入 `src/` 目录后，使用 .NET CLI 按常规方式编译或发布项目

### 启动服务

使用发布的 Release 包时，可直接运行其中的可执行文件

自行编译时，建议先发布为可执行文件，再运行

首次启动时，服务端会在程序运行目录下创建 `Config` 目录，并在需要时自动生成默认配置文件和数据库

如果数据库中尚不存在用户，服务端会初始化默认管理员账户，并将初始登录凭据保存到：

```text
Config/initial-admin-credentials.txt
```

请妥善保管初始凭据，并在首次登录后修改默认密码。不要将该文件或其中的密码分享给其他人

## 服务端配置

服务端首次启动时会自动创建：

```text
Config/
├── config.json
├── nihongo.db
└── initial-admin-credentials.txt
```

其中：

- `config.json`：服务端配置文件
- `nihongo.db`：默认 LiteDB 数据库文件
- `initial-admin-credentials.txt`：首次创建管理员账户时生成的初始凭据文件

服务器地址、监听端口、HTTPS、数据库文件路径以及 JWT 等配置均由 `config.json` 管理

默认配置使用 HTTP，监听地址为 `0.0.0.0:5000`。该配置适合本地或受信任网络中的初步测试，不应直接作为公网部署配置

如需在公网或其他不受信任的网络中提供服务，请正确配置 HTTPS，并限制服务器的网络访问范围

请妥善保管配置文件，尤其是 JWT 密钥、HTTPS 证书和证书密码。配置文件和数据库文件都不应公开分享

## API

服务端通过 HTTP API 向客户端提供功能

主要接口类别包括：

- 身份认证（Auth）
- 用户相关接口（User）
- 日语词汇管理（Words）
- 单词本管理（Vocabulary Notebooks）
- 服务状态检测（Status）

API 接口文档将单独维护

## 开发状态

NihongoHub Server 仍处于 Demo 开发阶段

目前已经实现基础的服务端初始化、数据库访问、身份认证和词汇数据管理等功能，但仍有部分功能尚未实现，整体测试也不代表已经达到生产环境使用标准

在后续开发中，API、数据结构和配置格式可能发生不兼容变更。升级前建议备份数据库和配置文件

## 问题反馈

如果你发现问题或希望提出功能建议，请通过 GitHub Issues 反馈：

[GitHub Issues](https://github.com/egoisticelily/nihongo-hub/issues)

## 许可证

本项目采用 MIT License。详情请参阅 [LICENSE](../../LICENSE)