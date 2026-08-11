
using glc;

internal static class Program
{
	[STAThread]
	private static void Main()
	{
		ApplicationConfiguration.Initialize();

		// GameMain 인스턴스 생성 및 리소스 자동 해제를 위한 using 선언
		using GameMain app = new ();

		// 캡슐화된 메인 게임 루프 실행
		app.Run();
	}
}
