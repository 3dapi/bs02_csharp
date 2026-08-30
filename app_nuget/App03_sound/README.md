# glc2d Audio 사용 예제

현재 샘플에서는 사운드를 두 가지 용도로 구분해서 사용합니다.

```csharp
private G2AudioSound? _soundEffect;
private G2AudioMp3? _backgroundMusic;
```

```text
G2AudioSound → WAV 효과음
G2AudioMp3   → MP3 배경음악
```

## WAV 효과음

짧은 효과음은 `G2AudioSound`를 사용합니다.

```csharp
_soundEffect = new G2AudioSound(
    "resource/audio/effect/move3.wav");
```

기본 `Play()`는 한 번 재생합니다.

```csharp
_soundEffect?.Play();
```

현재 샘플에서는 마우스 왼쪽 버튼을 클릭했을 때 WAV 효과음을 재생합니다.

```csharp
if (Input.IsButtonDown(MouseButtons.Left))
{
    _soundEffect?.Play();
}
```

WAV는 일반적으로 다음과 같은 짧은 효과음에 사용합니다.

```text
버튼 클릭
공격
피격
점프
아이템 획득
충돌
```

필요하면 반복 재생도 가능합니다.

```csharp
_soundEffect?.Play(true);
```

## MP3 배경음악

배경음악은 `G2AudioMp3`를 사용합니다.

```csharp
_backgroundMusic = new G2AudioMp3(
    "resource/audio/bgm/background.mp3");
```

현재 샘플에서는 초기화할 때 배경음악을 반복 재생합니다.

```csharp
_backgroundMusic.Play(true);
```

`G2AudioMp3.Play()`의 기본값은 반복 재생이므로 다음과 같이 작성해도 됩니다.

```csharp
_backgroundMusic.Play();
```

한 번만 재생하려면 다음과 같이 지정합니다.

```csharp
_backgroundMusic.Play(false);
```

MP3는 일반적으로 다음과 같은 배경음악에 사용합니다.

```text
Title BGM
Stage BGM
Battle BGM
Ending BGM
```

## WAV와 MP3 용도

| 클래스 | 파일 형식 | 주 용도 | 기본 재생 |
|---|---|---|---|
| `G2AudioSound` | WAV | 효과음 | 1회 |
| `G2AudioMp3` | MP3 | 배경음악 | 반복 |

현재 샘플의 사용 흐름은 다음과 같습니다.

```text
Initialize()
├─ WAV 효과음 로드
├─ MP3 배경음악 로드
└─ MP3 배경음악 반복 재생

Update()
└─ 마우스 좌클릭
      ↓
   WAV 효과음 1회 재생
```

## 사운드 리소스 해제

사용이 끝난 사운드 객체는 `Dispose()`에서 해제합니다.

```csharp
public void Dispose()
{
    _backgroundMusic?.Dispose();
    _soundEffect?.Dispose();
}
```
