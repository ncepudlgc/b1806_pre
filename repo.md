# Sony Bravia Control

## Repo简介

Sony Bravia Control 是一个开源的 .NET Core 集成库，用于与 Sony Bravia REST API 交互。该项目提供了完整的设备控制功能，包括电源管理、音频控制、内容播放、应用控制等，并支持设备监控和实时通知。

### 主要功能

1. **设备控制**
   - 电源状态管理（开启/关闭）
   - 音频控制（音量、静音等）
   - 内容播放控制
   - 应用控制
   - 输入源切换
   - 设置管理

2. **设备监控**
   - 实时监控设备电源状态变化
   - 实时监控播放内容变化
   - 使用 Reactive Extensions (IObservable) 提供流式更新

3. **通知 API**
   - 客户端可以订阅设备变化通知
   - 使用 Server-Sent Events (SSE) 推送实时通知
   - 支持订阅和取消订阅
   - 当设备监控器检测到设备变化时自动推送通知

### 技术栈

- **框架**: .NET 8.0
- **架构**: ASP.NET Core Web API
- **响应式编程**: System.Reactive (Rx.NET)
- **异步流**: IAsyncEnumerable
- **通知推送**: Server-Sent Events (SSE)
- **依赖注入**: Microsoft.Extensions.DependencyInjection

### 项目结构

```
src/SonyBraviaControl/
├── Bravia.Abstractions/          # 核心抽象接口
│   ├── INotificationService.cs  # 通知服务接口
│   ├── IDeviceMonitoringService.cs
│   └── ...
├── Bravia/                       # 核心库实现
│   ├── NotificationService.cs   # 通知服务实现
│   ├── DeviceMonitoringService.cs
│   └── ...
├── Bravia.Api/                   # Web API 项目
│   └── Controllers/
│       └── NotificationsController.cs  # 通知 API 控制器
├── Bravia.DeviceHost/            # 设备模拟主机（用于测试）
└── Bravia.UnitTests/             # 单元测试
```

### 通知 API 使用

#### 订阅通知
```
GET /{deviceId}/notifications/subscribe
```
使用 Server-Sent Events (SSE) 流式接收设备变化通知。

#### 取消订阅
```
POST /{deviceId}/notifications/unsubscribe
```
取消对指定设备的通知订阅。

### 通知格式

通知使用 JSON 格式，包含以下字段：
- `DeviceId`: 设备 ID
- `ChangeType`: 变化类型（"PowerStatus" 或 "PlayContent"）
- `Data`: 变化数据（PowerStatus 或 PlayContent 对象）
- `Timestamp`: 时间戳

## 题目Prompt

create a notifications api that allows clients to subscribe, receive, and unsubscribe to notifications when the device monitor detects any changes to a device.

## PR链接

待创建
