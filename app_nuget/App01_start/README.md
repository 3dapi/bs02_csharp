# glc2d Framework 사용 설명

`glc2d`는 C#에서 2D 게임을 작성하기 위한 소형 프레임워크입니다.

WinForms 창을 기반으로 Direct2D / DirectWrite를 이용해 화면과 문자열을 출력하고,
XAudio2와 NAudio를 이용해 WAV / MP3 사운드를 재생합니다.

NuGet 패키지:

https://www.nuget.org/packages/glc2dcsharp

> 이 문서는 **glc2d 엔진 사용법**을 설명합니다.  
> 문서에 등장하는 `GameMain`, `GameGlobal`, `SceneMain` 등의 코드는 사용 방법을 보여 주기 위한 **샘플 응용 코드**입니다.  
> 샘플의 클래스 구성은 glc2d가 요구하는 필수 구조가 아닙니다.

---

## 1. 프로젝트 생성과 glc2d 설치

### 1.1 Console 프로젝트 생성

먼저 Visual Studio에서 C# Console 프로젝트를 생성합니다.

프로젝트를 생성하면 기본적으로 `Program.cs`가 만들어집니다.

```csharp
Console.WriteLine("Hello, World!");
```

이 `Program.cs`는 glc2d가 만드는 파일이 아니라 **Console 프로젝트 생성 시 Visual Studio가 만드는 기본 파일**입니다.

패키지를 설치하기 전에 프로젝트가 정상적으로 빌드되는지 먼저 확인합니다.

```text
Console 프로젝트 생성
        ↓
Program.cs 존재
        ↓
프로젝트 빌드 확인
```

### 1.2 glc2dcsharp 설치

Visual Studio의 **NuGet 패키지 관리**에서 다음 패키지를 설치합니다.

```text
glc2dcsharp
```

또는 .NET CLI를 사용할 수 있습니다.

```bash
dotnet add package glc2dcsharp
```

패키지에 필요한 Vortice.Windows 계열 라이브러리와 NAudio 등의 의존성은 NuGet에서 함께 복원됩니다.

주요 라이브러리는 다음과 같습니다.

| 라이브러리 | 사용 목적 |
|---|---|
| `Vortice.Direct2D1` | 2D 화면 출력 |
| `Vortice.DirectWrite` | 문자열 출력 |
| `Vortice.WIC` | PNG, JPG 등의 이미지 로드 |
| `Vortice.WinForms` | RenderForm과 RenderLoop |
| `Vortice.XAudio2` | WAV / MP3 출력 |
| `NAudio` | MP3를 PCM 데이터로 디코딩 |

### 1.3 AppMain.cs 활성화

`glc2dcsharp`를 설치하면 `AppMain.cs`가 추가됩니다.

설치 직후에는 다음 줄이 주석 처리되어 있습니다.

```csharp
//#define ACTIVE_GLC2DLIB
```

이 상태에서는 `AppMain`의 `Main()` 함수가 컴파일되지 않습니다.

```csharp
internal static class AppMain
{
#if ACTIVE_GLC2DLIB

    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        using GameMain app = new();
        app.Run();
    }

#endif
}
```

따라서 패키지를 설치한 직후에는 기존 Console 프로젝트의 `Program.cs`가 프로그램 시작점입니다.

```text
Program.cs
    ↓
현재 Main()
```

glc2d를 실제 프로그램 시작점으로 사용하려면 다음 줄의 주석을 제거합니다.

```csharp
#define ACTIVE_GLC2DLIB
```

그러면 `AppMain.Main()`이 활성화됩니다.

### 1.4 기존 Program.cs 제거

`AppMain.Main()`을 활성화한 뒤에는 Console 프로젝트에서 처음 생성된 `Program.cs`를 제거합니다.

`Program.cs`와 `AppMain.Main()`이 동시에 활성화되면 프로그램 시작점이 두 개가 되기 때문입니다.

최종 흐름은 다음과 같습니다.

```text
1. Console 프로젝트 생성
        ↓
2. Program.cs로 빌드 확인
        ↓
3. glc2dcsharp 설치
        ↓
4. AppMain.cs 추가
        ↓
5. #define ACTIVE_GLC2DLIB 활성화
        ↓
6. 기존 Program.cs 제거
        ↓
7. AppMain.Main()이 프로그램 시작점
```

---

## 2. glc2d 엔진 구성

주요 엔진 클래스는 다음과 같습니다.

```text
glc2d
├─ G2AppBase.cs
├─ G2D2DContext.cs
├─ G2InputContext.cs
├─ G2Font.cs
├─ G2Texture.cs
├─ G2TextureLoader.cs
├─ G2AudioContext.cs
├─ G2AudioSound.cs
├─ G2AudioMp3.cs
└─ G2Util.cs
```

| 클래스 | 역할 |
|---|---|
| `G2AppBase` | 게임 창, 게임 루프, 시간, 입력, 렌더링, 전체화면 관리 |
| `G2D2DContext` | Direct2D / DirectWrite 생성 및 RenderTarget 관리 |
| `G2InputContext` | 키보드, 마우스 버튼, 위치, 이동량, 휠 입력 |
| `G2Font` | DirectWrite 문자열 출력 |
| `G2Texture` | Texture 관리 및 화면 출력 |
| `G2TextureLoader` | WIC를 이용한 이미지 파일 로드 |
| `G2AudioContext` | XAudio2 및 Mastering Voice 관리 |
| `G2AudioSound` | WAV 효과음 재생 |
| `G2AudioMp3` | MP3 디코딩 및 재생 |
| `G2Util` | 리소스 파일 경로 검색 |

일반적인 응용 프로그램에서는 엔진 내부 구현을 수정하기보다 `G2AppBase`를 상속하여 게임 코드를 작성합니다.

---

## 3. glc2d 응용 프로그램 시작

가장 기본적인 응용 클래스는 다음과 같이 작성할 수 있습니다.

```csharp
class GameMain : G2AppBase
{
    protected override void Initialize()
    {
    }

    protected override void Update()
    {
    }

    protected override void Render()
    {
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}
```

`GameMain`이라는 이름은 샘플에서 사용하는 이름입니다.

중요한 것은 `G2AppBase`를 상속하고 다음 함수를 구현하는 것입니다.

```text
Initialize()
Update()
Render()
```

필요한 리소스가 있다면 `Dispose()`에서 해제합니다.

---

## 4. 프로그램 실행 구조

`AppMain`에서는 응용 프로그램 객체를 생성하고 `Run()`을 호출합니다.

```csharp
using GameMain app = new();
app.Run();
```

실행 흐름은 다음과 같습니다.

```text
AppMain.Main()
        ↓
new GameMain
        ↓
G2AppBase 생성
        ├─ Window
        ├─ Direct2D / DirectWrite
        ├─ XAudio2
        └─ Input
        ↓
Run()
        ↓
Initialize()
        ↓
┌──────────────────────┐
│      Game Loop       │
│                      │
│  Input Update        │
│       ↓              │
│  Update()            │
│       ↓              │
│  Render()            │
│                      │
└──────────────────────┘
        ↓
Dispose()
```

`Initialize()`는 한 번 호출되고, 이후 `Update()`와 `Render()`가 반복됩니다.

---

## 5. 게임 기본 설정

`G2AppBase`는 기본 화면 크기와 게임 이름을 제공합니다.

```csharp
public virtual System.Drawing.Size ScreenSize
    => new(640, 480);

public virtual string GameName
    => "G2 Game";
```

응용 프로그램에서 필요한 경우 override할 수 있습니다.

```csharp
class GameMain : G2AppBase
{
    public override System.Drawing.Size ScreenSize
        => new(1024, 768);

    public override string GameName
        => "My Game";

    ...
}
```

샘플에서는 설정 값을 별도의 `GameGlobal`에 둘 수 있습니다.

```csharp
public static class GameGlobal
{
    public static readonly System.Drawing.Size ScreenSize
        = new(1024, 768);

    public static readonly string GameName
        = "Nemo .....";
}
```

그리고 `GameMain`에서 사용합니다.

```csharp
public override System.Drawing.Size ScreenSize
    => GameGlobal.ScreenSize;

public override string GameName
    => GameGlobal.GameName;
```

`GameGlobal`을 사용하는 방식은 **샘플 응용 프로그램의 구성 방법 중 하나**입니다.

---

## 6. Initialize / Update / Render

### Initialize

게임 시작 시 한 번 호출됩니다.

Texture, Font, Sound 등의 리소스와 게임 데이터를 준비합니다.

```csharp
protected override void Initialize()
{
}
```

### Update

매 프레임 게임 상태를 갱신합니다.

```csharp
protected override void Update()
{
}
```

일반적으로 다음과 같은 작업을 수행합니다.

```text
입력 처리
이동
게임 상태 변경
충돌 처리
게임 규칙 처리
```

### Render

매 프레임 화면을 출력합니다.

```csharp
protected override void Render()
{
}
```

일반적으로 Texture와 문자열 등을 출력합니다.

게임 상태 변경은 `Update()`에서 처리하고, `Render()`는 현재 게임 상태를 화면에 표현하는 용도로 사용하는 것이 좋습니다.

---

## 7. 시간

`G2AppBase`는 다음 시간을 제공합니다.

```csharp
DeltaTime
TotalTime
```

### DeltaTime

이전 프레임부터 현재 프레임까지 흐른 시간입니다.

```csharp
float speed = 200.0f;
x += speed * (float)DeltaTime;
```

### TotalTime

게임 루프가 시작된 뒤 흐른 전체 시간입니다.

```csharp
double elapsed = TotalTime;
```

예:

```csharp
protected override void Update()
{
    double elapsed = TotalTime;

    ClearColor = new Color4(
        red: (float)(Math.Sin(elapsed) * 0.5 + 0.5),
        green: (float)(Math.Sin(elapsed + Math.PI / 2.0) * 0.5 + 0.5),
        blue: (float)(Math.Sin(elapsed + Math.PI) * 0.5 + 0.5),
        alpha: 1.0f);
}
```

---

## 8. 화면 크기와 Scale

`ScreenSize`는 게임에서 사용하는 **기준 해상도**입니다.

```csharp
public override System.Drawing.Size ScreenSize
    => new(1024, 768);
```

실제 Window 크기가 바뀌면 glc2d가 RenderTarget에 Scale을 적용하여 기준 해상도에 맞게 출력합니다.

현재 배율은 다음과 같이 얻을 수 있습니다.

```csharp
float scaleX = G2AppBase.ScreenScaleX;
float scaleY = G2AppBase.ScreenScaleY;
```

마우스 좌표도 같은 기준 해상도로 변환됩니다.

