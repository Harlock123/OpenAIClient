# AGENTS.md

Notes for anyone, human or AI agent, changing this code. There is no README, so this file explains what is here.

## What this is

A small .NET 8 console experiment in calling a local language model over HTTP. Despite the name, what runs is
`OllamaClient`. It sends one fixed prompt (write a 1000-word story) to an Ollama server's `/api/generate` with the
model `codellama:34b`, reads the streamed reply line by line, and prints the story. It uses only the base class
library: `HttpClient` and `System.Text.Json`, with no NuGet packages.

## Commands

```bash
dotnet build OpenAIClient.sln          # needs the .NET 8 SDK (net8.0)
dotnet run --project OpenAIClient      # needs Ollama on http://localhost:11434 with codellama:34b pulled
```

There are no arguments, settings or environment variables: the server address, model and prompt are in
`OllamaClient.cs`. There are no tests.

## Layout

- `OpenAIClient/Program.cs`: `Main` makes an `OllamaClient`, calls `GenerateStoryAsync` and prints the result.
- `OpenAIClient/OllamaClient.cs`: `OllamaClient` (base URL defaults to `http://localhost:11434`), with
  `OllamaRequest` and `OllamaResponse`.
  - The reply is newline-delimited JSON. Each line's `response` is appended until `done` is true.
  - Double line breaks then become single ones, and the text is trimmed.
- `OpenAIClient/OpenAIClient.cs`: an earlier generic client that posts `{ "prompt": ... }` to a given URL and reads
  a `Result` field. The `.csproj` excludes it from the build (`<Compile Remove="OpenAIClient.cs" />`), so it is
  kept only for reference.

## Rules that keep it working

- **Errors come back as text.** `GenerateStoryAsync` catches every exception and returns
  `Error generating story: ...` instead of throwing. Callers that need to tell failure from success must change
  that.
- **Keep `OllamaResponse`'s `[JsonPropertyName]` attributes.** Ollama's fields are lowercase, and
  `System.Text.Json` matches names case-sensitively by default.
- **`OpenAIClient.cs` is not compiled.** To use it, remove the `Compile Remove` line. It has no namespace and
  doesn't match Ollama's or OpenAI's API as written.
- **No API keys in the code.** Nothing here sends a key. If you add a hosted API, read the key from the environment
  or user secrets, never from a committed file.
