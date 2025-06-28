# Ollama Manager

🚀 **Modern Web-based Ollama Management Interface**

Ollama Manager is an open-source web application that provides an intuitive and modern interface for managing your Ollama models. Built with OpenSilver and real-time SignalR communication, it offers a seamless experience for interacting with local AI models.

![image](https://github.com/user-attachments/assets/8c3bcfc6-ae3f-4d58-9cce-f18285506f1c)

![image](https://github.com/user-attachments/assets/1daeb5bd-a1d9-4cd0-bc15-3fd779950a4b)


## ✨ Features

### 🎯 Currently Available
- **📋 Model Overview**: View all installed models with real-time status indicators
- **⚡ Model Control**: Start and stop models with a single click
- **💬 Interactive Chat**: Chat with your running models in real-time
- **🔄 Real-time Updates**: All operations sync instantly via SignalR

### 🔮 Coming Soon
- **📥 Model Downloads**: Download new models directly from the interface
- **🗑️ Model Management**: Remove unused models to free up space
- **🎭 Multi-Model Chat**: Manage separate chat sessions for different models
- **📊 Performance Monitoring**: View model resource usage and performance metrics
- **🎨 Customizable Themes**: Personalize your management experience
- **👥 Multi-User Support**: Share and collaborate with team members

## 🏗️ Architecture

```
OpenSilver Frontend ↔ SignalR Hub ↔ Minimal API ↔ Ollama API/CLI
```

### Technology Stack
- **Frontend**: OpenSilver (Silverlight for modern web)
- **Backend**: ASP.NET Core Minimal API
- **Real-time Communication**: SignalR
- **Target**: Ollama API & Command Line Interface

## 🚀 Quick Start

### Prerequisites
- [.NET 8.0 or later](https://dotnet.microsoft.com/download)
- [Ollama](https://ollama.ai/) installed and running
- Modern web browser with WebAssembly support

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/ollama-manager.git
   cd ollama-manager
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Open your browser**
   Navigate to `https://localhost:5001`

## 🗺️ Roadmap

### Phase 1: Core Foundation ✅
- [x] Model listing and status monitoring
- [x] Basic model start/stop functionality
- [x] Real-time chat interface
- [x] SignalR integration

### Phase 2: Enhanced Management 🚧
- [ ] Model download and installation
- [ ] Model deletion and cleanup
- [ ] Multi-session chat management
- [ ] Configuration management

### Phase 3: Advanced Features 📋
- [ ] Performance monitoring and analytics
- [ ] User authentication and profiles
- [ ] Team collaboration features
- [ ] Plugin system for extensions
- [ ] REST API for third-party integrations
- [ ] Docker containerization

### Phase 4: Enterprise Ready 🎯
- [ ] Multi-instance management
- [ ] Advanced security features
- [ ] Audit logging and compliance
- [ ] High availability deployment
- [ ] Advanced monitoring and alerting

## 🤝 Contributing

We welcome contributions from the community! This project thrives on collaboration and diverse perspectives.

### Ways to Contribute
- 🐛 **Bug Reports**: Found an issue? Let us know!
- ✨ **Feature Requests**: Have an idea? We'd love to hear it!
- 📝 **Documentation**: Help improve our docs
- 💻 **Code Contributions**: Fix bugs, add features, improve performance
- 🧪 **Testing**: Help us ensure quality across different environments
- 🎨 **UI/UX Design**: Make the interface even better

### Getting Started
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Commit your changes (`git commit -m 'Add amazing feature'`)
5. Push to the branch (`git push origin feature/amazing-feature`)
6. Open a Pull Request

### Development Setup
```bash
# Clone your fork
git clone https://github.com/your-username/ollama-manager.git

# Install dependencies
dotnet restore

# Run in development mode
dotnet run --environment Development
```

### Code Guidelines
- Follow C# coding conventions
- Add unit tests for new features
- Update documentation for API changes
- Ensure SignalR hubs are properly tested

## 🏷️ Good First Issues

New to the project? Look for issues labeled with:
- `good-first-issue`: Perfect for newcomers
- `help-wanted`: We need your expertise
- `documentation`: Improve our docs
- `enhancement`: Add new features

## 💬 Community

- **Discussions**: Use GitHub Discussions for questions and ideas
- **Issues**: Report bugs and request features via GitHub Issues
- **Wiki**: Check our Wiki for detailed documentation

## 📋 Requirements

- **.NET 8.0+**: For running the Minimal API backend
- **Ollama**: Must be installed and accessible
- **Modern Browser**: Chrome, Firefox, Safari, or Edge with WebAssembly support

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Ollama Team**: For creating an amazing local AI platform
- **OpenSilver Team**: For bringing Silverlight to the modern web
- **Contributors**: Everyone who helps make this project better

---

**⭐ Star this repository if you find it useful!**

**🤝 Join our community of contributors and help shape the future of Ollama management!**
