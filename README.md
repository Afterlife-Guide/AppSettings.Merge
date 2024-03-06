# AppSettings.Merge GitHub Action for Blazor WebAssembly

This action merges the `appsettings.json` and `appsettings.{environment}.json` files from the source branch. It then
removes all other appsettings files from your Blazor WebAssembly project.

This is to deal with the issue with Blazor WebAssembly all appsettings files are copied to the output directory. Which
leaks all your appsettings files for every environment to the client.

## Usage

After you have published your Blazor WebAssembly project, you can use this action to merge the appsettings files.

```yaml
    - name: Combine Appsettings Files for environment
      uses: Afterlife-Guide/AppSettings.Merge@0.1.3.4
      with:
        app-environment: 'Production'
        path: '/web-publish/wwwroot/'
```

### Inputs

#### `app-environment`
A `string` of the environment to merge the appsettings files for.

#### `path`
A `string` of the path to the `wwwroot` directory of your Blazor WebAssembly project.

## Change Log

The change log can be found [here](CHANGELOG.md).

## License

[Apache 2.0](LICENSE.txt)

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.