using System;
using System.Collections.Generic;

namespace CutTheRope.ios
{
	internal class NSString : NSObject
	{
		private string value_;

		public NSString()
		{
			value_ = "";
		}

		public NSString(string rhs)
		{
			value_ = rhs;
		}

		public override string ToString()
		{
			return value_;
		}

		public int length()
		{
			if (value_ == null)
			{
				return 0;
			}
			return value_.Length;
		}

		public bool isEqualToString(NSString str)
		{
			return isEqualToString(str.value_);
		}

		public bool isEqualToString(string str)
		{
			return value_ == str;
		}

		public int IndexOf(char c)
		{
			return value_.IndexOf(c);
		}

		public NSRange rangeOfString(string str)
		{
			NSRange result = default(NSRange);
			result.length = 0u;
			result.location = 0u;
			if (str.Length > 0)
			{
				int num = value_.IndexOf(str);
				if (num > -1)
				{
					result.length = (uint)str.Length;
					result.location = (uint)num;
				}
			}
			return result;
		}

		public char characterAtIndex(int n)
		{
			return value_[n];
		}

		public char[] getCharacters()
		{
			return value_.ToCharArray();
		}

		public NSString substringWithRange(NSRange range)
		{
			return new NSString(value_.Substring((int)range.location, (int)range.length));
		}

		public NSString substringFromIndex(int n)
		{
			return new NSString(value_.Substring(n));
		}

		public NSString substringToIndex(int n)
		{
			return new NSString(value_.Substring(0, n));
		}

		public int intValue()
		{
			int ret = 0;
			Int32.TryParse(value_, out ret);
			return ret;
		}

		public bool boolValue()
		{
			bool ret = false;
			bool.TryParse(value_, out ret);
			return ret;
		}

		public float floatValue()
		{
			float ret = 0f;
			float.TryParse(value_, out ret);
			return ret;
		}

		public List<NSString> componentsSeparatedByString(char separator)
		{
			List<NSString> list = new List<NSString>();
			string[] array = value_.Split(separator);
			string[] array2 = array;
			foreach (string rhs in array2)
			{
				list.Add(new NSString(rhs));
			}
			return list;
		}

		public bool hasPrefix(NSString prefix)
		{
			return value_.StartsWith(prefix.ToString());
		}

		public bool hasSuffix(string p)
		{
			return value_.EndsWith(p);
		}
	}
}
