using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace InformationTheoryLab3
{
    public class RabinCipher
    {
        private BigInteger p, q, n, b;
        private BigInteger yp, yq;

        public void SetParameters(BigInteger p, BigInteger q, BigInteger b)
        {
            this.p = p;
            this.q = q;
            this.b = b;
            this.n = p * q;
            ExtendedEuclid(p, q, out BigInteger yp, out BigInteger yq);
            this.yp = yp;
            this.yq = yq;
        }


        public List<BigInteger> Encrypt(byte[] data)
        {
            List<BigInteger> result = new List<BigInteger>();
            foreach (byte m in data)
            {
                BigInteger mBig = m;
                BigInteger c = (mBig * (mBig + b)) % n;
                result.Add(c);
            }
            return result;
        }


        public byte[] Decrypt(BigInteger[] ciphertexts)
        {
            List<byte> result = new List<byte>();
            foreach (BigInteger c in ciphertexts)
            {
                byte m = DecryptSingle(c);
                result.Add(m);
            }
            return result.ToArray();
        }

        private byte DecryptSingle(BigInteger c)
        {
            BigInteger D = (b * b + 4 * c) % n;
            if (D < 0) D += n;

            BigInteger sqrtD_p = ModPow(D, (p + 1) / 4, p);
            BigInteger sqrtD_q = ModPow(D, (q + 1) / 4, q);

            BigInteger r1 = (yp * p * sqrtD_q + yq * q * sqrtD_p) % n;
            if (r1 < 0) r1 += n;
            BigInteger r2 = n - r1;
            BigInteger r3 = (yp * p * sqrtD_q - yq * q * sqrtD_p) % n;
            if (r3 < 0) r3 += n;
            BigInteger r4 = n - r3;

            List<BigInteger> roots = new List<BigInteger> { r1, r2, r3, r4 };


            foreach (BigInteger r in roots)
            {
                BigInteger m = RecoverMessage(r);
                if (m >= 0 && m <= 255)
                {
                    return (byte)m;
                }
            }
            throw new Exception("Не удалось найти подходящий байт при дешифровании. Проверьте параметры p,q,b.");
        }

        private BigInteger RecoverMessage(BigInteger r)
        {
            BigInteger numerator;
            if ((r - b) % 2 == 0)
            {
                numerator = (-b + r) / 2;
            }
            else
            {
                numerator = (-b + n + r) / 2;
            }
            BigInteger m = numerator % n;
            if (m < 0) m += n;
            return m;
        }

        private void ExtendedEuclid(BigInteger a, BigInteger b, out BigInteger x, out BigInteger y)
        {
            if (b == 0)
            {
                x = 1;
                y = 0;
                return;
            }
            ExtendedEuclid(b, a % b, out BigInteger x1, out BigInteger y1);
            x = y1;
            y = x1 - (a / b) * y1;
        }

        private BigInteger ModPow(BigInteger a, BigInteger z, BigInteger n)
        {
            BigInteger result = 1;
            a = a % n;
            while (z > 0)
            {
                if (z % 2 == 1)
                    result = (result * a) % n;
                a = (a * a) % n;
                z = z / 2;
            }
            return result;
        }
    }
}