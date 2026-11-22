# API Code Numbering Rules

> **Specification version:** 0.1.0 Beta  
> **Scope:** NihongoHub backend HTTP API

API Code is an application-level code defined by NihongoHub. It identifies the result of an API operation and the specific reason for an error, so that clients can recognize and handle the result.

API Code and HTTP Status Code are independent of each other. HTTP Status Code follows the HTTP standard and indicates how the HTTP request was handled. API Code indicates the specific result defined by NihongoHub. Clients should use API Code to determine the exact error and should not treat the two codes as interchangeable.

## 1. Numbering Rules

API Code uses integers. Number ranges are assigned to different functional modules.

| Number range | Category |
|---|---|
| `0` | Success |
| `1000–1099` | General request errors |
| `1100–1199` | Authentication errors |
| `1200–1299` | Permission errors |
| `2000–2099` | User management |
| `3000–3099` | Word management |
| `4000–4099` | Vocabulary notebook management |
| `5000–5099` | Learning and testing |
| `9000–9099` | Server internal errors |

Once a number has been assigned, its meaning must not be changed without a good reason. Numbers not listed here are reserved for future features.

## 2. Current API Codes

### 2.1 Success

| Code | Meaning |
|---:|---|
| `0` | Operation succeeded |

### 2.2 General Request Errors (1000–1099)

| Code | Meaning |
|---:|---|
| `1000` | Invalid request parameters |
| `1001` | Request body is empty |
| `1002` | Invalid JSON format |
| `1003` | Required parameter is missing |
| `1004` | Parameter value is invalid |
| `1005` | Unsupported request format |

### 2.3 Authentication Errors (1100–1199)

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

Additional rules:

- During login, an unknown username and an incorrect password both return `1101`. The specific reason is not disclosed to the client.
- If a user is disabled or deleted, or if the user's role in the database does not match the role in the Token, authentication fails and `1107` is returned.
- A database query error is not an authentication failure. Use a server internal error code instead.

### 2.4 Permission Errors (1200–1299)

| Code | Meaning |
|---:|---|
| `1200` | Not allowed to perform this operation |
| `1201` | Administrator permission is required |
| `1202` | Not allowed to access this resource |

### 2.5 User Management (2000–2099)

| Code | Meaning |
|---:|---|
| `2000` | User operation failed |
| `2001` | User does not exist |
| `2002` | Username is already in use |
| `2003` | User is disabled |
| `2004` | Invalid user data |
| `2005` | This operation is not allowed for this user |

During login, do not return `2003` directly to report an account status. Handle the situation according to the authentication error rules instead.

### 2.6 Word Management (3000–3099)

| Code | Meaning |
|---:|---|
| `3000` | Word operation failed |
| `3001` | Word does not exist |
| `3002` | Invalid word data |
| `3003` | Not allowed to operate on this word |
| `3004` | Word-related operation failed |

### 2.7 Vocabulary Notebook Management (4000–4099)

| Code | Meaning |
|---:|---|
| `4000` | Vocabulary notebook operation failed |
| `4001` | Vocabulary notebook does not exist |
| `4002` | Invalid vocabulary notebook data |
| `4003` | Not allowed to operate on this vocabulary notebook |
| `4004` | Vocabulary notebook-related operation failed |
| `4005` | The word is already linked to the vocabulary notebook |

### 2.8 Learning and Testing (5000–5099)

This number range is reserved for now and is not in use. The codes will be defined when the learning and testing features are developed.

### 2.9 Server Internal Errors (9000–9099)

| Code | Meaning |
|---:|---|
| `9000` | Server internal error |
| `9001` | Database operation failed |
| `9002` | Unknown error |
| `9003` | Server configuration error |
| `9004` | Internal processing failed |

Details about server internal errors must only be recorded in server-side logs. API responses must not expose exception stack traces, database paths, or other internal implementation details.

## 3. Usage Rules

- `code = 0` means the operation succeeded. A non-zero value indicates a specific error or failure.
- The meaning of a Code must always stay the same. The same Code must not be used for a different meaning in another API.
- When a batch operation partially succeeds, the response format and error details must be defined separately by the relevant API.
