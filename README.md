# How to write SOLID code

The way to design and write understandable, flexible, and maintainable code is rough. SOLID is the best way to reduce complexity and save ourselves a lot of headaches as our applications grow in size and functionalities.

The journey will start from the dark side of Pasta coding to move into the Eden of the SOLID principles applied to an existing codebase.

- Spaghetti coding: a codebase that violates several SOLID principles
- Step by step introduction of the SOLID principles
- Full adherence to SOLID principles on the whole codebase

## Requirements

- Dotnet SDK 6.0

To install SDK, please refer to [official microsoft download page](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## How to run

Open a terminal on current folder. To run a project use command:
  `dotnet run --project <project folder>/<project name>.csproj`

  ex. `dotnet run --project SolidPlayground-D/SolidPlayground-D.csproj`

To stop exectution press `ctrl + c` with focus on terminal window.

## Structure

SolidPlayground solution is composed by many projects. They can be split into categories:
 - **dependencies** to centralize some operations and to provide a mocked messaging library: 
    - `Core`
    - `Infrastructure` 
    - `MessagesFramework`
 - **proposed implementations** to see how a codebase can improve when applying SOLID principles (starting from messy SpaghettiCode project):
   - `SolidPlayground-SpaghettiCode`
   - `SolidPlayground-S`
   - `SolidPlayground-O`
   - `SolidPlayground-L`
   - `SolidPlayground-I`
   - `SolidPlayground-D`
   - `SolidPlayground-SOLID`
 - a **playground project** to experiment with the code:
    - `SolidPlayground`

## Contributing

See [Contributing](https://github.com/msc-technology/tech-speeches/blob/main/CONTRIBUTING.md) for information, guidelines, and making pull requests to contribute to this repository.
