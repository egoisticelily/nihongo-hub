# NihongoHub Server API Reference

> **API バージョン：** v1

このドキュメントでは、NihongoHub Server が提供する HTTP API について説明します。API の URL、リクエスト、レスポンス、エラーコード、特別な動作などを確認できます。

## 1. 基本ルール

### 1.1 API のパス

すべての API は、次のパスを使います。

```text
/api/v1/
```

リクエスト URL は、サーバーのアドレスと API のパスを合わせて作ります。

例：

```text
http://127.0.0.1:5000/api/v1/auth/login
```

サーバーのアドレスとポートは、サーバーの設定によって変わります。

### 1.2 HTTP Method と Content-Type

現在のすべての API は、次の Method を使います。

```http
POST
```

リクエストは JSON 形式で送信し、次の Header を設定してください。

```http
Content-Type: application/json
```

すべてのパラメータは HTTP リクエストの Body に入れます。URL の Query Parameter は使いません。

### 1.3 認証

ログイン API 以外のすべての API では、有効な JWT Token が必要です。

現在のバージョンでは、Token は HTTP Header の `Authorization: Bearer` ではなく、JSON Body の `token` フィールドに入れます。

認証が必要な API の多くは、次の形式を使います。

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {}
}
```

| フィールド | 型 | 説明 |
|---|---|---|
| `token` | string | JWT Token |
| `data` | object | API に送るデータ |

単語帳一覧の取得やサーバー状態の確認など、一部の API では `data` は必要ありません。Token だけを送信します。

ユーザー ID は Token から取得します。クライアントが `userid` を送る必要はありません。

### 1.4 共通レスポンス

通常の操作 API は、次のようなレスポンスを返します。

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

データを返す API は、次のような形式を使います。

```json
{
  "success": true,
  "code": 0,
  "message": "OK",
  "data": {}
}
```

| フィールド | 型 | 説明 |
|---|---|---|
| `success` | boolean | 操作が成功したかどうか |
| `code` | integer | NihongoHub API Code |
| `message` | string / null | 結果やエラーメッセージ |
| `data` | object / null | API が返すデータ |

エラーレスポンスには `data` がない場合もあります。これは API のレスポンス形式によって異なります。

### 1.5 HTTP Status Code と API Code

NihongoHub では、2 種類のステータス情報を使います。

- **HTTP Status Code：** HTTP リクエストの結果を表します。
- **API Code：** NihongoHub が定義した具体的な結果を表します。

クライアントは HTTP Status Code だけではなく、API Code を使ってエラーの内容を判断してください。

現在のコードでは、主に次のように対応しています。

| HTTP Status | API Code | 説明 |
|---:|---|---|
| 200 | `0` | 操作成功 |
| 400 | `1000–1004` | リクエストまたはパラメータのエラー |
| 401 | `1100–1107` | 認証エラー |
| 403 | `1200–1202` | 権限エラー |
| 415 | `1005` | 対応していない Content-Type |
| 500 | `9000–9004` | サーバー内部エラー |

### 1.6 日時

サーバーが返す作成日時と更新日時は、すべて UTC です。

日時の形式は ISO 8601 です。

例：

```text
2026-11-28T12:30:00Z
```

必要に応じて、クライアント側でローカル時間に変換してください。

---

## 2. 認証（Auth）

### 2.1 ログイン

**Endpoint**

```http
POST /api/v1/auth/login
```

**認証：** 不要。

**Request Body**

ログイン API は、共通の `{ token, data }` 形式を使いません。ユーザー名とパスワードを直接送信します。

```json
{
  "username": "admin",
  "password": "YOUR_PASSWORD"
}
```

| フィールド | 型 | 必須 | 説明 |
|---|---|---|---|
| `username` | string | はい | ユーザー名 |
| `password` | string | はい | パスワード |

**成功レスポンス**

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

`data` のフィールド：

| フィールド | 型 | 説明 |
|---|---|---|
| `token` | string | JWT Token |
| `userid` | integer | ユーザー ID |
| `usertype` | string | ユーザーの種類。現在は `admin` または `user` |
| `expiresat` | string | Token の有効期限。UTC |

Token の有効時間は、サーバー設定の `Jwt.ExpireMinutes` で決まります。デフォルトは 10080 分（7 日）です。

**エラーの可能性**

| HTTP Status | API Code | 状況 |
|---:|---:|---|
| 400 | 1001 | リクエスト Body が空 |
| 400 | 1002 | JSON 形式が正しくない、またはリクエストを読み取れない |
| 400 | 1004 | ユーザー名またはパスワードが空、または空白だけ |
| 401 | 1101 | ユーザー名またはパスワードが間違っている、またはユーザーが無効 |
| 415 | 1005 | 対応していない Content-Type |
| 500 | 9001 | データベース操作に失敗 |
| 500 | 9002 | 不明な内部エラー |
| 500 | 9004 | ログイン確認後にユーザーデータの問題が見つかった |

ユーザー名が存在しない場合、パスワードが間違っている場合、ユーザーが無効になっている場合は、すべて `1101` を返します。サーバーは具体的な理由をクライアントに知らせません。

---

## 3. ユーザー関連 API（User）

### 3.1 現在のユーザーのパスワードを変更する

**Endpoint**

```http
POST /api/v1/user/password-change
```

**認証：** 必要。

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "password": "NEW_PASSWORD"
  }
}
```

| フィールド | 型 | 必須 | 説明 |
|---|---|---|---|
| `token` | string | はい | 現在のユーザーの JWT Token |
| `data.password` | string | はい | 新しいパスワード |

**成功レスポンス**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**エラーの可能性**

| HTTP Status | API Code | 状況 |
|---:|---:|---|
| 400 | 1001 | Request Body または `data` が空 |
| 400 | 1002 | JSON 形式が正しくない |
| 400 | 1004 | 新しいパスワードが空、または空白だけ |
| 401 | 1102 | Token がない |
| 401 | 1103 | Token が無効 |
| 401 | 1104 | Token の有効期限が切れている |
| 401 | 1105 | Token がまだ有効ではない |
| 401 | 1106 | Token Claims が正しくない |
| 401 | 1107 | ユーザーの状態が無効 |
| 415 | 1005 | 対応していない Content-Type |
| 500 | 9001 | データベース操作に失敗 |
| 500 | 9002 | 不明な内部エラー |

**注意**

現在のバージョンでは、パスワード変更後に古い Token を無効にする処理はありません。古い Token は、有効期限が切れるまで使える場合があります。

---

## 4. 単語帳管理 API（Vocabulary Notebooks）

### 4.1 単語帳一覧を取得する

**Endpoint**

```http
POST /api/v1/vocabulary-notebooks/list
```

**認証：** 必要。

**Request Body**

この API は `data` を必要としません。Token だけを送信します。

```json
{
  "token": "YOUR_JWT_TOKEN"
}
```

**成功レスポンス**

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

`vocabularyNotebookList` は、現在のユーザーの単語帳一覧です。

単語帳のフィールド：

| フィールド | 型 | 説明 |
|---|---|---|
| `id` | integer | 単語帳 ID |
| `name` | string | 単語帳の名前 |
| `note` | string / null | メモ |
| `tags` | string[] | 単語帳のタグ |
| `createdat` | string | 作成日時。UTC |
| `updatedat` | string | 更新日時。UTC |

**デフォルト単語帳**

一覧には、次のデフォルト単語帳も含まれます。

| フィールド | 値 |
|---|---|
| `id` | `-1` |
| `name` | `default` |
| `note` | 空文字 |
| `tags` | 空の配列 |
| `createdat` | `2000-01-01T00:00:00Z` |
| `updatedat` | `2000-01-01T00:00:00Z` |

この単語帳はサーバーが作成する仮想の単語帳です。データベースには通常の単語帳として保存されていません。

通常の単語帳 API から変更または削除することはできません。

**エラーの可能性**

| HTTP Status | API Code | 状況 |
|---:|---:|---|
| 401 | 1102–1107 | Token またはユーザー確認に失敗 |
| 415 | 1005 | 対応していない Content-Type |
| 500 | 9001 | データベース操作に失敗 |
| 500 | 9002 | 不明な内部エラー |

### 4.2 タグで単語帳を検索する

**Endpoint**

```http
POST /api/v1/vocabulary-notebooks/search-by-tag
```

**認証：** 必要。

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "tag": "N2"
  }
}
```

| フィールド | 型 | 必須 | 説明 |
|---|---|---|---|
| `data.tag` | string | はい | 検索するタグ |

**成功レスポンス**

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

タグは完全一致で検索します。一部だけが同じ場合は一致しません。

仮想のデフォルト単語帳は検索結果に含まれません。

一致する単語帳がない場合も、成功と空のリストを返します。

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

**エラーの可能性**

| HTTP Status | API Code | 状況 |
|---:|---:|---|
| 400 | 1001 | `data` が空 |
| 400 | 1002 | JSON 形式が正しくない |
| 401 | 1102–1107 | Token またはユーザー確認に失敗 |
| 415 | 1005 | 対応していない Content-Type |
| 500 | 9001 | データベース操作に失敗 |
| 500 | 9002 | 不明な内部エラー |

### 4.3 単語帳を作成する

**Endpoint**

```http
POST /api/v1/vocabulary-notebooks/create
```

**認証：** 必要。

**Request Body**

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

| フィールド | 型 | 必須 | 説明 |
|---|---|---|---|
| `data.name` | string | はい | 単語帳の名前 |
| `data.note` | string / null | いいえ | メモ |
| `data.tags` | string[] | はい | タグのリスト |

単語帳 ID、ユーザー情報、作成日時、更新日時はサーバーが設定します。新しい単語帳の単語リストは空です。

**成功レスポンス**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**エラーの可能性**

| HTTP Status | API Code | 状況 |
|---:|---:|---|
| 400 | 1001 | `data` が空 |
| 400 | 1002 | JSON 形式が正しくない、または必須フィールドを読み取れない |
| 401 | 1102–1107 | Token またはユーザー確認に失敗 |
| 415 | 1005 | 対応していない Content-Type |
| 500 | 9001 | データベース操作に失敗 |
| 500 | 9002 | 不明な内部エラー |

現在のコードでは、単語帳の名前やタグの内容を完全には確認していません。クライアントは空の名前や意味のないタグを送らないようにしてください。

### 4.4 単語帳を更新する

**Endpoint**

```http
POST /api/v1/vocabulary-notebooks/update
```

**認証：** 必要。

**Request Body**

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

| フィールド | 型 | 必須 | 説明 |
|---|---|---|---|
| `data.id` | integer | はい | 単語帳 ID |
| `data.name` | string | はい | 新しい名前 |
| `data.note` | string / null | いいえ | 新しいメモ |
| `data.tags` | string[] | はい | 新しいタグのリスト |

**成功レスポンス**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**更新ルール**

- 現在のユーザーが所有する単語帳だけを更新できます。
- `name`、`note`、`tags` はリクエストの内容で更新されます。
- すでに登録されている単語リストはそのまま残ります。
- 作成日時は変わりません。
- 更新日時はサーバーが設定します。

この API は一部のフィールドだけを更新する機能ではありません。必須フィールドをすべて送信してください。

**エラーの可能性**

| HTTP Status | API Code | 状況 |
|---:|---:|---|
| 400 | 1001 | `data` が空 |
| 400 | 1002 | JSON 形式が正しくない |
| 401 | 1102–1107 | Token またはユーザー確認に失敗 |
| 415 | 1005 | 対応していない Content-Type |
| 500 | 9001 | データベース操作に失敗 |
| 500 | 9002 | 単語帳が存在しない、現在のユーザーの所有物ではない、またはその他の内部エラー |

現在の実装では、一部の「存在しない」「自分の単語帳ではない」という場合も `9002` を返します。

### 4.5 単語帳を削除する

**Endpoint**

```http
POST /api/v1/vocabulary-notebooks/delete
```

**認証：** 必要。

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "id": 1
  }
}
```

| フィールド | 型 | 必須 | 説明 |
|---|---|---|---|
| `data.id` | integer | はい | 削除する単語帳 ID |

**成功レスポンス**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**削除ルール**

単語帳を削除しても、その中の単語は削除されません。単語データは別に保存されています。この操作では、単語帳と単語の関連情報だけを削除します。

**エラーの可能性**

| HTTP Status | API Code | 状況 |
|---:|---:|---|
| 400 | 1001 | `data` が空 |
| 400 | 1002 | JSON 形式が正しくない |
| 401 | 1102–1107 | Token またはユーザー確認に失敗 |
| 415 | 1005 | 対応していない Content-Type |
| 500 | 9001 | データベース操作に失敗 |
| 500 | 9002 | 単語帳が存在しない、現在のユーザーの所有物ではない、またはその他の内部エラー |

### 4.6 複数の単語帳を削除する

**Endpoint**

```http
POST /api/v1/vocabulary-notebooks/delete-batch
```

**認証：** 必要。

**Request Body**

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

| フィールド | 型 | 必須 | 説明 |
|---|---|---|---|
| `data.notebooks` | object[] | はい | 削除する単語帳のリスト |
| `data.notebooks[].id` | integer | はい | 単語帳 ID |

**すべて成功した場合**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK",
  "data": null
}
```

**1 つ以上失敗した場合**

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

失敗した項目には次の情報が含まれます。

| フィールド | 型 | 説明 |
|---|---|---|
| `id` | integer / null | 失敗した単語帳 ID |
| `errormessage` | string | 失敗した理由 |
| `errorcategory` | string / null | エラーの種類 |

現在のエラーの種類には `None`、`Database`、`Unknown` などがあります。

**エラーの可能性**

| HTTP Status | API Code | 状況 |
|---:|---:|---|
| 400 | 1001 | `data` が空 |
| 400 | 1002 | JSON 形式が正しくない |
| 401 | 1102–1107 | Token またはユーザー確認に失敗 |
| 415 | 1005 | 対応していない Content-Type |
| 500 | 9000 | リストの中に失敗した項目がある |
| 500 | 9001 / 9002 | データベースまたはその他の内部エラー |

**注意**

一括操作は、1 つのデータベーストランザクションではありません。各項目を別々に処理します。他の項目が失敗しても、すでに成功した削除は元に戻りません。

`9000` が返された場合、`responseDataList` を確認し、もう一度単語帳一覧を取得して最終状態を確認してください。

### 4.7 単語を単語帳に追加する

**Endpoint**

```http
POST /api/v1/vocabulary-notebooks/addword-in-notebook
```

**認証：** 必要。

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "wordid": 100,
    "vocabularynotebookid": 1
  }
}
```

| フィールド | 型 | 必須 | 説明 |
|---|---|---|---|
| `data.wordid` | integer | はい | 単語 ID |
| `data.vocabularynotebookid` | integer | はい | 追加先の単語帳 ID |

**成功レスポンス**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**追加ルール**

- 単語が存在し、現在のユーザーの所有物である必要があります。
- 単語帳が存在し、現在のユーザーの所有物である必要があります。
- 同じ単語を同じ単語帳に 2 回追加することはできません。
- 成功すると、単語帳の更新日時が変わります。

**エラーの可能性**

| HTTP Status | API Code | 状況 |
|---:|---:|---|
| 400 | 1001 | `data` が空 |
| 400 | 1002 | JSON 形式が正しくない |
| 400 | 1004 | 単語または単語帳が存在しない、現在のユーザーの所有物ではない、またはすでに追加されている |
| 401 | 1102–1107 | Token またはユーザー確認に失敗 |
| 415 | 1005 | 対応していない Content-Type |
| 500 | 9001 | データベース操作に失敗 |
| 500 | 9002 | その他の内部エラー |

### 4.8 単語帳から単語を削除する

**Endpoint**

```http
POST /api/v1/vocabulary-notebooks/removeword-in-notebook
```

**認証：** 必要。

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "wordid": 100,
    "vocabularynotebookid": 1
  }
}
```

フィールドは「単語を単語帳に追加する」API と同じです。

**成功レスポンス**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**削除ルール**

- 単語と単語帳が存在し、現在のユーザーの所有物である必要があります。
- 指定した単語帳との関連だけを削除します。
- 単語自体は削除されません。
- その単語がもともと単語帳に入っていなくても、現在の実装では更新処理を行い、成功を返します。

**エラーの可能性**

| HTTP Status | API Code | 状況 |
|---:|---:|---|
| 400 | 1001 | `data` が空 |
| 400 | 1002 | JSON 形式が正しくない |
| 400 | 1004 | 単語または単語帳が存在しない、または現在のユーザーの所有物ではない |
| 401 | 1102–1107 | Token またはユーザー確認に失敗 |
| 415 | 1005 | 対応していない Content-Type |
| 500 | 9001 | データベース操作に失敗 |
| 500 | 9002 | その他の内部エラー |

---

## 5. 日本語の単語 API（Words）

### 5.1 文字で単語を検索する

**Endpoint**

```http
POST /api/v1/words/search
```

**認証：** 必要。

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "query": "日本語"
  }
}
```

| フィールド | 型 | 必須 | 説明 |
|---|---|---|---|
| `data.query` | string | はい | 検索する文字 |

次のフィールドを検索します。

- `kannji`：漢字。
- `kana`：かな。
- `meaning`：意味。

検索には部分一致を使います。現在のユーザーの単語だけが検索対象です。

検索文字の最初と最後にある空白は削除されます。空文字や空白だけの検索はできません。

**成功レスポンス**

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
        "meaning": "日本語",
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

`data.words` は検索に一致した単語のリストです。

単語オブジェクトのフィールド：

| フィールド | 型 | 説明 |
|---|---|---|
| `id` | integer | 単語 ID |
| `kannji` | string / null | 漢字。空の場合があります |
| `kana` | string | かな |
| `akusennto` | integer / null | 日本語のアクセント |
| `meaning` | string | 意味 |
| `note` | string / null | メモ |
| `vocabularynotebooks` | object[] / null | この単語が入っている単語帳 |
| `createdat` | string | 作成日時。UTC |
| `updatedat` | string | 更新日時。UTC |

`vocabularynotebooks` の各項目には、次の情報が含まれます。

| フィールド | 型 | 説明 |
|---|---|---|
| `id` | integer | 単語帳 ID |
| `name` | string | 単語帳の名前 |
| `tags` | string[] | 単語帳のタグ |

`data.messages` は、単語のレスポンスを作るときに発生した追加の問題を返します。各項目の形式は次のとおりです。

```json
{
  "id": 100,
  "errormessage": "単語帳情報の取得に失敗しました",
  "errorcategory": "Database"
}
```

単語が入っている単語帳の情報を取得できない場合、その単語は `words` に含まれないことがあります。その場合、エラー情報は `messages` に追加されます。API 全体は成功を返す場合があります。

**検索結果の順番**

現在の実装では、次の順番で検索結果を並べます。

1. 完全一致した単語。
2. 一致した割合が高い単語。
3. 一致した位置が前にある単語。
4. 単語 ID が小さい単語。

検索では大文字と小文字を区別します。ページ分けや件数を指定するパラメータはありません。

**一致する単語がない場合**

一致する単語がない場合も、成功と空のリストを返します。

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

**エラーの可能性**

| HTTP Status | API Code | 状況 |
|---:|---:|---|
| 400 | 1001 | `data` が空 |
| 400 | 1002 | JSON 形式が正しくない |
| 400 | 1004 | 検索文字が空、または空白だけ |
| 401 | 1102–1107 | Token またはユーザー確認に失敗 |
| 415 | 1005 | 対応していない Content-Type |
| 500 | 9001 | データベース操作に失敗 |
| 500 | 9002 | その他の内部エラー |

### 5.2 単語帳から単語を検索する

**Endpoint**

```http
POST /api/v1/words/search-by-notebook
```

**認証：** 必要。

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "id": 1
  }
}
```

| フィールド | 型 | 必須 | 説明 |
|---|---|---|---|
| `data.id` | integer | はい | 単語帳 ID |

**特別なルール**

`data.id` が `-1` の場合、現在のユーザーが持っているすべての単語を返します。

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "id": -1
  }
}
```

それ以外の ID の場合、サーバーは単語帳が存在し、現在のユーザーの所有物であることを確認してから単語を返します。

**成功レスポンス**

レスポンスの形式は「文字で単語を検索する」API と同じです。

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
        "meaning": "日本語",
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

**エラーの可能性**

| HTTP Status | API Code | 状況 |
|---:|---:|---|
| 400 | 1001 | `data` が空 |
| 400 | 1002 | JSON 形式が正しくない |
| 401 | 1102–1107 | Token またはユーザー確認に失敗 |
| 415 | 1005 | 対応していない Content-Type |
| 500 | 9001 | データベース操作に失敗 |
| 500 | 9002 | 単語帳が存在しない、現在のユーザーの所有物ではない、またはその他の内部エラー |

### 5.3 単語を作成する

**Endpoint**

```http
POST /api/v1/words/create
```

**認証：** 必要。

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "kannji": "日本語",
    "kana": "にほんご",
    "akusennto": 0,
    "meaning": "日本語",
    "note": ""
  }
}
```

| フィールド | 型 | 必須 | 説明 |
|---|---|---|---|
| `data.kannji` | string / null | いいえ | 漢字 |
| `data.kana` | string | はい | かな |
| `data.akusennto` | integer / null | いいえ | アクセント |
| `data.meaning` | string | はい | 意味 |
| `data.note` | string / null | いいえ | メモ |

**成功レスポンス**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

単語 ID、ユーザー情報、作成日時、更新日時はサーバーが設定します。

単語を作成しても、保存されている単語帳には自動で追加されません。必要な場合は、単語帳との関連付け API を使ってください。

**エラーの可能性**

| HTTP Status | API Code | 状況 |
|---:|---:|---|
| 400 | 1001 | `data` が空 |
| 400 | 1002 | JSON 形式が正しくない、または必須フィールドを読み取れない |
| 401 | 1102–1107 | Token またはユーザー確認に失敗 |
| 415 | 1005 | 対応していない Content-Type |
| 500 | 9001 | データベース操作に失敗 |
| 500 | 9002 | その他の内部エラー |

### 5.4 単語を更新する

**Endpoint**

```http
POST /api/v1/words/update
```

**認証：** 必要。

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "id": 100,
    "kannji": "日本語",
    "kana": "にほんご",
    "akusennto": 0,
    "meaning": "日本語",
    "note": "Updated note"
  }
}
```

| フィールド | 型 | 必須 | 説明 |
|---|---|---|---|
| `data.id` | integer | はい | 単語 ID |
| `data.kannji` | string / null | いいえ | 漢字 |
| `data.kana` | string | はい | かな |
| `data.akusennto` | integer / null | いいえ | アクセント |
| `data.meaning` | string | はい | 意味 |
| `data.note` | string / null | いいえ | メモ |

**成功レスポンス**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**更新ルール**

- 単語が存在し、現在のユーザーの所有物である必要があります。
- `id` で更新する単語を指定します。
- その他のフィールドは、リクエストの内容で更新します。
- 作成日時は変わりません。
- 更新日時はサーバーが設定します。

この API は一部のフィールドだけを更新する機能ではありません。必須フィールドをすべて送信してください。

**エラーの可能性**

| HTTP Status | API Code | 状況 |
|---:|---:|---|
| 400 | 1001 | `data` が空 |
| 400 | 1002 | JSON 形式が正しくない、または必須フィールドを読み取れない |
| 401 | 1102–1107 | Token またはユーザー確認に失敗 |
| 415 | 1005 | 対応していない Content-Type |
| 500 | 9001 | データベース操作に失敗 |
| 500 | 9002 | 単語が存在しない、現在のユーザーの所有物ではない、またはその他の内部エラー |

現在の実装では、一部の「存在しない」「自分の単語ではない」という場合も `9002` を返します。

### 5.5 単語を削除する

**Endpoint**

```http
POST /api/v1/words/delete
```

**認証：** 必要。

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "id": 100
  }
}
```

| フィールド | 型 | 必須 | 説明 |
|---|---|---|---|
| `data.id` | integer | はい | 削除する単語 ID |

**成功レスポンス**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**削除ルール**

単語を削除する前に、サーバーはその単語が入っている単語帳を検索し、関連付けを削除します。その後、単語データを削除します。

関連付けの削除中にエラーが発生した場合、サーバーは単語の削除を中止し、エラーを返します。

**エラーの可能性**

| HTTP Status | API Code | 状況 |
|---:|---:|---|
| 400 | 1001 | `data` が空 |
| 400 | 1002 | JSON 形式が正しくない |
| 401 | 1102–1107 | Token またはユーザー確認に失敗 |
| 415 | 1005 | 対応していない Content-Type |
| 500 | 9001 | データベース操作に失敗、または関連付けの削除に失敗 |
| 500 | 9002 | 単語が存在しない、現在のユーザーの所有物ではない、またはその他の内部エラー |

関連付けの削除と単語の削除は、1 つのトランザクションではありません。関連付けの削除に成功しても、単語の削除に失敗する場合があります。その場合、単語がどの単語帳にも入っていない状態になることがあります。

### 5.6 複数の単語を削除する

**Endpoint**

```http
POST /api/v1/words/delete-batch
```

**認証：** 必要。

**Request Body**

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

| フィールド | 型 | 必須 | 説明 |
|---|---|---|---|
| `data.words` | object[] | はい | 削除する単語のリスト |
| `data.words[].id` | integer | はい | 単語 ID |

**すべて成功した場合**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK",
  "data": null
}
```

**1 つ以上失敗した場合**

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

失敗した項目には次の情報が含まれます。

| フィールド | 型 | 説明 |
|---|---|---|
| `id` | integer / null | 失敗した単語 ID |
| `errormessage` | string | 失敗した理由 |
| `errorcategory` | string / null | エラーの種類 |

**エラーの可能性**

| HTTP Status | API Code | 状況 |
|---:|---:|---|
| 400 | 1001 | `data` が空 |
| 400 | 1002 | JSON 形式が正しくない |
| 401 | 1102–1107 | Token またはユーザー確認に失敗 |
| 415 | 1005 | 対応していない Content-Type |
| 500 | 9000 | リストの中に失敗した項目がある |
| 500 | 9001 / 9002 | その他のデータベースまたは内部エラー |

**注意**

一括削除は、1 つのトランザクションではありません。各単語を別々に処理します。他の単語の削除に失敗しても、すでに削除された単語は元に戻りません。

`9000` が返された場合、失敗した項目を確認し、もう一度単語を検索してください。すべての単語の削除に失敗したという意味ではありません。

---

## 6. サーバー状態（Status）

### 6.1 Heartbeat Detection

**Endpoint**

```http
POST /api/v1/status/heartbeat-detection
```

**認証：** 必要。

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN"
}
```

この API は `data` を必要としません。

**成功レスポンス**

HTTP Status Code：`200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

この API は Token と、データベースにある現在のユーザー情報を確認します。

確認する内容：

- ユーザーが存在するか。
- ユーザーが有効になっているか。
- データベースのユーザー権限と Token の権限が一致しているか。

そのため、この API は HTTP サーバーが起動しているかどうかだけではなく、現在のユーザーがまだサービスを利用できるかどうかも確認します。

**エラーの可能性**

| HTTP Status | API Code | 状況 |
|---:|---:|---|
| 401 | 1102 | Token がない |
| 401 | 1103 | Token が無効 |
| 401 | 1104 | Token の有効期限が切れている |
| 401 | 1105 | Token がまだ有効ではない |
| 401 | 1106 | Token Claims が正しくない |
| 401 | 1107 | ユーザーが存在しない、無効、または権限が一致しない |
| 500 | 9001 | ユーザー確認中にデータベースエラーが発生 |
| 500 | 9002 | その他の内部エラー |
| 415 | 1005 | 対応していない Content-Type |

---

## 7. API Code 一覧

以下は、現在のサーバーコードで定義されている API Code です。

### 7.1 成功

| Code | 意味 |
|---:|---|
| `0` | 操作成功 |

### 7.2 共通リクエストエラー

| Code | 意味 |
|---:|---|
| `1000` | リクエストパラメータが正しくない |
| `1001` | リクエスト Body が空 |
| `1002` | JSON 形式が正しくない |
| `1003` | 必須パラメータがない |
| `1004` | パラメータの値が正しくない |
| `1005` | 対応していない Content-Type |

### 7.3 認証エラー

| Code | 意味 |
|---:|---|
| `1100` | 認証失敗 |
| `1101` | ユーザー名またはパスワードが間違っている |
| `1102` | Token がない |
| `1103` | Token が無効 |
| `1104` | Token の有効期限が切れている |
| `1105` | Token がまだ有効ではない |
| `1106` | Token Claims が正しくない |
| `1107` | ユーザーの状態が無効 |

### 7.4 権限エラー

| Code | 意味 |
|---:|---|
| `1200` | この操作を行う権限がない |
| `1201` | 管理者権限が必要 |
| `1202` | このデータにアクセスする権限がない |

### 7.5 サーバー内部エラー

| Code | 意味 |
|---:|---|
| `9000` | サーバー内部エラー。一括操作で失敗した項目がある場合にも使用 |
| `9001` | データベース操作に失敗 |
| `9002` | 不明な内部エラー |
| `9003` | サーバー設定エラー |
| `9004` | 内部処理に失敗 |

**注意**

上記は現在のサーバーで定義されているコードです。すべてのコードが現在の API で使われているわけではありません。

`2000–4000` のコード範囲に関するエラー処理は、まだ完全には実装されていません。現在のバージョンでこれらのコードが返されるとは限りません。

クライアントはエラーメッセージの文章ではなく、API Code を使ってエラーを判断してください。

---

## 8. 現在の機能と制限

### 8.1 ユーザー管理

現在のバージョンでは、ログイン、Token の確認、現在のユーザーのパスワード変更に対応しています。

次の機能は、現在の HTTP API ではまだ利用できません。

- 一般ユーザーのアカウント作成。
- 管理者による他のユーザーの作成。
- 管理者による他のユーザーの削除、無効化、有効化。
- 管理者による他のユーザーのパスワード変更。

### 8.2 学習と練習

現在のバージョンは、主に単語と単語帳の管理機能を提供します。

学習記録、成績の集計、単語練習に関する完全なサーバー API は、まだ実装されていません。

### 8.3 一括操作

現在のコードでは、単語と単語帳の一括削除 API が登録されています。

一括単語作成と一括単語更新の Request Data はコードにありますが、現在の `ServerInit` には対応する HTTP Route が登録されていません。そのため、公開 API としては利用できません。

### 8.4 データの所有者

単語と単語帳には、それぞれ所有するユーザーが設定されています。

サーバーは Token から現在のユーザーを確認し、データを操作するときに所有者を確認します。クライアントは `userid` を送信して、データの所有者を指定しないでください。

### 8.5 ページ分け

現在の検索 API にはページ分けのパラメータがありません。一致するデータは、すべて 1 つのレスポンスで返されます。

### 8.6 入力確認とエラー処理

現在の Demo では、一部の入力確認がまだ完全ではありません。

単語や単語帳が存在しない場合、または現在のユーザーの所有物ではない場合でも、HTTP Status Code `500` と API Code `9002` が返されることがあります。

一括操作で一部の項目が失敗した場合、他の項目がすでに成功していても、失敗リストとともに `9000` が返されます。

クライアントは API Code とレスポンスの内容を確認してください。必要な場合は、もう一度データを取得して最終状態を確認してください。

---

## 9. API 一覧

| グループ | HTTP Method | URL |
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

現在のバージョンでは、**17 個の HTTP API** が登録されています。