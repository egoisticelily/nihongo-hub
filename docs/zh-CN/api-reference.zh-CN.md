# NihongoHub Server API Reference

> **API 版本：** v1

本文档说明 NihongoHub Server 当前提供的 HTTP API，包括接口地址、请求参数、响应结构、错误代码以及特殊行为。

本文档以 `backup` 分支当前代码为准。API、数据结构和错误处理方式可能在后续版本中发生变化。若文档与实际代码不一致，应以对应版本的服务端实现为准。

## 1. 基本约定

### 1.1 API 地址

所有 API 均使用以下路径前缀：

```text
/api/v1/
```

请求地址由服务器地址和 API 路径组成。

例如：

```text
http://127.0.0.1:5000/api/v1/auth/login
```

服务器地址和端口由服务端配置决定。本文档中的接口地址均不包含服务器域名和端口。

### 1.2 请求方式

目前所有已注册的 API 均使用：

```http
POST
```

请求需要使用 JSON 格式，并设置：

```http
Content-Type: application/json
```

接口参数通过 HTTP 请求体传递，不使用 URL 查询参数。

### 1.3 身份认证

除登录接口外，其余业务接口均需要提供有效的 JWT Token。

当前版本将 Token 放在 JSON 请求体的 `token` 字段中，而不是通过 HTTP Header 中的 `Authorization: Bearer` 传递。

需要身份认证的接口通常使用以下请求结构：

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {}
}
```

其中：

| 字段 | 类型 | 说明 |
|---|---|---|
| `token` | string | JWT Token |
| `data` | object | 当前接口的具体请求数据 |

部分接口不需要 `data`，例如获取单词本列表和服务状态检测。这些接口只需要提供 `token`。

用户 ID 由 Token 中的身份信息确定，客户端不需要自行提供 `userid`。服务端会根据当前用户身份查询和操作对应的数据。

### 1.4 通用响应结构

普通操作接口通常返回：

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

包含数据的接口通常返回：

```json
{
  "success": true,
  "code": 0,
  "message": "OK",
  "data": {}
}
```

字段说明：

| 字段 | 类型 | 说明 |
|---|---|---|
| `success` | boolean | 本次操作是否成功 |
| `code` | integer | NihongoHub API Code |
| `message` | string / null | 操作说明或错误信息 |
| `data` | object / null | 接口返回的数据，具体结构由接口决定 |

错误响应不一定包含 `data` 字段。是否返回该字段取决于对应接口使用的响应结构。

### 1.5 HTTP 状态码与 API Code

NihongoHub 使用两套独立的状态标识：

- HTTP Status Code：表示 HTTP 请求处理结果。
- API Code：表示 NihongoHub 内部定义的具体操作结果。

客户端应优先根据 API Code 判断具体错误，不应只依赖 HTTP 状态码。

当前代码中的主要映射如下：

| HTTP 状态码 | API Code 范围 | 含义 |
|---:|---|---|
| 200 | `0` | 操作成功 |
| 400 | `1000–1004` | 请求错误或参数错误 |
| 401 | `1100–1107` | 身份认证失败 |
| 403 | `1200–1202` | 权限不足 |
| 415 | `1005` | 不支持的请求格式 |
| 500 | `9000–9004` | 服务端内部错误 |

具体错误代码将在本文档后续章节说明。

### 1.6 时间格式

服务端返回的创建时间和更新时间均使用 UTC 时间。

时间采用 ISO 8601 格式，例如：

```text
2026-11-28T12:30:00Z
```

客户端应根据需要转换为本地时间显示。

---

## 2. 身份认证（Auth）

### 2.1 用户登录

**接口地址**

```http
POST /api/v1/auth/login
```

**身份认证：** 不需要。

**请求参数**

登录接口不使用通用 `{ token, data }` 请求结构，直接提交用户名和密码。

```json
{
  "username": "admin",
  "password": "YOUR_PASSWORD"
}
```

| 字段 | 类型 | 必填 | 说明 |
|---|---|---|---|
| `username` | string | 是 | 用户名 |
| `password` | string | 是 | 用户密码 |

**成功响应**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK",
  "data": {
    "token": "YOUR_JWT_TOKEN",
    "userid": 1,
    "usertype": "admin",
    "expiresat": "2026-12-05T12:30:00Z"
  }
}
```

`data` 字段：

| 字段 | 类型 | 说明 |
|---|---|---|
| `token` | string | JWT Token |
| `userid` | integer | 当前用户 ID |
| `usertype` | string | 用户类型，目前包括 `admin` 和 `user` |
| `expiresat` | string | Token 过期时间，UTC |

Token 的有效时间由服务端配置中的 `Jwt.ExpireMinutes` 决定。当前默认配置为 10080 分钟，即 7 天。

**可能的错误情况**

| HTTP | API Code | 情况 |
|---:|---:|---|
| 400 | 1001 | 请求体为空 |
| 400 | 1002 | JSON 格式错误，或请求结构无法正确解析 |
| 400 | 1004 | 用户名或密码为空或仅包含空白字符 |
| 401 | 1101 | 用户名或密码错误，或用户已被禁用 |
| 415 | 1005 | Content-Type 不受支持 |
| 500 | 9001 | 数据库操作失败 |
| 500 | 9002 | 未知内部错误 |
| 500 | 9004 | 登录成功后，用户身份数据出现内部异常 |

用户名不存在、密码错误和用户被禁用，均使用 `1101` 返回，不向客户端区分具体原因。

---

## 3. 用户相关接口（User）

### 3.1 修改当前用户密码

**接口地址**

```http
POST /api/v1/user/password-change
```

**身份认证：** 需要。

**请求参数**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "password": "NEW_PASSWORD"
  }
}
```

| 字段 | 类型 | 必填 | 说明 |
|---|---|---|---|
| `token` | string | 是 | 当前用户的 JWT Token |
| `data.password` | string | 是 | 新密码 |

**成功响应**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**可能的错误情况**

| HTTP | API Code | 情况 |
|---:|---:|---|
| 400 | 1001 | 请求体或 data 为空 |
| 400 | 1002 | JSON 格式错误 |
| 400 | 1004 | 新密码为空或仅包含空白字符 |
| 401 | 1102 | Token 缺失 |
| 401 | 1103 | Token 无效 |
| 401 | 1104 | Token 已过期 |
| 401 | 1105 | Token 尚未生效 |
| 401 | 1106 | Token Claims 无效 |
| 401 | 1107 | 用户身份已失效 |
| 415 | 1005 | Content-Type 不受支持 |
| 500 | 9001 | 数据库操作失败 |
| 500 | 9002 | 未知内部错误 |

**注意事项**

当前版本尚未实现密码修改后的 Token 撤销机制。密码修改成功后，原有 Token 仍可能继续使用，直到其自身失效。

---

## 4. 单词本管理（Vocabulary Notebooks）

### 4.1 获取单词本列表

**接口地址**

```http
POST /api/v1/vocabulary-notebooks/list
```

**身份认证：** 需要。

**请求参数**

本接口不需要 `data`，只提交 Token：

```json
{
  "token": "YOUR_JWT_TOKEN"
}
```

**成功响应**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK",
  "data": {
    "vocabularyNotebookList": [
      {
        "id": -1,
        "name": "default",
        "note": "",
        "tags": [],
        "createdat": "2000-01-01T00:00:00Z",
        "updatedat": "2000-01-01T00:00:00Z"
      },
      {
        "id": 1,
        "name": "JLPT N2",
        "note": "N2 vocabulary",
        "tags": [
          "N2"
        ],
        "createdat": "2026-11-28T12:00:00Z",
        "updatedat": "2026-11-28T12:00:00Z"
      }
    ]
  }
}
```

`vocabularyNotebookList` 是当前用户的单词本列表。

单词本对象包含：

| 字段 | 类型 | 说明 |
|---|---|---|
| `id` | integer | 单词本 ID |
| `name` | string | 单词本名称 |
| `note` | string / null | 备注 |
| `tags` | string[] | 单词本标签 |
| `createdat` | string | 创建时间，UTC |
| `updatedat` | string | 更新时间，UTC |

**默认单词本**

列表中会额外返回一个虚拟的默认单词本：

| 字段 | 值 |
|---|---|
| `id` | `-1` |
| `name` | `default` |
| `note` | 空字符串 |
| `tags` | 空数组 |
| `createdat` | `2000-01-01T00:00:00Z` |
| `updatedat` | `2000-01-01T00:00:00Z` |

该默认单词本由服务端生成，不是数据库中实际保存的单词本记录。

在查询单词时，可以使用 `id = -1` 表示查询当前用户的全部单词。默认单词本不能通过普通单词本管理接口进行修改或删除。

**可能的错误情况**

| HTTP | API Code | 情况 |
|---:|---:|---|
| 401 | 1102–1107 | Token 或用户身份校验失败 |
| 415 | 1005 | Content-Type 不受支持 |
| 500 | 9001 | 数据库操作失败 |
| 500 | 9002 | 未知内部错误 |

### 4.2 根据标签查询单词本

**接口地址**

```http
POST /api/v1/vocabulary-notebooks/search-by-tag
```

**身份认证：** 需要。

**请求参数**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "tag": "N2"
  }
}
```

| 字段 | 类型 | 必填 | 说明 |
|---|---|---|---|
| `data.tag` | string | 是 | 要查询的标签 |

**成功响应**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK",
  "data": {
    "vocabularyNotebookList": [
      {
        "id": 1,
        "name": "JLPT N2",
        "note": "N2 vocabulary",
        "tags": [
          "N2"
        ],
        "createdat": "2026-11-28T12:00:00Z",
        "updatedat": "2026-11-28T12:00:00Z"
      }
    ]
  }
}
```

查询使用标签的完整匹配，不是模糊搜索。当前实现不会返回虚拟默认单词本。

没有匹配结果时，仍然返回成功，列表为空：

```json
{
  "success": true,
  "code": 0,
  "message": "OK",
  "data": {
    "vocabularyNotebookList": []
  }
}
```

**可能的错误情况**

| HTTP | API Code | 情况 |
|---:|---:|---|
| 400 | 1001 | data 为空 |
| 400 | 1002 | JSON 格式错误 |
| 401 | 1102–1107 | Token 或用户身份校验失败 |
| 415 | 1005 | Content-Type 不受支持 |
| 500 | 9001 | 数据库操作失败 |
| 500 | 9002 | 未知内部错误 |

### 4.3 创建单词本

**接口地址**

```http
POST /api/v1/vocabulary-notebooks/create
```

**身份认证：** 需要。

**请求参数**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "name": "JLPT N2",
    "note": "N2 vocabulary",
    "tags": [
      "N2",
      "JLPT"
    ]
  }
}
```

| 字段 | 类型 | 必填 | 说明 |
|---|---|---|---|
| `data.name` | string | 是 | 单词本名称 |
| `data.note` | string / null | 否 | 备注 |
| `data.tags` | string[] | 是 | 标签列表 |

创建成功后，单词本由服务端自动生成 ID、用户归属和时间信息。新建单词本的词汇列表为空。

**成功响应**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**可能的错误情况**

| HTTP | API Code | 情况 |
|---:|---:|---|
| 400 | 1001 | data 为空 |
| 400 | 1002 | JSON 格式错误或必需字段无法解析 |
| 401 | 1102–1107 | Token 或用户身份校验失败 |
| 415 | 1005 | Content-Type 不受支持 |
| 500 | 9001 | 数据库操作失败 |
| 500 | 9002 | 未知内部错误 |

当前代码没有对单词本名称和标签内容进行完整的业务校验。字段存在不代表内容一定有效，客户端应避免提交空名称或无意义标签。

### 4.4 修改单词本

**接口地址**

```http
POST /api/v1/vocabulary-notebooks/update
```

**身份认证：** 需要。

**请求参数**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "id": 1,
    "name": "JLPT N2 Words",
    "note": "Updated note",
    "tags": [
      "N2",
      "JLPT"
    ]
  }
}
```

| 字段 | 类型 | 必填 | 说明 |
|---|---|---|---|
| `data.id` | integer | 是 | 要修改的单词本 ID |
| `data.name` | string | 是 | 新名称 |
| `data.note` | string / null | 否 | 新备注 |
| `data.tags` | string[] | 是 | 新标签列表 |

**成功响应**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**更新规则**

- 只能修改当前用户自己的单词本。
- `name`、`note` 和 `tags` 会根据请求数据更新。
- 已有的词汇关联列表不会因本次更新而被替换。
- 原始创建时间会被保留。
- 更新时间由服务端自动更新。

本接口不是部分字段更新。客户端应提交完整的必需字段。

**可能的错误情况**

| HTTP | API Code | 情况 |
|---:|---:|---|
| 400 | 1001 | data 为空 |
| 400 | 1002 | JSON 格式错误 |
| 401 | 1102–1107 | Token 或用户身份校验失败 |
| 415 | 1005 | Content-Type 不受支持 |
| 500 | 9001 | 数据库操作失败 |
| 500 | 9002 | 单词本不存在、无权操作或其他内部错误 |

当前实现将部分单词本不存在或不属于当前用户的情况映射为 `9002`，而不是独立的资源不存在或权限错误代码。

### 4.5 删除单词本

**接口地址**

```http
POST /api/v1/vocabulary-notebooks/delete
```

**身份认证：** 需要。

**请求参数**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "id": 1
  }
}
```

| 字段 | 类型 | 必填 | 说明 |
|---|---|---|---|
| `data.id` | integer | 是 | 要删除的单词本 ID |

**成功响应**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**删除规则**

删除单词本不会删除其中的单词。单词数据独立保存，删除单词本只会删除单词本记录及其关联列表。

**可能的错误情况**

| HTTP | API Code | 情况 |
|---:|---:|---|
| 400 | 1001 | data 为空 |
| 400 | 1002 | JSON 格式错误 |
| 401 | 1102–1107 | Token 或用户身份校验失败 |
| 415 | 1005 | Content-Type 不受支持 |
| 500 | 9001 | 数据库操作失败 |
| 500 | 9002 | 单词本不存在、无权操作或其他内部错误 |

### 4.6 批量删除单词本

**接口地址**

```http
POST /api/v1/vocabulary-notebooks/delete-batch
```

**身份认证：** 需要。

**请求参数**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "notebooks": [
      {
        "id": 1
      },
      {
        "id": 2
      },
      {
        "id": 3
      }
    ]
  }
}
```

| 字段 | 类型 | 必填 | 说明 |
|---|---|---|---|
| `data.notebooks` | object[] | 是 | 要删除的单词本列表 |
| `data.notebooks[].id` | integer | 是 | 单词本 ID |

**全部成功**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK",
  "data": null
}
```

**部分或全部失败**

HTTP Status Code：`500`

API Code：`9000`

```json
{
  "success": false,
  "code": 9000,
  "message": "一共有:1 个错误",
  "data": {
    "responseDataList": [
      {
        "id": 3,
        "errormessage": "该单词本不存在",
        "errorcategory": "None"
      }
    ]
  }
}
```

失败记录包含：

| 字段 | 类型 | 说明 |
|---|---|---|
| `id` | integer / null | 操作失败的单词本 ID |
| `errormessage` | string | 失败原因 |
| `errorcategory` | string / null | 错误类别 |

`errorcategory` 目前可能包括 `None`、`Database`、`Unknown`。

**可能的错误情况**

| HTTP | API Code | 情况 |
|---:|---:|---|
| 400 | 1001 | data 为空 |
| 400 | 1002 | JSON 格式错误 |
| 401 | 1102–1107 | Token 或用户身份校验失败 |
| 415 | 1005 | Content-Type 不受支持 |
| 500 | 9000 | 批量操作中至少有一个项目失败 |
| 500 | 9001 / 9002 | 请求处理过程中的数据库或其他内部错误 |

**注意事项**

批量操作不是一个整体事务。服务端会逐项执行删除，某些项目成功而其他项目失败时，已经成功的删除不会自动回滚。

当收到 `9000` 时，客户端应读取 `responseDataList`，并重新查询单词本列表，以确认最终状态。

### 4.7 将单词添加到单词本

**接口地址**

```http
POST /api/v1/vocabulary-notebooks/addword-in-notebook
```

**身份认证：** 需要。

**请求参数**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "wordid": 100,
    "vocabularynotebookid": 1
  }
}
```

| 字段 | 类型 | 必填 | 说明 |
|---|---|---|---|
| `data.wordid` | integer | 是 | 单词 ID |
| `data.vocabularynotebookid` | integer | 是 | 目标单词本 ID |

**成功响应**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**添加规则**

- 单词必须存在，且属于当前用户。
- 单词本必须存在，且属于当前用户。
- 同一个单词不能重复添加到同一个单词本。
- 添加成功后，单词本的更新时间会被更新。

**可能的错误情况**

| HTTP | API Code | 情况 |
|---:|---:|---|
| 400 | 1001 | data 为空 |
| 400 | 1002 | JSON 格式错误 |
| 400 | 1004 | 单词或单词本不存在、数据不属于当前用户，或单词已经存在于该单词本 |
| 401 | 1102–1107 | Token 或用户身份校验失败 |
| 415 | 1005 | Content-Type 不受支持 |
| 500 | 9001 | 数据库操作失败 |
| 500 | 9002 | 其他内部错误 |

### 4.8 从单词本移除单词

**接口地址**

```http
POST /api/v1/vocabulary-notebooks/removeword-in-notebook
```

**身份认证：** 需要。

**请求参数**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "wordid": 100,
    "vocabularynotebookid": 1
  }
}
```

字段与“将单词添加到单词本”一致。

**成功响应**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**移除规则**

- 单词和单词本必须存在，并属于当前用户。
- 只移除单词与指定单词本之间的关联。
- 不会删除单词本身。
- 即使单词原本不在该单词本中，当前实现仍会执行更新并返回成功。

**可能的错误情况**

| HTTP | API Code | 情况 |
|---:|---:|---|
| 400 | 1001 | data 为空 |
| 400 | 1002 | JSON 格式错误 |
| 400 | 1004 | 单词或单词本不存在，或不属于当前用户 |
| 401 | 1102–1107 | Token 或用户身份校验失败 |
| 415 | 1005 | Content-Type 不受支持 |
| 500 | 9001 | 数据库操作失败 |
| 500 | 9002 | 其他内部错误 |

---

## 5. 日语词汇管理（Words）

### 5.1 根据内容搜索单词

**接口地址**

```http
POST /api/v1/words/search
```

**身份认证：** 需要。

**请求参数**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "query": "日本語"
  }
}
```

| 字段 | 类型 | 必填 | 说明 |
|---|---|---|---|
| `data.query` | string | 是 | 搜索内容 |

搜索范围包括：

- `kannji`：汉字。
- `kana`：假名。
- `meaning`：释义。

当前搜索采用包含匹配。只返回当前用户自己的词汇。

搜索内容首尾的空白字符会被去除。空字符串或仅包含空白字符的查询会被拒绝。

**成功响应**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK",
  "data": {
    "words": [
      {
        "id": 100,
        "kannji": "日本語",
        "kana": "にほんご",
        "akusennto": 0,
        "meaning": "日语",
        "note": "",
        "vocabularynotebooks": [
          {
            "id": 1,
            "name": "JLPT N2",
            "tags": [
              "N2"
            ]
          }
        ],
        "createdat": "2026-11-28T12:00:00Z",
        "updatedat": "2026-11-28T12:00:00Z"
      }
    ],
    "messages": []
  }
}
```

`data.words` 为匹配到的单词列表。

单词对象字段：

| 字段 | 类型 | 说明 |
|---|---|---|
| `id` | integer | 单词 ID |
| `kannji` | string / null | 汉字写法，可为空 |
| `kana` | string | 假名写法 |
| `akusennto` | integer / null | 日语音调 |
| `meaning` | string | 释义 |
| `note` | string / null | 备注 |
| `vocabularynotebooks` | object[] / null | 当前包含此单词的单词本 |
| `createdat` | string | 创建时间，UTC |
| `updatedat` | string | 更新时间，UTC |

`vocabularynotebooks` 中的对象包含：

| 字段 | 类型 | 说明 |
|---|---|---|
| `id` | integer | 单词本 ID |
| `name` | string | 单词本名称 |
| `tags` | string[] | 单词本标签 |

`data.messages` 用于返回组装单词响应时产生的附加问题，结构如下：

```json
{
  "id": 100,
  "errormessage": "查询单词本时发生错误",
  "errorcategory": "Database"
}
```

如果获取单词所属单词本信息时出现问题，该单词可能不会出现在 `words` 中，而相关错误会放入 `messages`。这种情况下，接口整体仍可能返回成功。

**搜索排序**

当前实现会优先排列完全匹配的结果，然后依据匹配比例、匹配位置和单词 ID 排序。

搜索区分大小写，当前没有提供分页参数或结果数量限制。

**无匹配结果**

没有符合条件的单词时，返回成功和空列表：

```json
{
  "success": true,
  "code": 0,
  "message": "OK",
  "data": {
    "words": [],
    "messages": []
  }
}
```

**可能的错误情况**

| HTTP | API Code | 情况 |
|---:|---:|---|
| 400 | 1001 | data 为空 |
| 400 | 1002 | JSON 格式错误 |
| 400 | 1004 | query 为空或仅包含空白字符 |
| 401 | 1102–1107 | Token 或用户身份校验失败 |
| 415 | 1005 | Content-Type 不受支持 |
| 500 | 9001 | 数据库操作失败 |
| 500 | 9002 | 其他内部错误 |

### 5.2 根据单词本查询单词

**接口地址**

```http
POST /api/v1/words/search-by-notebook
```

**身份认证：** 需要。

**请求参数**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "id": 1
  }
}
```

| 字段 | 类型 | 必填 | 说明 |
|---|---|---|---|
| `data.id` | integer | 是 | 单词本 ID |

**特殊规则**

当 `data.id` 为 `-1` 时，服务端会返回当前用户的全部单词。

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "id": -1
  }
}
```

这是对虚拟默认单词本的特殊处理，不代表数据库中存在 ID 为 `-1` 的实际单词本记录。

使用其他 ID 时，服务端会查询对应的单词本，并检查其是否属于当前用户。

**成功响应**

响应结构与“根据内容搜索单词”一致：

```json
{
  "success": true,
  "code": 0,
  "message": "OK",
  "data": {
    "words": [
      {
        "id": 100,
        "kannji": "日本語",
        "kana": "にほんご",
        "akusennto": 0,
        "meaning": "日语",
        "note": "",
        "vocabularynotebooks": [],
        "createdat": "2026-11-28T12:00:00Z",
        "updatedat": "2026-11-28T12:00:00Z"
      }
    ],
    "messages": []
  }
}
```

**可能的错误情况**

| HTTP | API Code | 情况 |
|---:|---:|---|
| 400 | 1001 | data 为空 |
| 400 | 1002 | JSON 格式错误 |
| 401 | 1102–1107 | Token 或用户身份校验失败 |
| 415 | 1005 | Content-Type 不受支持 |
| 500 | 9001 | 数据库操作失败 |
| 500 | 9002 | 单词本不存在、不属于当前用户，或其他内部错误 |

### 5.3 创建单词

**接口地址**

```http
POST /api/v1/words/create
```

**身份认证：** 需要。

**请求参数**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "kannji": "日本語",
    "kana": "にほんご",
    "akusennto": 0,
    "meaning": "日语",
    "note": ""
  }
}
```

| 字段 | 类型 | 必填 | 说明 |
|---|---|---|---|
| `data.kannji` | string / null | 否 | 汉字写法 |
| `data.kana` | string | 是 | 假名写法 |
| `data.akusennto` | integer / null | 否 | 音调 |
| `data.meaning` | string | 是 | 释义 |
| `data.note` | string / null | 否 | 备注 |

**成功响应**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

单词 ID、创建时间、更新时间和用户归属由服务端生成。

创建单词时不会自动将其关联到某个实际保存的单词本。需要建立关联时，应另外调用单词本关联接口。

**可能的错误情况**

| HTTP | API Code | 情况 |
|---:|---:|---|
| 400 | 1001 | data 为空 |
| 400 | 1002 | JSON 格式错误或必需字段无法解析 |
| 401 | 1102–1107 | Token 或用户身份校验失败 |
| 415 | 1005 | Content-Type 不受支持 |
| 500 | 9001 | 数据库操作失败 |
| 500 | 9002 | 其他内部错误 |

### 5.4 修改单词

**接口地址**

```http
POST /api/v1/words/update
```

**身份认证：** 需要。

**请求参数**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "id": 100,
    "kannji": "日本語",
    "kana": "にほんご",
    "akusennto": 0,
    "meaning": "日语",
    "note": "更新后的备注"
  }
}
```

| 字段 | 类型 | 必填 | 说明 |
|---|---|---|---|
| `data.id` | integer | 是 | 要修改的单词 ID |
| `data.kannji` | string / null | 否 | 汉字写法 |
| `data.kana` | string | 是 | 假名写法 |
| `data.akusennto` | integer / null | 否 | 音调 |
| `data.meaning` | string | 是 | 释义 |
| `data.note` | string / null | 否 | 备注 |

**成功响应**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**更新规则**

- 单词必须存在，且属于当前用户。
- `id` 用于确定需要修改的单词。
- 其他字段根据本次请求提交的数据更新。
- 原始创建时间会被保留。
- 更新时间由服务端自动更新。
- 本接口不是部分字段更新，客户端应提交完整的必需字段。

**可能的错误情况**

| HTTP | API Code | 情况 |
|---:|---:|---|
| 400 | 1001 | data 为空 |
| 400 | 1002 | JSON 格式错误或必需字段无法解析 |
| 401 | 1102–1107 | Token 或用户身份校验失败 |
| 415 | 1005 | Content-Type 不受支持 |
| 500 | 9001 | 数据库操作失败 |
| 500 | 9002 | 单词不存在、不属于当前用户，或其他内部错误 |

当前实现将部分单词不存在或不属于当前用户的情况映射为 `9002`。

### 5.5 删除单词

**接口地址**

```http
POST /api/v1/words/delete
```

**身份认证：** 需要。

**请求参数**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "id": 100
  }
}
```

| 字段 | 类型 | 必填 | 说明 |
|---|---|---|---|
| `data.id` | integer | 是 | 要删除的单词 ID |

**成功响应**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**删除规则**

删除单词前，服务端会先查询包含该单词的单词本，并尝试移除所有相关关联，然后删除单词记录。

如果清理关联时发生错误，服务端会停止后续删除操作并返回错误。

**可能的错误情况**

| HTTP | API Code | 情况 |
|---:|---:|---|
| 400 | 1001 | data 为空 |
| 400 | 1002 | JSON 格式错误 |
| 401 | 1102–1107 | Token 或用户身份校验失败 |
| 415 | 1005 | Content-Type 不受支持 |
| 500 | 9001 | 数据库操作失败，或清理关联时发生数据库错误 |
| 500 | 9002 | 单词不存在、不属于当前用户，或其他内部错误 |

需要注意，清理关联和删除单词不是一个整体事务。如果关联清理成功，但后续单词删除失败，单词可能已经不再属于任何单词本。

### 5.6 批量删除单词

**接口地址**

```http
POST /api/v1/words/delete-batch
```

**身份认证：** 需要。

**请求参数**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "words": [
      {
        "id": 100
      },
      {
        "id": 101
      },
      {
        "id": 102
      }
    ]
  }
}
```

| 字段 | 类型 | 必填 | 说明 |
|---|---|---|---|
| `data.words` | object[] | 是 | 要删除的单词列表 |
| `data.words[].id` | integer | 是 | 单词 ID |

**全部成功**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK",
  "data": null
}
```

**部分或全部失败**

HTTP Status Code：`500`

API Code：`9000`

```json
{
  "success": false,
  "code": 9000,
  "message": "一共有:1 个错误",
  "data": {
    "responseDataList": [
      {
        "id": 102,
        "errormessage": "该单词不存在",
        "errorcategory": "None"
      }
    ]
  }
}
```

失败记录包含：

| 字段 | 类型 | 说明 |
|---|---|---|
| `id` | integer / null | 操作失败的单词 ID |
| `errormessage` | string | 失败原因 |
| `errorcategory` | string / null | 错误类别 |

**可能的错误情况**

| HTTP | API Code | 情况 |
|---:|---:|---|
| 400 | 1001 | data 为空 |
| 400 | 1002 | JSON 格式错误 |
| 401 | 1102–1107 | Token 或用户身份校验失败 |
| 415 | 1005 | Content-Type 不受支持 |
| 500 | 9000 | 批量操作中至少有一个项目失败 |
| 500 | 9001 / 9002 | 其他数据库或内部错误 |

**注意事项**

批量删除会逐项执行，并非整体事务。某些单词可能已经成功删除，而其他单词仍然存在。

当收到 `9000` 时，客户端应读取失败列表并重新查询相关数据，不应直接假设所有单词都删除失败。

---

## 6. 服务状态检测（Status）

### 6.1 心跳检测

**接口地址**

```http
POST /api/v1/status/heartbeat-detection
```

**身份认证：** 需要。

**请求参数**

```json
{
  "token": "YOUR_JWT_TOKEN"
}
```

本接口不需要 `data`。

**成功响应**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

该接口会验证 Token，并检查对应用户在数据库中的状态，包括：

- 用户是否存在。
- 用户是否启用。
- Token 中的用户身份是否与数据库中的身份一致。

因此，这不仅是一个 HTTP 服务是否启动的检测接口，也会验证当前用户是否仍然具有有效身份。

**可能的错误情况**

| HTTP | API Code | 情况 |
|---:|---:|---|
| 401 | 1102 | Token 缺失 |
| 401 | 1103 | Token 无效 |
| 401 | 1104 | Token 已过期 |
| 401 | 1105 | Token 尚未生效 |
| 401 | 1106 | Token Claims 无效 |
| 401 | 1107 | 用户不存在、被禁用或身份不一致 |
| 500 | 9001 | 用户身份查询时发生数据库错误 |
| 500 | 9002 | 其他内部错误 |
| 415 | 1005 | Content-Type 不受支持 |

---

## 7. API Code 参考

以下代码定义来自当前 `backup` 分支的：

`src/Models/Code/ApiCode.cs`

### 7.1 成功

| Code | 含义 |
|---:|---|
| `0` | 操作成功 |

### 7.2 通用请求错误

| Code | 含义 |
|---:|---|
| `1000` | 请求参数无效 |
| `1001` | 请求体为空 |
| `1002` | JSON 格式错误 |
| `1003` | 缺少必要参数 |
| `1004` | 参数值不符合要求 |
| `1005` | 不支持的请求格式 |

### 7.3 身份认证错误

| Code | 含义 |
|---:|---|
| `1100` | 身份认证失败 |
| `1101` | 用户名或密码错误 |
| `1102` | Token 缺失 |
| `1103` | Token 无效 |
| `1104` | Token 已过期 |
| `1105` | Token 尚未生效 |
| `1106` | Token Claims 无效 |
| `1107` | 用户身份已失效 |

### 7.4 权限错误

| Code | 含义 |
|---:|---|
| `1200` | 无权执行此操作 |
| `1201` | 需要管理员权限 |
| `1202` | 无权访问该资源 |

### 7.5 服务端错误

| Code | 含义 |
|---:|---|
| `9000` | 服务端内部错误，当前也用于批量操作存在失败项的情况 |
| `9001` | 数据库操作失败 |
| `9002` | 未知内部错误 |
| `9003` | 服务端配置错误 |
| `9004` | 内部业务处理失败 |

**说明**

上述列表是当前代码中定义的 API Code，并不代表所有代码都已经被当前路由使用。具体接口可能只使用其中一部分代码。

客户端不应自行根据错误消息文本推断错误类型，应以 API Code 为准。

---

## 8. 当前版本的功能范围与限制

以下内容是当前 Demo 版本需要特别注意的地方。

### 8.1 用户管理

当前已提供登录、Token 身份校验和修改当前用户密码的接口。

以下功能尚未通过当前 HTTP API 对外提供：

- 普通用户注册或创建账号。
- 管理员创建其他用户。
- 管理员删除、禁用或启用其他用户。
- 管理员修改其他用户的密码。

### 8.2 学习与练习

当前版本主要提供词汇和单词本的数据管理能力。

学习记录、成绩统计及完整的练习相关服务端 API 尚未实现。

### 8.3 批量操作

当前代码注册了批量删除单词和批量删除单词本接口。

虽然请求 DTO 中存在批量创建和批量更新单词的数据结构，但当前 `ServerInit` 并未注册相应的 HTTP 路由。因此，这些功能不能视为已经开放的 API。

### 8.4 数据归属

单词和单词本均具有所属用户。

服务端会根据 Token 确定当前用户，并在数据操作中检查数据归属。客户端不应自行传入 `userid` 来指定数据所有者。

### 8.5 默认单词本

默认单词本是由服务端在查询列表时生成的虚拟对象，ID 固定为 `-1`。

在单词查询接口中，使用 `id = -1` 表示当前用户的全部单词。

它不是数据库中的普通单词本记录，不能通过普通单词本接口创建、修改或删除。

### 8.6 数据分页

当前查询接口没有提供分页参数，搜索结果会直接返回符合条件的数据。

### 8.7 错误处理与输入校验

当前 Demo 的部分输入校验尚不完整。某些资源不存在或不属于当前用户的情况，也可能被映射为 `9002`，并返回 HTTP 500。

批量删除出现部分失败时，会使用 `9000` 返回失败项信息，即使部分项目已经成功处理。

客户端应结合 API Code、响应数据及重新查询的结果判断实际状态，不应只根据 HTTP 状态码判断数据是否已发生变化。

---

## 9. 当前接口清单

| 模块 | HTTP Method | URL |
|---|---|---|
| Auth | POST | `/api/v1/auth/login` |
| User | POST | `/api/v1/user/password-change` |
| Vocabulary Notebooks | POST | `/api/v1/vocabulary-notebooks/list` |
| Vocabulary Notebooks | POST | `/api/v1/vocabulary-notebooks/search-by-tag` |
| Vocabulary Notebooks | POST | `/api/v1/vocabulary-notebooks/create` |
| Vocabulary Notebooks | POST | `/api/v1/vocabulary-notebooks/update` |
| Vocabulary Notebooks | POST | `/api/v1/vocabulary-notebooks/delete` |
| Vocabulary Notebooks | POST | `/api/v1/vocabulary-notebooks/delete-batch` |
| Vocabulary Notebooks | POST | `/api/v1/vocabulary-notebooks/addword-in-notebook` |
| Vocabulary Notebooks | POST | `/api/v1/vocabulary-notebooks/removeword-in-notebook` |
| Words | POST | `/api/v1/words/search` |
| Words | POST | `/api/v1/words/search-by-notebook` |
| Words | POST | `/api/v1/words/create` |
| Words | POST | `/api/v1/words/update` |
| Words | POST | `/api/v1/words/delete` |
| Words | POST | `/api/v1/words/delete-batch` |
| Status | POST | `/api/v1/status/heartbeat-detection` |

当前版本共注册 **17 个 HTTP API 路由**。