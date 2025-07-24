// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MKxStore247.Models;

namespace MKxStore247.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<UserApplication> _userManager;
        private readonly SignInManager<UserApplication> _signInManager;

        public IndexModel(
            UserManager<UserApplication> userManager,
            SignInManager<UserApplication> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Họ và tên")]
            [StringLength(100, ErrorMessage = "Họ tên không được vượt quá {1} ký tự.")]
            public string FullName { get; set; }

            [Phone]
            [Display(Name = "Số điện thoại")]
            public string PhoneNumber { get; set; }

            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Display(Name = "Giới tính")]
            public string Gender { get; set; }

            [Display(Name = "Ngày sinh")]
            [DataType(DataType.Date)]
            public DateTime? DateOfBirth { get; set; }

            [Display(Name = "Địa chỉ")]
            [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá {1} ký tự.")]
            public string Address { get; set; }

            [Display(Name = "Avatar URL")]
            [Url(ErrorMessage = "URL avatar không hợp lệ.")]
            public string AvatarUrl { get; set; }
        }

        private async Task LoadAsync(UserApplication user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var email = await _userManager.GetEmailAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Email = email,
                FullName = user.FullName,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                AvatarUrl = user.AvatarUrl
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // Update phone number
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Lỗi không mong muốn khi cập nhật số điện thoại.";
                    return RedirectToPage();
                }
            }

            // Update email
            var email = await _userManager.GetEmailAsync(user);
            if (Input.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    StatusMessage = "Lỗi không mong muốn khi cập nhật email.";
                    return RedirectToPage();
                }
            }

            // Update custom properties
            bool needsUpdate = false;

            if (user.FullName != Input.FullName)
            {
                user.FullName = Input.FullName;
                needsUpdate = true;
            }

            if (user.Gender != Input.Gender)
            {
                user.Gender = Input.Gender;
                needsUpdate = true;
            }

            if (user.DateOfBirth != Input.DateOfBirth)
            {
                user.DateOfBirth = Input.DateOfBirth;
                needsUpdate = true;
            }

            if (user.Address != Input.Address)
            {
                user.Address = Input.Address;
                needsUpdate = true;
            }

            if (user.AvatarUrl != Input.AvatarUrl)
            {
                user.AvatarUrl = Input.AvatarUrl;
                needsUpdate = true;
            }

            if (needsUpdate)
            {
                user.UpdatedAt = DateTime.Now;
                user.UpdatedBy = user.Id; // hoặc lấy từ current user context

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    StatusMessage = "Lỗi không mong muốn khi cập nhật thông tin cá nhân.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Thông tin cá nhân đã được cập nhật thành công.";
            return RedirectToPage();
        }
    }
}