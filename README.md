# Ollama Manager

A web-based Ollama model management application developed with OpenSilver.

This project also includes a WPF desktop version migrated from the OpenSilver version, allowing developers to compare and learn from the development experience across both platforms.

<img width="773" height="495" alt="OllamaManager" src="https://github.com/user-attachments/assets/2decdf5b-066f-478b-8f0e-0fd1576f4af0" />


## Table of Contents

- [Screenshots](#screenshots)
- [Key Features](#key-features)
- [Project Structure](#project-structure)
- [Tech Stack](#tech-stack)
- [Server Architecture](#server-architecture)
- [Getting Started](#getting-started)
- [Development Roadmap](#development-roadmap)
- [Contributing](#contributing)
- [License](#license)
- [Notes](#notes)

## Screenshots

Manage Ollama models easily with an intuitive interface and chat with them in real-time.

| Main Interface | Chat Interface |
|----------------|----------------|
| ![Main Interface](https://github.com/user-attachments/assets/8c3bcfc6-ae3f-4d58-9cce-f18285506f1c) | ![Chat Interface](https://github.com/user-attachments/assets/1daeb5bd-a1d9-4cd0-bc15-3fd779950a4b) |

## Key Features

- View installed model list
- Start/stop models
- Real-time chat
- Model status monitoring

## Project Structure

```
src/
├── client-opensilver/    # OpenSilver web client
├── client-wpf/          # WPF desktop client
└── server-minimalapi/   # Shared backend server
```

## Tech Stack

- **Web Client**: OpenSilver (.NET Standard 2.0)
- **Desktop Client**: WPF (.NET 9.0)
- **Backend**: ASP.NET Core Minimal API (.NET 9.0)
- **Real-time Communication**: SignalR

## Server Architecture

### Minimal API Structure
- **GET** `/api/models` - Retrieve installed models list and status
- **POST** `/api/models/{modelName}/start` - Start model
- **POST** `/api/models/{modelName}/stop` - Stop model
- **POST** `/api/chat` - Chat with model

### Ollama API Integration
The backend server manages models using the Ollama API:
- `http://localhost:11434/api/tags` - Installed models list
- `http://localhost:11434/api/ps` - Check running models
- `http://localhost:11434/api/generate` - Model load/unload and chat

### Real-time Monitoring
- Real-time model status updates via SignalR Hub
- Background service for detecting model status changes

## Getting Started

### Prerequisites
- **Ollama**: Install from [ollama.com](https://ollama.com)
- **Model Installation**: Example: run `ollama pull llama3.2`
- **Visual Studio 2022**
- **.NET 9.0 SDK**
- **WASM Tools**: `dotnet workload install wasm-tools`
- **OpenSilver SDK**: Download `OpenSilver_SDK_v3.2.0.4.vsix` from [www.opensilver.net](https://www.opensilver.net) and install

### Server Execution

First, run the backend server:
```bash
cd src/server-minimalapi/LocalLLMServer
dotnet run --launch-profile https
```
The server will run at `https://localhost:7262`.

### OpenSilver Web Client

```bash
cd src/client-opensilver/OllamaHub.Browser
dotnet run
```

Access via browser at `http://localhost:55592`

### WPF Version

You can also run the WPF version using the same server:
```bash
cd src/client-wpf/OllamaHub
dotnet run
```

## Development Roadmap

### Currently Implemented
- Model list retrieval
- Model control (start/stop)
- Real-time chat

### Planned Features
- Model downloads
- Model deletion
- Multi-session chat
- Performance monitoring

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Create a Pull Request

## License

MIT License

## Notes

- This is an excellent example for verifying code compatibility between WPF and OpenSilver
- OpenSilver is based on .NET Standard 2.0 and provides a WPF-like development experience on the web
