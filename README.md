# Ollama Manager
A web application for managing Ollama models, developed with OpenSilver.
This project also provides a desktop version migrated from the OpenSilver version to WPF, allowing you to compare and learn the development experience between the two platforms.

## Table of Contents
- [Screenshots](#screenshots)
- [Key Features](#key-features)
- [Project Structure](#project-structure)
- [Tech Stack](#tech-stack)
- [Server Architecture](#server-architecture)
- [How to Run](#how-to-run)
- [Development Roadmap](#development-roadmap)
- [Contributing](#contributing)
- [License](#license)
- [Notes](#notes)

## Screenshots
Easily manage Ollama models and chat in real-time with an intuitive interface.

| Main Screen | Chat Screen |
|-------------|-------------|
| ![Main Screen](https://github.com/user-attachments/assets/8c3bcfc6-ae3f-4d58-9cce-f18285506f1c) | ![Chat Screen](https://github.com/user-attachments/assets/1daeb5bd-a1d9-4cd0-bc15-3fd779950a4b) |

## Key Features
- View installed model list
- Start/Stop models
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
- **GET** `/api/models` - Retrieve installed model list and status
- **POST** `/api/models/{modelName}/start` - Start model
- **POST** `/api/models/{modelName}/stop` - Stop model
- **POST** `/api/chat` - Chat with model

### Ollama API Integration
The backend server manages models using the Ollama API:
- `http://localhost:11434/api/tags` - List of installed models
- `http://localhost:11434/api/ps` - Check running models
- `http://localhost:11434/api/generate` - Model load/unload and chat

### Real-time Monitoring
- Real-time model status updates via SignalR Hub
- Background service to detect model status changes

## How to Run

### Prerequisites
- .NET 9.0 SDK
- Ollama installed and running

### Web Version (OpenSilver)
1. Run the backend server
```bash
cd src/server-minimalapi
dotnet run
```

2. Run the web client
```bash
cd src/client-opensilver
dotnet run
```

3. Access `https://localhost:5001` in your browser

### Desktop Version (WPF)
1. Run the backend server (same as above)
2. Run the WPF application
   - Execute `dotnet run` in the `src/client-wpf` folder

## Development Roadmap

### Currently Implemented
- Model list retrieval
- Model control (start/stop)
- Real-time chat

### Planned Features
- Model download
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
- This is an excellent example to verify code compatibility between WPF and OpenSilver
- OpenSilver is based on .NET Standard 2.0 and provides a WPF-like development experience on the web
