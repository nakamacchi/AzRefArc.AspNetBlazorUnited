using System.Net.Http.Json;

namespace AzRefArc.AspNetBlazorUnited.Client.Utilities
{
    public static class HttpExtension
    {
        public static async Task<TResult> PostAsJsonFromJsonAsync<TParam, TResult>(this HttpClient httpClient, string? requestUri, TParam value)
        {
            // HTTP 呼び出し
            HttpResponseMessage response = await httpClient.PostAsJsonAsync(requestUri, value);
            // 結果解析とデシリアライズ
            response.EnsureSuccessStatusCode();
            TResult? result = await response.Content.ReadFromJsonAsync<TResult>();
            return result!;
        }
    }
}
