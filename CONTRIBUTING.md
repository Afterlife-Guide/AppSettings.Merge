# Contributing

## Branch Naming

Our branch naming convention is as follows:
* `feature/issue-<issue number>-<short description>`
* Please add **NO NUMBERS** to the `<short description>` part of the branch name. As this will mess up the issue tracking.

## Pull Requests

After forking the repository please create a pull request before creating the
fix. This way we can talk about how the fix will be implemented. This will
greatly increase your chance of your patch getting merged into the code base.

Please create all pull requests from the `develop` branch.

### Commit Template

Please run the following to make sure you commit messages conform to the project
standards.

```bash
git config --local commit.template .gitmessage
```