## Prerequisites

- .NET 10 SDK

## Build

```
dotnet build
```

Run all commands from the solution root (`FramedMessage/`).

## Usage

Start the server:

```
dotnet run --project FramedMessage.Server -- <port> <output-file>
```

Send a file:

```
dotnet run --project FramedMessage.Client -- <input.bin> [host] [port]
```

Example:

```
dotnet run --project FramedMessage.Server -- 9001 output.txt
dotnet run --project FramedMessage.Client -- sample-input-ascii.bin localhost 9001
dotnet run --project FramedMessage.Client -- sample-input-hr.bin localhost 9001
```
