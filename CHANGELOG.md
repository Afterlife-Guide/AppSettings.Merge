# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Security

- Fixed BLOCKER security vulnerability (SonarQube githubactions:S8263) where user-controlled data was passed directly as action parameters in GitHub Actions workflows. 
- Refactored add-hotfix-metadata.yml, draft-new-release.yml, and gitstream.yml to use environment variables and validated step outputs. 
- Added input validation to prevent shell injection attacks.

## [0.3.2.19] - 2025-12-12

## [0.3.1.18] - 2025-12-11

### Fixed

- Bug with deployment (#324)

### Security

- Fixed command injection vulnerabilities in multiple GitHub Actions workflows by moving user-controlled data to environment variables instead of direct interpolation in run blocks. This affects the following workflows: draft-new-release.yml, blocked-issue.yml, completed-feature-workflow.yml, in-progress-feature-workflow.yml, branch-hotfix.yml, step-push-package.yml, step-tag-release.yml, step-build.yml, and step-version.yml (#315)

## [0.3.0.16] - 2025-11-18

### Changed

- Upgraded all projects and CI/CD pipelines from .NET 8 to .NET 10 (#299)

## [0.2.11.15] - 2025-11-17

## [0.2.10.14] - 2025-10-24

### Changed

- Migrated from `thomaseizinger/keep-a-changelog-new-release` to `baynezy/ChangeLogger.Action` (#217)
- Added GH_TOKEN environment variable to all relevant GitHub Action workflows (#222)
- Set up copilot environment (#230)

### Security

- Enhanced security by replacing tag-based GitHub Action references with full commit SHA hashes to prevent supply chain attacks (#243)
- Fixed command injection vulnerability in completed-feature-workflow.yml by using environment variables instead of direct interpolation of user-controlled data (#232)

### Fixed

- Changed lowercase "as" to uppercase "AS" in Dockerfile for SonarQube compliance (#234)
- Made ReplacePath method static in MergeService to address SonarQube rule S2325 (#235)

## [0.2.9.13] - 2025-04-11

## [0.2.8.12] - 2025-04-10

## [0.2.7.11] - 2025-02-13

## [0.2.6.10] - 2025-02-12

## [0.2.5.9] - 2025-01-16

## [0.2.4.8] - 2024-10-15

### Fixed

- Removed all usage of `set-output` in GitHub Actions.

## [0.2.3.7] - 2024-04-10

## [0.2.2.8] - 2024-04-05

## [0.2.1.6] - 2024-04-04

## [0.2.0.6] - 2024-04-04

## [0.1.4.5] - 2024-03-06

## [0.1.3.4] - 2023-11-17

[unreleased]: https://github.com/Afterlife-Guide/AppSettings.Merge/compare/0.3.2.19...HEAD
[0.3.2.19]: https://github.com/Afterlife-Guide/AppSettings.Merge/compare/0.3.1.18...0.3.2.19
[0.3.1.18]: https://github.com/Afterlife-Guide/AppSettings.Merge/compare/0.3.0.16...0.3.1.18
[0.3.0.16]: https://github.com/Afterlife-Guide/AppSettings.Merge/compare/0.2.11.15...0.3.0.16
[0.2.11.15]: https://github.com/Afterlife-Guide/AppSettings.Merge/compare/0.2.10.14...0.2.11.15
[0.2.10.14]: https://github.com/Afterlife-Guide/AppSettings.Merge/compare/0.2.9.13...0.2.10.14
[0.2.9.13]: https://github.com/Afterlife-Guide/AppSettings.Merge/compare/0.2.8.12...0.2.9.13
[0.2.8.12]: https://github.com/Afterlife-Guide/AppSettings.Merge/compare/0.2.7.11...0.2.8.12
[0.2.7.11]: https://github.com/Afterlife-Guide/AppSettings.Merge/compare/0.2.6.10...0.2.7.11
[0.2.6.10]: https://github.com/Afterlife-Guide/AppSettings.Merge/compare/0.2.5.9...0.2.6.10
[0.2.5.9]: https://github.com/Afterlife-Guide/AppSettings.Merge/compare/0.2.4.8...0.2.5.9
[0.2.4.8]: https://github.com/Afterlife-Guide/AppSettings.Merge/compare/0.2.3.7...0.2.4.8
[0.2.3.7]: https://github.com/Afterlife-Guide/AppSettings.Merge/compare/0.2.2.8...0.2.3.7
[0.2.2.8]: https://github.com/Afterlife-Guide/AppSettings.Merge/compare/0.2.1.6...0.2.2.8
[0.2.1.6]: https://github.com/Afterlife-Guide/AppSettings.Merge/compare/0.2.0.6...0.2.1.6
[0.2.0.6]: https://github.com/Afterlife-Guide/AppSettings.Merge/compare/0.1.4.5...0.2.0.6
[0.1.4.5]: https://github.com/Afterlife-Guide/AppSettings.Merge/compare/0.1.3.4...0.1.4.5
[0.1.3.4]: https://github.com/Afterlife-Guide/AppSettings.Merge/compare/b9c7c0d263bd538401345ed79e6de5f620e8ddc1...0.1.3.4
