using System;

namespace Photon.SocketServer.Numeric
{
	// Token: 0x02000004 RID: 4
	internal class BigInteger
	{
		// Token: 0x06000011 RID: 17 RVA: 0x0000228F File Offset: 0x0000048F
		public BigInteger()
		{
			this.data = new uint[70];
			this.dataLength = 1;
		}
		
		// Token: 0x06000012 RID: 18 RVA: 0x000022B8 File Offset: 0x000004B8
		public BigInteger(long value)
		{
			this.data = new uint[70];
			long num = value;
			this.dataLength = 0;
			while (value != 0L && this.dataLength < 70)
			{
				this.data[this.dataLength] = (uint)(value & 0xFFFFFFFFu);
				value >>= 32;
				this.dataLength++;
			}
			if (num > 0L)
			{
				if (value != 0L || (this.data[69] & 2147483648U) != 0U)
				{
					throw new ArithmeticException("Positive overflow in constructor.");
				}
			}
			else if (num < 0L)
			{
				if (value != -1L || (this.data[this.dataLength - 1] & 2147483648U) == 0U)
				{
					throw new ArithmeticException("Negative underflow in constructor.");
				}
			}
			if (this.dataLength == 0)
			{
				this.dataLength = 1;
			}
		}
		
		// Token: 0x06000013 RID: 19 RVA: 0x000023C0 File Offset: 0x000005C0
		public BigInteger(ulong value)
		{
			this.data = new uint[70];
			this.dataLength = 0;
			while (value != 0UL && this.dataLength < 70)
			{
				this.data[this.dataLength] = (uint)(value & 0xFFFFFFFFu);
				value >>= 32;
				this.dataLength++;
			}
			if (value != 0UL || (this.data[69] & 2147483648U) != 0U)
			{
				throw new ArithmeticException("Positive overflow in constructor.");
			}
			if (this.dataLength == 0)
			{
				this.dataLength = 1;
			}
		}
		
		// Token: 0x06000014 RID: 20 RVA: 0x00002474 File Offset: 0x00000674
		public BigInteger(BigInteger bi)
		{
			this.data = new uint[70];
			this.dataLength = bi.dataLength;
			for (int i = 0; i < this.dataLength; i++)
			{
				this.data[i] = bi.data[i];
			}
		}
		
		// Token: 0x06000015 RID: 21 RVA: 0x000024D0 File Offset: 0x000006D0
		public BigInteger(string value, int radix)
		{
			BigInteger bigInteger = new BigInteger(1L);
			BigInteger bigInteger2 = new BigInteger();
			value = value.ToUpper().Trim();
			int num = 0;
			if (value[0] == '-')
			{
				num = 1;
			}
			for (int i = value.Length - 1; i >= num; i--)
			{
				int num2 = (int)value[i];
				if (num2 >= 48 && num2 <= 57)
				{
					num2 -= 48;
				}
				else if (num2 >= 65 && num2 <= 90)
				{
					num2 = num2 - 65 + 10;
				}
				else
				{
					num2 = 9999999;
				}
				if (num2 >= radix)
				{
					throw new ArithmeticException("Invalid string in constructor.");
				}
				if (value[0] == '-')
				{
					num2 = -num2;
				}
				bigInteger2 += bigInteger * num2;
				if (i - 1 >= num)
				{
					bigInteger *= radix;
				}
			}
			if (value[0] == '-')
			{
				if ((bigInteger2.data[69] & 2147483648U) == 0U)
				{
					throw new ArithmeticException("Negative underflow in constructor.");
				}
			}
			else if ((bigInteger2.data[69] & 2147483648U) != 0U)
			{
				throw new ArithmeticException("Positive overflow in constructor.");
			}
			this.data = new uint[70];
			for (int i = 0; i < bigInteger2.dataLength; i++)
			{
				this.data[i] = bigInteger2.data[i];
			}
			this.dataLength = bigInteger2.dataLength;
		}
		
		// Token: 0x06000016 RID: 22 RVA: 0x00002698 File Offset: 0x00000898
		public BigInteger(byte[] inData)
		{
			this.dataLength = inData.Length >> 2;
			int num = inData.Length & 3;
			if (num != 0)
			{
				this.dataLength++;
			}
			if (this.dataLength > 70)
			{
				throw new ArithmeticException("Byte overflow in constructor.");
			}
			this.data = new uint[70];
			int i = inData.Length - 1;
			int num2 = 0;
			while (i >= 3)
			{
				this.data[num2] = (uint)(((int)inData[i - 3] << 24) + ((int)inData[i - 2] << 16) + ((int)inData[i - 1] << 8) + (int)inData[i]);
				i -= 4;
				num2++;
			}
			if (num == 1)
			{
				this.data[this.dataLength - 1] = (uint)inData[0];
			}
			else if (num == 2)
			{
				this.data[this.dataLength - 1] = (uint)(((int)inData[0] << 8) + (int)inData[1]);
			}
			else if (num == 3)
			{
				this.data[this.dataLength - 1] = (uint)(((int)inData[0] << 16) + ((int)inData[1] << 8) + (int)inData[2]);
			}
			while (this.dataLength > 1 && this.data[this.dataLength - 1] == 0U)
			{
				this.dataLength--;
			}
		}
		
		// Token: 0x06000017 RID: 23 RVA: 0x000027EC File Offset: 0x000009EC
		public BigInteger(byte[] inData, int inLen)
		{
			this.dataLength = inLen >> 2;
			int num = inLen & 3;
			if (num != 0)
			{
				this.dataLength++;
			}
			if (this.dataLength > 70 || inLen > inData.Length)
			{
				throw new ArithmeticException("Byte overflow in constructor.");
			}
			this.data = new uint[70];
			int i = inLen - 1;
			int num2 = 0;
			while (i >= 3)
			{
				this.data[num2] = (uint)(((int)inData[i - 3] << 24) + ((int)inData[i - 2] << 16) + ((int)inData[i - 1] << 8) + (int)inData[i]);
				i -= 4;
				num2++;
			}
			if (num == 1)
			{
				this.data[this.dataLength - 1] = (uint)inData[0];
			}
			else if (num == 2)
			{
				this.data[this.dataLength - 1] = (uint)(((int)inData[0] << 8) + (int)inData[1]);
			}
			else if (num == 3)
			{
				this.data[this.dataLength - 1] = (uint)(((int)inData[0] << 16) + ((int)inData[1] << 8) + (int)inData[2]);
			}
			if (this.dataLength == 0)
			{
				this.dataLength = 1;
			}
			while (this.dataLength > 1 && this.data[this.dataLength - 1] == 0U)
			{
				this.dataLength--;
			}
		}
		
		// Token: 0x06000018 RID: 24 RVA: 0x0000295C File Offset: 0x00000B5C
		public BigInteger(uint[] inData)
		{
			this.dataLength = inData.Length;
			if (this.dataLength > 70)
			{
				throw new ArithmeticException("Byte overflow in constructor.");
			}
			this.data = new uint[70];
			int i = this.dataLength - 1;
			int num = 0;
			while (i >= 0)
			{
				this.data[num] = inData[i];
				i--;
				num++;
			}
			while (this.dataLength > 1 && this.data[this.dataLength - 1] == 0U)
			{
				this.dataLength--;
			}
		}
		
		// Token: 0x06000019 RID: 25 RVA: 0x00002A0C File Offset: 0x00000C0C
		public static implicit operator BigInteger(long value)
		{
			return new BigInteger(value);
		}
		
		// Token: 0x0600001A RID: 26 RVA: 0x00002A24 File Offset: 0x00000C24
		public static implicit operator BigInteger(ulong value)
		{
			return new BigInteger(value);
		}
		
		// Token: 0x0600001B RID: 27 RVA: 0x00002A3C File Offset: 0x00000C3C
		public static implicit operator BigInteger(int value)
		{
			return new BigInteger((long)value);
		}
		
		// Token: 0x0600001C RID: 28 RVA: 0x00002A58 File Offset: 0x00000C58
		public static implicit operator BigInteger(uint value)
		{
			return new BigInteger((ulong)value);
		}
		
		// Token: 0x0600001D RID: 29 RVA: 0x00002A74 File Offset: 0x00000C74
		public static BigInteger operator +(BigInteger bi1, BigInteger bi2)
		{
			BigInteger bigInteger = new BigInteger();
			bigInteger.dataLength = ((bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength);
			long num = 0L;
			for (int i = 0; i < bigInteger.dataLength; i++)
			{
				long num2 = (long)((ulong)bi1.data[i] + (ulong)bi2.data[i] + (ulong)num);
				num = num2 >> 32;
				bigInteger.data[i] = (uint)(num2 & 0xFFFFFFFFu);
			}
			if (num != 0L && bigInteger.dataLength < 70)
			{
				bigInteger.data[bigInteger.dataLength] = (uint)num;
				bigInteger.dataLength++;
			}
			while (bigInteger.dataLength > 1 && bigInteger.data[bigInteger.dataLength - 1] == 0U)
			{
				bigInteger.dataLength--;
			}
			int num3 = 69;
			if ((bi1.data[num3] & 2147483648U) == (bi2.data[num3] & 2147483648U) && (bigInteger.data[num3] & 2147483648U) != (bi1.data[num3] & 2147483648U))
			{
				throw new ArithmeticException();
			}
			return bigInteger;
		}
		
		// Token: 0x0600001E RID: 30 RVA: 0x00002BBC File Offset: 0x00000DBC
		public static BigInteger operator ++(BigInteger bi1)
		{
			BigInteger bigInteger = new BigInteger(bi1);
			long num = 1L;
			int num2 = 0;
			while (num != 0L && num2 < 70)
			{
				long num3 = (long)((ulong)bigInteger.data[num2]);
				num3 += 1L;
				bigInteger.data[num2] = (uint)(num3 & 0xFFFFFFFFu);
				num = num3 >> 32;
				num2++;
			}
			if (num2 > bigInteger.dataLength)
			{
				bigInteger.dataLength = num2;
			}
			else
			{
				while (bigInteger.dataLength > 1 && bigInteger.data[bigInteger.dataLength - 1] == 0U)
				{
					bigInteger.dataLength--;
				}
			}
			int num4 = 69;
			if ((bi1.data[num4] & 2147483648U) == 0U && (bigInteger.data[num4] & 2147483648U) != (bi1.data[num4] & 2147483648U))
			{
				throw new ArithmeticException("Overflow in ++.");
			}
			return bigInteger;
		}
		
		// Token: 0x0600001F RID: 31 RVA: 0x00002CB8 File Offset: 0x00000EB8
		public static BigInteger operator -(BigInteger bi1, BigInteger bi2)
		{
			BigInteger bigInteger = new BigInteger();
			bigInteger.dataLength = ((bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength);
			long num = 0L;
			for (int i = 0; i < bigInteger.dataLength; i++)
			{
				long num2 = (long)((ulong)bi1.data[i] - (ulong)bi2.data[i] - (ulong)num);
				bigInteger.data[i] = (uint)(num2 & 0xFFFFFFFFu);
				if (num2 < 0L)
				{
					num = 1L;
				}
				else
				{
					num = 0L;
				}
			}
			if (num != 0L)
			{
				for (int i = bigInteger.dataLength; i < 70; i++)
				{
					bigInteger.data[i] = uint.MaxValue;
				}
				bigInteger.dataLength = 70;
			}
			while (bigInteger.dataLength > 1 && bigInteger.data[bigInteger.dataLength - 1] == 0U)
			{
				bigInteger.dataLength--;
			}
			int num3 = 69;
			if ((bi1.data[num3] & 2147483648U) != (bi2.data[num3] & 2147483648U) && (bigInteger.data[num3] & 2147483648U) != (bi1.data[num3] & 2147483648U))
			{
				throw new ArithmeticException();
			}
			return bigInteger;
		}
		
		// Token: 0x06000020 RID: 32 RVA: 0x00002E0C File Offset: 0x0000100C
		public static BigInteger operator --(BigInteger bi1)
		{
			BigInteger bigInteger = new BigInteger(bi1);
			bool flag = true;
			int num = 0;
			while (flag && num < 70)
			{
				long num2 = (long)((ulong)bigInteger.data[num]);
				num2 -= 1L;
				bigInteger.data[num] = (uint)(num2 & 0xFFFFFFFFu);
				if (num2 >= 0L)
				{
					flag = false;
				}
				num++;
			}
			if (num > bigInteger.dataLength)
			{
				bigInteger.dataLength = num;
			}
			while (bigInteger.dataLength > 1 && bigInteger.data[bigInteger.dataLength - 1] == 0U)
			{
				bigInteger.dataLength--;
			}
			int num3 = 69;
			if ((bi1.data[num3] & 2147483648U) != 0U && (bigInteger.data[num3] & 2147483648U) != (bi1.data[num3] & 2147483648U))
			{
				throw new ArithmeticException("Underflow in --.");
			}
			return bigInteger;
		}
		
		// Token: 0x06000021 RID: 33 RVA: 0x00002F08 File Offset: 0x00001108
		public static BigInteger operator *(BigInteger bi1, BigInteger bi2)
		{
			int num = 69;
			bool flag = false;
			bool flag2 = false;
			try
			{
				if ((bi1.data[num] & 2147483648U) != 0U)
				{
					flag = true;
					bi1 = -bi1;
				}
				if ((bi2.data[num] & 2147483648U) != 0U)
				{
					flag2 = true;
					bi2 = -bi2;
				}
			}
			catch (Exception)
			{
			}
			BigInteger bigInteger = new BigInteger();
			try
			{
				for (int i = 0; i < bi1.dataLength; i++)
				{
					if (bi1.data[i] != 0U)
					{
						ulong num2 = 0UL;
						int j = 0;
						int num3 = i;
						while (j < bi2.dataLength)
						{
							ulong num4 = (ulong)bi1.data[i] * (ulong)bi2.data[j] + (ulong)bigInteger.data[num3] + num2;
							bigInteger.data[num3] = (uint)(num4 &  (ulong)uint.MaxValue);
							num2 = num4 >> 32;
							j++;
							num3++;
						}
						if (num2 != 0UL)
						{
							bigInteger.data[i + bi2.dataLength] = (uint)num2;
						}
					}
				}
			}
			catch (Exception)
			{
				throw new ArithmeticException("Multiplication overflow.");
			}
			bigInteger.dataLength = bi1.dataLength + bi2.dataLength;
			if (bigInteger.dataLength > 70)
			{
				bigInteger.dataLength = 70;
			}
			while (bigInteger.dataLength > 1 && bigInteger.data[bigInteger.dataLength - 1] == 0U)
			{
				bigInteger.dataLength--;
			}
			if ((bigInteger.data[num] & 2147483648U) != 0U)
			{
				if (flag != flag2 && bigInteger.data[num] == 2147483648U)
				{
					if (bigInteger.dataLength == 1)
					{
						return bigInteger;
					}
					bool flag3 = true;
					int i = 0;
					while (i < bigInteger.dataLength - 1 && flag3)
					{
						if (bigInteger.data[i] != 0U)
						{
							flag3 = false;
						}
						i++;
					}
					if (flag3)
					{
						return bigInteger;
					}
				}
				throw new ArithmeticException("Multiplication overflow.");
			}
			BigInteger bigInteger2;
			if (flag != flag2)
			{
				bigInteger2 = -bigInteger;
			}
			else
			{
				bigInteger2 = bigInteger;
			}
			return bigInteger2;
		}
		
		// Token: 0x06000022 RID: 34 RVA: 0x00003190 File Offset: 0x00001390
		public static BigInteger operator <<(BigInteger bi1, int shiftVal)
		{
			BigInteger bigInteger = new BigInteger(bi1);
			bigInteger.dataLength = BigInteger.shiftLeft(bigInteger.data, shiftVal);
			return bigInteger;
		}
		
		// Token: 0x06000023 RID: 35 RVA: 0x000031BC File Offset: 0x000013BC
		private static int shiftLeft(uint[] buffer, int shiftVal)
		{
			int num = 32;
			int num2 = buffer.Length;
			while (num2 > 1 && buffer[num2 - 1] == 0U)
			{
				num2--;
			}
			for (int i = shiftVal; i > 0; i -= num)
			{
				if (i < num)
				{
					num = i;
				}
				ulong num3 = 0UL;
				for (int j = 0; j < num2; j++)
				{
					ulong num4 = (ulong)buffer[j] << num;
					num4 |= num3;
					buffer[j] = (uint)(num4 & (ulong)uint.MaxValue);
					num3 = num4 >> 32;
				}
				if (num3 != 0UL)
				{
					if (num2 + 1 <= buffer.Length)
					{
						buffer[num2] = (uint)num3;
						num2++;
					}
				}
			}
			return num2;
		}
		
		// Token: 0x06000024 RID: 36 RVA: 0x00003280 File Offset: 0x00001480
		public static BigInteger operator >>(BigInteger bi1, int shiftVal)
		{
			BigInteger bigInteger = new BigInteger(bi1);
			bigInteger.dataLength = BigInteger.shiftRight(bigInteger.data, shiftVal);
			if ((bi1.data[69] & 2147483648U) != 0U)
			{
				for (int i = 69; i >= bigInteger.dataLength; i--)
				{
					bigInteger.data[i] = uint.MaxValue;
				}
				uint num = 2147483648U;
				for (int i = 0; i < 32; i++)
				{
					if ((bigInteger.data[bigInteger.dataLength - 1] & num) != 0U)
					{
						break;
					}
					bigInteger.data[bigInteger.dataLength - 1] |= num;
					num >>= 1;
				}
				bigInteger.dataLength = 70;
			}
			return bigInteger;
		}
		
		// Token: 0x06000025 RID: 37 RVA: 0x00003350 File Offset: 0x00001550
		private static int shiftRight(uint[] buffer, int shiftVal)
		{
			int num = 32;
			int num2 = 0;
			int num3 = buffer.Length;
			while (num3 > 1 && buffer[num3 - 1] == 0U)
			{
				num3--;
			}
			for (int i = shiftVal; i > 0; i -= num)
			{
				if (i < num)
				{
					num = i;
					num2 = 32 - num;
				}
				ulong num4 = 0UL;
				for (int j = num3 - 1; j >= 0; j--)
				{
					ulong num5 = (ulong)buffer[j] >> num;
					num5 |= num4;
					num4 = (ulong)buffer[j] << num2;
					buffer[j] = (uint)num5;
				}
			}
			while (num3 > 1 && buffer[num3 - 1] == 0U)
			{
				num3--;
			}
			return num3;
		}
		
		// Token: 0x06000026 RID: 38 RVA: 0x00003418 File Offset: 0x00001618
		public static BigInteger operator ~(BigInteger bi1)
		{
			BigInteger bigInteger = new BigInteger(bi1);
			for (int i = 0; i < 70; i++)
			{
				bigInteger.data[i] = ~bi1.data[i];
			}
			bigInteger.dataLength = 70;
			while (bigInteger.dataLength > 1 && bigInteger.data[bigInteger.dataLength - 1] == 0U)
			{
				bigInteger.dataLength--;
			}
			return bigInteger;
		}
		
		// Token: 0x06000027 RID: 39 RVA: 0x00003490 File Offset: 0x00001690
		public static BigInteger operator -(BigInteger bi1)
		{
			BigInteger bigInteger;
			if (bi1.dataLength == 1 && bi1.data[0] == 0U)
			{
				bigInteger = new BigInteger();
			}
			else
			{
				BigInteger bigInteger2 = new BigInteger(bi1);
				for (int i = 0; i < 70; i++)
				{
					bigInteger2.data[i] = ~bi1.data[i];
				}
				long num = 1L;
				int num2 = 0;
				while (num != 0L && num2 < 70)
				{
					long num3 = (long)((ulong)bigInteger2.data[num2]);
					num3 += 1L;
					bigInteger2.data[num2] = (uint)(num3 & 0xFFFFFFFFu);
					num = num3 >> 32;
					num2++;
				}
				if ((bi1.data[69] & 2147483648U) == (bigInteger2.data[69] & 2147483648U))
				{
					throw new ArithmeticException("Overflow in negation.\n");
				}
				bigInteger2.dataLength = 70;
				while (bigInteger2.dataLength > 1 && bigInteger2.data[bigInteger2.dataLength - 1] == 0U)
				{
					bigInteger2.dataLength--;
				}
				bigInteger = bigInteger2;
			}
			return bigInteger;
		}
		
		// Token: 0x06000028 RID: 40 RVA: 0x000035B8 File Offset: 0x000017B8
		public static bool operator ==(BigInteger bi1, BigInteger bi2)
		{
			return bi1.Equals(bi2);
		}
		
		// Token: 0x06000029 RID: 41 RVA: 0x000035D4 File Offset: 0x000017D4
		public static bool operator !=(BigInteger bi1, BigInteger bi2)
		{
			return !bi1.Equals(bi2);
		}
		
		// Token: 0x0600002A RID: 42 RVA: 0x000035F0 File Offset: 0x000017F0
		public override bool Equals(object o)
		{
			BigInteger bigInteger = (BigInteger)o;
			bool flag;
			if (this.dataLength != bigInteger.dataLength)
			{
				flag = false;
			}
			else
			{
				for (int i = 0; i < this.dataLength; i++)
				{
					if (this.data[i] != bigInteger.data[i])
					{
						return false;
					}
				}
				flag = true;
			}
			return flag;
		}
		
		// Token: 0x0600002B RID: 43 RVA: 0x00003654 File Offset: 0x00001854
		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}
		
		// Token: 0x0600002C RID: 44 RVA: 0x00003674 File Offset: 0x00001874
		public static bool operator >(BigInteger bi1, BigInteger bi2)
		{
			int num = 69;
			bool flag;
			if ((bi1.data[num] & 2147483648U) != 0U && (bi2.data[num] & 2147483648U) == 0U)
			{
				flag = false;
			}
			else if ((bi1.data[num] & 2147483648U) == 0U && (bi2.data[num] & 2147483648U) != 0U)
			{
				flag = true;
			}
			else
			{
				int num2 = ((bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength);
				num = num2 - 1;
				while (num >= 0 && bi1.data[num] == bi2.data[num])
				{
					num--;
				}
				flag = num >= 0 && bi1.data[num] > bi2.data[num];
			}
			return flag;
		}
		
		// Token: 0x0600002D RID: 45 RVA: 0x0000375C File Offset: 0x0000195C
		public static bool operator <(BigInteger bi1, BigInteger bi2)
		{
			int num = 69;
			bool flag;
			if ((bi1.data[num] & 2147483648U) != 0U && (bi2.data[num] & 2147483648U) == 0U)
			{
				flag = true;
			}
			else if ((bi1.data[num] & 2147483648U) == 0U && (bi2.data[num] & 2147483648U) != 0U)
			{
				flag = false;
			}
			else
			{
				int num2 = ((bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength);
				num = num2 - 1;
				while (num >= 0 && bi1.data[num] == bi2.data[num])
				{
					num--;
				}
				flag = num >= 0 && bi1.data[num] < bi2.data[num];
			}
			return flag;
		}
		
		// Token: 0x0600002E RID: 46 RVA: 0x00003844 File Offset: 0x00001A44
		public static bool operator >=(BigInteger bi1, BigInteger bi2)
		{
			return bi1 == bi2 || bi1 > bi2;
		}
		
		// Token: 0x0600002F RID: 47 RVA: 0x0000386C File Offset: 0x00001A6C
		public static bool operator <=(BigInteger bi1, BigInteger bi2)
		{
			return bi1 == bi2 || bi1 < bi2;
		}
		
		// Token: 0x06000030 RID: 48 RVA: 0x00003894 File Offset: 0x00001A94
		private static void multiByteDivide(BigInteger bi1, BigInteger bi2, BigInteger outQuotient, BigInteger outRemainder)
		{
			uint[] array = new uint[70];
			int num = bi1.dataLength + 1;
			uint[] array2 = new uint[num];
			uint num2 = 2147483648U;
			uint num3 = bi2.data[bi2.dataLength - 1];
			int num4 = 0;
			int num5 = 0;
			while (num2 != 0U && (num3 & num2) == 0U)
			{
				num4++;
				num2 >>= 1;
			}
			for (int i = 0; i < bi1.dataLength; i++)
			{
				array2[i] = bi1.data[i];
			}
			BigInteger.shiftLeft(array2, num4);
			bi2 <<= num4;
			int j = num - bi2.dataLength;
			int num6 = num - 1;
			ulong num7 = (ulong)bi2.data[bi2.dataLength - 1];
			ulong num8 = (ulong)bi2.data[bi2.dataLength - 2];
			int num9 = bi2.dataLength + 1;
			uint[] array3 = new uint[num9];
			while (j > 0)
			{
				ulong num10 = ((ulong)array2[num6] << 32) + (ulong)array2[num6 - 1];
				ulong num11 = num10 / num7;
				ulong num12 = num10 % num7;
				bool flag = false;
				while (!flag)
				{
					flag = true;
					if (num11 == 4294967296UL || num11 * num8 > (num12 << 32) + (ulong)array2[num6 - 2])
					{
						num11 -= 1UL;
						num12 += num7;
						if (num12 < 4294967296UL)
						{
							flag = false;
						}
					}
				}
				for (int k = 0; k < num9; k++)
				{
					array3[k] = array2[num6 - k];
				}
				BigInteger bigInteger = new BigInteger(array3);
				BigInteger bigInteger2 = bi2 * (long)num11;
				while (bigInteger2 > bigInteger)
				{
					num11 -= 1UL;
					bigInteger2 -= bi2;
				}
				BigInteger bigInteger3 = bigInteger - bigInteger2;
				for (int k = 0; k < num9; k++)
				{
					array2[num6 - k] = bigInteger3.data[bi2.dataLength - k];
				}
				array[num5++] = (uint)num11;
				num6--;
				j--;
			}
			outQuotient.dataLength = num5;
			int l = 0;
			int m = outQuotient.dataLength - 1;
			while (m >= 0)
			{
				outQuotient.data[l] = array[m];
				m--;
				l++;
			}
			while (l < 70)
			{
				outQuotient.data[l] = 0U;
				l++;
			}
			while (outQuotient.dataLength > 1 && outQuotient.data[outQuotient.dataLength - 1] == 0U)
			{
				outQuotient.dataLength--;
			}
			if (outQuotient.dataLength == 0)
			{
				outQuotient.dataLength = 1;
			}
			outRemainder.dataLength = BigInteger.shiftRight(array2, num4);
			for (l = 0; l < outRemainder.dataLength; l++)
			{
				outRemainder.data[l] = array2[l];
			}
			while (l < 70)
			{
				outRemainder.data[l] = 0U;
				l++;
			}
		}
		
		// Token: 0x06000031 RID: 49 RVA: 0x00003BCC File Offset: 0x00001DCC
		private static void singleByteDivide(BigInteger bi1, BigInteger bi2, BigInteger outQuotient, BigInteger outRemainder)
		{
			uint[] array = new uint[70];
			int num = 0;
			int i;
			for (i = 0; i < 70; i++)
			{
				outRemainder.data[i] = bi1.data[i];
			}
			outRemainder.dataLength = bi1.dataLength;
			while (outRemainder.dataLength > 1 && outRemainder.data[outRemainder.dataLength - 1] == 0U)
			{
				outRemainder.dataLength--;
			}
			ulong num2 = (ulong)bi2.data[0];
			int j = outRemainder.dataLength - 1;
			ulong num3 = (ulong)outRemainder.data[j];
			if (num3 >= num2)
			{
				ulong num4 = num3 / num2;
				array[num++] = (uint)num4;
				outRemainder.data[j] = (uint)(num3 % num2);
			}
			j--;
			while (j >= 0)
			{
				num3 = ((ulong)outRemainder.data[j + 1] << 32) + (ulong)outRemainder.data[j];
				ulong num4 = num3 / num2;
				array[num++] = (uint)num4;
				outRemainder.data[j + 1] = 0U;
				outRemainder.data[j--] = (uint)(num3 % num2);
			}
			outQuotient.dataLength = num;
			int k = 0;
			i = outQuotient.dataLength - 1;
			while (i >= 0)
			{
				outQuotient.data[k] = array[i];
				i--;
				k++;
			}
			while (k < 70)
			{
				outQuotient.data[k] = 0U;
				k++;
			}
			while (outQuotient.dataLength > 1 && outQuotient.data[outQuotient.dataLength - 1] == 0U)
			{
				outQuotient.dataLength--;
			}
			if (outQuotient.dataLength == 0)
			{
				outQuotient.dataLength = 1;
			}
			while (outRemainder.dataLength > 1 && outRemainder.data[outRemainder.dataLength - 1] == 0U)
			{
				outRemainder.dataLength--;
			}
		}
		
		// Token: 0x06000032 RID: 50 RVA: 0x00003DD0 File Offset: 0x00001FD0
		public static BigInteger operator /(BigInteger bi1, BigInteger bi2)
		{
			BigInteger bigInteger = new BigInteger();
			BigInteger bigInteger2 = new BigInteger();
			int num = 69;
			bool flag = false;
			bool flag2 = false;
			if ((bi1.data[num] & 2147483648U) != 0U)
			{
				bi1 = -bi1;
				flag2 = true;
			}
			if ((bi2.data[num] & 2147483648U) != 0U)
			{
				bi2 = -bi2;
				flag = true;
			}
			BigInteger bigInteger3;
			if (bi1 < bi2)
			{
				bigInteger3 = bigInteger;
			}
			else
			{
				if (bi2.dataLength == 1)
				{
					BigInteger.singleByteDivide(bi1, bi2, bigInteger, bigInteger2);
				}
				else
				{
					BigInteger.multiByteDivide(bi1, bi2, bigInteger, bigInteger2);
				}
				if (flag2 != flag)
				{
					bigInteger3 = -bigInteger;
				}
				else
				{
					bigInteger3 = bigInteger;
				}
			}
			return bigInteger3;
		}
		
		// Token: 0x06000033 RID: 51 RVA: 0x00003E94 File Offset: 0x00002094
		public static BigInteger operator %(BigInteger bi1, BigInteger bi2)
		{
			BigInteger bigInteger = new BigInteger();
			BigInteger bigInteger2 = new BigInteger(bi1);
			int num = 69;
			bool flag = false;
			if ((bi1.data[num] & 2147483648U) != 0U)
			{
				bi1 = -bi1;
				flag = true;
			}
			if ((bi2.data[num] & 2147483648U) != 0U)
			{
				bi2 = -bi2;
			}
			BigInteger bigInteger3;
			if (bi1 < bi2)
			{
				bigInteger3 = bigInteger2;
			}
			else
			{
				if (bi2.dataLength == 1)
				{
					BigInteger.singleByteDivide(bi1, bi2, bigInteger, bigInteger2);
				}
				else
				{
					BigInteger.multiByteDivide(bi1, bi2, bigInteger, bigInteger2);
				}
				if (flag)
				{
					bigInteger3 = -bigInteger2;
				}
				else
				{
					bigInteger3 = bigInteger2;
				}
			}
			return bigInteger3;
		}
		
		// Token: 0x06000034 RID: 52 RVA: 0x00003F50 File Offset: 0x00002150
		public static BigInteger operator &(BigInteger bi1, BigInteger bi2)
		{
			BigInteger bigInteger = new BigInteger();
			int num = ((bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength);
			for (int i = 0; i < num; i++)
			{
				uint num2 = bi1.data[i] & bi2.data[i];
				bigInteger.data[i] = num2;
			}
			bigInteger.dataLength = 70;
			while (bigInteger.dataLength > 1 && bigInteger.data[bigInteger.dataLength - 1] == 0U)
			{
				bigInteger.dataLength--;
			}
			return bigInteger;
		}
		
		// Token: 0x06000035 RID: 53 RVA: 0x00003FF8 File Offset: 0x000021F8
		public static BigInteger operator |(BigInteger bi1, BigInteger bi2)
		{
			BigInteger bigInteger = new BigInteger();
			int num = ((bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength);
			for (int i = 0; i < num; i++)
			{
				uint num2 = bi1.data[i] | bi2.data[i];
				bigInteger.data[i] = num2;
			}
			bigInteger.dataLength = 70;
			while (bigInteger.dataLength > 1 && bigInteger.data[bigInteger.dataLength - 1] == 0U)
			{
				bigInteger.dataLength--;
			}
			return bigInteger;
		}
		
		// Token: 0x06000036 RID: 54 RVA: 0x000040A0 File Offset: 0x000022A0
		public static BigInteger operator ^(BigInteger bi1, BigInteger bi2)
		{
			BigInteger bigInteger = new BigInteger();
			int num = ((bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength);
			for (int i = 0; i < num; i++)
			{
				uint num2 = bi1.data[i] ^ bi2.data[i];
				bigInteger.data[i] = num2;
			}
			bigInteger.dataLength = 70;
			while (bigInteger.dataLength > 1 && bigInteger.data[bigInteger.dataLength - 1] == 0U)
			{
				bigInteger.dataLength--;
			}
			return bigInteger;
		}
		
		// Token: 0x06000037 RID: 55 RVA: 0x00004148 File Offset: 0x00002348
		public BigInteger max(BigInteger bi)
		{
			BigInteger bigInteger;
			if (this > bi)
			{
				bigInteger = new BigInteger(this);
			}
			else
			{
				bigInteger = new BigInteger(bi);
			}
			return bigInteger;
		}
		
		// Token: 0x06000038 RID: 56 RVA: 0x00004178 File Offset: 0x00002378
		public BigInteger min(BigInteger bi)
		{
			BigInteger bigInteger;
			if (this < bi)
			{
				bigInteger = new BigInteger(this);
			}
			else
			{
				bigInteger = new BigInteger(bi);
			}
			return bigInteger;
		}
		
		// Token: 0x06000039 RID: 57 RVA: 0x000041A8 File Offset: 0x000023A8
		public BigInteger abs()
		{
			BigInteger bigInteger;
			if ((this.data[69] & 2147483648U) != 0U)
			{
				bigInteger = -this;
			}
			else
			{
				bigInteger = new BigInteger(this);
			}
			return bigInteger;
		}
		
		// Token: 0x0600003A RID: 58 RVA: 0x000041E0 File Offset: 0x000023E0
		public override string ToString()
		{
			return this.ToString(10);
		}
		
		// Token: 0x0600003B RID: 59 RVA: 0x000041FC File Offset: 0x000023FC
		public string ToString(int radix)
		{
			if (radix < 2 || radix > 36)
			{
				throw new ArgumentException("Radix must be >= 2 and <= 36");
			}
			string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			string text2 = "";
			BigInteger bigInteger = this;
			bool flag = false;
			if ((bigInteger.data[69] & 2147483648U) != 0U)
			{
				flag = true;
				try
				{
					bigInteger = -bigInteger;
				}
				catch (Exception)
				{
				}
			}
			BigInteger bigInteger2 = new BigInteger();
			BigInteger bigInteger3 = new BigInteger();
			BigInteger bigInteger4 = new BigInteger((long)radix);
			if (bigInteger.dataLength == 1 && bigInteger.data[0] == 0U)
			{
				text2 = "0";
			}
			else
			{
				while (bigInteger.dataLength > 1 || (bigInteger.dataLength == 1 && bigInteger.data[0] != 0U))
				{
					BigInteger.singleByteDivide(bigInteger, bigInteger4, bigInteger2, bigInteger3);
					if (bigInteger3.data[0] < 10U)
					{
						text2 = bigInteger3.data[0] + text2;
					}
					else
					{
						text2 = text[(int)(bigInteger3.data[0] - 10U)] + text2;
					}
					bigInteger = bigInteger2;
				}
				if (flag)
				{
					text2 = "-" + text2;
				}
			}
			return text2;
		}
		
		// Token: 0x0600003C RID: 60 RVA: 0x0000436C File Offset: 0x0000256C
		public string ToHexString()
		{
			string text = this.data[this.dataLength - 1].ToString("X");
			for (int i = this.dataLength - 2; i >= 0; i--)
			{
				text += this.data[i].ToString("X8");
			}
			return text;
		}
		
		// Token: 0x0600003D RID: 61 RVA: 0x000043D8 File Offset: 0x000025D8
		public BigInteger ModPow(BigInteger exp, BigInteger n)
		{
			if ((exp.data[69] & 2147483648U) != 0U)
			{
				throw new ArithmeticException("Positive exponents only.");
			}
			BigInteger bigInteger = 1;
			bool flag = false;
			BigInteger bigInteger2;
			if ((this.data[69] & 2147483648U) != 0U)
			{
				bigInteger2 = -this % n;
				flag = true;
			}
			else
			{
				bigInteger2 = this % n;
			}
			if ((n.data[69] & 2147483648U) != 0U)
			{
				n = -n;
			}
			BigInteger bigInteger3 = new BigInteger();
			int num = n.dataLength << 1;
			bigInteger3.data[num] = 1U;
			bigInteger3.dataLength = num + 1;
			bigInteger3 /= n;
			int num2 = exp.bitCount();
			int num3 = 0;
			for (int i = 0; i < exp.dataLength; i++)
			{
				uint num4 = 1U;
				int j = 0;
				while (j < 32)
				{
					if ((exp.data[i] & num4) != 0U)
					{
						bigInteger = this.BarrettReduction(bigInteger * bigInteger2, n, bigInteger3);
					}
					num4 <<= 1;
					bigInteger2 = this.BarrettReduction(bigInteger2 * bigInteger2, n, bigInteger3);
					if (bigInteger2.dataLength == 1 && bigInteger2.data[0] == 1U)
					{
						if (flag && (exp.data[0] & 1U) != 0U)
						{
							return -bigInteger;
						}
						return bigInteger;
					}
					else
					{
						num3++;
						if (num3 == num2)
						{
							break;
						}
						j++;
					}
				}
			}
			if (flag && (exp.data[0] & 1U) != 0U)
			{
				return -bigInteger;
			}
			return bigInteger;
		}
		
		// Token: 0x0600003E RID: 62 RVA: 0x000045AC File Offset: 0x000027AC
		private BigInteger BarrettReduction(BigInteger x, BigInteger n, BigInteger constant)
		{
			int num = n.dataLength;
			int num2 = num + 1;
			int num3 = num - 1;
			BigInteger bigInteger = new BigInteger();
			int i = num3;
			int num4 = 0;
			while (i < x.dataLength)
			{
				bigInteger.data[num4] = x.data[i];
				i++;
				num4++;
			}
			bigInteger.dataLength = x.dataLength - num3;
			if (bigInteger.dataLength <= 0)
			{
				bigInteger.dataLength = 1;
			}
			BigInteger bigInteger2 = bigInteger * constant;
			BigInteger bigInteger3 = new BigInteger();
			i = num2;
			num4 = 0;
			while (i < bigInteger2.dataLength)
			{
				bigInteger3.data[num4] = bigInteger2.data[i];
				i++;
				num4++;
			}
			bigInteger3.dataLength = bigInteger2.dataLength - num2;
			if (bigInteger3.dataLength <= 0)
			{
				bigInteger3.dataLength = 1;
			}
			BigInteger bigInteger4 = new BigInteger();
			int num5 = ((x.dataLength > num2) ? num2 : x.dataLength);
			for (i = 0; i < num5; i++)
			{
				bigInteger4.data[i] = x.data[i];
			}
			bigInteger4.dataLength = num5;
			BigInteger bigInteger5 = new BigInteger();
			for (i = 0; i < bigInteger3.dataLength; i++)
			{
				if (bigInteger3.data[i] != 0U)
				{
					ulong num6 = 0UL;
					int num7 = i;
					num4 = 0;
					while (num4 < n.dataLength && num7 < num2)
					{
						ulong num8 = (ulong)bigInteger3.data[i] * (ulong)n.data[num4] + (ulong)bigInteger5.data[num7] + num6;
						bigInteger5.data[num7] = (uint)(num8 &  (ulong)uint.MaxValue);
						num6 = num8 >> 32;
						num4++;
						num7++;
					}
					if (num7 < num2)
					{
						bigInteger5.data[num7] = (uint)num6;
					}
				}
			}
			bigInteger5.dataLength = num2;
			while (bigInteger5.dataLength > 1 && bigInteger5.data[bigInteger5.dataLength - 1] == 0U)
			{
				bigInteger5.dataLength--;
			}
			bigInteger4 -= bigInteger5;
			if ((bigInteger4.data[69] & 2147483648U) != 0U)
			{
				BigInteger bigInteger6 = new BigInteger();
				bigInteger6.data[num2] = 1U;
				bigInteger6.dataLength = num2 + 1;
				bigInteger4 += bigInteger6;
			}
			while (bigInteger4 >= n)
			{
				bigInteger4 -= n;
			}
			return bigInteger4;
		}
		
		// Token: 0x0600003F RID: 63 RVA: 0x0000486C File Offset: 0x00002A6C
		public BigInteger gcd(BigInteger bi)
		{
			BigInteger bigInteger;
			if ((this.data[69] & 2147483648U) != 0U)
			{
				bigInteger = -this;
			}
			else
			{
				bigInteger = this;
			}
			BigInteger bigInteger2;
			if ((bi.data[69] & 2147483648U) != 0U)
			{
				bigInteger2 = -bi;
			}
			else
			{
				bigInteger2 = bi;
			}
			BigInteger bigInteger3 = bigInteger2;
			while (bigInteger.dataLength > 1 || (bigInteger.dataLength == 1 && bigInteger.data[0] != 0U))
			{
				bigInteger3 = bigInteger;
				bigInteger = bigInteger2 % bigInteger;
				bigInteger2 = bigInteger3;
			}
			return bigInteger3;
		}
		
		// Token: 0x06000040 RID: 64 RVA: 0x00004908 File Offset: 0x00002B08
		public static BigInteger GenerateRandom(int bits)
		{
			BigInteger bigInteger = new BigInteger();
			bigInteger.genRandomBits(bits, new Random());
			return bigInteger;
		}
		
		// Token: 0x06000041 RID: 65 RVA: 0x00004930 File Offset: 0x00002B30
		public void genRandomBits(int bits, Random rand)
		{
			int num = bits >> 5;
			int num2 = bits & 31;
			if (num2 != 0)
			{
				num++;
			}
			if (num > 70)
			{
				throw new ArithmeticException("Number of required bits > maxLength.");
			}
			for (int i = 0; i < num; i++)
			{
				this.data[i] = (uint)(rand.NextDouble() * 4294967296.0);
			}
			for (int i = num; i < 70; i++)
			{
				this.data[i] = 0U;
			}
			if (num2 != 0)
			{
				uint num3 = 1U << num2 - 1;
				this.data[num - 1] |= num3;
				num3 = uint.MaxValue >> 32 - num2;
				this.data[num - 1] &= num3;
			}
			else
			{
				this.data[num - 1] |= 2147483648U;
			}
			this.dataLength = num;
			if (this.dataLength == 0)
			{
				this.dataLength = 1;
			}
		}
		
		// Token: 0x06000042 RID: 66 RVA: 0x00004A4C File Offset: 0x00002C4C
		public int bitCount()
		{
			while (this.dataLength > 1 && this.data[this.dataLength - 1] == 0U)
			{
				this.dataLength--;
			}
			uint num = this.data[this.dataLength - 1];
			uint num2 = 2147483648U;
			int num3 = 32;
			while (num3 > 0 && (num & num2) == 0U)
			{
				num3--;
				num2 >>= 1;
			}
			return num3 + (this.dataLength - 1 << 5);
		}
		
		// Token: 0x06000043 RID: 67 RVA: 0x00004ADC File Offset: 0x00002CDC
		public bool FermatLittleTest(int confidence)
		{
			BigInteger bigInteger;
			if ((this.data[69] & 2147483648U) != 0U)
			{
				bigInteger = -this;
			}
			else
			{
				bigInteger = this;
			}
			if (bigInteger.dataLength == 1)
			{
				if (bigInteger.data[0] == 0U || bigInteger.data[0] == 1U)
				{
					return false;
				}
				if (bigInteger.data[0] == 2U || bigInteger.data[0] == 3U)
				{
					return true;
				}
			}
			bool flag;
			if ((bigInteger.data[0] & 1U) == 0U)
			{
				flag = false;
			}
			else
			{
				int num = bigInteger.bitCount();
				BigInteger bigInteger2 = new BigInteger();
				BigInteger bigInteger3 = bigInteger - new BigInteger(1L);
				Random random = new Random();
				for (int i = 0; i < confidence; i++)
				{
					bool flag2 = false;
					while (!flag2)
					{
						int j;
						for (j = 0; j < 2; j = (int)(random.NextDouble() * (double)num))
						{
						}
						bigInteger2.genRandomBits(j, random);
						int num2 = bigInteger2.dataLength;
						if (num2 > 1 || (num2 == 1 && bigInteger2.data[0] != 1U))
						{
							flag2 = true;
						}
					}
					BigInteger bigInteger4 = bigInteger2.gcd(bigInteger);
					if (bigInteger4.dataLength == 1 && bigInteger4.data[0] != 1U)
					{
						return false;
					}
					BigInteger bigInteger5 = bigInteger2.ModPow(bigInteger3, bigInteger);
					int num3 = bigInteger5.dataLength;
					if (num3 > 1 || (num3 == 1 && bigInteger5.data[0] != 1U))
					{
						return false;
					}
				}
				flag = true;
			}
			return flag;
		}
		
		// Token: 0x06000044 RID: 68 RVA: 0x00004CB0 File Offset: 0x00002EB0
		public bool RabinMillerTest(int confidence)
		{
			BigInteger bigInteger;
			if ((this.data[69] & 2147483648U) != 0U)
			{
				bigInteger = -this;
			}
			else
			{
				bigInteger = this;
			}
			if (bigInteger.dataLength == 1)
			{
				if (bigInteger.data[0] == 0U || bigInteger.data[0] == 1U)
				{
					return false;
				}
				if (bigInteger.data[0] == 2U || bigInteger.data[0] == 3U)
				{
					return true;
				}
			}
			bool flag;
			if ((bigInteger.data[0] & 1U) == 0U)
			{
				flag = false;
			}
			else
			{
				BigInteger bigInteger2 = bigInteger - new BigInteger(1L);
				int num = 0;
				for (int i = 0; i < bigInteger2.dataLength; i++)
				{
					uint num2 = 1U;
					for (int j = 0; j < 32; j++)
					{
						if ((bigInteger2.data[i] & num2) != 0U)
						{
							i = bigInteger2.dataLength;
							break;
						}
						num2 <<= 1;
						num++;
					}
				}
				BigInteger bigInteger3 = bigInteger2 >> num;
				int num3 = bigInteger.bitCount();
				BigInteger bigInteger4 = new BigInteger();
				Random random = new Random();
				for (int k = 0; k < confidence; k++)
				{
					bool flag2 = false;
					while (!flag2)
					{
						int l;
						for (l = 0; l < 2; l = (int)(random.NextDouble() * (double)num3))
						{
						}
						bigInteger4.genRandomBits(l, random);
						int num4 = bigInteger4.dataLength;
						if (num4 > 1 || (num4 == 1 && bigInteger4.data[0] != 1U))
						{
							flag2 = true;
						}
					}
					BigInteger bigInteger5 = bigInteger4.gcd(bigInteger);
					if (bigInteger5.dataLength == 1 && bigInteger5.data[0] != 1U)
					{
						return false;
					}
					BigInteger bigInteger6 = bigInteger4.ModPow(bigInteger3, bigInteger);
					bool flag3 = false;
					if (bigInteger6.dataLength == 1 && bigInteger6.data[0] == 1U)
					{
						flag3 = true;
					}
					int num5 = 0;
					while (!flag3 && num5 < num)
					{
						if (bigInteger6 == bigInteger2)
						{
							flag3 = true;
							break;
						}
						bigInteger6 = bigInteger6 * bigInteger6 % bigInteger;
						num5++;
					}
					if (!flag3)
					{
						return false;
					}
				}
				flag = true;
			}
			return flag;
		}
		
		// Token: 0x06000045 RID: 69 RVA: 0x00004F44 File Offset: 0x00003144
		public bool SolovayStrassenTest(int confidence)
		{
			BigInteger bigInteger;
			if ((this.data[69] & 2147483648U) != 0U)
			{
				bigInteger = -this;
			}
			else
			{
				bigInteger = this;
			}
			if (bigInteger.dataLength == 1)
			{
				if (bigInteger.data[0] == 0U || bigInteger.data[0] == 1U)
				{
					return false;
				}
				if (bigInteger.data[0] == 2U || bigInteger.data[0] == 3U)
				{
					return true;
				}
			}
			bool flag;
			if ((bigInteger.data[0] & 1U) == 0U)
			{
				flag = false;
			}
			else
			{
				int num = bigInteger.bitCount();
				BigInteger bigInteger2 = new BigInteger();
				BigInteger bigInteger3 = bigInteger - 1;
				BigInteger bigInteger4 = bigInteger3 >> 1;
				Random random = new Random();
				for (int i = 0; i < confidence; i++)
				{
					bool flag2 = false;
					while (!flag2)
					{
						int j;
						for (j = 0; j < 2; j = (int)(random.NextDouble() * (double)num))
						{
						}
						bigInteger2.genRandomBits(j, random);
						int num2 = bigInteger2.dataLength;
						if (num2 > 1 || (num2 == 1 && bigInteger2.data[0] != 1U))
						{
							flag2 = true;
						}
					}
					BigInteger bigInteger5 = bigInteger2.gcd(bigInteger);
					if (bigInteger5.dataLength == 1 && bigInteger5.data[0] != 1U)
					{
						return false;
					}
					BigInteger bigInteger6 = bigInteger2.ModPow(bigInteger4, bigInteger);
					if (bigInteger6 == bigInteger3)
					{
						bigInteger6 = -1;
					}
					BigInteger bigInteger7 = BigInteger.Jacobi(bigInteger2, bigInteger);
					if (bigInteger6 != bigInteger7)
					{
						return false;
					}
				}
				flag = true;
			}
			return flag;
		}
		
		// Token: 0x06000046 RID: 70 RVA: 0x0000512C File Offset: 0x0000332C
		public bool LucasStrongTest()
		{
			BigInteger bigInteger;
			if ((this.data[69] & 2147483648U) != 0U)
			{
				bigInteger = -this;
			}
			else
			{
				bigInteger = this;
			}
			if (bigInteger.dataLength == 1)
			{
				if (bigInteger.data[0] == 0U || bigInteger.data[0] == 1U)
				{
					return false;
				}
				if (bigInteger.data[0] == 2U || bigInteger.data[0] == 3U)
				{
					return true;
				}
			}
			return (bigInteger.data[0] & 1U) != 0U && this.LucasStrongTestHelper(bigInteger);
		}
		
		// Token: 0x06000047 RID: 71 RVA: 0x000051DC File Offset: 0x000033DC
		private bool LucasStrongTestHelper(BigInteger thisVal)
		{
			long num = 5L;
			long num2 = -1L;
			long num3 = 0L;
			bool flag = false;
			while (!flag)
			{
				int num4 = BigInteger.Jacobi(num, thisVal);
				if (num4 != -1)
				{
					if (num4 != 0 || !(Math.Abs(num) < thisVal))
					{
						if (num3 == 20L)
						{
							BigInteger bigInteger = thisVal.sqrt();
							if (bigInteger * bigInteger == thisVal)
							{
								return false;
							}
						}
						num = (Math.Abs(num) + 2L) * num2;
						num2 = -num2;
						goto IL_A6;
					}
					return false;
				}
				flag = true;
			IL_A6:
					num3 += 1L;
			}
			long num5 = 1L - num >> 2;
			BigInteger bigInteger2 = thisVal + 1;
			int num6 = 0;
			for (int i = 0; i < bigInteger2.dataLength; i++)
			{
				uint num7 = 1U;
				for (int j = 0; j < 32; j++)
				{
					if ((bigInteger2.data[i] & num7) != 0U)
					{
						i = bigInteger2.dataLength;
						break;
					}
					num7 <<= 1;
					num6++;
				}
			}
			BigInteger bigInteger3 = bigInteger2 >> num6;
			BigInteger bigInteger4 = new BigInteger();
			int num8 = thisVal.dataLength << 1;
			bigInteger4.data[num8] = 1U;
			bigInteger4.dataLength = num8 + 1;
			bigInteger4 /= thisVal;
			BigInteger[] array = BigInteger.LucasSequenceHelper(1, num5, bigInteger3, thisVal, bigInteger4, 0);
			bool flag2 = false;
			if ((array[0].dataLength == 1 && array[0].data[0] == 0U) || (array[1].dataLength == 1 && array[1].data[0] == 0U))
			{
				flag2 = true;
			}
			for (int j = 1; j < num6; j++)
			{
				if (!flag2)
				{
					array[1] = thisVal.BarrettReduction(array[1] * array[1], thisVal, bigInteger4);
					array[1] = (array[1] - (array[2] << 1)) % thisVal;
					if (array[1].dataLength == 1 && array[1].data[0] == 0U)
					{
						flag2 = true;
					}
				}
				array[2] = thisVal.BarrettReduction(array[2] * array[2], thisVal, bigInteger4);
			}
			if (flag2)
			{
				BigInteger bigInteger5 = thisVal.gcd(num5);
				if (bigInteger5.dataLength == 1 && bigInteger5.data[0] == 1U)
				{
					if ((array[2].data[69] & 2147483648U) != 0U)
					{
						// TODO: I think this is just a test but this line of code needs to be added back.
						// BigInteger[] array2;
						// (array2 = array)[2] = array2[2] + thisVal;
					}
					BigInteger bigInteger6 = num5 * (long)BigInteger.Jacobi(num5, thisVal) % thisVal;
					if ((bigInteger6.data[69] & 2147483648U) != 0U)
					{
						bigInteger6 += thisVal;
					}
					if (array[2] != bigInteger6)
					{
						flag2 = false;
					}
				}
			}
			return flag2;
		}
		
		// Token: 0x06000048 RID: 72 RVA: 0x0000553C File Offset: 0x0000373C
		public bool isProbablePrime(int confidence)
		{
			BigInteger bigInteger;
			if ((this.data[69] & 2147483648U) != 0U)
			{
				bigInteger = -this;
			}
			else
			{
				bigInteger = this;
			}
			for (int i = 0; i < BigInteger.primesBelow2000.Length; i++)
			{
				BigInteger bigInteger2 = BigInteger.primesBelow2000[i];
				if (bigInteger2 >= bigInteger)
				{
					break;
				}
				BigInteger bigInteger3 = bigInteger % bigInteger2;
				if (bigInteger3.IntValue() == 0)
				{
					return false;
				}
			}
			return bigInteger.RabinMillerTest(confidence);
		}
		
		// Token: 0x06000049 RID: 73 RVA: 0x000055E4 File Offset: 0x000037E4
		public bool isProbablePrime()
		{
			BigInteger bigInteger;
			if ((this.data[69] & 2147483648U) != 0U)
			{
				bigInteger = -this;
			}
			else
			{
				bigInteger = this;
			}
			if (bigInteger.dataLength == 1)
			{
				if (bigInteger.data[0] == 0U || bigInteger.data[0] == 1U)
				{
					return false;
				}
				if (bigInteger.data[0] == 2U || bigInteger.data[0] == 3U)
				{
					return true;
				}
			}
			bool flag;
			if ((bigInteger.data[0] & 1U) == 0U)
			{
				flag = false;
			}
			else
			{
				for (int i = 0; i < BigInteger.primesBelow2000.Length; i++)
				{
					BigInteger bigInteger2 = BigInteger.primesBelow2000[i];
					if (bigInteger2 >= bigInteger)
					{
						break;
					}
					BigInteger bigInteger3 = bigInteger % bigInteger2;
					if (bigInteger3.IntValue() == 0)
					{
						return false;
					}
				}
				BigInteger bigInteger4 = bigInteger - new BigInteger(1L);
				int num = 0;
				for (int j = 0; j < bigInteger4.dataLength; j++)
				{
					uint num2 = 1U;
					for (int k = 0; k < 32; k++)
					{
						if ((bigInteger4.data[j] & num2) != 0U)
						{
							j = bigInteger4.dataLength;
							break;
						}
						num2 <<= 1;
						num++;
					}
				}
				BigInteger bigInteger5 = bigInteger4 >> num;
				int num3 = bigInteger.bitCount();
				BigInteger bigInteger6 = 2;
				BigInteger bigInteger7 = bigInteger6.ModPow(bigInteger5, bigInteger);
				bool flag2 = false;
				if (bigInteger7.dataLength == 1 && bigInteger7.data[0] == 1U)
				{
					flag2 = true;
				}
				int num4 = 0;
				while (!flag2 && num4 < num)
				{
					if (bigInteger7 == bigInteger4)
					{
						flag2 = true;
						break;
					}
					bigInteger7 = bigInteger7 * bigInteger7 % bigInteger;
					num4++;
				}
				if (flag2)
				{
					flag2 = this.LucasStrongTestHelper(bigInteger);
				}
				flag = flag2;
			}
			return flag;
		}
		
		// Token: 0x0600004A RID: 74 RVA: 0x00005828 File Offset: 0x00003A28
		public int IntValue()
		{
			return (int)this.data[0];
		}
		
		// Token: 0x0600004B RID: 75 RVA: 0x00005844 File Offset: 0x00003A44
		public long LongValue()
		{
			long num = 0L;
			num = (long)((ulong)this.data[0]);
			try
			{
				num |= (long)((long)((ulong)this.data[1]) << 32);
			}
			catch (Exception)
			{
				if ((this.data[0] & 2147483648U) != 0U)
				{
					num = (long)this.data[0];
				}
			}
			return num;
		}
		
		// Token: 0x0600004C RID: 76 RVA: 0x000058AC File Offset: 0x00003AAC
		public static int Jacobi(BigInteger a, BigInteger b)
		{
			if ((b.data[0] & 1U) == 0U)
			{
				throw new ArgumentException("Jacobi defined only for odd integers.");
			}
			if (a >= b)
			{
				a %= b;
			}
			int num;
			if (a.dataLength == 1 && a.data[0] == 0U)
			{
				num = 0;
			}
			else if (a.dataLength == 1 && a.data[0] == 1U)
			{
				num = 1;
			}
			else if (a < 0)
			{
				if (((b - 1).data[0] & 2U) == 0U)
				{
					num = BigInteger.Jacobi(-a, b);
				}
				else
				{
					num = -BigInteger.Jacobi(-a, b);
				}
			}
			else
			{
				int num2 = 0;
				for (int i = 0; i < a.dataLength; i++)
				{
					uint num3 = 1U;
					for (int j = 0; j < 32; j++)
					{
						if ((a.data[i] & num3) != 0U)
						{
							i = a.dataLength;
							break;
						}
						num3 <<= 1;
						num2++;
					}
				}
				BigInteger bigInteger = a >> num2;
				int num4 = 1;
				if ((num2 & 1) != 0 && ((b.data[0] & 7U) == 3U || (b.data[0] & 7U) == 5U))
				{
					num4 = -1;
				}
				if ((b.data[0] & 3U) == 3U && (bigInteger.data[0] & 3U) == 3U)
				{
					num4 = -num4;
				}
				if (bigInteger.dataLength == 1 && bigInteger.data[0] == 1U)
				{
					num = num4;
				}
				else
				{
					num = num4 * BigInteger.Jacobi(b % bigInteger, bigInteger);
				}
			}
			return num;
		}
		
		// Token: 0x0600004D RID: 77 RVA: 0x00005AA8 File Offset: 0x00003CA8
		public static BigInteger genPseudoPrime(int bits, int confidence, Random rand)
		{
			BigInteger bigInteger = new BigInteger();
			bool flag = false;
			while (!flag)
			{
				bigInteger.genRandomBits(bits, rand);
				bigInteger.data[0] |= 1U;
				flag = bigInteger.isProbablePrime(confidence);
			}
			return bigInteger;
		}
		
		// Token: 0x0600004E RID: 78 RVA: 0x00005AFC File Offset: 0x00003CFC
		public BigInteger genCoPrime(int bits, Random rand)
		{
			bool flag = false;
			BigInteger bigInteger = new BigInteger();
			while (!flag)
			{
				bigInteger.genRandomBits(bits, rand);
				BigInteger bigInteger2 = bigInteger.gcd(this);
				if (bigInteger2.dataLength == 1 && bigInteger2.data[0] == 1U)
				{
					flag = true;
				}
			}
			return bigInteger;
		}
		
		// Token: 0x0600004F RID: 79 RVA: 0x00005B5C File Offset: 0x00003D5C
		public BigInteger modInverse(BigInteger modulus)
		{
			BigInteger[] array = new BigInteger[] { 0, 1 };
			BigInteger[] array2 = new BigInteger[2];
			BigInteger[] array3 = new BigInteger[] { 0, 0 };
			int num = 0;
			BigInteger bigInteger = modulus;
			BigInteger bigInteger2 = this;
			while (bigInteger2.dataLength > 1 || (bigInteger2.dataLength == 1 && bigInteger2.data[0] != 0U))
			{
				BigInteger bigInteger3 = new BigInteger();
				BigInteger bigInteger4 = new BigInteger();
				if (num > 1)
				{
					BigInteger bigInteger5 = (array[0] - array[1] * array2[0]) % modulus;
					array[0] = array[1];
					array[1] = bigInteger5;
				}
				if (bigInteger2.dataLength == 1)
				{
					BigInteger.singleByteDivide(bigInteger, bigInteger2, bigInteger3, bigInteger4);
				}
				else
				{
					BigInteger.multiByteDivide(bigInteger, bigInteger2, bigInteger3, bigInteger4);
				}
				array2[0] = array2[1];
				array3[0] = array3[1];
				array2[1] = bigInteger3;
				array3[1] = bigInteger4;
				bigInteger = bigInteger2;
				bigInteger2 = bigInteger4;
				num++;
			}
			if (array3[0].dataLength > 1 || (array3[0].dataLength == 1 && array3[0].data[0] != 1U))
			{
				throw new ArithmeticException("No inverse!");
			}
			BigInteger bigInteger6 = (array[0] - array[1] * array2[0]) % modulus;
			if ((bigInteger6.data[69] & 2147483648U) != 0U)
			{
				bigInteger6 += modulus;
			}
			return bigInteger6;
		}
		
		// Token: 0x06000050 RID: 80 RVA: 0x00005D0C File Offset: 0x00003F0C
		public byte[] GetBytes()
		{
			byte[] array;
			if (this == 0)
			{
				array = new byte[1];
			}
			else
			{
				int num = this.bitCount();
				int num2 = num >> 3;
				if ((num & 7) != 0)
				{
					num2++;
				}
				byte[] array2 = new byte[num2];
				int num3 = num2 & 3;
				if (num3 == 0)
				{
					num3 = 4;
				}
				int num4 = 0;
				for (int i = this.dataLength - 1; i >= 0; i--)
				{
					uint num5 = this.data[i];
					for (int j = num3 - 1; j >= 0; j--)
					{
						array2[num4 + j] = (byte)(num5 & 255U);
						num5 >>= 8;
					}
					num4 += num3;
					num3 = 4;
				}
				array = array2;
			}
			return array;
		}
		
		// Token: 0x06000051 RID: 81 RVA: 0x00005DE8 File Offset: 0x00003FE8
		public void setBit(uint bitNum)
		{
			uint num = bitNum >> 5;
			byte b = (byte)(bitNum & 31U);
			uint num2 = 1U << (int)b;
			this.data[(int)((UIntPtr)num)] |= num2;
			if ((ulong)num >= (ulong)((long)this.dataLength))
			{
				this.dataLength = (int)(num + 1U);
			}
		}
		
		// Token: 0x06000052 RID: 82 RVA: 0x00005E3C File Offset: 0x0000403C
		public void unsetBit(uint bitNum)
		{
			uint num = bitNum >> 5;
			if ((ulong)num < (ulong)((long)this.dataLength))
			{
				byte b = (byte)(bitNum & 31U);
				uint num2 = 1U << (int)b;
				uint num3 = uint.MaxValue ^ num2;
				this.data[(int)((UIntPtr)num)] &= num3;
				if (this.dataLength > 1 && this.data[this.dataLength - 1] == 0U)
				{
					this.dataLength--;
				}
			}
		}
		
		// Token: 0x06000053 RID: 83 RVA: 0x00005EC8 File Offset: 0x000040C8
		public BigInteger sqrt()
		{
			uint num = (uint)this.bitCount();
			if ((num & 1U) != 0U)
			{
				num = (num >> 1) + 1U;
			}
			else
			{
				num >>= 1;
			}
			uint num2 = num >> 5;
			byte b = (byte)(num & 31U);
			BigInteger bigInteger = new BigInteger();
			uint num3;
			if (b == 0)
			{
				num3 = 2147483648U;
			}
			else
			{
				num3 = 1U << (int)b;
				num2 += 1U;
			}
			bigInteger.dataLength = (int)num2;
			for (int i = (int)(num2 - 1U); i >= 0; i--)
			{
				while (num3 != 0U)
				{
					bigInteger.data[i] ^= num3;
					if (bigInteger * bigInteger > this)
					{
						bigInteger.data[i] ^= num3;
					}
					num3 >>= 1;
				}
				num3 = 2147483648U;
			}
			return bigInteger;
		}
		
		// Token: 0x06000054 RID: 84 RVA: 0x00005FC0 File Offset: 0x000041C0
		public static BigInteger[] LucasSequence(BigInteger P, BigInteger Q, BigInteger k, BigInteger n)
		{
			BigInteger[] array;
			if (k.dataLength == 1 && k.data[0] == 0U)
			{
				array = new BigInteger[]
				{
					0,
					2 % n,
					1 % n
				};
			}
			else
			{
				BigInteger bigInteger = new BigInteger();
				int num = n.dataLength << 1;
				bigInteger.data[num] = 1U;
				bigInteger.dataLength = num + 1;
				bigInteger /= n;
				int num2 = 0;
				for (int i = 0; i < k.dataLength; i++)
				{
					uint num3 = 1U;
					for (int j = 0; j < 32; j++)
					{
						if ((k.data[i] & num3) != 0U)
						{
							i = k.dataLength;
							break;
						}
						num3 <<= 1;
						num2++;
					}
				}
				BigInteger bigInteger2 = k >> num2;
				array = BigInteger.LucasSequenceHelper(P, Q, bigInteger2, n, bigInteger, num2);
			}
			return array;
		}
		
		// Token: 0x06000055 RID: 85 RVA: 0x000060D0 File Offset: 0x000042D0
		private static BigInteger[] LucasSequenceHelper(BigInteger P, BigInteger Q, BigInteger k, BigInteger n, BigInteger constant, int s)
		{
			BigInteger[] array = new BigInteger[3];
			if ((k.data[0] & 1U) == 0U)
			{
				throw new ArgumentException("Argument k must be odd.");
			}
			int num = k.bitCount();
			uint num2 = 1U << (num & 31) - 1;
			BigInteger bigInteger = 2 % n;
			BigInteger bigInteger2 = 1 % n;
			BigInteger bigInteger3 = P % n;
			BigInteger bigInteger4 = bigInteger2;
			bool flag = true;
			for (int i = k.dataLength - 1; i >= 0; i--)
			{
				while (num2 != 0U)
				{
					if (i == 0 && num2 == 1U)
					{
						break;
					}
					if ((k.data[i] & num2) != 0U)
					{
						bigInteger4 = bigInteger4 * bigInteger3 % n;
						bigInteger = (bigInteger * bigInteger3 - P * bigInteger2) % n;
						bigInteger3 = n.BarrettReduction(bigInteger3 * bigInteger3, n, constant);
						bigInteger3 = (bigInteger3 - (bigInteger2 * Q << 1)) % n;
						if (flag)
						{
							flag = false;
						}
						else
						{
							bigInteger2 = n.BarrettReduction(bigInteger2 * bigInteger2, n, constant);
						}
						bigInteger2 = bigInteger2 * Q % n;
					}
					else
					{
						bigInteger4 = (bigInteger4 * bigInteger - bigInteger2) % n;
						bigInteger3 = (bigInteger * bigInteger3 - P * bigInteger2) % n;
						bigInteger = n.BarrettReduction(bigInteger * bigInteger, n, constant);
						bigInteger = (bigInteger - (bigInteger2 << 1)) % n;
						if (flag)
						{
							bigInteger2 = Q % n;
							flag = false;
						}
						else
						{
							bigInteger2 = n.BarrettReduction(bigInteger2 * bigInteger2, n, constant);
						}
					}
					num2 >>= 1;
				}
				num2 = 2147483648U;
			}
			bigInteger4 = (bigInteger4 * bigInteger - bigInteger2) % n;
			bigInteger = (bigInteger * bigInteger3 - P * bigInteger2) % n;
			if (flag)
			{
				flag = false;
			}
			else
			{
				bigInteger2 = n.BarrettReduction(bigInteger2 * bigInteger2, n, constant);
			}
			bigInteger2 = bigInteger2 * Q % n;
			for (int i = 0; i < s; i++)
			{
				bigInteger4 = bigInteger4 * bigInteger % n;
				bigInteger = (bigInteger * bigInteger - (bigInteger2 << 1)) % n;
				if (flag)
				{
					bigInteger2 = Q % n;
					flag = false;
				}
				else
				{
					bigInteger2 = n.BarrettReduction(bigInteger2 * bigInteger2, n, constant);
				}
			}
			array[0] = bigInteger4;
			array[1] = bigInteger;
			array[2] = bigInteger2;
			return array;
		}
		
		// Token: 0x06000056 RID: 86 RVA: 0x000063D4 File Offset: 0x000045D4
		public static void MulDivTest(int rounds)
		{
			Random random = new Random();
			byte[] array = new byte[64];
			byte[] array2 = new byte[64];
			for (int i = 0; i < rounds; i++)
			{
				int num;
				for (num = 0; num == 0; num = (int)(random.NextDouble() * 65.0))
				{
				}
				int num2;
				for (num2 = 0; num2 == 0; num2 = (int)(random.NextDouble() * 65.0))
				{
				}
				bool flag = false;
				while (!flag)
				{
					for (int j = 0; j < 64; j++)
					{
						if (j < num)
						{
							array[j] = (byte)(random.NextDouble() * 256.0);
						}
						else
						{
							array[j] = 0;
						}
						if (array[j] != 0)
						{
							flag = true;
						}
					}
				}
				flag = false;
				while (!flag)
				{
					for (int j = 0; j < 64; j++)
					{
						if (j < num2)
						{
							array2[j] = (byte)(random.NextDouble() * 256.0);
						}
						else
						{
							array2[j] = 0;
						}
						if (array2[j] != 0)
						{
							flag = true;
						}
					}
				}
				while (array[0] == 0)
				{
					array[0] = (byte)(random.NextDouble() * 256.0);
				}
				while (array2[0] == 0)
				{
					array2[0] = (byte)(random.NextDouble() * 256.0);
				}
				Console.WriteLine(i);
				BigInteger bigInteger = new BigInteger(array, num);
				BigInteger bigInteger2 = new BigInteger(array2, num2);
				BigInteger bigInteger3 = bigInteger / bigInteger2;
				BigInteger bigInteger4 = bigInteger % bigInteger2;
				BigInteger bigInteger5 = bigInteger3 * bigInteger2 + bigInteger4;
				if (bigInteger5 != bigInteger)
				{
					Console.WriteLine("Error at " + i);
					Console.WriteLine(bigInteger + "\n");
					Console.WriteLine(bigInteger2 + "\n");
					Console.WriteLine(bigInteger3 + "\n");
					Console.WriteLine(bigInteger4 + "\n");
					Console.WriteLine(bigInteger5 + "\n");
					break;
				}
			}
		}
		
		// Token: 0x06000057 RID: 87 RVA: 0x00006630 File Offset: 0x00004830
		public static void RSATest(int rounds)
		{
			Random random = new Random(1);
			byte[] array = new byte[64];
			BigInteger bigInteger = new BigInteger("a932b948feed4fb2b692609bd22164fc9edb59fae7880cc1eaff7b3c9626b7e5b241c27a974833b2622ebe09beb451917663d47232488f23a117fc97720f1e7", 16);
			BigInteger bigInteger2 = new BigInteger("4adf2f7a89da93248509347d2ae506d683dd3a16357e859a980c4f77a4e2f7a01fae289f13a851df6e9db5adaa60bfd2b162bbbe31f7c8f828261a6839311929d2cef4f864dde65e556ce43c89bbbf9f1ac5511315847ce9cc8dc92470a747b8792d6a83b0092d2e5ebaf852c85cacf34278efa99160f2f8aa7ee7214de07b7", 16);
			BigInteger bigInteger3 = new BigInteger("e8e77781f36a7b3188d711c2190b560f205a52391b3479cdb99fa010745cbeba5f2adc08e1de6bf38398a0487c4a73610d94ec36f17f3f46ad75e17bc1adfec99839589f45f95ccc94cb2a5c500b477eb3323d8cfab0c8458c96f0147a45d27e45a4d11d54d77684f65d48f15fafcc1ba208e71e921b9bd9017c16a5231af7f", 16);
			Console.WriteLine("e =\n" + bigInteger.ToString(10));
			Console.WriteLine("\nd =\n" + bigInteger2.ToString(10));
			Console.WriteLine("\nn =\n" + bigInteger3.ToString(10) + "\n");
			for (int i = 0; i < rounds; i++)
			{
				int num;
				for (num = 0; num == 0; num = (int)(random.NextDouble() * 65.0))
				{
				}
				bool flag = false;
				while (!flag)
				{
					for (int j = 0; j < 64; j++)
					{
						if (j < num)
						{
							array[j] = (byte)(random.NextDouble() * 256.0);
						}
						else
						{
							array[j] = 0;
						}
						if (array[j] != 0)
						{
							flag = true;
						}
					}
				}
				while (array[0] == 0)
				{
					array[0] = (byte)(random.NextDouble() * 256.0);
				}
				Console.Write("Round = " + i);
				BigInteger bigInteger4 = new BigInteger(array, num);
				BigInteger bigInteger5 = bigInteger4.ModPow(bigInteger, bigInteger3);
				BigInteger bigInteger6 = bigInteger5.ModPow(bigInteger2, bigInteger3);
				if (bigInteger6 != bigInteger4)
				{
					Console.WriteLine("\nError at round " + i);
					Console.WriteLine(bigInteger4 + "\n");
					break;
				}
				Console.WriteLine(" <PASSED>.");
			}
		}
		
		// Token: 0x06000058 RID: 88 RVA: 0x00006890 File Offset: 0x00004A90
		public static void RSATest2(int rounds)
		{
			Random random = new Random();
			byte[] array = new byte[64];
			byte[] array2 = new byte[]
			{
				133, 132, 100, 253, 112, 106, 159, 240, 148, 12,
				62, 44, 116, 52, 5, 201, 85, 179, 133, 50,
				152, 113, 249, 65, 33, 95, 2, 158, 234, 86,
				141, 140, 68, 204, 238, 238, 61, 44, 157, 44,
				18, 65, 30, 241, 197, 50, 195, 170, 49, 74,
				82, 216, 232, 175, 66, 244, 114, 161, 42, 13,
				151, 177, 49, 179
			};
			byte[] array3 = new byte[]
			{
				153, 152, 202, 184, 94, 215, 229, 220, 40, 92,
				111, 14, 21, 9, 89, 110, 132, 243, 129, 205,
				222, 66, 220, 147, 194, 122, 98, 172, 108, 175,
				222, 116, 227, 203, 96, 32, 56, 156, 33, 195,
				220, 200, 162, 77, 198, 42, 53, 127, 243, 169,
				232, 29, 123, 44, 120, 250, 184, 2, 85, 128,
				155, 194, 165, 203
			};
			BigInteger bigInteger = new BigInteger(array2);
			BigInteger bigInteger2 = new BigInteger(array3);
			BigInteger bigInteger3 = (bigInteger - 1) * (bigInteger2 - 1);
			BigInteger bigInteger4 = bigInteger * bigInteger2;
			for (int i = 0; i < rounds; i++)
			{
				BigInteger bigInteger5 = bigInteger3.genCoPrime(512, random);
				BigInteger bigInteger6 = bigInteger5.modInverse(bigInteger3);
				Console.WriteLine("\ne =\n" + bigInteger5.ToString(10));
				Console.WriteLine("\nd =\n" + bigInteger6.ToString(10));
				Console.WriteLine("\nn =\n" + bigInteger4.ToString(10) + "\n");
				int num;
				for (num = 0; num == 0; num = (int)(random.NextDouble() * 65.0))
				{
				}
				bool flag = false;
				while (!flag)
				{
					for (int j = 0; j < 64; j++)
					{
						if (j < num)
						{
							array[j] = (byte)(random.NextDouble() * 256.0);
						}
						else
						{
							array[j] = 0;
						}
						if (array[j] != 0)
						{
							flag = true;
						}
					}
				}
				while (array[0] == 0)
				{
					array[0] = (byte)(random.NextDouble() * 256.0);
				}
				Console.Write("Round = " + i);
				BigInteger bigInteger7 = new BigInteger(array, num);
				BigInteger bigInteger8 = bigInteger7.ModPow(bigInteger5, bigInteger4);
				BigInteger bigInteger9 = bigInteger8.ModPow(bigInteger6, bigInteger4);
				if (bigInteger9 != bigInteger7)
				{
					Console.WriteLine("\nError at round " + i);
					Console.WriteLine(bigInteger7 + "\n");
					break;
				}
				Console.WriteLine(" <PASSED>.");
			}
		}
		
		// Token: 0x06000059 RID: 89 RVA: 0x00006AC8 File Offset: 0x00004CC8
		public static void SqrtTest(int rounds)
		{
			Random random = new Random();
			for (int i = 0; i < rounds; i++)
			{
				int num;
				for (num = 0; num == 0; num = (int)(random.NextDouble() * 1024.0))
				{
				}
				Console.Write("Round = " + i);
				BigInteger bigInteger = new BigInteger();
				bigInteger.genRandomBits(num, random);
				BigInteger bigInteger2 = bigInteger.sqrt();
				BigInteger bigInteger3 = (bigInteger2 + 1) * (bigInteger2 + 1);
				if (bigInteger3 <= bigInteger)
				{
					Console.WriteLine("\nError at round " + i);
					Console.WriteLine(bigInteger + "\n");
					break;
				}
				Console.WriteLine(" <PASSED>.");
			}
		}
		
		// Token: 0x0600005A RID: 90 RVA: 0x00006C3C File Offset: 0x00004E3C
		public static void Main(string[] args)
		{
			byte[] array = new byte[]
			{
				0, 133, 132, 100, 253, 112, 106, 159, 240, 148,
				12, 62, 44, 116, 52, 5, 201, 85, 179, 133,
				50, 152, 113, 249, 65, 33, 95, 2, 158, 234,
				86, 141, 140, 68, 204, 238, 238, 61, 44, 157,
				44, 18, 65, 30, 241, 197, 50, 195, 170, 49,
				74, 82, 216, 232, 175, 66, 244, 114, 161, 42,
				13, 151, 177, 49, 179
			};
			byte[] array2 = new byte[]
			{
				0, 153, 152, 202, 184, 94, 215, 229, 220, 40,
				92, 111, 14, 21, 9, 89, 110, 132, 243, 129,
				205, 222, 66, 220, 147, 194, 122, 98, 172, 108,
				175, 222, 116, 227, 203, 96, 32, 56, 156, 33,
				195, 220, 200, 162, 77, 198, 42, 53, 127, 243,
				169, 232, 29, 123, 44, 120, 250, 184, 2, 85,
				128, 155, 194, 165, 203
			};
			Console.WriteLine("List of primes < 2000\n---------------------");
			int num = 100;
			int num2 = 0;
			for (int i = 0; i < 2000; i++)
			{
				if (i >= num)
				{
					Console.WriteLine();
					num += 100;
				}
				BigInteger bigInteger = new BigInteger((long)(-(long)i));
				if (bigInteger.isProbablePrime())
				{
					Console.Write(i + ", ");
					num2++;
				}
			}
			Console.WriteLine("\nCount = " + num2);
			BigInteger bigInteger2 = new BigInteger(array);
			Console.WriteLine("\n\nPrimality testing for\n" + bigInteger2.ToString() + "\n");
			Console.WriteLine("SolovayStrassenTest(5) = " + bigInteger2.SolovayStrassenTest(5));
			Console.WriteLine("RabinMillerTest(5) = " + bigInteger2.RabinMillerTest(5));
			Console.WriteLine("FermatLittleTest(5) = " + bigInteger2.FermatLittleTest(5));
			Console.WriteLine("isProbablePrime() = " + bigInteger2.isProbablePrime());
			Console.Write("\nGenerating 512-bits random pseudoprime. . .");
			Random random = new Random();
			BigInteger bigInteger3 = BigInteger.genPseudoPrime(512, 5, random);
			Console.WriteLine("\n" + bigInteger3);
		}
		
		// Token: 0x04000003 RID: 3
		private const int maxLength = 70;
		
		// Token: 0x04000004 RID: 4
		public static readonly int[] primesBelow2000 = new int[]
		{
			2, 3, 5, 7, 11, 13, 17, 19, 23, 29,
			31, 37, 41, 43, 47, 53, 59, 61, 67, 71,
			73, 79, 83, 89, 97, 101, 103, 107, 109, 113,
			127, 131, 137, 139, 149, 151, 157, 163, 167, 173,
			179, 181, 191, 193, 197, 199, 211, 223, 227, 229,
			233, 239, 241, 251, 257, 263, 269, 271, 277, 281,
			283, 293, 307, 311, 313, 317, 331, 337, 347, 349,
			353, 359, 367, 373, 379, 383, 389, 397, 401, 409,
			419, 421, 431, 433, 439, 443, 449, 457, 461, 463,
			467, 479, 487, 491, 499, 503, 509, 521, 523, 541,
			547, 557, 563, 569, 571, 577, 587, 593, 599, 601,
			607, 613, 617, 619, 631, 641, 643, 647, 653, 659,
			661, 673, 677, 683, 691, 701, 709, 719, 727, 733,
			739, 743, 751, 757, 761, 769, 773, 787, 797, 809,
			811, 821, 823, 827, 829, 839, 853, 857, 859, 863,
			877, 881, 883, 887, 907, 911, 919, 929, 937, 941,
			947, 953, 967, 971, 977, 983, 991, 997, 1009, 1013,
			1019, 1021, 1031, 1033, 1039, 1049, 1051, 1061, 1063, 1069,
			1087, 1091, 1093, 1097, 1103, 1109, 1117, 1123, 1129, 1151,
			1153, 1163, 1171, 1181, 1187, 1193, 1201, 1213, 1217, 1223,
			1229, 1231, 1237, 1249, 1259, 1277, 1279, 1283, 1289, 1291,
			1297, 1301, 1303, 1307, 1319, 1321, 1327, 1361, 1367, 1373,
			1381, 1399, 1409, 1423, 1427, 1429, 1433, 1439, 1447, 1451,
			1453, 1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499, 1511,
			1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583,
			1597, 1601, 1607, 1609, 1613, 1619, 1621, 1627, 1637, 1657,
			1663, 1667, 1669, 1693, 1697, 1699, 1709, 1721, 1723, 1733,
			1741, 1747, 1753, 1759, 1777, 1783, 1787, 1789, 1801, 1811,
			1823, 1831, 1847, 1861, 1867, 1871, 1873, 1877, 1879, 1889,
			1901, 1907, 1913, 1931, 1933, 1949, 1951, 1973, 1979, 1987,
			1993, 1997, 1999
		};
		
		// Token: 0x04000005 RID: 5
		private uint[] data = null;
		
		// Token: 0x04000006 RID: 6
		public int dataLength;
	}
}
