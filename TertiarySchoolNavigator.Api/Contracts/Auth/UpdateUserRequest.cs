﻿namespace TertiarySchoolNavigator.Api.Contracts.Auth
{
    public class UpdateUserRequest
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
