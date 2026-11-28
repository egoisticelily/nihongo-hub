# Contributing

Thank you for your interest in NihongoHub Server.

This project is currently in the Demo development stage and is maintained by a single developer. Issues and suggestions are welcome. Pull Requests are also welcome, but all external contributions are subject to maintainer review and are not guaranteed to be merged.

## Reporting Issues

When submitting an Issue, please include as much of the following information as possible:

- The feature or API involved.
- The version, branch, or Git commit you are using.
- Your runtime environment.
- Steps to reproduce the issue.
- The actual result and the expected result.

Do not include passwords, JWTs, secret keys, certificates, user data, or other sensitive information in public Issues.

For security vulnerabilities, follow the private reporting instructions in [SECURITY.md](SECURITY.md).

## Submitting Pull Requests

For larger changes, consider opening an Issue first to discuss the purpose and proposed implementation. This helps avoid duplicated work and changes that conflict with the project's design direction.

When submitting a Pull Request:

- Explain the purpose and scope of the changes.
- Keep the changes focused and avoid unrelated refactoring.
- Follow the existing code style and naming conventions.
- Clearly describe changes to APIs, data structures, and configuration.
- Provide appropriate verification steps and state what you have actually tested.

The maintainer may request revisions or decline a contribution based on project plans or design decisions.

## Code and Security Requirements

Do not commit any of the following:

- Real user passwords or account credentials.
- JWT signing keys or other access secrets.
- Private keys, certificates, or certificate passwords.
- Personal data or real user databases.
- Unrelated build artifacts or runtime files.

Use fictional data for development and testing.

## Compatibility

The project is still in the Demo stage. APIs, data structures, and configuration formats may change.

Changes that affect APIs, data structures, or configuration formats must clearly describe their potential impact on existing clients and deployments.

## License

By submitting a contribution, you agree that it will be distributed under the terms of this project's MIT License.