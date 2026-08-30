using Microsoft.VisualBasic.Devices;
using System.Windows.Forms;
using Vortice.Mathematics;

namespace AppGlc
{
	class SceneMain : IDisposable
	{
		private G2Animation? _mario;
		// -------------------------------------------------------------------------------------------------------------------------------------------------------------
		// -------------------------------------------------------------------------------------------------------------------------------------------------------------
		public void Initialize()
		{
			_mario = new G2Animation("resource/texture/mario.png", 50, 66, 18, 120);
		}
		public void Update()
		{
			var Input = G2AppBase.Instance?.Input ?? throw new InvalidOperationException("SceneMain::Update::G2AppBase instance is not initialized.");

			if (Input.IsKeyDown(System.Windows.Forms.Keys.Escape))
			{
				// 종료 확인 팝업 창 표시
				DialogResult result = MessageBox.Show("정말 종료하시겠습니까?", "프로그램 종료", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				// '예'를 누른 경우에만 프로그램 종료
				if (result == DialogResult.Yes)
				{
					G2AppBase.Instance.Close();
				}
			}

			_mario?.Update();
		}
		public void Render()
		{
			var input = G2AppBase.Instance?.Input?? throw new InvalidOperationException();
			var mouse = input.MousePosition;
			_mario?.Draw(mouse.X, mouse.Y, 4.0f);
		}

		public void Dispose()
		{
			_mario?.Dispose();
		}
	}
}
