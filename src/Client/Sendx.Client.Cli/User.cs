using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Security;

namespace Sendx.Client.Cli
{
	public class User
	{
		[StringLength(50, ErrorMessage = "{0} length must be less than or equal {1}.")]
		public string Password
		{
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					_password = new SecureString();
					foreach (var c in value.ToCharArray()) 
						_password.AppendChar(c);
				}
				else
					_password = null;
			}
			get
			{
				if (_password != null)
				{
					var unmanaged = IntPtr.Zero;
					try
					{
						unmanaged = Marshal.SecureStringToGlobalAllocUnicode(_password);
						return Marshal.PtrToStringUni(unmanaged);
					}
					finally
					{
						Marshal.ZeroFreeGlobalAllocUnicode(unmanaged);
					}
				}

				return null;
			}
		}
		
		private SecureString _password;
	}
}
