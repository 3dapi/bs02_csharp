// -------------------------------------------------------------------------------------------------------------------------------------------------------------
// Author: 3dapi (https://github.com/3dapi)
// -------------------------------------------------------------------------------------------------------------------------------------------------------------

using AppGlc;
using System.Windows.Forms;
using Vortice.Mathematics;

class GameMain : G2AppBase
{
	public override System.Drawing.Size ScreenSize => GameGlobal.ScreenSize;
	public override string GameName => GameGlobal.GameName;

	SceneMain sceneMain = new SceneMain();

	protected override void Initialize()
	{
		sceneMain = new SceneMain();
		sceneMain.Initialize();
	}

	protected override void Update()
	{
		double elapsed = TotalTime;

		this.ClearColor = new Color4(
			red: (float)(Math.Sin(elapsed) * 0.5 + 0.5),
			green: (float)(Math.Sin(elapsed + Math.PI / 2.0) * 0.5 + 0.5),
			blue: (float)(Math.Sin(elapsed + Math.PI) * 0.5 + 0.5),
			alpha: 1.0f);

		sceneMain.Update();
	}

	protected override void Render()
	{
		sceneMain.Render();
	}

	public override void Dispose()
	{
		base.Dispose();
		((IDisposable)sceneMain).Dispose();
	}
}
