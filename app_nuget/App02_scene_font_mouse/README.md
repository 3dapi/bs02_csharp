# glc2d Sample Application

이 프로젝트는 `glc2d`를 이용한 간단한 응용 프로그램 샘플입니다.

샘플은 `GameMain`에서 게임의 전체 실행 흐름을 연결하고, `SceneMain`에서 마우스 좌표를 문자열로 출력하는 예제를 보여 줍니다.

---

## 1. 구성

```text
GameMain.cs
SceneMain.cs
```

| 파일 | 역할 |
|---|---|
| `GameMain.cs` | `G2AppBase`를 상속하여 응용 프로그램의 초기화, 갱신, 출력을 연결 |
| `SceneMain.cs` | 폰트를 생성하고 마우스 좌표를 갱신하여 화면에 출력 |

---

## 2. GameMain

`GameMain`은 `G2AppBase`를 상속합니다.

```csharp
class GameMain : G2AppBase
```

화면 크기와 게임 이름은 `GameGlobal`의 값을 사용합니다.

```csharp
public override System.Drawing.Size ScreenSize
    => GameGlobal.ScreenSize;

public override string GameName
    => GameGlobal.GameName;
```

`SceneMain` 객체를 이용하여 실제 장면의 처리를 연결합니다.

```csharp
SceneMain sceneMain = new SceneMain();
```

### Initialize

`SceneMain`을 생성하고 초기화합니다.

```csharp
protected override void Initialize()
{
    sceneMain = new SceneMain();
    sceneMain.Initialize();
}
```

### Update

`TotalTime`을 이용하여 시간에 따라 화면 배경색을 변화시킵니다.

```csharp
double elapsed = TotalTime;

this.ClearColor = new Color4(
    red: (float)(Math.Sin(elapsed) * 0.5 + 0.5),
    green: (float)(Math.Sin(elapsed + Math.PI / 2.0) * 0.5 + 0.5),
    blue: (float)(Math.Sin(elapsed + Math.PI) * 0.5 + 0.5),
    alpha: 1.0f);
```

이후 `SceneMain.Update()`를 호출합니다.

```csharp
sceneMain.Update();
```

### Render

화면 출력은 `SceneMain.Render()`에 전달합니다.

```csharp
protected override void Render()
{
    sceneMain.Render();
}
```

### Dispose

프로그램 종료 시 리소스를 해제합니다.

```csharp
public override void Dispose()
{
    base.Dispose();
    ((IDisposable)sceneMain).Dispose();
}
```

---

## 3. SceneMain

`SceneMain`은 `IDisposable`을 구현합니다.

```csharp
class SceneMain : IDisposable
```

현재 샘플에서는 두 개의 `G2Font`를 사용합니다.

```csharp
private G2Font? _systemFont;
private G2Font? _cursorFont;
```

- `_systemFont` : 화면 상단의 마우스 좌표 출력
- `_cursorFont` : 마우스 커서 근처의 좌표 출력

---

## 4. Font 사용

문자열 출력은 `G2Font`를 사용합니다.

### 4.1 Font 생성

가장 간단한 생성 방법은 Font 이름과 크기를 지정하는 것입니다.

```csharp
G2Font font = new G2Font("Arial", 32);
```

현재 샘플에서는 다음과 같이 두 개의 Font를 생성합니다.

```csharp
public void Initialize()
{
    _systemFont = new G2Font("Arial", 32);
    _cursorFont = new G2Font("Arial", 18);
}
```

각 Font의 용도는 다음과 같습니다.

```text
_systemFont : Arial, 32
              화면 왼쪽 위에 마우스 좌표 출력

_cursorFont : Arial, 18
              마우스 커서 근처에 좌표 출력
```

### 4.2 G2Font 생성자

`G2Font`는 다음 항목을 지정할 수 있습니다.

```csharp
new G2Font(fontFamilyName, fontSize, fontWeight, fontStyle, textAlignment, paragraphAlignment);
```

| 인수 | 의미 | 기본값 |
|---|---|---|
| `fontFamilyName` | Font 이름 | 필수 |
| `fontSize` | Font 크기 | 필수 |
| `fontWeight` | 글자 굵기 | `FontWeight.Heavy` |
| `fontStyle` | 글자 스타일 | `FontStyle.Normal` |
| `textAlignment` | 가로 정렬 | `TextAlignment.Leading` |
| `paragraphAlignment` | 세로 정렬 | `ParagraphAlignment.Near` |

예:

```csharp
G2Font font = new G2Font("Arial", 24, FontWeight.Normal, FontStyle.Normal, TextAlignment.Center, ParagraphAlignment.Center);
```

### 4.3 문자열 출력

문자열은 `DrawText()`로 출력합니다.

```csharp
font.DrawText(text, layoutRect, color);
```

현재 샘플의 화면 상단 출력:

```csharp
_systemFont?.DrawText(
    _mouseInfoText,
    new Rect(20, 20, 600, 100),
    new Color4(0.0f, 1.0f, 1.0f, 1.0f));
```

| 인수 | 의미 |
|---|---|
| `text` | 출력할 문자열 |
| `layoutRect` | 문자열을 배치할 화면 영역 |
| `color` | 문자열 색상 |

### 4.4 문자열 출력 영역

`Rect`는 문자열을 배치할 영역입니다.

```csharp
new Rect(x, y, width, height)
```

예:

```csharp
new Rect(20, 20, 600, 100)
```

은 `(20, 20)` 위치에서 시작하는 `600 x 100` 크기의 영역을 의미합니다.

마우스 커서 근처에 출력할 때는 현재 마우스 좌표를 이용해 영역을 만듭니다.

```csharp
Rect cursorRect = new(mousePos.X + 5, mousePos.Y + 5, 200, 50);
```

### 4.5 문자열 색상

문자열 색상은 `Color4`로 지정합니다.

```csharp
new Color4(red, green, blue, alpha);
```

현재 샘플에서는 화면 상단 문자열을 Cyan으로 출력합니다.

```csharp
new Color4(0.0f, 1.0f, 1.0f, 1.0f)
```

마우스 커서 옆 좌표는 Yellow로 출력합니다.

```csharp
new Color4(1.0f, 1.0f, 0.0f,1.0f)
```

### 4.6 문자열 정렬

`G2Font`는 생성할 때 문자열의 가로/세로 정렬을 지정할 수 있습니다.

가로 정렬 예:

```text
TextAlignment.Leading
TextAlignment.Center
TextAlignment.Trailing
```

세로 정렬 예:

```text
ParagraphAlignment.Near
ParagraphAlignment.Center
ParagraphAlignment.Far
```

가운데 정렬 Font 예:

```csharp
G2Font centerFont = new G2Font(
    "Arial",
    32,
    FontWeight.Normal,
    FontStyle.Normal,
    TextAlignment.Center,
    ParagraphAlignment.Center);
```

`DrawText()`의 `Rect`가 문자열 정렬의 기준 영역이 됩니다.

### 4.7 Font 리소스 공유

`G2Font`는 Font 이름, 크기, 굵기, 스타일, 정렬 설정이 모두 같은 경우 내부 `IDWriteTextFormat`을 공유합니다.

```csharp
G2Font font1 = new G2Font("Arial", 32);
G2Font font2 = new G2Font("Arial", 32);
```

두 객체는 같은 설정이므로 내부 TextFormat을 공유합니다.

각 `G2Font` 객체는 문자열 색상을 변경하기 위한 Brush는 별도로 가지고 있습니다.

### 4.8 Font 해제

`G2Font`는 `IDisposable`을 구현합니다.

현재 샘플에서는 `SceneMain.Dispose()`에서 생성한 Font를 해제합니다.

```csharp
public void Dispose()
{
    _systemFont?.Dispose();
    _cursorFont?.Dispose();
}
```

같은 설정의 Font가 내부 `TextFormat`을 공유하는 경우 참조 횟수를 감소시키고, 마지막 Font가 해제될 때 `TextFormat`도 해제됩니다.

---

## 5. 마우스 좌표 갱신

`Update()`에서 현재 마우스 위치를 얻습니다.

```csharp
var mousePos = G2AppBase.Instance?.Input?.MousePosition
    ?? throw new InvalidOperationException("G2AppBase instance is not initialized.");
```

마우스 위치를 문자열로 저장합니다.

```csharp
_mouseInfoText = $"실시간 마우스 좌표: (X: {(int)mousePos.X}, Y: {(int)mousePos.Y})";

_cursorText = $"({(int)mousePos.X}, {(int)mousePos.Y})";
```

---

## 6. 마우스 좌표 출력

`Render()`에서는 두 위치에 마우스 좌표를 출력합니다.

### 화면 상단

```csharp
_systemFont?.DrawText( _mouseInfoText,
    new Rect(20, 20, 600, 100),
    new Color4(0.0f, 1.0f, 1.0f, 1.0f));
```

화면 왼쪽 위에 현재 마우스 좌표를 출력합니다.

### 마우스 커서 위치

현재 마우스 위치를 다시 얻은 뒤 출력 영역을 만듭니다.

```csharp
Rect cursorRect =
    new(mousePos.X + 5, mousePos.Y + 5, 200, 50);
```

마우스 위치에서 `(5, 5)`만큼 떨어진 곳에 좌표를 출력합니다.

```csharp
_cursorFont?.DrawText(
    _cursorText,
    cursorRect,
    new Color4(1.0f, 1.0f, 0.0f, 1.0f));
```

---

## 7. Scene 리소스 해제

`SceneMain.Dispose()`에서 생성한 Font를 해제합니다.

```csharp
public void Dispose()
{
    _systemFont?.Dispose();
    _cursorFont?.Dispose();
}
```

---

## 8. 실행 흐름

```text
GameMain.Initialize()
        ↓
SceneMain.Initialize()
        ↓
┌─────────────────────────┐
│       Game Loop         │
│                         │
│ GameMain.Update()       │
│   ├─ 배경색 변경        │
│   └─ SceneMain.Update() │
│          ↓              │
│      마우스 좌표 갱신   │
│                         │
│ GameMain.Render()       │
│   └─ SceneMain.Render() │
│          ↓              │
│      마우스 좌표 출력   │
└─────────────────────────┘
        ↓
Dispose()
```

---

## 9. 샘플에서 확인할 수 있는 내용

이 샘플은 다음과 같은 `glc2d` 기본 사용 방법을 보여 줍니다.

- `G2AppBase` 상속
- `Initialize()`, `Update()`, `Render()` 구성
- `TotalTime` 사용
- `ClearColor` 변경
- `G2AppBase.Instance.Input.MousePosition` 사용
- `G2Font` 생성과 문자열 출력
- `IDisposable`을 이용한 리소스 해제
- `GameMain`과 `SceneMain`의 역할 분리
