# Ollama Manager

OpenSilver로 개발된 Ollama 모델 관리 웹 애플리케이션입니다. 

OpenSilver 버전을 WPF로 마이그레이션한 데스크톱 버전도 함께 제공하여 두 플랫폼 간의 개발 경험을 비교하고 학습할 수 있도록 구성했습니다.

## 목차

- [스크린샷](#스크린샷)
- [주요 기능](#주요-기능)
- [프로젝트 구조](#프로젝트-구조)
- [기술 스택](#기술-스택)
- [서버 아키텍처](#서버-아키텍처)
- [실행 방법](#실행-방법)
- [개발 계획](#개발-계획)
- [기여하기](#기여하기)
- [라이센스](#라이센스)
- [참고사항](#참고사항)

## 스크린샷

직관적인 인터페이스로 Ollama 모델을 쉽게 관리하고 실시간으로 채팅할 수 있습니다.

| 메인 화면 | 채팅 화면 |
|-----------|-----------|
| ![메인 화면](https://github.com/user-attachments/assets/8c3bcfc6-ae3f-4d58-9cce-f18285506f1c) | ![채팅 화면](https://github.com/user-attachments/assets/1daeb5bd-a1d9-4cd0-bc15-3fd779950a4b) |

## 주요 기능

- 설치된 모델 목록 조회
- 모델 시작/중지
- 실시간 채팅
- 모델 상태 모니터링

## 프로젝트 구조

```
src/
├── client-opensilver/    # OpenSilver 웹 클라이언트
├── client-wpf/          # WPF 데스크톱 클라이언트
└── server-minimalapi/   # 공유 백엔드 서버
```

## 기술 스택

- **웹 클라이언트**: OpenSilver (.NET Standard 2.0)
- **데스크톱 클라이언트**: WPF (.NET 9.0)
- **백엔드**: ASP.NET Core Minimal API (.NET 9.0)
- **실시간 통신**: SignalR

## 서버 아키텍처

### Minimal API 구조
- **GET** `/api/models` - 설치된 모델 목록 및 상태 조회
- **POST** `/api/models/{modelName}/start` - 모델 시작
- **POST** `/api/models/{modelName}/stop` - 모델 중지
- **POST** `/api/chat` - 모델과 채팅

### Ollama API 연동
백엔드 서버는 Ollama API를 활용하여 모델을 관리합니다:
- `http://localhost:11434/api/tags` - 설치된 모델 목록
- `http://localhost:11434/api/ps` - 실행 중인 모델 확인
- `http://localhost:11434/api/generate` - 모델 로드/언로드 및 채팅

### 실시간 모니터링
- SignalR Hub를 통한 실시간 모델 상태 업데이트
- 백그라운드 서비스로 모델 상태 변화 감지

## 실행 방법

### 사전 요구사항
- **Ollama**: [ollama.com](https://ollama.com)에서 설치
- **모델 설치**: 예시로 `ollama pull llama3.2` 실행
- **Visual Studio 2022**
- **.NET 9.0 SDK**
- **WASM Tools**: `dotnet workload install wasm-tools`
- **OpenSilver SDK**: [www.opensilver.net](https://www.opensilver.net)에서 `OpenSilver_SDK_v3.2.0.4.vsix` 다운로드 후 설치

### 서버 실행

먼저 백엔드 서버를 실행합니다:
```bash
cd src/server-minimalapi/LocalLLMServer
dotnet run --launch-profile https
```
서버가 `https://localhost:7262`에서 실행됩니다.

### OpenSilver 웹 클라이언트

```bash
cd src/client-opensilver/OllamaHub.Browser
dotnet run
```

브라우저에서 `http://localhost:55592` 접속

### WPF 버전

동일한 서버를 사용하여 WPF 버전도 실행할 수 있습니다:
```bash
cd src/client-wpf/OllamaHub
dotnet run
```

## 개발 계획

### 현재 구현됨
- 모델 목록 조회
- 모델 제어 (시작/중지)
- 실시간 채팅

### 예정 기능
- 모델 다운로드
- 모델 삭제
- 다중 세션 채팅
- 성능 모니터링

## 기여하기

1. 저장소 포크
2. 기능 브랜치 생성
3. 변경사항 커밋
4. Pull Request 생성

## 라이센스

MIT License

## 참고사항

- WPF와 OpenSilver 간의 코드 호환성을 확인할 수 있는 좋은 예제입니다
- OpenSilver는 .NET Standard 2.0을 기반으로 하여 웹에서 WPF와 유사한 개발 경험을 제공합니다
