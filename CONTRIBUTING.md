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

We use tags for versioning, the tag name needs to be a [legal nuget version number](https://docs.microsoft.com/en-us/nuget/reference/package-versioning)
e.g. 0.9.7 or 1.13.15.

To create new tags either do:

1. `git tag -a [tag name] -m [tag description]`
2. `git push origin [tag name]`

or use the "Releases" tab on github to add a tag.

Tags are not pushed by default, though you can use `git push --tags` in order to push tags,
make sure you do not push local tags unintentionally.

Tags are unique, and map directly to the semantic versioning number in nuget.

There will always be a build, if there is no specific tag for the commit in question, a tag will be generated on the form

`[latest tag in history]-build.[commits since last tag]`

You might have to restart the build in jenkins to deploy a build that already built without tag.

Note that tags are connected to commits, but if you create a merge commit merging into master then git may try to generate a new tag for you.
The easiest solution is to only tag master.
