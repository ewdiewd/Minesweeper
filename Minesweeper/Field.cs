using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Minesweeper
{
    class Field:INotifyPropertyChanged
    {

        public int ModeGrid;
        private int Width;
        private int Height;
        private string Difficulty;
        public int width{ get { return this.Width; }set { Width = value; OnPropertyChanged("Width");} }
        public int height{ get { return this.Height; }set { Height = value; OnPropertyChanged("Height");} }
        public string difficulty { get { return this.Difficulty; } set { Difficulty = value; OnPropertyChanged("Difficulty"); } }
        public bool state =false;
        public int[,] f;
        public int[] Fl;
        public event PropertyChangedEventHandler? PropertyChanged;

        public Field(string difficulty) 
        {
            f= Generator(difficulty, out int modeGrid,out int[] F);
            ModeGrid = modeGrid;
            this.difficulty = difficulty;
            this.Fl = F;
            //if (this.PropertyChanged != null)
            //{
            //    OnPropertyChanged("Width");
            //    OnPropertyChanged("Height");
            //}
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
          //MessageBox.Show("我启动了");
        }
        public int[,] Generator(string mode, out int ModeGrid,out int[] F)
        {
            int Row, Col, Mine;
            switch (mode)
            {
                case "Easy":
                    Row = 9; Col = 9; Mine = 10;
                    break;
                case "Normal":
                    Row = 16; Col = 16; Mine = 40;
                    break;
                case "Hard":
                    Row = 16; Col = 30; Mine = 99;
                    break;
                default:
                    Row = 0; Col = 0; Mine = 0;
                    break;
            }
            ModeGrid = Row * Col;
            int[] sheet1 = new int[ModeGrid];
            Random random = new Random();
            while (Mine > 0)
            {
                int check = random.Next(ModeGrid);
                if (sheet1[check] == 9)//数字9表示雷
                {
                    continue;
                }
                sheet1[check] = 9;
                Mine--;
            }
            int[,] sheet2 = new int[Row, Col]; int temp_k = 0;
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Col; j++)
                {
                    sheet2[i, j] = sheet1[temp_k];
                    temp_k++;
                }
            }
            int[,] sheet3 = Padding(sheet2);
            int[,] sheet4 = new int[Row, Col];
            for (int i = 1; i < sheet3.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < sheet3.GetLength(1) - 1; j++)
                {
                    sheet4[i - 1, j - 1] = F3x3(sheet3, i, j);
                }
            }
            int[] sheet5 = new int[ModeGrid]; temp_k = 0;
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Col; j++)
                {
                    sheet5[temp_k] = sheet4[i,j];
                    temp_k++;
                }
            }
            F = sheet5;
            return sheet4;

        }
        int[,] Padding(int[,] sheet, int s = 1) //填充函数
        {
            int r = sheet.GetLength(0) + 2 * s; int c = sheet.GetLength(1) + 2 * s;
            int[,] padsheet = new int[r, c];
            for (int i = 1; i < r - 1; i++)
            {
                for (int j = 1; j < c - 1; j++)
                {
                    padsheet[i, j] = sheet[i - 1, j - 1];
                }
            }
            return padsheet;
        }
        int F3x3(int[,] padsheet, int x, int y) //加法
        {
            int result = 0;
            if (padsheet[x, y] == 9)
            {
                return 9;
            }
            for (int i = x - 1; i < x - 1 + 3; i++)
            {
                for (int j = y - 1; j < y - 1 + 3; j++)
                {
                    if (padsheet[i, j] == 9)
                    {
                        result++;
                    }
                }
            }
            return result;
        }


    }


}
