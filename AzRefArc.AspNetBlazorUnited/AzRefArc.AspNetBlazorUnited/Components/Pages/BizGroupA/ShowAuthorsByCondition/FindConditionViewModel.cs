using System.ComponentModel.DataAnnotations;

namespace AzRefArc.AspNetBlazorUnited.Components.Pages.BizGroupA.ShowAuthorsByCondition
{
    [CustomValidation(typeof(FindConditionViewModel), "CheckRequireState")]
    [CustomValidation(typeof(FindConditionViewModel), "CheckRequirePhone")]
    [CustomValidation(typeof(FindConditionViewModel), "CheckRequireContract")]
    [CustomValidation(typeof(FindConditionViewModel), "CheckRequireAuFname")]
    [CustomValidation(typeof(FindConditionViewModel), "CheckAtLeastOneCheck")]
    public class FindConditionViewModel
    {
        public bool IsEnabledState { get; set; }
        public bool IsEnabledPhone { get; set; }
        public bool IsEnabledContract { get; set; }
        public bool IsEnabledAuFname { get; set; }

        [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "州は半角 2 文字です。")]
        public string State { get; set; } = "";

        [RegularExpression(@"^\d{3} \d{3}-\d{4}$", ErrorMessage = "電話番号は 012 456-7894 のような形式で入力してください。")]
        public string Phone { get; set; } = "";

        public bool? Contract
        {
            get { return (ContractString.ToLower() == "true" ? true : (ContractString.ToLower() == "false" ? false : null)); }
            set { ContractString = (value.HasValue == false ? String.Empty : (value.Value.ToString())); }
        }

        // データバインド用 (ドロップダウンリストには bool 型を bind できないため)
        public string ContractString { get; set; } = string.Empty;

        [RegularExpression(@"^[\u0020-\u007e]{1,20}$", ErrorMessage = "名前は英数半角 20 文字以内で入力してください。")]
        public string AuFname { get; set; } = "";

        public static ValidationResult CheckRequireState(FindConditionViewModel vm, ValidationContext ctx)
        {
            if (vm.IsEnabledState == true && string.IsNullOrEmpty(vm.State) == true)
                return new ValidationResult("州が指定されていません。", new List<string>() { "IsEnabledState", "State" });
            return ValidationResult.Success!;
        }

        public static ValidationResult CheckRequirePhone(FindConditionViewModel vm, ValidationContext ctx)
        {
            if (vm.IsEnabledPhone == true && string.IsNullOrEmpty(vm.Phone) == true)
                return new ValidationResult("電話番号が指定されていません。", new List<string>() { "IsEnabledPhone", "Phone" });
            return ValidationResult.Success!;
        }

        public static ValidationResult CheckRequireContract(FindConditionViewModel vm, ValidationContext ctx)
        {
            if (vm.IsEnabledContract == true && vm.Contract == null)
                return new ValidationResult("契約有無が指定されていません。", new List<string>() { "IsEnabledContract", "Contract" });
            return ValidationResult.Success!;
        }

        public static ValidationResult CheckRequireAuFname(FindConditionViewModel vm, ValidationContext ctx)
        {
            if (vm.IsEnabledAuFname == true && string.IsNullOrEmpty(vm.AuFname) == true)
                return new ValidationResult("名前が指定されていません。", new List<string>() { "IsEnabledAuFname", "AuFname" });
            return ValidationResult.Success!;
        }

        public static ValidationResult CheckAtLeastOneCheck(FindConditionViewModel vm, ValidationContext ctx)
        {
            if (vm.IsEnabledState == false && vm.IsEnabledPhone == false && vm.IsEnabledContract == false && vm.IsEnabledAuFname == false)
                return new ValidationResult("少なくとも一つの検索条件を指定してください。", new List<string>() { "IsEnabledState", "IsEnabledPhone", "IsEnabledContract", "IsEnabledAuFname" });
            return ValidationResult.Success!;
        }
    }
}
