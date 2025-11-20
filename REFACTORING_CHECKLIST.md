# リファクタリング実装チェックリスト

このチェックリストは、推奨されたリファクタリングを段階的に実装する際の作業リストです。

## 使い方
- [ ] チェックボックスにチェックを入れて進捗を管理
- 各項目には優先度が付いています（🔴高 🟡中 ⚪低）
- 詳細は REFACTORING_RECOMMENDATIONS.md を参照

---

## Phase 1: 緊急対応（推奨期間: 1週間）

### 🔴 高優先度 - セキュリティ関連

- [ ] **async void の修正**
  - [ ] `AzRefArc.AspNetBlazorUnited.Client/ExceptionFileLogger.cs` の Log メソッドを修正
  - [ ] 適切なエラーハンドリングを追加
  - [ ] 動作確認・テスト実施
  - **ファイル**: `ExceptionFileLogger.cs` (Client)
  - **見積もり**: 30分

- [ ] **設定値検証の強化**
  - [ ] `Program.cs` の `bool.Parse()` を `bool.TryParse()` に変更
  - [ ] その他の設定値パースも確認
  - [ ] 無効な設定値のテストケース追加
  - **ファイル**: `Program.cs`
  - **見積もり**: 30分

### 🔴 高優先度 - コード重複削減

- [ ] **Shared プロジェクトの作成**
  - [ ] `AzRefArc.AspNetBlazorUnited.Shared` プロジェクト作成
  - [ ] ソリューションに追加
  - [ ] Server/Client プロジェクトから参照追加
  - **見積もり**: 15分

- [ ] **FindConditionViewModel の統合**
  - [ ] BizGroupA の FindConditionViewModel を Shared に移動
  - [ ] BizGroupC の FindConditionViewModel を削除
  - [ ] 両方のページから Shared を参照
  - [ ] ビルド・動作確認
  - **ファイル**: 
    - `BizGroupA/ShowAuthorsByCondition/FindConditionViewModel.cs`
    - `BizGroupC/ShowAuthorsByCondition/FindConditionViewModel.cs`
  - **見積もり**: 20分

- [ ] **FilterByTopNViewModel の統合**
  - [ ] BizGroupA の FilterByTopNViewModel を Shared に移動
  - [ ] BizGroupC の FilterByTopNViewModel を削除
  - [ ] 両方のページから Shared を参照
  - [ ] ビルド・動作確認
  - **ファイル**: 
    - `BizGroupA/ShowAuthorsByTopN/FilterByTopNViewModel.cs`
    - `BizGroupC/ShowAuthorsByTopN/FilterByTopNViewModel.cs`
  - **見積もり**: 20分

**Phase 1 完了確認**
- [ ] すべてのテストがパス
- [ ] ビルドエラーなし
- [ ] セキュリティスキャンクリア
- [ ] コードレビュー完了

---

## Phase 2: リファクタリング（推奨期間: 2-3週間）

### 🔴 高優先度 - 設定の共通化

- [ ] **DbContext 設定の拡張メソッド化**
  - [ ] `DbContextExtensions.cs` クラスを作成
  - [ ] `AddPubsDbContext<T>` 拡張メソッドを実装
  - [ ] Program.cs で使用
  - [ ] 動作確認（両方の DbContext で）
  - **新規ファイル**: `Extensions/DbContextExtensions.cs`
  - **見積もり**: 1時間

### 🔴 高優先度 - ExceptionFileLogger のリファクタリング

- [ ] **共通ロジックの抽出**
  - [ ] `ExceptionFormatterBase` クラスを Shared に作成
  - [ ] `ConvertExceptionToString` メソッドを共通化
  - [ ] メソッドを小さな単位に分割:
    - [ ] `BuildGeneralInformation()`
    - [ ] `BuildExceptionInformation()`
    - [ ] `BuildExceptionProperties()`
  - [ ] Server 版の ExceptionFileLogger を更新
  - [ ] Client 版の ExceptionFileLogger を更新
  - [ ] 動作確認（エラーログ出力テスト）
  - **見積もり**: 2時間

### 🟡 中優先度 - カスタム属性の作成

- [ ] **バリデーション属性の作成**
  - [ ] `Attributes` フォルダを Shared に作成
  - [ ] `UsStateAttribute` を実装
  - [ ] `PhoneNumberAttribute` を実装
  - [ ] 既存の ViewModel で使用
  - [ ] バリデーションテスト実施
  - **新規ファイル**: 
    - `Attributes/UsStateAttribute.cs`
    - `Attributes/PhoneNumberAttribute.cs`
  - **見積もり**: 1時間

### 🟡 中優先度 - クエリビルダーの実装

- [ ] **AuthorQueryBuilder クラスの作成**
  - [ ] `Builders/AuthorQueryBuilder.cs` を作成
  - [ ] Filter メソッドを実装:
    - [ ] `FilterByState()`
    - [ ] `FilterByPhone()`
    - [ ] `FilterByContract()`
    - [ ] `FilterByFirstName()`
  - [ ] FilterByCondition.razor で使用
  - [ ] 動作確認（検索機能テスト）
  - **新規ファイル**: `Builders/AuthorQueryBuilder.cs`
  - **見積もり**: 1.5時間

**Phase 2 完了確認**
- [ ] すべてのテストがパス
- [ ] ビルドエラーなし
- [ ] パフォーマンステスト実施
- [ ] コードレビュー完了

---

## Phase 3: 最適化（推奨期間: 4週間）

### 🟡 中優先度 - パフォーマンス最適化

- [ ] **USStatesUtil の最適化**
  - [ ] Lazy 初期化を実装
  - [ ] `GetAllStatesReadOnly()` メソッドを追加
  - [ ] 既存の呼び出し箇所を更新
  - [ ] パフォーマンステスト実施
  - **ファイル**: `Data/USStateUtil.cs`
  - **見積もり**: 30分

### 🟡 中優先度 - 設定の外部化

- [ ] **設定クラスの作成**
  - [ ] `DataProtectionSettings.cs` を作成
  - [ ] `DatabaseSettings.cs` を作成
  - [ ] appsettings.json に設定追加
  - [ ] Program.cs で使用
  - [ ] 環境別設定テスト
  - **新規ファイル**: 
    - `Configuration/DataProtectionSettings.cs`
    - `Configuration/DatabaseSettings.cs`
  - **見積もり**: 1時間

- [ ] **エラーメッセージの外部化**
  - [ ] `ValidationMessages.cs` を作成
  - [ ] すべてのエラーメッセージを定数化
  - [ ] ViewModel で使用
  - [ ] （オプション）リソースファイル化
  - **新規ファイル**: `Resources/ValidationMessages.cs`
  - **見積もり**: 1時間

### 🟡 中優先度 - 共通コンポーネント

- [ ] **AuthorSearchForm コンポーネント**
  - [ ] `Components/Shared/AuthorSearchForm.razor` を作成
  - [ ] BizGroupA の FilterByCondition から抽出
  - [ ] パラメータで動作切り替え可能にする
  - [ ] BizGroupA と BizGroupC で使用
  - [ ] UI・動作確認
  - **新規ファイル**: `Components/Shared/AuthorSearchForm.razor`
  - **見積もり**: 2時間

### ⚪ 低優先度 - 依存性注入の活用

- [ ] **IStateProvider インターフェース**
  - [ ] `Interfaces/IStateProvider.cs` を作成
  - [ ] `USStateProvider` 実装クラスを作成
  - [ ] DI コンテナに登録
  - [ ] 既存のコードで使用
  - [ ] 単体テスト作成
  - **新規ファイル**: 
    - `Interfaces/IStateProvider.cs`
    - `Services/USStateProvider.cs`
  - **見積もり**: 1時間

**Phase 3 完了確認**
- [ ] すべてのテストがパス
- [ ] ビルドエラーなし
- [ ] パフォーマンス改善確認
- [ ] コードカバレッジ確認
- [ ] 最終コードレビュー完了

---

## 低優先度項目（時間があれば実施）

### ⚪ コード品質

- [ ] **マジックナンバーの定数化**
  - [ ] 定数クラスを作成
  - [ ] マジックナンバーを置換
  - **見積もり**: 30分

- [ ] **空 catch ブロックの改善**
  - [ ] Debug.WriteLine を追加
  - [ ] 適切なログ出力
  - **見積もり**: 20分

### ⚪ 構造改善

- [ ] **レコード型への変換**
  - [ ] DTOクラスをレコード型に変換
  - [ ] イミュータビリティの確保
  - **見積もり**: 1時間

- [ ] **プロジェクト構造の見直し**
  - [ ] フォルダ構造の整理
  - [ ] 名前空間の統一
  - **見積もり**: 2時間

---

## 各フェーズ後の確認事項

### ビルド確認
```bash
dotnet build AzRefArc.AspNetBlazorUnited.sln
```

### テスト実行（テストプロジェクトがある場合）
```bash
dotnet test AzRefArc.AspNetBlazorUnited.sln
```

### アプリケーション起動確認
```bash
dotnet run --project AzRefArc.AspNetBlazorUnited
```

### 主要機能の動作確認
- [ ] 著者一覧表示
- [ ] 条件検索
- [ ] TopN検索
- [ ] データ編集
- [ ] エラー処理

---

## 進捗管理

### Phase 1
- 開始日: ___/___/___
- 完了予定日: ___/___/___
- 実際の完了日: ___/___/___
- 担当者: ___________

### Phase 2
- 開始日: ___/___/___
- 完了予定日: ___/___/___
- 実際の完了日: ___/___/___
- 担当者: ___________

### Phase 3
- 開始日: ___/___/___
- 完了予定日: ___/___/___
- 実際の完了日: ___/___/___
- 担当者: ___________

---

## トラブルシューティング

### ビルドエラーが発生した場合
1. 名前空間の using を確認
2. プロジェクト参照を確認
3. NuGet パッケージの復元

### 動作が変わってしまった場合
1. git で変更を確認
2. 変更前後の動作を比較
3. 必要に応じてロールバック

### テストが失敗する場合
1. テストコードを確認
2. 期待値と実際の値を比較
3. ログを確認

---

## 参考資料

- 詳細な説明: [REFACTORING_RECOMMENDATIONS.md](./REFACTORING_RECOMMENDATIONS.md)
- 概要: [REFACTORING_SUMMARY.md](./REFACTORING_SUMMARY.md)
- .NET ドキュメント: https://docs.microsoft.com/ja-jp/dotnet/
- Blazor ドキュメント: https://docs.microsoft.com/ja-jp/aspnet/core/blazor/

---

**更新日**: 2025-11-20
**バージョン**: 1.0
