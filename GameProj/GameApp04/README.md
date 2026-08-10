# GameApp 정리본

기존 구조를 유지하면서 요청한 기능만 남긴 버전이다.

## 유지한 기능

- `AppGlobal.ScreenSize`
- `AppGlobal.GameName`
- Alt + Enter 전체 화면 토글
- 기준 해상도에 따른 화면 Scale
- Mouse 좌표의 기준 해상도 변환
- `Initialize() / Update() / Render()`
- `DeltaTime / TotalTime`
- Mouse `Down / Pressed / Released / WheelDelta`
- Font 캐시
- Texture 캐시 + 참조 카운트
- WIC Bitmap 로딩

## 제거한 기능

- `_disposed`
- `ThrowIfDisposed()`
- `_hasRun`
- `GC.SuppressFinalize(this)`
- `RenderTargetGeneration`
- `RecreateRenderTarget()`
- `D2DERR_RECREATE_TARGET`
- `OnRenderTargetRecreated()`
- RenderTarget 재생성용 Brush/Texture 처리

## Texture 캐시

같은 경로로 `AppTexture`를 여러 개 생성하면 실제 `ID2D1Bitmap`은 한 번만 생성한다.

```text
new AppTexture("enemy.png") → Count 1
new AppTexture("enemy.png") → Count 2
new AppTexture("enemy.png") → Count 3
```

`Dispose()` 때 Count를 감소시키고 0이 되면 Bitmap을 해제하고 캐시에서 제거한다.
