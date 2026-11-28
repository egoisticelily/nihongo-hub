# Security Policy

## Project Security Status

NihongoHub Server is currently in the Demo development stage. It has not undergone a complete security audit and is not guaranteed to be suitable for production environments or systems that store important data.

The maintainer takes security reports seriously. However, this project is maintained by a single developer, so fixed response times and resolution deadlines cannot be guaranteed.

## Reporting a Vulnerability

If you discover a potential security vulnerability in NihongoHub Server, please report it through either of the following private channels.

### Option 1: GitHub Private Vulnerability Reporting

Use **Report a vulnerability** on this repository's **Security / Advisories** page to submit a private vulnerability report through GitHub.

### Option 2: Email

Send your report to:

**egoisticelily@gmail.com**

Suggested email subject:

`[NihongoHub Security] Brief description of the issue`

Please do not disclose unpatched vulnerabilities through public Issues, Pull Requests, or other public channels.

## Information to Include

To help the maintainer evaluate and reproduce the issue, please include as much of the following information as possible:

- The affected branch, version, or Git commit.
- The environment, such as the operating system, .NET runtime version, and deployment method.
- The affected API, feature, or source code location.
- The conditions and steps required to reproduce the issue.
- The potential impact, such as unauthorized access, data disclosure, or privilege escalation.
- Relevant verification materials or proof-of-concept code.

Before submitting logs, configuration snippets, or other materials, remove passwords, JWTs, secret keys, user data, and any other sensitive information.

## Handling and Disclosure

After receiving a report, the maintainer will assess the issue based on its reproducibility, impact, and actual risk.

Please do not publicly disclose vulnerability details before the maintainer has had a reasonable opportunity to investigate and address the issue. Do not use a vulnerability to access, modify, or damage other people's data or services.

Confirmed issues may be addressed through code changes, documentation updates, or security advisories. As the project is maintained by a single developer, a response or fix within a specific timeframe cannot be guaranteed.

## Scope

This policy primarily covers the NihongoHub Server source code and official project content maintained in this repository.

The security of forks, modified versions, and services deployed and operated by third parties is the responsibility of their respective maintainers or operators, including the security of their code changes, runtime environments, configuration, and operations.

For issues that exist only in third-party modifications or deployments, please contact the relevant maintainer or service operator.