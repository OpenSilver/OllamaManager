# Ollama Manager

🚀 **Modern Web-based Ollama Management Interface**

Ollama Manager is an open-source web application that provides an intuitive and modern interface for managing your Ollama models. Built with OpenSilver and real-time SignalR communication, it offers a seamless experience for interacting with local AI models.

![image](https://github.com/user-attachments/assets/8c3bcfc6-ae3f-4d58-9cce-f18285506f1c)

![image](https://github.com/user-attachments/assets/1daeb5bd-a1d9-4cd0-bc15-3fd779950a4b)

## 🔄 Dual Platform Implementation

This project showcases the power of OpenSilver by providing **both OpenSilver and WPF implementations** side by side. This unique approach offers valuable insights for developers:

### 📁 Project Structure
```
src/
├── client-opensilver/    # OpenSilver web implementation
├── client-wpf/          # Traditional WPF desktop implementation  
└── server-minimalapi/   # Shared backend for both clients
```

### 🎯 For WPF Developers

**Discover OpenSilver's WPF Compatibility**: Compare the two implementations to see how OpenSilver maintains the familiar WPF development experience while targeting the web. This side-by-side comparison demonstrates:

- **Identical XAML Patterns**: See how your existing XAML knowledge translates directly to web development
- **Shared Development Concepts**: Data binding, commands, user controls, and MVVM patterns work the same way
- **Code Reusability**: Understand how much of your WPF codebase can be directly migrated to OpenSilver
- **Platform-Specific Adaptations**: Learn what minimal changes are needed when targeting the web

**Perfect Learning Resource**: Whether you're a WPF veteran curious about web development or exploring modern alternatives to traditional desktop apps, this repository serves as a practical guide for understanding OpenSilver's capabilities and compatibility.

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
┌─────────────────┬─────────────────┐
│  OpenSilver     │      WPF        │
│  (Web Client)   │  (Desktop App)  │
└─────────┬───────┴─────────┬───────┘
          │                 │
          └─────────────────┼─────────────────┐
                           │                  │
                    SignalR Hub              │
                           │                  │
                    Minimal API              │
                           │                  │
                    Ollama API/CLI           │
                                            │
                           Direct Connection for WPF
```

### Technology Stack
- **Frontend (Web)**: OpenSilver (Silverlight for modern web)
- **Frontend (Desktop)**: WPF (.NET)
- **Backend**: ASP.NET Core Minimal API
- **Real-time Communication**: SignalR
- **Target**: Ollama API & Command Line Interface

## 🚀 Quick Start

### Prerequisites
- [.NET 8.0 or later](https://dotnet.microsoft.com/download)
- [Ollama](https://ollama.ai/) installed and running
- Modern web browser with WebAssembly support (for OpenSilver)

### Running the OpenSilver Web Version

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/ollama-manager.git
   cd ollama-manager
   ```

2. **Start the backend server**
   ```bash
   cd src/server-minimalapi
   dotnet restore
   dotnet run
   ```

3. **Run the OpenSilver client**
   ```bash
   cd src/client-opensilver
   dotnet restore
   dotnet run
   ```

4. **Open your browser**
   Navigate to `https://localhost:5001`

### Running the WPF Desktop Version

1. **Start the backend server** (same as above)
   ```bash
   cd src/server-minimalapi
   dotnet restore
   dotnet run
   ```

2. **Run the WPF application**
   ```bash
   cd src/client-wpf
   dotnet restore
   dotnet run
   ```

### Comparing Implementations

To see the similarities and differences between OpenSilver and WPF:

1. Open both `src/client-opensilver/` and `src/client-wpf/` folders
2. Compare corresponding files (XAML views, ViewModels, etc.)
3. Notice how the core application logic remains virtually identical
4. Observe platform-specific adaptations and optimizations

## 🗺️ Roadmap

### Phase 1: Core Foundation ✅
- [x] Model listing and status monitoring
- [x] Basic model start/stop functionality
- [x] Real-time chat interface
- [x] SignalR integration
- [x] Dual platform implementation (OpenSilver + WPF)

### Phase 2: Enhanced Management 🚧
- [ ] Model download and installation
- [ ] Model deletion and cleanup
- [ ] Multi-session chat management
- [ ] Configuration management
- [ ] Cross-platform feature parity

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
- 🔄 **Platform Parity**: Help maintain feature consistency between OpenSilver and WPF versions

### Getting Started
1. Fork the repository
2. Choose your platform (`client-opensilver` or `client-wpf`)
3. Create a feature branch (`git checkout -b feature/amazing-feature`)
4. Make your changes
5. Commit your changes (`git commit -m 'Add amazing feature'`)
6. Push to the branch (`git push origin feature/amazing-feature`)
7. Open a Pull Request

### Development Setup
```bash
# Clone your fork
git clone https://github.com/opensilver/ollamamanager.git

# Install dependencies for all projects
dotnet restore

# Run backend (required for both clients)
cd src/server-minimalapi
dotnet run --environment Development

# In another terminal, run your chosen client:
# For OpenSilver:
cd src/client-opensilver
dotnet run

# For WPF:
cd src/client-wpf
dotnet run
```

### Code Guidelines
- Follow C# coding conventions
- Maintain feature parity between OpenSilver and WPF versions when possible
- Add unit tests for new features
- Update documentation for API changes
- Ensure SignalR hubs are properly tested
- Document any platform-specific differences

## 🏷️ Good First Issues

New to the project? Look for issues labeled with:
- `good-first-issue`: Perfect for newcomers
- `help-wanted`: We need your expertise
- `documentation`: Improve our docs
- `enhancement`: Add new features
- `opensilver`: OpenSilver-specific improvements
- `wpf`: WPF-specific improvements
- `platform-parity`: Help maintain consistency between platforms

## 💬 Community

- **Discussions**: Use GitHub Discussions for questions and ideas
- **Issues**: Report bugs and request features via GitHub Issues
- **Wiki**: Check our Wiki for detailed documentation and platform comparison guides

## 📋 Requirements

### For OpenSilver (Web)
- **.NET 8.0+**: For running the application
- **Modern Browser**: Chrome, Firefox, Safari, or Edge with WebAssembly support
- **Ollama**: Must be installed and accessible

### For WPF (Desktop)
- **.NET 8.0+**: For running the WPF application
- **Windows**: WPF requires Windows environment
- **Ollama**: Must be installed and accessible

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Ollama Team**: For creating an amazing local AI platform
- **OpenSilver Team**: For bringing Silverlight to the modern web and maintaining excellent WPF compatibility
- **Microsoft**: For the robust WPF framework that inspired this implementation
- **Contributors**: Everyone who helps make this project better

---

**⭐ Star this repository if you find it useful!**

**🤝 Join our community of contributors and help shape the future of Ollama management!**

**🔄 Explore both platforms and discover how OpenSilver brings WPF to the web!**
