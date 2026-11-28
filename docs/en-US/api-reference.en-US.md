# NihongoHub Server API Reference

> **API Version:** v1

This document describes the HTTP APIs provided by NihongoHub Server, including endpoint URLs, request parameters, response data, error codes, and special cases.

## 1. General Rules

### 1.1 API Base Path

All APIs use the following path prefix:

```text
/api/v1/
```

The full request URL consists of the server address and the API path.

For example:

```text
http://127.0.0.1:5000/api/v1/auth/login
```

The server address and port depend on the server configuration.

### 1.2 HTTP Method and Content Type

All currently available APIs use:

```http
POST
```

Requests must use JSON and include the following header:

```http
Content-Type: application/json
```

All parameters are sent in the HTTP request body. URL query parameters are not used.

### 1.3 Authentication

Except for the login API, all business APIs require a valid JWT Token.

The current version sends the Token in the JSON request body through the `token` field. It does not use the `Authorization: Bearer` HTTP header.

Most authenticated APIs use the following request structure:

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {}
}
```

| Field | Type | Description |
|---|---|---|
| `token` | string | JWT Token |
| `data` | object | Request data for the current API |

Some APIs do not need the `data` field. For example, listing vocabulary notebooks and checking server status only require a Token.

The server gets the user ID from the Token. Clients do not need to send `userid` themselves.

### 1.4 General Response Structure

A response for a simple operation usually looks like this:

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

A response that contains data usually looks like this:

```json
{
  "success": true,
  "code": 0,
  "message": "OK",
  "data": {}
}
```

| Field | Type | Description |
|---|---|---|
| `success` | boolean | Whether the operation succeeded |
| `code` | integer | NihongoHub API Code |
| `message` | string / null | Operation result or error message |
| `data` | object / null | Data returned by the API |

Error responses may not contain the `data` field. This depends on the response type used by each API.

### 1.5 HTTP Status Code and API Code

NihongoHub uses two types of status information:

- **HTTP Status Code:** Shows the result of the HTTP request.
- **API Code:** Shows the specific result defined by NihongoHub.

Clients should use the API Code to identify the exact result instead of relying only on the HTTP Status Code.

The current code uses the following mappings:

| HTTP Status | API Code | Description |
|---:|---|---|
| 200 | `0` | Operation succeeded |
| 400 | `1000–1004` | Request or parameter error |
| 401 | `1100–1107` | Authentication error |
| 403 | `1200–1202` | Permission error |
| 415 | `1005` | Unsupported content type |
| 500 | `9000–9004` | Server internal error |

### 1.6 Date and Time

All creation and update times returned by the server use UTC.

The time format follows ISO 8601. For example:

```text
2026-11-28T12:30:00Z
```

Clients should convert this value to local time when needed.

---

## 2. Authentication (Auth)

### 2.1 User Login

**Endpoint**

```http
POST /api/v1/auth/login
```

**Authentication:** Not required.

**Request Body**

The login API does not use the common `{ token, data }` structure. It directly accepts the username and password.

```json
{
  "username": "admin",
  "password": "YOUR_PASSWORD"
}
```

| Field | Type | Required | Description |
|---|---|---|---|
| `username` | string | Yes | Username |
| `password` | string | Yes | Password |

**Success Response**

HTTP Status Code: `200`

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

Fields in `data`:

| Field | Type | Description |
|---|---|---|
| `token` | string | JWT Token |
| `userid` | integer | User ID |
| `usertype` | string | User type: `admin` or `user` |
| `expiresat` | string | Token expiration time in UTC |

The Token lifetime is controlled by the server setting `Jwt.ExpireMinutes`. The default value is 10080 minutes, or 7 days.

**Possible Errors**

| HTTP Status | API Code | Situation |
|---:|---:|---|
| 400 | 1001 | Request body is empty |
| 400 | 1002 | Invalid JSON or request data cannot be parsed |
| 400 | 1004 | Username or password is empty or contains only whitespace |
| 401 | 1101 | Username or password is incorrect, or the user is disabled |
| 415 | 1005 | Unsupported content type |
| 500 | 9001 | Database operation failed |
| 500 | 9002 | Unknown internal error |
| 500 | 9004 | User data is invalid after successful login verification |

An unknown username, an incorrect password, and a disabled user all return `1101`. The server does not tell the client which case occurred.

---

## 3. User APIs (User)

### 3.1 Change Current User Password

**Endpoint**

```http
POST /api/v1/user/password-change
```

**Authentication:** Required.

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "password": "NEW_PASSWORD"
  }
}
```

| Field | Type | Required | Description |
|---|---|---|---|
| `token` | string | Yes | Current user's JWT Token |
| `data.password` | string | Yes | New password |

**Success Response**

HTTP Status Code: `200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**Possible Errors**

| HTTP Status | API Code | Situation |
|---:|---:|---|
| 400 | 1001 | Request body or `data` is empty |
| 400 | 1002 | Invalid JSON |
| 400 | 1004 | New password is empty or contains only whitespace |
| 401 | 1102 | Token is missing |
| 401 | 1103 | Token is invalid |
| 401 | 1104 | Token has expired |
| 401 | 1105 | Token is not valid yet |
| 401 | 1106 | Token Claims are invalid |
| 401 | 1107 | User identity is no longer valid |
| 415 | 1005 | Unsupported content type |
| 500 | 9001 | Database operation failed |
| 500 | 9002 | Unknown internal error |

**Note**

The current version does not revoke existing Tokens after a password change. An old Token may still be used until it expires or becomes invalid for another reason.

---

## 4. Vocabulary Notebook APIs (Vocabulary Notebooks)

### 4.1 Get Vocabulary Notebook List

**Endpoint**

```http
POST /api/v1/vocabulary-notebooks/list
```

**Authentication:** Required.

**Request Body**

This API does not need `data`. Only the Token is required.

```json
{
  "token": "YOUR_JWT_TOKEN"
}
```

**Success Response**

HTTP Status Code: `200`

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

`vocabularyNotebookList` contains the current user's vocabulary notebooks.

Fields in a vocabulary notebook:

| Field | Type | Description |
|---|---|---|
| `id` | integer | Vocabulary notebook ID |
| `name` | string | Notebook name |
| `note` | string / null | Note |
| `tags` | string[] | Notebook tags |
| `createdat` | string | Creation time in UTC |
| `updatedat` | string | Update time in UTC |

**Default Vocabulary Notebook**

The list also contains a virtual default notebook:

| Field | Value |
|---|---|
| `id` | `-1` |
| `name` | `default` |
| `note` | Empty string |
| `tags` | Empty array |
| `createdat` | `2000-01-01T00:00:00Z` |
| `updatedat` | `2000-01-01T00:00:00Z` |

This notebook is generated by the server. It is not stored as a normal notebook record in the database.

The default notebook cannot be changed or deleted through the normal notebook APIs.

**Possible Errors**

| HTTP Status | API Code | Situation |
|---:|---:|---|
| 401 | 1102–1107 | Token or user identity check failed |
| 415 | 1005 | Unsupported content type |
| 500 | 9001 | Database operation failed |
| 500 | 9002 | Unknown internal error |

### 4.2 Search Vocabulary Notebooks by Tag

**Endpoint**

```http
POST /api/v1/vocabulary-notebooks/search-by-tag
```

**Authentication:** Required.

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "tag": "N2"
  }
}
```

| Field | Type | Required | Description |
|---|---|---|---|
| `data.tag` | string | Yes | Tag to search for |

**Success Response**

HTTP Status Code: `200`

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

The search uses an exact tag match, not a partial match.

The virtual default notebook is not included in this search.

When no notebook matches the tag, the API still returns success with an empty list:

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

**Possible Errors**

| HTTP Status | API Code | Situation |
|---:|---:|---|
| 400 | 1001 | `data` is empty |
| 400 | 1002 | Invalid JSON |
| 401 | 1102–1107 | Token or user identity check failed |
| 415 | 1005 | Unsupported content type |
| 500 | 9001 | Database operation failed |
| 500 | 9002 | Unknown internal error |

### 4.3 Create Vocabulary Notebook

**Endpoint**

```http
POST /api/v1/vocabulary-notebooks/create
```

**Authentication:** Required.

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

| Field | Type | Required | Description |
|---|---|---|---|
| `data.name` | string | Yes | Notebook name |
| `data.note` | string / null | No | Note |
| `data.tags` | string[] | Yes | Tag list |

The server creates the notebook ID, user ownership, and time fields. A new notebook starts with an empty word list.

**Success Response**

HTTP Status Code: `200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**Possible Errors**

| HTTP Status | API Code | Situation |
|---:|---:|---|
| 400 | 1001 | `data` is empty |
| 400 | 1002 | Invalid JSON or required fields cannot be parsed |
| 401 | 1102–1107 | Token or user identity check failed |
| 415 | 1005 | Unsupported content type |
| 500 | 9001 | Database operation failed |
| 500 | 9002 | Unknown internal error |

The current code does not fully check the notebook name or tag contents. Clients should avoid sending an empty name or meaningless tags.

### 4.4 Update Vocabulary Notebook

**Endpoint**

```http
POST /api/v1/vocabulary-notebooks/update
```

**Authentication:** Required.

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

| Field | Type | Required | Description |
|---|---|---|---|
| `data.id` | integer | Yes | Notebook ID |
| `data.name` | string | Yes | New notebook name |
| `data.note` | string / null | No | New note |
| `data.tags` | string[] | Yes | New tag list |

**Success Response**

HTTP Status Code: `200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**Update Rules**

- Only notebooks owned by the current user can be updated.
- `name`, `note`, and `tags` are updated using the request data.
- The existing word list is kept.
- The original creation time is kept.
- The update time is set by the server.

This API does not support partial updates. Clients should send all required fields.

**Possible Errors**

| HTTP Status | API Code | Situation |
|---:|---:|---|
| 400 | 1001 | `data` is empty |
| 400 | 1002 | Invalid JSON |
| 401 | 1102–1107 | Token or user identity check failed |
| 415 | 1005 | Unsupported content type |
| 500 | 9001 | Database operation failed |
| 500 | 9002 | Notebook does not exist, does not belong to the current user, or another internal error |

The current implementation maps some "not found" and "not owned by current user" cases to `9002`.

### 4.5 Delete Vocabulary Notebook

**Endpoint**

```http
POST /api/v1/vocabulary-notebooks/delete
```

**Authentication:** Required.

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "id": 1
  }
}
```

| Field | Type | Required | Description |
|---|---|---|---|
| `data.id` | integer | Yes | Notebook ID |

**Success Response**

HTTP Status Code: `200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**Delete Rules**

Deleting a notebook does not delete the words inside it. Word data is stored separately. This operation only deletes the notebook and its word links.

**Possible Errors**

| HTTP Status | API Code | Situation |
|---:|---:|---|
| 400 | 1001 | `data` is empty |
| 400 | 1002 | Invalid JSON |
| 401 | 1102–1107 | Token or user identity check failed |
| 415 | 1005 | Unsupported content type |
| 500 | 9001 | Database operation failed |
| 500 | 9002 | Notebook does not exist, does not belong to the current user, or another internal error |

### 4.6 Delete Multiple Vocabulary Notebooks

**Endpoint**

```http
POST /api/v1/vocabulary-notebooks/delete-batch
```

**Authentication:** Required.

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

| Field | Type | Required | Description |
|---|---|---|---|
| `data.notebooks` | object[] | Yes | List of notebooks to delete |
| `data.notebooks[].id` | integer | Yes | Notebook ID |

**When All Items Succeed**

HTTP Status Code: `200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK",
  "data": null
}
```

**When One or More Items Fail**

HTTP Status Code: `500`

API Code: `9000`

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

Each failure item contains:

| Field | Type | Description |
|---|---|---|
| `id` | integer / null | Notebook ID that failed |
| `errormessage` | string | Failure reason |
| `errorcategory` | string / null | Error category |

Current error categories may include `None`, `Database`, and `Unknown`.

**Possible Errors**

| HTTP Status | API Code | Situation |
|---:|---:|---|
| 400 | 1001 | `data` is empty |
| 400 | 1002 | Invalid JSON |
| 401 | 1102–1107 | Token or user identity check failed |
| 415 | 1005 | Unsupported content type |
| 500 | 9000 | At least one item in the batch failed |
| 500 | 9001 / 9002 | Database or other internal error |

**Important**

Batch operations are not a single database transaction. Each item is processed separately. Successful deletions are not rolled back when another item fails.

When the API returns `9000`, clients should read `responseDataList` and query the notebook list again to check the final result.

### 4.7 Add a Word to a Vocabulary Notebook

**Endpoint**

```http
POST /api/v1/vocabulary-notebooks/addword-in-notebook
```

**Authentication:** Required.

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

| Field | Type | Required | Description |
|---|---|---|---|
| `data.wordid` | integer | Yes | Word ID |
| `data.vocabularynotebookid` | integer | Yes | Target notebook ID |

**Success Response**

HTTP Status Code: `200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**Rules**

- The word must exist and belong to the current user.
- The notebook must exist and belong to the current user.
- The same word cannot be added to the same notebook more than once.
- The notebook update time is changed after a successful operation.

**Possible Errors**

| HTTP Status | API Code | Situation |
|---:|---:|---|
| 400 | 1001 | `data` is empty |
| 400 | 1002 | Invalid JSON |
| 400 | 1004 | Word or notebook does not exist, does not belong to the current user, or the word is already in the notebook |
| 401 | 1102–1107 | Token or user identity check failed |
| 415 | 1005 | Unsupported content type |
| 500 | 9001 | Database operation failed |
| 500 | 9002 | Another internal error |

### 4.8 Remove a Word from a Vocabulary Notebook

**Endpoint**

```http
POST /api/v1/vocabulary-notebooks/removeword-in-notebook
```

**Authentication:** Required.

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

The fields are the same as the "Add a Word to a Vocabulary Notebook" API.

**Success Response**

HTTP Status Code: `200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**Rules**

- The word and notebook must exist and belong to the current user.
- Only the link between the word and the selected notebook is removed.
- The word itself is not deleted.
- If the word is not in the notebook, the current implementation still performs the update and returns success.

**Possible Errors**

| HTTP Status | API Code | Situation |
|---:|---:|---|
| 400 | 1001 | `data` is empty |
| 400 | 1002 | Invalid JSON |
| 400 | 1004 | Word or notebook does not exist, or does not belong to the current user |
| 401 | 1102–1107 | Token or user identity check failed |
| 415 | 1005 | Unsupported content type |
| 500 | 9001 | Database operation failed |
| 500 | 9002 | Another internal error |

---

## 5. Japanese Word APIs (Words)

### 5.1 Search Words by Text

**Endpoint**

```http
POST /api/v1/words/search
```

**Authentication:** Required.

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "query": "日本語"
  }
}
```

| Field | Type | Required | Description |
|---|---|---|---|
| `data.query` | string | Yes | Search text |

The search checks the following fields:

- `kannji`: Kanji.
- `kana`: Kana.
- `meaning`: Meaning.

The search uses partial matching and only returns words owned by the current user.

Spaces at the beginning and end of the search text are removed. An empty query or a query containing only whitespace is not accepted.

**Success Response**

HTTP Status Code: `200`

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
        "meaning": "Japanese language",
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

`data.words` contains the matched words.

Fields in a word object:

| Field | Type | Description |
|---|---|---|
| `id` | integer | Word ID |
| `kannji` | string / null | Kanji form |
| `kana` | string | Kana form |
| `akusennto` | integer / null | Japanese pitch accent |
| `meaning` | string | Meaning |
| `note` | string / null | Note |
| `vocabularynotebooks` | object[] / null | Notebooks containing this word |
| `createdat` | string | Creation time in UTC |
| `updatedat` | string | Update time in UTC |

Each item in `vocabularynotebooks` contains:

| Field | Type | Description |
|---|---|---|
| `id` | integer | Notebook ID |
| `name` | string | Notebook name |
| `tags` | string[] | Notebook tags |

`data.messages` contains extra problems that occur when the server builds the word response. Its item structure is:

```json
{
  "id": 100,
  "errormessage": "Failed to read notebook information",
  "errorcategory": "Database"
}
```

If the server cannot get the notebook information for a word, that word may not appear in `words`. The related error is added to `messages`. The whole API may still return success.

**Search Order**

The current implementation sorts results in this order:

1. Exact matches first.
2. Higher match ratio first.
3. Earlier match position first.
4. Smaller word ID first.

The search is case-sensitive. There are no pagination or result-count parameters.

**No Matching Results**

If no word matches, the API still returns success with empty lists:

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

**Possible Errors**

| HTTP Status | API Code | Situation |
|---:|---:|---|
| 400 | 1001 | `data` is empty |
| 400 | 1002 | Invalid JSON |
| 400 | 1004 | Query is empty or contains only whitespace |
| 401 | 1102–1107 | Token or user identity check failed |
| 415 | 1005 | Unsupported content type |
| 500 | 9001 | Database operation failed |
| 500 | 9002 | Another internal error |

### 5.2 Search Words by Notebook

**Endpoint**

```http
POST /api/v1/words/search-by-notebook
```

**Authentication:** Required.

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "id": 1
  }
}
```

| Field | Type | Required | Description |
|---|---|---|---|
| `data.id` | integer | Yes | Notebook ID |

**Special Rule**

When `data.id` is `-1`, the server returns all words owned by the current user.

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "id": -1
  }
}
```

For any other ID, the server checks whether the notebook exists and belongs to the current user before returning its words.

**Success Response**

The response structure is the same as the "Search Words by Text" API:

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
        "meaning": "Japanese language",
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

**Possible Errors**

| HTTP Status | API Code | Situation |
|---:|---:|---|
| 400 | 1001 | `data` is empty |
| 400 | 1002 | Invalid JSON |
| 401 | 1102–1107 | Token or user identity check failed |
| 415 | 1005 | Unsupported content type |
| 500 | 9001 | Database operation failed |
| 500 | 9002 | Notebook does not exist, does not belong to the current user, or another internal error |

### 5.3 Create Word

**Endpoint**

```http
POST /api/v1/words/create
```

**Authentication:** Required.

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "kannji": "日本語",
    "kana": "にほんご",
    "akusennto": 0,
    "meaning": "Japanese language",
    "note": ""
  }
}
```

| Field | Type | Required | Description |
|---|---|---|---|
| `data.kannji` | string / null | No | Kanji form |
| `data.kana` | string | Yes | Kana form |
| `data.akusennto` | integer / null | No | Pitch accent |
| `data.meaning` | string | Yes | Meaning |
| `data.note` | string / null | No | Note |

**Success Response**

HTTP Status Code: `200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

The server creates the word ID, user ownership, creation time, and update time.

Creating a word does not automatically link it to a saved notebook. Use the notebook link API to create that link when needed.

**Possible Errors**

| HTTP Status | API Code | Situation |
|---:|---:|---|
| 400 | 1001 | `data` is empty |
| 400 | 1002 | Invalid JSON or required fields cannot be parsed |
| 401 | 1102–1107 | Token or user identity check failed |
| 415 | 1005 | Unsupported content type |
| 500 | 9001 | Database operation failed |
| 500 | 9002 | Another internal error |

### 5.4 Update Word

**Endpoint**

```http
POST /api/v1/words/update
```

**Authentication:** Required.

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "id": 100,
    "kannji": "日本語",
    "kana": "にほんご",
    "akusennto": 0,
    "meaning": "Japanese language",
    "note": "Updated note"
  }
}
```

| Field | Type | Required | Description |
|---|---|---|---|
| `data.id` | integer | Yes | Word ID |
| `data.kannji` | string / null | No | Kanji form |
| `data.kana` | string | Yes | Kana form |
| `data.akusennto` | integer / null | No | Pitch accent |
| `data.meaning` | string | Yes | Meaning |
| `data.note` | string / null | No | Note |

**Success Response**

HTTP Status Code: `200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**Update Rules**

- The word must exist and belong to the current user.
- `id` identifies the word to update.
- Other fields are updated using the request data.
- The original creation time is kept.
- The update time is set by the server.

This API does not support partial updates. Clients should send all required fields.

**Possible Errors**

| HTTP Status | API Code | Situation |
|---:|---:|---|
| 400 | 1001 | `data` is empty |
| 400 | 1002 | Invalid JSON or required fields cannot be parsed |
| 401 | 1102–1107 | Token or user identity check failed |
| 415 | 1005 | Unsupported content type |
| 500 | 9001 | Database operation failed |
| 500 | 9002 | Word does not exist, does not belong to the current user, or another internal error |

The current implementation maps some "not found" and "not owned by current user" cases to `9002`.

### 5.5 Delete Word

**Endpoint**

```http
POST /api/v1/words/delete
```

**Authentication:** Required.

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN",
  "data": {
    "id": 100
  }
}
```

| Field | Type | Required | Description |
|---|---|---|---|
| `data.id` | integer | Yes | Word ID |

**Success Response**

HTTP Status Code: `200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

**Delete Rules**

Before deleting a word, the server searches for notebooks that contain it and tries to remove the related links. It then deletes the word record.

If an error occurs while removing the links, the server stops the deletion and returns an error.

**Possible Errors**

| HTTP Status | API Code | Situation |
|---:|---:|---|
| 400 | 1001 | `data` is empty |
| 400 | 1002 | Invalid JSON |
| 401 | 1102–1107 | Token or user identity check failed |
| 415 | 1005 | Unsupported content type |
| 500 | 9001 | Database operation failed, including an error while removing links |
| 500 | 9002 | Word does not exist, does not belong to the current user, or another internal error |

Removing links and deleting the word are not a single transaction. If link removal succeeds but word deletion fails, the word may no longer belong to any notebook.

### 5.6 Delete Multiple Words

**Endpoint**

```http
POST /api/v1/words/delete-batch
```

**Authentication:** Required.

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

| Field | Type | Required | Description |
|---|---|---|---|
| `data.words` | object[] | Yes | List of words to delete |
| `data.words[].id` | integer | Yes | Word ID |

**When All Items Succeed**

HTTP Status Code: `200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK",
  "data": null
}
```

**When One or More Items Fail**

HTTP Status Code: `500`

API Code: `9000`

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

Each failure item contains:

| Field | Type | Description |
|---|---|---|
| `id` | integer / null | Word ID that failed |
| `errormessage` | string | Failure reason |
| `errorcategory` | string / null | Error category |

**Possible Errors**

| HTTP Status | API Code | Situation |
|---:|---:|---|
| 400 | 1001 | `data` is empty |
| 400 | 1002 | Invalid JSON |
| 401 | 1102–1107 | Token or user identity check failed |
| 415 | 1005 | Unsupported content type |
| 500 | 9000 | At least one item in the batch failed |
| 500 | 9001 / 9002 | Other database or internal error |

**Important**

Batch deletion is not a single transaction. Each word is processed separately. Some words may have been deleted even when other words fail.

When the API returns `9000`, clients should read the failure list and search the related words again. Do not assume that all words failed to delete.

---

## 6. Server Status (Status)

### 6.1 Heartbeat Detection

**Endpoint**

```http
POST /api/v1/status/heartbeat-detection
```

**Authentication:** Required.

**Request Body**

```json
{
  "token": "YOUR_JWT_TOKEN"
}
```

This API does not need `data`.

**Success Response**

HTTP Status Code: `200`

```json
{
  "success": true,
  "code": 0,
  "message": "OK"
}
```

This API checks the Token and the current user's status in the database. It checks whether:

- The user exists.
- The user is enabled.
- The user's role in the database matches the role in the Token.

Therefore, this API does more than check whether the HTTP server is running. It also checks whether the current user can still use the service.

**Possible Errors**

| HTTP Status | API Code | Situation |
|---:|---:|---|
| 401 | 1102 | Token is missing |
| 401 | 1103 | Token is invalid |
| 401 | 1104 | Token has expired |
| 401 | 1105 | Token is not valid yet |
| 401 | 1106 | Token Claims are invalid |
| 401 | 1107 | User does not exist, is disabled, or has a different role |
| 500 | 9001 | Database error while checking user identity |
| 500 | 9002 | Another internal error |
| 415 | 1005 | Unsupported content type |

---

## 7. API Code Reference

The following codes are currently defined by the server.

### 7.1 Success

| Code | Meaning |
|---:|---|
| `0` | Operation succeeded |

### 7.2 General Request Errors

| Code | Meaning |
|---:|---|
| `1000` | Invalid request parameters |
| `1001` | Request body is empty |
| `1002` | Invalid JSON format |
| `1003` | Required parameter is missing |
| `1004` | Parameter value is invalid |
| `1005` | Unsupported content type |

### 7.3 Authentication Errors

| Code | Meaning |
|---:|---|
| `1100` | Authentication failed |
| `1101` | Incorrect username or password |
| `1102` | Token is missing |
| `1103` | Token is invalid |
| `1104` | Token has expired |
| `1105` | Token is not valid yet |
| `1106` | Token Claims are invalid |
| `1107` | User identity is no longer valid |

### 7.4 Permission Errors

| Code | Meaning |
|---:|---|
| `1200` | Not allowed to perform this operation |
| `1201` | Administrator permission is required |
| `1202` | Not allowed to access this resource |

### 7.5 Server Internal Errors

| Code | Meaning |
|---:|---|
| `9000` | Server internal error; also used when a batch operation has failed items |
| `9001` | Database operation failed |
| `9002` | Unknown internal error |
| `9003` | Server configuration error |
| `9004` | Internal processing failed |

**Note**

The list above contains the codes currently defined in the server. Not every code is used by the current APIs.

The error handling for the `2000–4000` code ranges is not yet fully implemented. Do not assume that these codes are returned by the current version.

Clients should use the API Code instead of trying to identify errors from the message text.

---

## 8. Current Features and Limitations

### 8.1 User Management

The current version provides login, Token validation, and password changes for the current user.

The following features are not yet available through the current HTTP APIs:

- Creating accounts for regular users.
- Creating other users as an administrator.
- Deleting, disabling, or enabling other users as an administrator.
- Changing another user's password as an administrator.

### 8.2 Learning and Practice

The current version mainly provides word and notebook management.

Learning records, score statistics, and complete server APIs for vocabulary practice have not yet been implemented.

### 8.3 Batch Operations

The current code registers batch deletion APIs for words and vocabulary notebooks.

Request data types for batch word creation and batch word updates are also present in the code. However, the current `ServerInit` does not register HTTP routes for these operations. They are not available as public APIs.

### 8.4 Data Ownership

Each word and vocabulary notebook belongs to a user.

The server gets the current user from the Token and checks data ownership when performing operations. Clients should not send `userid` to choose the owner of data.

### 8.5 Pagination

The current search APIs do not provide pagination parameters. All matching results are returned in one response.

### 8.6 Input Validation and Error Handling

Some input checks are not yet complete in the current demo.

Some cases, such as a missing word or notebook, or data that does not belong to the current user, may be returned as `9002` with HTTP Status Code `500`.

When a batch operation has some failed items, the server returns `9000` with the failure list, even if other items have already been processed successfully.

Clients should check the API Code and response data. When needed, they should query the data again to confirm the final result.

---

## 9. API List

| Group | HTTP Method | URL |
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

The current version provides **17 registered HTTP API endpoints**.