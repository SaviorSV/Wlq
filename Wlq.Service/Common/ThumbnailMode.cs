
namespace Wlq.Service
{
	public enum ThumbnailMode
	{
		/// <summary>
		/// 指定高宽缩放（可能变形）
		/// </summary>
		HeightWidth = 1,

		/// <summary>
		/// 指定宽，高按比例  
		/// </summary>
		Width = 2,

		/// <summary>
		/// 指定高，宽按比例
		/// </summary>
		Height = 3,

		/// <summary>
		/// 指定高宽裁减（不变形）
		/// </summary>
		Cut = 4
	}
}
