using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache._21single
{
    public class BasicStrategy
    {
        private Hashtable hashtableX = null;
        private Hashtable hashtableY = null;
        private string[,] Matrix = null;

		public BasicStrategy()
		{
			hashtableX = GetHtX();
			hashtableY = GetHtY();
			Matrix = GetMatrix();
		}

		/// <summary>
		/// 生成哈希对应表  dealer
		/// </summary>
		/// <returns></returns>
		private Hashtable GetHtX()
		{
			//策略数组X轴对应表，X表示列数
			Hashtable htX = new Hashtable();

			//表示策略数组对应的X轴取值
			htX.Add("2", 0);
			htX.Add("3", 1);
			htX.Add("4", 2);
			htX.Add("5", 3);
			htX.Add("6", 4);
			htX.Add("7", 5);
			htX.Add("8", 6);
			htX.Add("9", 7);
			htX.Add("10", 8); //表示 10,J,Q,K
			htX.Add("1", 9);  //表示 A

			return htX;
		}

		/// <summary>
		/// 生成哈希对应表  player
		/// </summary>
		/// <returns></returns>
		private Hashtable GetHtY()
		{
			//策略数组Y轴对应表，Y表示行数
			Hashtable htY = new Hashtable();

			//表示策略数组对应的Y轴取值


			htY.Add("H20", 0);
			htY.Add("H19", 1);
			htY.Add("H18", 2);
			htY.Add("H17", 3);
			htY.Add("H16", 4);
			htY.Add("H15", 5);
			htY.Add("H14", 6);
			htY.Add("H13", 7);
			htY.Add("H12", 8);
			htY.Add("H11", 9);
			htY.Add("H10", 10);
			htY.Add("H9", 11);
			htY.Add("H8", 12);
			htY.Add("H7", 13);
			htY.Add("H6", 14);
			htY.Add("H5", 15);

			htY.Add("A9", 16);
			htY.Add("A8", 17);
			htY.Add("A7", 18);
			htY.Add("A6", 19);
			htY.Add("A5", 20);
			htY.Add("A4", 21);
			htY.Add("A3", 22);
			htY.Add("A2", 23);

			htY.Add("AA", 24);
			htY.Add("TT", 25);
			htY.Add("99", 26);
			htY.Add("88", 27);
			htY.Add("77", 28);
			htY.Add("66", 29);
			htY.Add("55", 30);
			htY.Add("44", 31);
			htY.Add("33", 32);
			htY.Add("22", 33);

			return htY;
		}

		/// <summary>
		/// 获得策略数组1, A17Stop 标准策略，no surrender/stop at soft 17
		/// </summary>
		/// <returns></returns>
		private string[,] GetMatrix()
		{
			
			string[,] multiArray = new string[34, 10] {
			{"S","S","S","S","S","S","S","S","S","S"},
			{"S","S","S","S","S","S","S","S","S","S"},
			{"S","S","S","S","S","S","S","S","S","S"},
			{"S","S","S","S","S","S","S","S","S","S"},

			{"S","S","S","S","S","H","H","H","H","H"},
			{"S","S","S","S","S","H","H","H","H","H"},
			{"S","S","S","S","S","H","H","H","H","H"},
			{"S","S","S","S","S","H","H","H","H","H"},

			{"H","H","S","S","S","H","H","H","H","H"},
			{"D","D","D","D","D","D","D","D","D","D"},//
			{"D","D","D","D","D","D","D","D","H","H"},
			{"H","D","D","D","D","H","H","H","H","H"},
			{"H","H","H","H","H","H","H","H","H","H"},
			{"H","H","H","H","H","H","H","H","H","H"},
			{"H","H","H","H","H","H","H","H","H","H"},
			{"H","H","H","H","H","H","H","H","H","H"},

			{"S","S","S","S","S","S","S","S","S","S"},
			{"S","S","S","S","D","S","S","S","S","S"},
			{"D","D","D","D","D","S","S","H","H","H"},
			{"H","D","D","D","D","H","H","H","H","H"},
			{"H","H","D","D","D","H","H","H","H","H"},
			{"H","H","D","D","D","H","H","H","H","H"},
			{"H","H","H","D","D","H","H","H","H","H"},
			{"H","H","H","D","D","H","H","H","H","H"},
			{"P","P","P","P","P","P","P","P","P","P"},
			{"S","S","S","S","S","S","S","S","S","S"},
			{"P","P","P","P","P","S","P","P","S","S"},
			{"P","P","P","P","P","P","P","P","P","P"},
			{"P","P","P","P","P","P","H","H","H","H"},
			{"P","P","P","P","P","H","H","H","H","H"},
			{"D","D","D","D","D","D","D","D","H","H"},
			{"H","H","H","P","P","P","H","H","H","H"},
			{"P","P","P","P","P","P","H","H","H","H"},
			{"P","P","P","P","P","P","H","H","H","H"}

			};

			return multiArray;
		}

		public string trueStrategy(string dealerCardType, string playerCardType)
		{
			int intX = (int)hashtableX[dealerCardType];
			int intY = (int)hashtableY[playerCardType];
			string realAns = Matrix[intY, intX];
			return realAns;
		}



	}
}
