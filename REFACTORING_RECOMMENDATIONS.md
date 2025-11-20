# リファクタリング推奨事項

このドキュメントは、AzRefArc.AspNetBlazorUnited プロジェクトのコードベースを分析した結果、リファクタリング候補となる項目をまとめたものです。

## 目次
1. [コード重複の削減](#1-コード重複の削減)
2. [共通化と再利用性の向上](#2-共通化と再利用性の向上)
3. [コードの可読性向上](#3-コードの可読性向上)
4. [保守性の向上](#4-保守性の向上)
5. [セキュリティと堅牢性](#5-セキュリティと堅牢性)

---

## 1. コード重複の削減

### 1.1 ExceptionFileLogger の重複（高優先度）
**問題点：**
- `AzRefArc.AspNetBlazorUnited/ExceptionFileLogger.cs`
- `AzRefArc.AspNetBlazorUnited.Client/ExceptionFileLogger.cs`

これら2つのファイルには `ConvertExceptionToString()` メソッドに完全に同じロジックがあります（約60行）。

**推奨対応：**
```
AzRefArc.AspNetBlazorUnited.Shared プロジェクトを作成し、共通ロジックを移動：
- ExceptionFormatterBase クラスを作成
- ConvertExceptionToString() メソッドを共通化
- サーバー版とクライアント版でそれぞれ継承して実装
```

**効果：**
- コード量削減: 約60行
- 保守性向上: バグ修正が1箇所で済む
- 一貫性向上: 例外ログフォーマットの統一

### 1.2 FindConditionViewModel の完全重複（高優先度）
**問題点：**
- `AzRefArc.AspNetBlazorUnited/Components/Pages/BizGroupA/ShowAuthorsByCondition/FindConditionViewModel.cs`
- `AzRefArc.AspNetBlazorUnited/Components/Pages/BizGroupC/ShowAuthorsByCondition/FindConditionViewModel.cs`

これらのファイルは完全に同一です（70行）。

**推奨対応：**
```
共通の場所に移動：
AzRefArc.AspNetBlazorUnited/ViewModels/FindConditionViewModel.cs
または
AzRefArc.AspNetBlazorUnited/Models/Search/FindConditionViewModel.cs
```

**効果：**
- コード量削減: 70行
- 保守性向上: バリデーションロジックの変更が1箇所で済む

### 1.3 FilterByTopNViewModel の完全重複（高優先度）
**問題点：**
- `AzRefArc.AspNetBlazorUnited/Components/Pages/BizGroupA/ShowAuthorsByTopN/FilterByTopNViewModel.cs`
- `AzRefArc.AspNetBlazorUnited/Components/Pages/BizGroupC/ShowAuthorsByTopN/FilterByTopNViewModel.cs`

これらのファイルは完全に同一です（11行）。

**推奨対応：**
```
共通の場所に移動：
AzRefArc.AspNetBlazorUnited/ViewModels/FilterByTopNViewModel.cs
```

**効果：**
- コード量削減: 11行
- 一貫性向上

### 1.4 FilterByCondition.razor の類似パターン（中優先度）
**問題点：**
BizGroupA と BizGroupC の `FilterByCondition.razor` ファイルが非常に似ています。主な違い：
- BizGroupC: `isFormDisabled` フラグによる連打防止機能
- BizGroupC: `AuthorsQuickGrid` コンポーネントを使用
- BizGroupA: `AuthorsDataGrid` コンポーネントを使用

**推奨対応：**
```
1. 共通コンポーネントを作成：
   Components/Shared/AuthorSearchForm.razor

2. パラメータで動作を切り替え：
   [Parameter] public bool EnableFormDisable { get; set; }
   [Parameter] public RenderFragment<List<Author>> ResultsTemplate { get; set; }
```

**効果：**
- UIロジックの統一
- 新機能追加時の作業量削減

---

## 2. 共通化と再利用性の向上

### 2.1 DbContext 設定の重複（高優先度）
**問題点：**
`Program.cs` 内で PubsDbContext と DataProtectionKeyDbContext の設定コードが重複しています（各20行程度）。

**推奨対応：**
```csharp
// 拡張メソッドを作成
public static class DbContextExtensions
{
    public static void AddPubsDbContext<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment,
        string connectionStringName = "PubsDbContext")
        where T : DbContext
    {
        services.AddDbContextFactory<T>(opt =>
        {
            if (environment.IsDevelopment())
            {
                opt = opt.EnableSensitiveDataLogging().EnableDetailedErrors();
            }
            opt.UseSqlServer(
                configuration.GetConnectionString(connectionStringName),
                providerOptions =>
                {
                    providerOptions.EnableRetryOnFailure();
                });
        });
    }
}

// Program.cs での使用
builder.Services.AddPubsDbContext<PubsDbContext>(
    builder.Configuration, 
    builder.Environment);
```

**効果：**
- コード量削減: 約30-40行
- 設定の一貫性向上
- 新しいDbContextの追加が容易に

### 2.2 USStatesUtil の改善（中優先度）
**問題点：**
`GetAllStates()` メソッドが呼ばれるたびに新しい辞書を作成しています。

**推奨対応：**
```csharp
public static class USStatesUtil
{
    private static readonly Lazy<SortedDictionary<string, string>> _states = 
        new Lazy<SortedDictionary<string, string>>(() => 
        {
            return new SortedDictionary<string, string>()
            {
                {"AL", "Alabama"},
                // ... 省略
            };
        });
    
    public static SortedDictionary<string, string> GetAllStates()
    {
        return new SortedDictionary<string, string>(_states.Value);
    }
    
    // または読み取り専用で返す
    public static IReadOnlyDictionary<string, string> GetAllStatesReadOnly()
    {
        return _states.Value;
    }
}
```

**効果：**
- パフォーマンス向上
- メモリ使用量削減

---

## 3. コードの可読性向上

### 3.1 ConvertExceptionToString メソッドの分割（中優先度）
**問題点：**
`ExceptionFileLogger.cs` の `ConvertExceptionToString()` メソッドが長く（約65行）、複数の責務を持っています。

**推奨対応：**
```csharp
private static string ConvertExceptionToString(Exception exception)
{
    if (exception == null) return "例外オブジェクト情報はありません。\r\n\r\n";
    
    var builder = new StringBuilder();
    builder.Append(BuildGeneralInformation());
    builder.Append(BuildExceptionInformation(exception));
    return builder.ToString();
}

private static string BuildGeneralInformation()
{
    // 一般情報の構築
}

private static string BuildExceptionInformation(Exception exception)
{
    // 例外情報の構築
}

private static string BuildExceptionProperties(Exception exception)
{
    // 例外プロパティの構築
}
```

**効果：**
- 可読性向上
- テスタビリティ向上
- 保守性向上

### 3.2 マジックナンバーの定数化（低優先度）
**問題点：**
コード内に直接書かれた数値や文字列。

**推奨対応：**
```csharp
// ExceptionFileLogger.cs
private const string ExceptionHeaderFormat = "****** 一般情報 ******\r\n\r\n";
private const string ExceptionDetailHeaderFormat = "\r\n****** 例外情報 ******";

// FilterByCondition.razor
private const int MinimumSearchConditionsRequired = 1;
```

**効果：**
- 可読性向上
- 変更時の影響範囲の明確化

### 3.3 クエリビルダーパターンの適用（中優先度）
**問題点：**
`FilterByCondition.razor` の GetData() メソッド内で、複数のif文を使ってクエリを構築しています。

**推奨対応：**
```csharp
public class AuthorQueryBuilder
{
    private IQueryable<Author> _query;
    
    public AuthorQueryBuilder(IQueryable<Author> query)
    {
        _query = query;
    }
    
    public AuthorQueryBuilder FilterByState(bool enabled, string? state)
    {
        if (enabled && !string.IsNullOrEmpty(state))
            _query = _query.Where(a => a.State == state);
        return this;
    }
    
    public AuthorQueryBuilder FilterByPhone(bool enabled, string? phone)
    {
        if (enabled && !string.IsNullOrEmpty(phone))
            _query = _query.Where(a => a.Phone == phone);
        return this;
    }
    
    // ... 他のフィルター
    
    public IQueryable<Author> Build() => _query;
}

// 使用例
var query = new AuthorQueryBuilder(pubs.Authors)
    .FilterByState(vm.IsEnabledState, vm.State)
    .FilterByPhone(vm.IsEnabledPhone, vm.Phone)
    .FilterByContract(vm.IsEnabledContract, vm.Contract)
    .FilterByFirstName(vm.IsEnabledAuFname, vm.AuFname)
    .Build();
```

**効果：**
- 可読性向上
- テスタビリティ向上
- 再利用性向上

---

## 4. 保守性の向上

### 4.1 設定値の外部化（高優先度）
**問題点：**
`Program.cs` でハードコードされている設定値。

**推奨対応：**
```csharp
// appsettings.json
{
  "DataProtection": {
    "UseSharedKeyOnDatabase": false,
    "ApplicationName": "AzRefArc.AspNetBlazorUnited"
  },
  "Database": {
    "EnableRetryOnFailure": true,
    "ConnectionTimeout": 30
  }
}

// 設定クラス
public class DataProtectionSettings
{
    public bool UseSharedKeyOnDatabase { get; set; }
    public string ApplicationName { get; set; } = string.Empty;
}

// Program.cs
var dataProtectionSettings = builder.Configuration
    .GetSection("DataProtection")
    .Get<DataProtectionSettings>();
```

**効果：**
- 設定変更がコード変更不要に
- 環境ごとの設定管理が容易に

### 4.2 エラーメッセージの外部化（中優先度）
**問題点：**
バリデーションエラーメッセージが各ViewModelにハードコードされています。

**推奨対応：**
```csharp
// Resources/ValidationMessages.resx を作成
// または appsettings.json に定義

public static class ValidationMessages
{
    public const string StateRequired = "州が指定されていません。";
    public const string PhoneRequired = "電話番号が指定されていません。";
    public const string StateFormat = "州は半角 2 文字です。";
    public const string PhoneFormat = "電話番号は 012 456-7894 のような形式で入力してください。";
    // ... 他のメッセージ
}
```

**効果：**
- 多言語化対応が容易に
- メッセージの一貫性向上

### 4.3 カスタム属性の作成（中優先度）
**問題点：**
正規表現のパターンが複数箇所に散らばっています。

**推奨対応：**
```csharp
[AttributeUsage(AttributeTargets.Property)]
public class UsStateAttribute : RegularExpressionAttribute
{
    public UsStateAttribute() 
        : base(@"^[A-Z]{2}$")
    {
        ErrorMessage = "州は半角 2 文字です。";
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class PhoneNumberAttribute : RegularExpressionAttribute
{
    public PhoneNumberAttribute() 
        : base(@"^\d{3} \d{3}-\d{4}$")
    {
        ErrorMessage = "電話番号は 012 456-7894 のような形式で入力してください。";
    }
}

// 使用例
public class FindConditionViewModel
{
    [UsState]
    public string State { get; set; } = "";
    
    [PhoneNumber]
    public string Phone { get; set; } = "";
}
```

**効果：**
- バリデーションロジックの再利用
- 一貫性の向上
- 変更の容易性

---

## 5. セキュリティと堅牢性

### 5.1 設定値の検証強化（高優先度）
**問題点：**
`Program.cs` での設定値の解析にエラーハンドリングが不足しています。

**推奨対応：**
```csharp
// 現在
string? useSharedKeyOnDatabase = builder.Configuration["DataProtection:UseSharedKeyOnDatabase"];
if (string.IsNullOrEmpty(useSharedKeyOnDatabase) == false && bool.Parse(useSharedKeyOnDatabase))
{
    // ...
}

// 改善後
string? useSharedKeyOnDatabase = builder.Configuration["DataProtection:UseSharedKeyOnDatabase"];
if (!string.IsNullOrEmpty(useSharedKeyOnDatabase) && 
    bool.TryParse(useSharedKeyOnDatabase, out bool shouldUseSharedKey) && 
    shouldUseSharedKey)
{
    // ...
}
```

**効果：**
- 無効な設定値による実行時エラーの防止
- より堅牢なアプリケーション

### 5.2 空例外ハンドラーの改善（中優先度）
**問題点：**
`ExceptionFileLogger.cs` に空の catch ブロックがあります。

**推奨対応：**
```csharp
// 現在
catch (Exception)
{
}

// 改善後
catch (Exception ex)
{
    // リフレクション操作は失敗する可能性があるため、
    // 最低限のログは残す
    Debug.WriteLine($"Failed to get property value: {ex.Message}");
}
```

**効果：**
- デバッグ時の問題特定が容易に
- 予期しない問題の早期発見

### 5.3 async void の使用見直し（高優先度）
**問題点：**
`ExceptionFileLogger.cs` (Client版) の Log メソッドが async void です。

**推奨対応：**
```csharp
// 問題のあるコード
public async void Log<TState>(...)
{
    // ...
}

// ILogger インターフェースは void を要求するため、
// 内部で Task を適切に処理
public void Log<TState>(...)
{
    if (!IsEnabled(logLevel)) return;
    
    _ = LogAsync(logLevel, eventId, state, exception, formatter);
}

private async Task LogAsync<TState>(
    LogLevel logLevel, 
    EventId eventId, 
    TState state, 
    Exception? exception, 
    Func<TState, Exception?, string> formatter)
{
    try
    {
        string existingData = await jSRuntime.InvokeAsync<string>("localStorage.getItem", "exceptionData");
        await jSRuntime.InvokeVoidAsync("localStorage.setItem", "exceptionData",
            existingData + "\r\n" + $"{formatter(state, exception)}" + "\r\n" + 
            (exception == null ? "" : ConvertExceptionToString(exception)));
    }
    catch (Exception ex)
    {
        // ログ出力の失敗は無視するが、デバッグ時には通知
        Debug.WriteLine($"Failed to log exception: {ex.Message}");
    }
}
```

**効果：**
- 例外の適切な処理
- デバッグの容易性向上

---

## 6. その他の改善提案

### 6.1 プロジェクト構造の見直し（低優先度）
**推奨対応：**
```
現在の構造:
AzRefArc.AspNetBlazorUnited/
AzRefArc.AspNetBlazorUnited.Client/

提案する構造:
AzRefArc.AspNetBlazorUnited/          (Server)
AzRefArc.AspNetBlazorUnited.Client/   (Client)
AzRefArc.AspNetBlazorUnited.Shared/   (Shared - 新規)
  - ViewModels/
  - Models/
  - Utilities/
  - Validators/
```

### 6.2 依存性注入の活用（中優先度）
**問題点：**
直接 `USStatesUtil.GetAllStates()` を呼び出しています。

**推奨対応：**
```csharp
public interface IStateProvider
{
    IReadOnlyDictionary<string, string> GetAllStates();
}

public class USStateProvider : IStateProvider
{
    // 実装
}

// 利用側
@inject IStateProvider StateProvider

protected override void OnInitialized()
{
    states = new SortedDictionary<string, string>(StateProvider.GetAllStates());
    states.Add("", "");
}
```

**効果：**
- テスタビリティ向上
- 将来的な拡張性（他の国への対応など）

### 6.3 レコード型の活用（低優先度）
**問題点：**
DTOクラスが可変になっています。

**推奨対応：**
```csharp
// AuthorOverview.cs など
public record AuthorOverview
{
    public required string AuthorId { get; init; }
    public required string AuthorName { get; init; }
    public string? Phone { get; init; }
    public string? State { get; init; }
    public bool Contract { get; init; }
}
```

**効果：**
- イミュータビリティによる安全性向上
- 簡潔な構文

---

## 優先順位まとめ

### 高優先度（推奨）
1. ExceptionFileLogger の重複削減
2. FindConditionViewModel の統合
3. FilterByTopNViewModel の統合
4. DbContext 設定の共通化
5. 設定値の検証強化
6. async void の使用見直し

### 中優先度
1. FilterByCondition.razor の共通化
2. USStatesUtil の改善
3. ConvertExceptionToString メソッドの分割
4. クエリビルダーパターンの適用
5. エラーメッセージの外部化
6. カスタム属性の作成

### 低優先度
1. マジックナンバーの定数化
2. プロジェクト構造の見直し
3. レコード型の活用

---

## 実装時の注意点

1. **段階的な実施**: すべてのリファクタリングを一度に行うのではなく、小さな単位で実施してください。
2. **テストの実施**: 各リファクタリング後、必ず動作確認を行ってください。
3. **後方互換性**: 既存の機能を壊さないように注意してください。
4. **コードレビュー**: チーム内でレビューを行い、方針を確認してください。
5. **ドキュメント更新**: コード変更に伴い、必要に応じてドキュメントを更新してください。

---

## 結論

このプロジェクトは全体的に良く構造化されていますが、Blazor United の性質上、サーバーとクライアントで類似したコードが発生しやすくなっています。上記のリファクタリングを実施することで、以下の効果が期待できます：

- **コード量**: 約200-300行の削減
- **保守性**: バグ修正や機能追加の工数削減
- **可読性**: コードの理解が容易に
- **品質**: より堅牢なアプリケーション

特に高優先度の項目から着手することをお勧めします。
