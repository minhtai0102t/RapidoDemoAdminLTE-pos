using DemoAdminLTE.DAL;
using DemoAdminLTE.Models;
using System;
using System.Linq;
using System.Web.Helpers;
using System.Web.Security;

namespace DemoAdminLTE.CustomAuthentication
{
    public class CustomMembership : MembershipProvider
    {


        /// <summary>  
        ///   
        /// </summary>  
        /// <param name="username"></param>  
        /// <param name="password"></param>  
        /// <returns></returns>  
        public override bool ValidateUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            using (DemoContext dbContext = new DemoContext())
            {
                var user = (from us in dbContext.Users
                            where string.Compare(username, us.Username, StringComparison.OrdinalIgnoreCase) == 0
                            //&& us.IsApproved == true
                            select us).FirstOrDefault();

                return (user != null && Crypto.VerifyHashedPassword(user.PasswordHash, password)) ? true : false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static User ValidateUserByPhone(string phone, string password)
        {
            if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            using (DemoContext dbContext = new DemoContext())
            {
                var user = (from us in dbContext.Users
                            where string.Compare(phone, us.Phone, StringComparison.OrdinalIgnoreCase) == 0
                            //&& us.IsApproved == true
                            select us).FirstOrDefault();

                if (user != null && Crypto.VerifyHashedPassword(user.PasswordHash, password))
                {
                    return user;
                }
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static User ValidateUserByEmail(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            using (DemoContext dbContext = new DemoContext())
            {
                var user = (from us in dbContext.Users
                            where string.Compare(email, us.Email, StringComparison.OrdinalIgnoreCase) == 0
                            //&& us.IsApproved == true
                            select us).FirstOrDefault();
                if(user != null && Crypto.VerifyHashedPassword(user.PasswordHash, password))
                {
                    return user;
                }
                return null;
            }
        }

        /// <summary>  
        ///   
        /// </summary>  
        /// <param name="username"></param>  
        /// <param name="password"></param>  
        /// <param name="email"></param>  
        /// <param name="passwordQuestion"></param>  
        /// <param name="passwordAnswer"></param>  
        /// <param name="isApproved"></param>  
        /// <param name="providerUserKey"></param>  
        /// <param name="status"></param>  
        /// <returns></returns>  
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        /// <summary>  
        ///   
        /// </summary>  
        /// <param name="username"></param>  
        /// <param name="userIsOnline"></param>  
        /// <returns></returns>  
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            using (DemoContext dbContext = new DemoContext())
            {
                var user = (from us in dbContext.Users
                            where string.Compare(username, us.Username, StringComparison.OrdinalIgnoreCase) == 0
                            select us).FirstOrDefault();

                if (user == null)
                {
                    return null;
                }
                var selectedUser = new CustomMembershipUser(user);

                return selectedUser;
            }
        }

        public override string GetUserNameByEmail(string email)
        {
            using (DemoContext dbContext = new DemoContext())
            {
                string username = (from u in dbContext.Users
                                   where string.Compare(email, u.Email) == 0
                                   select u.Username).FirstOrDefault();

                return !string.IsNullOrEmpty(username) ? username : string.Empty;
            }
        }

        public static string GetUserNameByPhonenumber(string phone)
        {
            using (DemoContext dbContext = new DemoContext())
            {
                string username = (from u in dbContext.Users
                                   where string.Compare(phone, u.Phone) == 0
                                   select u.Username).FirstOrDefault();

                return !string.IsNullOrEmpty(username) ? username : string.Empty;
            }
        }

        #region Overrides of Membership Provider  

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool EnablePasswordReset
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool EnablePasswordRetrieval
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int MinRequiredPasswordLength
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int PasswordAttemptWindow
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string PasswordStrengthRegularExpression
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool RequiresUniqueEmail
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}