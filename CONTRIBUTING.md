# Contributing Guidelines

Write commits according to: <https://chris.beams.io/posts/git-commit/>

Follow these steps when creating a pull request:

0. Update the [CHANGELOG.md](./CHANGELOG.md)
1. `git add <your changes>`
2. `git commit -m"<your commit message>"`
3. `git rebase master -i`
    During rebase:
    - Squash all commits into one.
    - Create a short commit message describing the pull request
    - Write a longer description of the pull request under according to
      <https://chris.beams.io/posts/git-commit/.>
4. `git push -f`

If you have to fix something in your pull request:

1. `git add <your changes>`
2. `git commit --amend` or `git rebase master -i`
3. `git push -f`

Prerequisites:
run
`git config --global alias.pushf "push --force-with-lease"`
for safer force pushing.
Also make sure master is up to date.

Since we are force pushing, you should never push to master or to someone elses branch.

## Releasing

The SDK uses [Semantic versioning](https://semver.org/). To release a new version of the SDK, update the [version](https://github.com/cognitedata/cognite-sdk-dotnet/blob/master/version) file to contain the new version and nothing else.

This will automatically create a new tag and release when merged to master. A new version will be released if there is no tag with the same name as the version given in the version file.
