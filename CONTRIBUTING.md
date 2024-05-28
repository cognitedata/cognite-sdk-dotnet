# Contributing to Cognite .NET SDK

Thank you for considering contributing to the Cognite .NET SDK! We welcome contributions from the community to help improve the SDK. Below are the guidelines for contributing to the project.

## Development Instructions

### Setup

Get the code!

```bash
git clone https://github.com/cognitedata/cognite-sdk-dotnet.git
cd cognite-sdk-dotnet
```

We use [Paket](https://fsprojects.github.io/Paket/) for dependency management. Ensure you have .NET SDK installed.

Install dependencies and initialize the environment with these commands:
```bash
dotnet tool restore
dotnet paket install
```

### Getting access to the test CDF project for running integration tests
- Request access to the appropriate AAD tenant.
- Set environment variables listed below. The READ credentials should be for the `publicdata` project.
  - `TEST_TENANT_ID_WRITE`
  - `TEST_CLIENT_ID_WRITE`
  - `TEST_CLIENT_SECRET_WRITE`
  - `TEST_TENANT_ID_READ`
  - `TEST_CLIENT_ID_READ`
  - `TEST_CLIENT_SECRET_READ`

### Testing
If you have appropriate credentials (see Environment Variables above), you can run the integration tests:
```bash
sh ./test.sh
```

### Releasing

The SDK uses [Semantic versioning](https://semver.org/). To release a new version of the SDK, update the [version](https://github.com/cognitedata/cognite-sdk-dotnet/blob/master/version) file to contain the new version and nothing else.

This will automatically create a new tag and release when merged to master. A new version will be released if there is no tag with the same name as the version given in the version file.