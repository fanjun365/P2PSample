using System;
using System.Collections.Generic;
using ZLBase.Communicate;

namespace FanJun.P2PSample.Server
{
    internal class MockMembershipService : IMembershipService
    {
        private Dictionary<string, string> m_dict = new Dictionary<string, string>();

        static MockMembershipService s_instance;

        public static MockMembershipService Instance
        {
            get { return s_instance; }
        }

        static MockMembershipService()
        {
            s_instance = new MockMembershipService();
            s_instance.CreateUser("admin", "", "admin", "");
            s_instance.CreateUser("u01", "", "123", "");
            s_instance.CreateUser("u02", "", "123", "");
            s_instance.CreateUser("u03", "", "123", "");
            s_instance.CreateUser("u04", "", "123", "");
            s_instance.CreateUser("u05", "", "123", "");
            s_instance.CreateUser("u06", "", "123", "");
            s_instance.CreateUser("u07", "", "123", "");
            s_instance.CreateUser("u08", "", "123", "");
            s_instance.CreateUser("u09", "", "123", "");
            s_instance.CreateUser("u10", "", "123", "");
            s_instance.CreateUser("fx_installer", "", "zlsoft2015", "");
        }
        private MockMembershipService()
        {
        }

        #region IMembershipService ≥…‘±

        public void CreateUser(string username, string displayName, string password, string comment)
        {
            if (!string.IsNullOrEmpty(username) && !m_dict.ContainsKey(username))
            {
                m_dict.Add(username, password);
            }
        }

        public void LockUser(string username)
        {
        }

        public void UnlockUser(string username)
        {
        }

        public void UpdateUser(string username, string displayName, string comment)
        {
        }

        public void DeleteUser(string username)
        {
            if (!string.IsNullOrEmpty(username) && m_dict.ContainsKey(username))
            {
                m_dict.Remove(username);
            }
        }

        public void ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (!string.IsNullOrEmpty(username) && m_dict.ContainsKey(username))
            {
                m_dict[username] = newPassword;
            }
        }

        public bool ValidateUser(string username, string password)
        {
            if (!string.IsNullOrEmpty(username) && m_dict.ContainsKey(username))
            {
                return m_dict[username] == password;
            }
            return false;
        }

        public bool ExistsUser(string username)
        {
            if (!string.IsNullOrEmpty(username) && m_dict.ContainsKey(username))
            {
                return true;
            }
            return false;
        }

        public User[] GetAllUsers()
        {
            List<User> list = new List<User>();
            foreach (string var in m_dict.Keys)
            {
                list.Add(GetUser(var));
            }
            return list.ToArray();
        }

        public User GetUser(string username)
        {
            return new User(username, username, false, DateTime.MinValue, DateTime.MinValue, string.Empty);
        }

        public void ResetPassword(string username, string newPassword)
        {
            if (!string.IsNullOrEmpty(username) && m_dict.ContainsKey(username))
            {
                m_dict[username] = newPassword;
            }
        }

        #endregion
    }
}
