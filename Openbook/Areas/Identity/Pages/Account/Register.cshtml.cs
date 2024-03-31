// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Openbook.Data;
using Openbook.Servicios;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Openbook.Areas.Identity.Pages.Account
{
	public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ApplicationDbContext _context;


        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
			public string? Role { get; set; }
			[ValidateNever]
			public IEnumerable<SelectListItem> RoleList { get; set; }
		}


        public async Task OnGetAsync(string returnUrl = null)
        {
			//if (!_roleManager.RoleExistsAsync(SD.Role_user_SuperAdmin).GetAwaiter().GetResult())
			//{
				_roleManager.CreateAsync(new IdentityRole(SD.Role_user_SuperAdmin)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Accountant)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Client)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Role_User)).GetAwaiter().GetResult();
			//}
			ReturnUrl = returnUrl;
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			Input = new InputModel()
			{
				RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
				{
					Text = i,
					Value = i
				}),
			};
		}

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
   //         returnUrl ??= Url.Content("~/");
   //         ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
   //         if (ModelState.IsValid)
   //         {
   //             var usuario = new IdentityUser() { Email = Input.Email, UserName = Input.Email };

   //             var resultado = await _userManager.CreateAsync(usuario, password: Input.Password);
                
   //             if (resultado.Succeeded)
   //             {
			//		var claimsPersonalizados = new List<Claim>()
			//{
			//	new Claim(Constantes.ClaimTenantId, usuario.Id)
			//};

			//		await _userManager.AddClaimsAsync(usuario, claimsPersonalizados);
			//		//if (Input.Role == null)
			//		//{
			//			await _userManager.AddToRoleAsync(usuario, "Company");
			//		//}
			//		//else
			//		//{
			//		//	await _userManager.AddToRoleAsync(usuario, Input.Role);
			//		//}
			//		await _signInManager.SignInAsync(usuario, isPersistent: true);
   //                 return LocalRedirect("/setup");
   //             }
   //             else
   //             {
   //                 foreach (var error in resultado.Errors)
   //                 {
   //                     //ModelState.AddModelError(string.Empty, error.Description);
   //                 }
   //                 //return 0;
   //             }
   //         }

   //         // If we got this far, something failed, redisplay form
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
