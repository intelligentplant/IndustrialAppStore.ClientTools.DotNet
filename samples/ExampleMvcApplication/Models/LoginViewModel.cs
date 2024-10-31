using Microsoft.AspNetCore.Mvc;

namespace ExampleMvcApplication.Models {
    public class LoginViewModel {

        public bool IsPersistent { get; set; }

        [HiddenInput]
        public string RedirectUrl { get; set; } = default!;

    }
}
