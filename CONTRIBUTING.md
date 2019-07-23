# Contributing Guidelines

Write commits according to: <https://chris.beams.io/posts/git-commit/>

Follow these steps when creating a pull request:

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
