﻿using System.ComponentModel.DataAnnotations;

namespace AzRefArc.AspNetBlazorUnited.Components.Pages.BizGroupB.EditAuthorByOptimistic
{
    [CustomValidation(typeof(EditViewModel), "NameAndPhoneCheck")]
    public class EditViewModel
    {
        // 入力対象でないフィールドにはデータアノテーションを付与する必要はない
        [Display(Name = "著者ID")]
        public string AuthorId { get; set; } = String.Empty;

        [Display(Name = "著者名（名）")]
        [Required(ErrorMessage = "著者名（名）は必須入力です。")]
        [RegularExpression(@"^[\u0020-\u007e]{1,20}$", ErrorMessage = "著者名（名）は半角 20 文字以内で入力してください。")]
        public string AuthorFirstName { get; set; } = String.Empty;

        [Display(Name = "著者名（姓）")]
        [Required(ErrorMessage = "著者名（姓）は必須入力です。")]
        [RegularExpression(@"^[\u0020-\u007e]{1,40}$", ErrorMessage = "著者名（姓）は半角 40 文字以内で入力してください。")]
        public string AuthorLastName { get; set; } = String.Empty;

        [Display(Name = "電話番号")]
        [Required(ErrorMessage = "電話番号は必須入力です。")]
        [RegularExpression(@"^\d{3} \d{3}-\d{4}$", ErrorMessage = "電話番号は 012 345-6789 のように入力してください。")]
        public string Phone { get; set; } = String.Empty;

        [Display(Name = "州")]
        [Required(ErrorMessage = "州は必須入力です。")]
        [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "州はアルファベット 2 文字で指定してください。")]
        public string State { get; set; } = String.Empty;

        // フォームレベルでの単体入力チェック項目に対するロジックなどを実装
        public static ValidationResult NameAndPhoneCheck(EditViewModel vm, ValidationContext ctx)
        {
            if (vm.AuthorFirstName == "Nobuyuki" && vm.AuthorLastName == "Akama")
                return new ValidationResult("Nobuyuki Akama という名前は予約済みのため登録できません。", new List<string>() { "AuthorFirstName", "AuthorLastName" });
            return ValidationResult.Success!;
        }
    }
}
