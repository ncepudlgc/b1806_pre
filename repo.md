# b1806_pre

## Repo简介

Sony Bravia Control 是一个开源的 .NET Core 集成库，用于与 Sony Bravia REST API 进行交互。

**主要功能：**
- 完整的 Sony Bravia REST API 实现
- 设备连接管理
- 音频控制（音量、静音等）
- 系统控制（电源状态、播放内容等）
- 应用控制
- 输入源切换
- 设备监控服务（监控设备状态变化）
- 通知服务（订阅设备变更通知）

**技术栈：**
- C# / .NET 8.0
- ASP.NET Core
- System.Reactive (Rx.NET) - 响应式编程
- System.Threading.Channels - 异步流处理
- Server-Sent Events (SSE) - 实时通知
- Dependency Injection

**项目结构：**
- `src/SonyBraviaControl/Bravia.Abstractions/` - 核心抽象接口，支持共享依赖注入
  - `IAudioService.cs` - 音频服务接口
  - `ISystemService.cs` - 系统服务接口
  - `IAVContentService.cs` - 音视频内容服务接口
  - `IAppControlService.cs` - 应用控制服务接口
  - `IDeviceMonitoringService.cs` - 设备监控服务接口
  - `INotificationService.cs` - 通知服务接口
- `src/SonyBraviaControl/Bravia/` - 核心库实现
  - `AudioService.cs` - 音频服务实现
  - `SystemService.cs` - 系统服务实现
  - `DeviceMonitoringService.cs` - 设备监控服务实现，使用 IObservable 流
  - `NotificationService.cs` - 通知服务实现
  - `Startup.cs` - 服务注册和配置
- `src/SonyBraviaControl/Bravia.Api/` - Web API 测试实现
  - `Controllers/` - API 控制器（AudioController, SystemController, NotificationsController 等）
- `src/SonyBraviaControl/Bravia.DeviceHost/` - 控制台应用程序，运行 Kestrel 模拟 Bravia 远程主机
- `src/SonyBraviaControl/Bravia.UnitTests/` - 单元测试

**核心组件：**
- ConnectionManager - 管理设备连接
- DeviceMonitoringService - 监控设备状态变化（电源状态、播放内容），使用 IObservable 流
- NotificationService - 管理设备变更通知订阅，支持 Server-Sent Events
- BraviaRequestFactory - 构建 Bravia API 请求
- HttpRequestService - 处理 HTTP 请求

**API 端点：**
- `/api/audio/*` - 音频控制
- `/api/system/*` - 系统控制
- `/api/connections/*` - 连接管理
- `/api/notifications/{id?}/subscribe` - 订阅设备变更通知（SSE）
- `/api/notifications/{id?}/unsubscribe` - 取消订阅

## 题目Prompt

create a notifications api that allows clients to subscribe, receive, and unsubscribe to notifications when the device monitor detects any changes to a device.

## PR链接

https://github.com/ncepudlgc/b1806_pre/pull/3
