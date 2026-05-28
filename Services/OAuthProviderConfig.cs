namespace ShopWebApp.Services
{
    /// <summary>
    /// Lưu trạng thái các OAuth provider đã được cấu hình credentials.
    /// Inject vào Controller/View để ẩn/hiện nút login tương ứng.
    /// </summary>
    public class OAuthProviderConfig
    {
        public bool GoogleEnabled { get; set; }
        public bool FacebookEnabled { get; set; }

        /// <summary>Có ít nhất 1 provider OAuth được kích hoạt</summary>
        public bool AnyEnabled => GoogleEnabled || FacebookEnabled;
    }
}
