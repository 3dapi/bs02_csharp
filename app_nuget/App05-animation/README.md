# glc2d Animation 사용 예제

현재 샘플에서는 `G2Animation`을 이용해 Sprite Animation을 출력합니다.

```csharp
private G2Animation? _mario;
```

## Animation 생성

Animation 객체는 `Initialize()`에서 생성합니다.

```csharp
_mario = new G2Animation(
    "resource/texture/mario.png",
    50,
    66,
    18,
    120);
```

첫 번째 인수는 Animation에 사용할 이미지 파일입니다.

```text
resource/texture/mario.png
```

뒤의 숫자 인수 `50`, `66`, `18`, `120`의 정확한 의미와 단위는 현재 제공된 `SceneMain.cs`만으로는 확인할 수 없습니다.  
해당 내용은 `G2Animation` 클래스의 생성자 정의를 기준으로 확인해야 합니다.

---

## Animation 갱신

Animation은 매 프레임 `Update()`를 호출합니다.

```csharp
_mario?.Update();
```

현재 샘플에서는 `SceneMain.Update()`에서 호출합니다.

```csharp
public void Update()
{
    ...

    _mario?.Update();
}
```

Animation 프레임의 변경은 `G2Animation.Update()`에서 처리됩니다.

---

## Animation 출력

현재 샘플에서는 마우스 위치에 Animation을 출력합니다.

```csharp
var input = G2AppBase.Instance?.Input
    ?? throw new InvalidOperationException();

var mouse = input.MousePosition;

_mario?.Draw(
    mouse.X,
    mouse.Y,
    4.0f);
```

따라서 Animation은 현재 마우스 좌표를 따라 이동합니다.

```text
MousePosition
    ↓
mouse.X, mouse.Y
    ↓
G2Animation.Draw()
```

`Draw()`에는 현재 샘플에서 다음 값이 전달됩니다.

```csharp
Draw(
    mouse.X,
    mouse.Y,
    4.0f);
```

- 첫 번째 값: 출력 X 좌표
- 두 번째 값: 출력 Y 좌표
- 세 번째 값 `4.0f`: 현재 샘플에서 전달하는 추가 출력 인수

세 번째 인수의 정확한 의미는 `G2Animation.Draw()` 정의를 기준으로 확인해야 합니다.

---

## Animation 사용 흐름

현재 샘플의 Animation 처리 흐름은 다음과 같습니다.

```text
Initialize()
    ↓
G2Animation 생성
    ↓
Update()
    ↓
G2Animation.Update()
    ↓
Render()
    ↓
현재 마우스 위치 얻기
    ↓
G2Animation.Draw()
```

즉 Animation 객체는 `Initialize()`에서 한 번 생성하고,  
게임 루프에서는 `Update()`와 `Draw()`를 반복 호출합니다.

---

## Animation 리소스 해제

사용이 끝난 Animation 객체는 `Dispose()`에서 해제합니다.

```csharp
public void Dispose()
{
    _mario?.Dispose();
}
```

`G2Animation` 객체를 생성했다면 Scene이나 응용 프로그램이 종료될 때 `Dispose()`를 호출합니다.
