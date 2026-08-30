# glc2d Texture 사용 예제

현재 샘플에서는 `G2Texture`를 이용해 이미지를 화면에 출력합니다.

```csharp
private G2Texture? _checkerTexture;
```

## Texture 생성

Texture는 `Initialize()`에서 생성합니다.

```csharp
_checkerTexture = new G2Texture(
    "resource/texture/res_checker.png");
```

이미지 파일 경로를 전달하면 `G2Texture`가 내부에서 이미지를 로드합니다.

Texture는 매 프레임 생성하지 않고, 일반적으로 `Initialize()`에서 한 번 생성한 뒤 `Render()`에서 반복해서 사용합니다.

---

## Texture 전체 출력

Texture 전체를 화면에 출력하려면 `Draw()`를 호출합니다.

```csharp
_checkerTexture?.Draw();
```

현재 샘플에서는 다음 코드로 Texture 전체를 출력합니다.

```csharp
public void Render()
{
    _checkerTexture?.Draw();
}
```

---

## Texture 일부 영역 출력

원본 Texture의 일부 영역만 선택하여 화면의 지정된 위치와 크기로 출력할 수 있습니다.

```csharp
_checkerTexture?.Draw(
    new Rect(400, 300, 300, 200),
    new Rect(200, 100, 300, 200));
```

`Draw()`의 두 `Rect`는 다음 의미입니다.

```csharp
Draw(
    destination,
    source);
```

| 인수 | 의미 |
|---|---|
| `destination` | 화면에 출력할 위치와 크기 |
| `source` | 원본 Texture에서 사용할 영역 |

현재 샘플에서는 다음과 같습니다.

```csharp
new Rect(400, 300, 300, 200)
```

화면의 `(400, 300)` 위치에 `300 x 200` 크기로 출력합니다.

```csharp
new Rect(200, 100, 300, 200)
```

원본 Texture의 `(200, 100)` 위치에서 `300 x 200` 크기의 영역을 사용합니다.

즉 다음과 같은 구조입니다.

```text
원본 Texture
    ↓
(200, 100) 위치에서
300 x 200 영역 선택
    ↓
화면의 (400, 300) 위치에
300 x 200 크기로 출력
```

---

## Texture 출력 예

현재 샘플의 `Render()`에서는 같은 Texture를 두 번 출력합니다.

```csharp
public void Render()
{
    _checkerTexture?.Draw();

    _checkerTexture?.Draw(
        new Rect(400, 300, 300, 200),
        new Rect(200, 100, 300, 200));
}
```

첫 번째 `Draw()`는 Texture 전체를 출력하고,  
두 번째 `Draw()`는 원본 Texture의 일부 영역을 선택해서 화면의 지정된 영역에 출력합니다.

---

## Texture 리소스 해제

사용이 끝난 Texture는 `Dispose()`에서 해제합니다.

```csharp
public void Dispose()
{
    _checkerTexture?.Dispose();
}
```

현재 샘플에서는 `SceneMain.Dispose()`에서 다른 리소스와 함께 Texture를 해제합니다.

Texture를 생성했다면 Scene이나 응용 프로그램이 종료될 때 반드시 `Dispose()`를 호출해야 합니다.
